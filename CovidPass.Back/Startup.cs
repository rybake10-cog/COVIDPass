// © 2020 Partners HealthCare Technology & Innovation Department
using System;
using System.Data;
using System.IO;
using System.Text;
using Azure.Core;
using Azure.Identity;
using CovidPass.Back.Middleware;
using CovidPass.Back.Middleware.Cqrs;
using CovidPass.Core;
using CovidPass.Core.External.Auth;
using CovidPass.Core.External.Email;
using CovidPass.Core.Models.Config;
using CovidPass.Db;
using CovidPass.Db.Migrations;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CovidPass.Back
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetSection("Connections").GetValue<string>("Default");

            //db
            services.AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSqlServer2016()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(IDatabase).Assembly).For.Migrations()
                    .WithVersionTable(new VersionTable())
                );
            services.AddTransient<IDbConnection, SqlConnection>(sp => new SqlConnection(connectionString));
            services.AddScoped<IDatabase, Database>();

            var authConfig = Configuration.GetSection("Auth").Get<AuthConfigModel>() ?? new AuthConfigModel();
            services.AddSingleton(authConfig);
            var authentication = services.AddAuthentication(o =>
            {
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            if (authConfig.IsEntraId)
            {
                var entraId = authConfig.EntraId;
                entraId.Validate();
                authentication.AddJwtBearer(options =>
                {
                    options.Authority = entraId.Authority;
                    options.MapInboundClaims = false;
                    options.TokenValidationParameters.ValidAudiences = entraId.ValidAudiences;
                    options.TokenValidationParameters.NameClaimType = entraId.LoginClaim;
                });
                services.AddSingleton(entraId);
                services.AddTransient<IClaimsTransformation, EntraIdClaimsTransformation>();
                services.AddSingleton<TokenCredential>(_ => new DefaultAzureCredential());
                services.AddHttpClient<IUserLookupService, GraphUserLookupService>();
            }
            else
            {
                authentication.AddJwtBearer(options =>
                {
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authConfig.Key ?? string.Empty));
                    options.TokenValidationParameters.ValidIssuer = authConfig.Issuer;
                    options.TokenValidationParameters.ValidAudience = authConfig.Audience;
                    options.TokenValidationParameters.IssuerSigningKey = key;
                    options.TokenValidationParameters.ValidateIssuerSigningKey = true;
                    options.TokenValidationParameters.ValidateLifetime = true;
                    options.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
                });
                services.AddTransient<IUserLookupService, LdapUserLookupService>();
            }

            services.AddAuthorization();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Anchor).Assembly));
            services.AddControllers();
            services.AddResponseCompression();
            services.AddHealthChecks();
            services.AddHttpContextAccessor();
            services.Configure<ForwardedHeadersOptions>(o =>
            {
                o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                o.KnownIPNetworks.Clear();
                o.KnownProxies.Clear();
            });

            services.AddTransient<IDirectoryService, DirectoryService>();
            services.AddSingleton<IEmailService, EmailService>();
            services.AddHttpClient();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();
            var rewriteUrlSection = Configuration.GetValue<string>("RewriteUrlSection");
            app.UseRewriter(new RewriteOptions().AddRewrite($@"{rewriteUrlSection}(.*?)$", "$1", false));
            app.UseResponseCompression();
            app.UseExceptionConverter();
            app.UseAuthentication();
            app.Use(async (context, next) =>
            {
                var path = context.Request.Path.Value;
                if (!path.StartsWith("/api") && !Path.HasExtension(path))
                    context.Request.Path = "/index.html";
                await next();
            });
            app.UseCqrs();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/api/health");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}

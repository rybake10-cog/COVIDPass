// © 2020 Partners HealthCare Technology & Innovation Department
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using CovidPass.Core;
using Scrutor;
using CovidPass.Core.Infrastructure.Services;
using CovidPass.Core.Models.Config;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IO;
using CovidPass.Back.Middleware.Cqrs;
using CovidPass.Back.Middleware;
using FluentMigrator.Runner;
using CovidPass.Db;
using CovidPass.Db.Migrations;
using System.Data;
using System.Data.SqlClient;
using CovidPass.Core.External.Auth;
using CovidPass.Core.External.Email;
using Microsoft.AspNetCore.Rewrite;

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

            var authConfig = Configuration.GetSection("Auth").Get<AuthConfigModel>();

            services
                .AddAuthentication(o =>
                {
                    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authConfig.Key));
                    options.TokenValidationParameters.ValidIssuer = authConfig.Issuer;
                    options.TokenValidationParameters.ValidAudience = authConfig.Audience;
                    options.TokenValidationParameters.IssuerSigningKey = key;
                    options.TokenValidationParameters.ValidateIssuerSigningKey = true;
                    options.TokenValidationParameters.ValidateLifetime = true;
                    options.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
                });
            services.AddAuthorization();

            services.AddMediatR(typeof(Anchor).Assembly);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
            services.AddResponseCompression();

            services.AddTransient<IDirectoryService, DirectoryService>();
            services.AddSingleton<IEmailService, EmailService>();

            services.AddHttpClient();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}

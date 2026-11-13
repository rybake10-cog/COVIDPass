// © 2020 Partners HealthCare Technology & Innovation Department
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Identity;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;

namespace CovidPass.Back
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Info("Start");
                var host = CreateHostBuilder(args).Build();
                var config = host.Services.GetRequiredService<IConfiguration>();
                if (config.GetValue("Database:MigrateOnStartup", true))
                {
                    using var scope = host.Services.CreateScope();
                    scope.ServiceProvider.GetRequiredService<IMigrationRunner>().MigrateUp();
                }
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                logger.Error("Stopped program because of exception" + ex.ToString());
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    // Secrets live in Azure Key Vault in Azure; names use "--" for sections, e.g. "Auth--Key".
                    var vaultUri = config.Build()["KeyVault:Uri"];
                    if (!string.IsNullOrWhiteSpace(vaultUri))
                        config.AddAzureKeyVault(new Uri(vaultUri), new DefaultAzureCredential());
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                    webBuilder.UseIISIntegration();
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseWebRoot("wwwroot");
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();
    }
}

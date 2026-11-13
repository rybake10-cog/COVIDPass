// © 2020 Partners HealthCare Technology & Innovation Department
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentMigrator.Runner;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Info("Start");
                var host = CreateWebHostBuilder(args).Build();
                var migrationRunner = host.Services.GetRequiredService<IMigrationRunner>();
                migrationRunner.MigrateUp();
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                logger.Error("Stopped program because of exception" + ex.ToString());
                throw;
            }
            finally
            {
                //NLog.LogManager.Shutdown();
            }
            await Task.Delay(10000);
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
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
                    // logging.AddConsole();
                    // logging.AddDebug();
                    // logging.AddEventLog();
                    // logging.AddEventSourceLogger();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();
    }
}

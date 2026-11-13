using System;
using System.Threading.Tasks;
using CovidPass.Db;
using CovidPass.Db.Migrations;
using Dapper;
using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CovidPass.Tests
{
    /// <summary>
    /// Runs the FluentMigrator migrations and the insert path against a real SQL Server / Azure SQL database.
    /// Set COVIDPASS_TEST_SQL to a connection string to enable; skipped otherwise.
    /// </summary>
    public class SqlServerMigrationTests
    {
        private static readonly string ConnectionString = Environment.GetEnvironmentVariable("COVIDPASS_TEST_SQL");

        [SkippableFact]
        public async Task MigrationsApplyAndApplicationsCanBeInserted()
        {
            Skip.If(string.IsNullOrWhiteSpace(ConnectionString), "COVIDPASS_TEST_SQL not set");

            using (var provider = new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb.AddSqlServer2016()
                    .WithGlobalConnectionString(ConnectionString)
                    .ScanIn(typeof(IDatabase).Assembly).For.Migrations()
                    .WithVersionTable(new VersionTable()))
                .BuildServiceProvider())
            {
                provider.GetRequiredService<IMigrationRunner>().MigrateUp();
            }

            var login = "it-" + Guid.NewGuid().ToString("N").Substring(0, 8);
            using (var db = new Database(new SqlConnection(ConnectionString)))
            {
                await db.InsertApplication("Doe", "Jane", login, "1234567", "jane.doe@example.org", "Site 1", "JD",
                    true, false, false, false, false, false, false, false, false, false, "en", login, DateTime.UtcNow);
            }

            using var connection = new SqlConnection(ConnectionString);
            var count = await connection.ExecuteScalarAsync<int>(
                $"select count(*) from {CovidPass.Common.Constants.DbConstants.SchemaName}.{CovidPass.Common.Constants.DbConstants.ApplicationTable} where login = @login",
                new { login });
            Assert.Equal(1, count);
        }
    }
}

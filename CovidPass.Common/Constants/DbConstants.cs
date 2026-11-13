// © 2020 Partners HealthCare Technology & Innovation Department
namespace CovidPass.Common.Constants
{
    public class DbConstants
    {
        public static string TablePrefix { get; } = "cp_";
        public static string SchemaName { get; } = "dbo";
        public static string ApplicationTable { get; } = TablePrefix + "application";
        public static string VersionTable { get; } = TablePrefix + "version";
    }
}

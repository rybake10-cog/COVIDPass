// © 2020 Partners HealthCare Technology & Innovation Department
using CovidPass.Common.Constants;
using FluentMigrator.Runner.VersionTableInfo;

namespace CovidPass.Db.Migrations
{
    [VersionTableMetaData]
    public class VersionTable : IVersionTableMetaData
    {
        public string ColumnName => "Version";
        public string SchemaName => DbConstants.SchemaName;
        public string TableName => DbConstants.VersionTable;
        public string UniqueIndexName => "UC_Version";
        public string AppliedOnColumnName => "AppliedOn";
        public string DescriptionColumnName => "Description";
        public object ApplicationContext { get; set; }
        public bool OwnsSchema => true;
    }
}

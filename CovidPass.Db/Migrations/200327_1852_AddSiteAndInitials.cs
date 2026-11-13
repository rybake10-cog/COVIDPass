// © 2020 Partners HealthCare Technology & Innovation Department
using FluentMigrator;
using CovidPass.Common.Constants;

namespace CovidPass.Db.Migrations
{
    [Migration(200327_1852, TransactionBehavior.Default, "Add Site And Initials")]
    public class AddSiteAndInitials : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table(DbConstants.ApplicationTable)
                .InSchema(DbConstants.SchemaName)
                .AddColumn("site").AsString(50).Nullable()
                .AddColumn("initials").AsString(50).Nullable();
        }
    }
}

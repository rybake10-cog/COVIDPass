// © 2020 Partners HealthCare Technology & Innovation Department
using FluentMigrator;
using CovidPass.Common.Constants;

namespace CovidPass.Db.Migrations
{
    [Migration(200410_1049, TransactionBehavior.Default, "Add Language")]
    public class AddLanguage : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table(DbConstants.ApplicationTable)
                .InSchema(DbConstants.SchemaName)
                .AddColumn("language").AsString(150).Nullable();
        }
    }
}

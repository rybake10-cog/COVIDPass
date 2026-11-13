// © 2020 Partners HealthCare Technology & Innovation Department
using FluentMigrator;
using CovidPass.Common.Constants;

namespace CovidPass.Db.Migrations
{
    [Migration(200326_2209, TransactionBehavior.Default, "Add 3 new columns")]
    public class add3NewColumns : ForwardOnlyMigration
    {
        public override void Up()
        {

            Alter.Table(DbConstants.ApplicationTable)
                .InSchema(DbConstants.SchemaName)
                .AddColumn("runnynose").AsBoolean().Nullable();

            Alter.Table(DbConstants.ApplicationTable)
                .InSchema(DbConstants.SchemaName)
                .AddColumn("muscleache").AsBoolean().Nullable();

            Alter.Table(DbConstants.ApplicationTable)
                .InSchema(DbConstants.SchemaName)
                .AddColumn("newlossofsmell").AsBoolean().Nullable();



        }
    }
}

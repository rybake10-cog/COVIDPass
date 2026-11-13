// © 2020 Partners HealthCare Technology & Innovation Department
using FluentMigrator;
using CovidPass.Common.Constants;

namespace CovidPass.Db.Migrations
{
    [Migration(200324_0943, TransactionBehavior.Default, "Init")]
    public class Init : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.Table(DbConstants.ApplicationTable)
                .InSchema(DbConstants.SchemaName)
                .WithColumn("id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("lastname").AsString(250).Nullable()
                .WithColumn("firstname").AsString(250).Nullable()
                .WithColumn("login").AsString(50).Nullable()
                .WithColumn("employeeid").AsString(50).Nullable()
                .WithColumn("email").AsString(250).Nullable()
                .WithColumn("nosymptoms").AsBoolean().Nullable()
                .WithColumn("influenza").AsBoolean().Nullable()
                .WithColumn("fever").AsBoolean().Nullable()
                .WithColumn("cough").AsBoolean().Nullable()
                .WithColumn("shortnessofbreath").AsBoolean().Nullable()
                .WithColumn("sorethroat").AsBoolean().Nullable()
                .WithColumn("diarrheanausea").AsBoolean().Nullable()
                .WithColumn("createdby").AsString(50).Nullable()
                .WithColumn("created").AsDateTime2().Nullable();
        }
    }
}

using FluentMigrator;

namespace LightsOut.GameLogic.Migrations
{
    [Migration(20220403204000, "Added Timestamp and Username in HighScore table")]
    public class UpdateHighScore_Username_Timestamp_Table : Migration
    {
        public override void Down() { }

        public override void Up()
        {
            Alter.Table("HighScores")
                .AddColumn("Timestamp").AsDateTime2().WithDefaultValue(SystemMethods.CurrentUTCDateTime).NotNullable()
                .AddColumn("Username").AsString().NotNullable();
        }
    }
}
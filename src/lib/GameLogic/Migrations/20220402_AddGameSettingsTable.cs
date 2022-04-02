using System.Collections.Generic;
using FluentMigrator;

namespace LightsOut.GameLogic.Migrations
{
    [Migration(20220402132400, "Added Game Settings table.")]
    public class AddGameSettingsTable : Migration
    {
        public override void Down() { }

        public override void Up()
        {
            Create.Table("GameSettings")
                .WithDescription("Stores all game settings for different complexity levels and game durations.")
                .WithColumn("Id").AsInt16().PrimaryKey("PK_GameSettings").Identity()
                .WithColumn("ComplexityLevel").AsByte().Unique("UK_GameSettings_ComplexityLevel").NotNullable()
                .WithColumn("NoOfRows").AsByte().NotNullable()
                .WithColumn("NoOfColumns").AsByte().NotNullable()
                .WithColumn("NoOfSwitchedOnLights").AsByte().NotNullable()
                .WithColumn("GameMaxDuration").AsTime().NotNullable().WithDefaultValue("00:30:00");

            Insert.IntoTable("GameSettings").Row(new Dictionary<string, object>
            {
                ["ComplexityLevel"] = 1,
                ["NoOfRows"] = 5,
                ["NoOfColumns"] = 5,
                ["NoOfSwitchedOnLights"] = 9,
                ["GameMaxDuration"] = "00:15:00"
            });

            Insert.IntoTable("GameSettings").Row(new Dictionary<string, object>
            {
                ["ComplexityLevel"] = 2,
                ["NoOfRows"] = 6,
                ["NoOfColumns"] = 6,
                ["NoOfSwitchedOnLights"] = 10,
                ["GameMaxDuration"] = "00:10:00"
            });

            Insert.IntoTable("GameSettings").Row(new Dictionary<string, object>
            {
                ["ComplexityLevel"] = 3,
                ["NoOfRows"] = 9,
                ["NoOfColumns"] = 9,
                ["NoOfSwitchedOnLights"] = 11,
                ["GameMaxDuration"] = "00:05:00"
            });
        }
    }
}
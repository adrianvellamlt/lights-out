using FluentMigrator;

namespace LightsOut.GameLogic.Migrations
{
    [Migration(20220402163800, "Added HighScores table.")]
    public class AddHighScoresTable : Migration
    {
        public override void Down() { }

        public override void Up()
        {
            Create.Table("HighScores")
                .WithDescription("Stores all high scores.")
                .WithColumn("GameStateId").AsGuid().PrimaryKey("PK_HighScores")
                .WithColumn("ComplexityLevel").AsByte().NotNullable()
                .WithColumn("NoOfRows").AsByte().NotNullable()
                .WithColumn("NoOfColumns").AsByte().NotNullable()
                .WithColumn("RemainingLights").AsByte().NotNullable()
                .WithColumn("TimeTaken").AsTime().NotNullable()
                .WithColumn("NoOfMoves").AsInt16().NotNullable();
        }
    }
}
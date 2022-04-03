using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LightsOut.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace LightsOut.GameLogic
{
    public class HighScoreService : IHighScoreService
    {
        private IOptionsMonitor<ConnectionStringOptions> ConnectionStrings { get; }
        public HighScoreService(IOptionsMonitor<ConnectionStringOptions> connectionStrings)
            => ConnectionStrings = connectionStrings;

        public async Task AddHighScoreAsync(HighScore highScore, CancellationToken cancellationToken)
        {
            var connStr = ConnectionStrings.CurrentValue;

            if (string.IsNullOrEmpty(connStr.SQLite)) return;

            using var conn = new SqliteConnection(connStr.SQLite);

            await conn.OpenAsync(cancellationToken);

            await conn.ExecuteAsync
            (
                new CommandDefinition
                (
                    @"INSERT INTO [HighScores]
                    (
                          [GameStateId]
                        , [Username]
                        , [ComplexityLevel]
                        , [NoOfRows]
                        , [NoOfColumns]
                        , [RemainingLights]
                        , [TimeTaken]
                        , [NoOfMoves]
                    )
                    VALUES
                    (
                          @GameStateId
                        , @Username
                        , @ComplexityLevel
                        , @NoOfRows
                        , @NoOfColumns
                        , @RemainingLights
                        , @TimeTaken
                        , @NoOfMoves
                    )",
                    highScore,
                    cancellationToken: cancellationToken
                )
            );
        }

        public async Task<IEnumerable<HighScore>> GetBestHighScoresAsync(ushort limit, CancellationToken cancellationToken)
        {
            const string Query = 
@"SELECT 
      LOWER([GameStateId])
    , [Username]
    , [ComplexityLevel]
    , [NoOfRows]
    , [NoOfColumns]
    , [RemainingLights]
    , [TimeTaken] AS [TimeTakenStr]
    , [NoOfMoves]
FROM HighScores 
ORDER BY 
    [ComplexityLevel] DESC, 
    [RemainingLights] ASC, 
    [NoOfMoves] ASC, 
    [TimeTaken] DESC 
LIMIT {0};";

            var connStr = ConnectionStrings.CurrentValue;

            if (string.IsNullOrEmpty(connStr.SQLite)) return Enumerable.Empty<HighScore>();

            using var conn = new SqliteConnection(connStr.SQLite);

            await  conn.OpenAsync(cancellationToken);

            var highScores = await conn.QueryAsync<HighScore>(new CommandDefinition(string.Format(Query, limit), cancellationToken: cancellationToken));

            return highScores;
        }
    }
}
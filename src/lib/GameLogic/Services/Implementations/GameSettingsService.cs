using System;
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
    public class GameSettingsService : IGameSettingsService
    {
        private IOptionsMonitor<ConnectionStringOptions> ConnectionStrings { get; }
        public GameSettingsService(IOptionsMonitor<ConnectionStringOptions> connectionStringOptions)
        {
            ConnectionStrings = connectionStringOptions;
        }

        public async Task<GameSettings?> GetGameSettingAsync(ushort gameId, CancellationToken cancellationToken)
        {
            var gameSettings = await GetGameSettingsAsync(cancellationToken, gameId);

            return gameSettings.SingleOrDefault();
        }

        public Task<IEnumerable<GameSettings>> GetGameSettingsAsync(CancellationToken cancellationToken)
            => GetGameSettingsAsync(cancellationToken, null);

        private async Task<IEnumerable<GameSettings>> GetGameSettingsAsync(CancellationToken cancellationToken, ushort? gameId = null)
        {
            var connStr = ConnectionStrings.CurrentValue;

            if (string.IsNullOrWhiteSpace(connStr.SQLite)) return Enumerable.Empty<GameSettings>();

            using var conn = new SqliteConnection(connStr.SQLite);

            await conn.OpenAsync(cancellationToken);

            var builder = new SqlBuilder();

            var template = builder.AddTemplate(
                @"SELECT 
                      [Id]
                    , [ComplexityLevel]
                    , [NoOfRows]
                    , [NoOfColumns]
                    , [NoOfSwitchedOnLights]
                    , [GameMaxDuration] AS [GameMaxDurationStr]
                FROM [GameSettings] 
                /**where**/");

            if (gameId.HasValue)
            {
                builder.Where("[GameSettings].[Id] = @Id", new { Id = gameId.Value });
            }

            var gameSettings = await conn.QueryAsync<GameSettings>(
                new CommandDefinition(
                    template.RawSql,
                    template.Parameters,
                    cancellationToken: cancellationToken
                )
            );

            return gameSettings;
        }

        public async Task UpdateGameSettingAsync(GameSettings gameSettings, CancellationToken cancellationToken)
        {
            var connStr = ConnectionStrings.CurrentValue;

            if (string.IsNullOrWhiteSpace(connStr.SQLite)) return;

            using var conn = new SqliteConnection(connStr.SQLite);

            await conn.OpenAsync(cancellationToken);

            await conn.ExecuteAsync(
                new CommandDefinition(
                    @"UPDATE [GameSettings]
                    SET
                        ComplexityLevel = @ComplexityLevel,
                        NoOfRows = @NoOfRows,
                        NoOfColumns = @NoOfColumns,
                        NoOfSwitchedOnLights = @NoOfSwitchedOnLights,
                        GameMaxDuration = @GameMaxDuration
                    WHERE [Id] = @Id",
                    gameSettings,
                    cancellationToken: cancellationToken
                )
            );
        }

        public async Task<ushort> AddGameSettingAsync(GameSettings gameSettings, CancellationToken cancellationToken)
        {
            var connStr = ConnectionStrings.CurrentValue;

            if (string.IsNullOrWhiteSpace(connStr.SQLite)) throw new ApplicationException("Conn str not setup");

            using var conn = new SqliteConnection(connStr.SQLite);

            await conn.OpenAsync(cancellationToken);

            return await conn.ExecuteScalarAsync<ushort>(
                new CommandDefinition(
                    @"INSERT INTO [GameSettings]
                    (
                        ComplexityLevel,
                        NoOfRows,
                        NoOfColumns,
                        NoOfSwitchedOnLights,
                        GameMaxDuration
                    )
                    VALUES
                    (
                        @ComplexityLevel,
                        @NoOfRows,
                        @NoOfColumns,
                        @NoOfSwitchedOnLights,
                        @GameMaxDuration
                    )
                    RETURNING [Id]",
                    gameSettings,
                    cancellationToken: cancellationToken
                )
            );
        }

        public async Task DeleteGameSettingAsync(ushort gameId, CancellationToken cancellationToken)
        {
            var connStr = ConnectionStrings.CurrentValue;

            if (string.IsNullOrWhiteSpace(connStr.SQLite)) return;

            using var conn = new SqliteConnection(connStr.SQLite);

            await conn.OpenAsync(cancellationToken);

            var gameSettings = await conn.ExecuteAsync(
                new CommandDefinition(
                    "DELETE FROM [GameSettings] WHERE Id = @Id",
                    new { Id = gameId },
                    cancellationToken: cancellationToken
                )
            );
        }
    }
}
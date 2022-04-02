using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using FluentMigrator.Runner;
using LightsOut.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LightsOut.Web
{
    public class DatabaseMigratorBootstrapper : IBootstrapper
    {
        private ILogger<DatabaseMigratorBootstrapper> Logger { get; }
        private IServiceScopeFactory ServiceScopeFactory { get; }
        public DatabaseMigratorBootstrapper(ILogger<DatabaseMigratorBootstrapper> logger, IServiceScopeFactory serviceScopeFactory)
        {
            Logger = logger;
            ServiceScopeFactory = serviceScopeFactory;
        }

        public async Task BootstrapAsync(CancellationToken cancellationToken)
        {
            using var scope = ServiceScopeFactory.CreateScope();

            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

            var connectionStrings = scope.ServiceProvider.GetRequiredService<IOptionsMonitor<ConnectionStringOptions>>();

            if (string.IsNullOrEmpty(connectionStrings.CurrentValue.SQLite))
            {
                Logger.LogError("Connection string is not setup.");

                return;
            }

            runner.ListMigrations();

            using var conn = new SqliteConnection(connectionStrings.CurrentValue.SQLite);

            await conn.OpenAsync(cancellationToken);

            var versionsInfo = await conn.QueryAsync(new CommandDefinition("SELECT * FROM [VersionInfo]", cancellationToken: cancellationToken));

            var appliedVersions = new HashSet<long>(versionsInfo.Select(x => (long)x.Version));

            foreach (var migration in runner.MigrationLoader.LoadMigrations())
            {
                // this migration is already applied
                if (appliedVersions.Contains(migration.Key)) continue;

                Logger.LogInformation("Applying migration version: {versionId}", migration.Key);

                runner.MigrateDown(migration.Key);

                runner.MigrateUp(migration.Key);
            }
        }
    }
}
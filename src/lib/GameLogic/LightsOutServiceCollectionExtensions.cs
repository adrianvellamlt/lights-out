using LightsOut.GameLogic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LightsOutServiceCollectionExtensions
    {
        public static IServiceCollection AddLightsOut(this IServiceCollection services)
        {
            services.AddScoped<IGameSettingsService, GameSettingsService>();

            services.AddScoped<IGameStateService, GameStateService>();

            return services;
        }
    }
}
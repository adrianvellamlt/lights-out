using System;

namespace LightsOut.GameLogic
{
    public record GameState(int Id, DateTime StartTimeUtc, LightsOut Game, DateTime? SurrenderedAtUtc);
}
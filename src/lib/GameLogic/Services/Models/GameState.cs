using System;

namespace LightsOut.GameLogic
{
    public record GameState(Guid Id, ushort GameId, DateTime StartTimeUtc, LightsOut Game)
    {
        public DateTime? SurrenderedAtUTC { get; private set; }
        public DateTime? CompletedAtUTC { get; private set; }

        public ushort NoOfMoves { get; private set; }

        public void SetSurrenderedTimeStamp(DateTime timestamp) => SurrenderedAtUTC = timestamp;
        public void SetCompletedTimeStamp(DateTime timestamp) => CompletedAtUTC = timestamp;
        public void IncrementMoveCounter() => NoOfMoves++;
    };
}
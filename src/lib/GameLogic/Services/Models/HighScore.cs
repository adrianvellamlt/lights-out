using System;

namespace LightsOut.GameLogic
{
    public class HighScore
    {
        public Guid GameStateId { get; set; }
        public string  Username { get; set; } = string.Empty;
        public byte ComplexityLevel { get; set; }
        public byte NoOfRows { get; set; }
        public byte NoOfColumns { get; set; }
        public byte RemainingLights { get; set; }
        public string TimeTakenStr { get; set; } = "00:00:00";
        public TimeSpan TimeTaken
        {
            get => TimeSpan.Parse(TimeTakenStr);
            set => TimeTakenStr = value.ToString();
        }
        public ushort NoOfMoves { get; set; }
    }
}
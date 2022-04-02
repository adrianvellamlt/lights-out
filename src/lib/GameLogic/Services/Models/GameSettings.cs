using System;

namespace LightsOut.GameLogic
{
    public class GameSettings
    {
        public ushort Id { get; set; }
        public byte ComplexityLevel { get; set; }
        public byte NoOfRows { get; set; }
        public byte NoOfColumns { get; set; }
        public byte NoOfSwitchedOnLights { get; set; }
        public string GameMaxDurationStr { get; set; } = "00:30:00";
        public TimeSpan GameMaxDuration
        {
            get => TimeSpan.Parse(GameMaxDurationStr);
            set => GameMaxDurationStr = value.ToString();
        }
    }
}
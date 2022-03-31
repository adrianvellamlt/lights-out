using System;

namespace LightsOut.GameLogic
{
    public class GameSettings
    {
        public byte NoOfRows { get; set; }
        public byte NoOfColumns { get; set; }
        public byte NoOfSwitchedOnLights { get; set; }
        public TimeSpan? GameMaxDuration { get; set; } = TimeSpan.FromHours(4);
    }
}
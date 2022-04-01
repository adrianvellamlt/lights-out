using System;

namespace LightsOut.Infrastructure
{
    public interface ISystemClock
    {
        DateTimeOffset UtcNow { get; }
        DateTimeOffset Now { get; }
    }

    public class RealSystemClock : ISystemClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

        public DateTimeOffset Now => DateTimeOffset.Now;
    }
}
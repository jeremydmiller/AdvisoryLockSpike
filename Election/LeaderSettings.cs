using System;
using Baseline.Dates;
using Timer = System.Timers.Timer;

namespace AdvisoryLockSpike.Election
{
    public class LeaderSettings<T> where T : IActiveProcess
    {
        public TimeSpan OwnershipPollingTime { get; set; } = 5.Seconds();
        
        // It's a random number here so that if you spin
        // up multiple nodes at the same time, they won't
        // all collide trying to grab ownership at the exact
        // same time
        public TimeSpan FirstPollingTime { get; set; } 
            = new Random().Next(100, 3000).Milliseconds();
        
        // This would be something meaningful
        public int LockId { get; set; }
        
        public string ConnectionString { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.Data
{
    record BallLogData
    {
        public int BallId { get; set; }
        public IVector Position { get; set; } = IVector.Zero;
        public IVector Velocity { get; set; } = IVector.Zero;
        public DateTime Timestamp { get; set; }
    }
}

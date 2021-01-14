using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngineLib.Engine {
    public static class EngineTimer {
        public static double RunTime { get; internal set; }
        public static double LastCycleTime { get; internal set; }
    }
}

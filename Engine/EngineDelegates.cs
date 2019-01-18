using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngineLib.Engine
{
    public static class EngineDelegates
    {
        public delegate void OnStart();
        public delegate void OnUpdate();
        public delegate void OnGUIRender();
    }
}

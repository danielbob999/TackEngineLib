using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngineLib.Engine
{
    public abstract class EngineModule
    {
        internal abstract void Start();
        internal abstract void Update();
        internal abstract void Render();
        internal abstract void Close();
    }
}

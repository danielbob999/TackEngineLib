using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;
using TackEngineLib.Engine;

namespace TackEngineLib.Objects.Components
{
    public class Light : TackComponent
    {
        public Sprite lightSprite;
        private Colour4b m_Colour;
        private float m_Intensity; // Intensity (or transparency) of the light. Ranged from 0 to 1

        // Properties
        public Colour4b Colour { get { return m_Colour; } set { m_Colour = value; } }
        public float Intensity
        {
            get { return m_Intensity; }
            set
            {
                if (value < 0)
                {
                    m_Intensity = 0;
                    TackConsole.EngineLog(EngineLogType.Error, "Cannot set Light intensity to less than 0. Intensity has been reset 0");
                } else if (value > 1)
                {
                    m_Intensity = 1;
                    TackConsole.EngineLog(EngineLogType.Error, "Cannot set Light intensity to more than 1. Intensity has been reset 1");
                }
                else
                {
                    m_Intensity = value;
                }
            }
        }

        public override void OnStart()
        {
            base.OnStart();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnRender()
        {
            base.OnRender();
        }
    }
}

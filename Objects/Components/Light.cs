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
        private Colour4b mColour;
        private float mIntensity; // Intensity (or transparency) of the light. Ranged from 0 to 1

        // Properties
        public Colour4b Colour { get { return mColour; } set { mColour = value; } }
        public float Intensity
        {
            get { return mIntensity; }
            set
            {
                if (value < 0)
                {
                    mIntensity = 0;
                    TackConsole.EngineLog(EngineLogType.Error, "Cannot set Light intensity to less than 0. Intensity has been reset 0");
                } else if (value > 1)
                {
                    mIntensity = 1;
                    TackConsole.EngineLog(EngineLogType.Error, "Cannot set Light intensity to more than 1. Intensity has been reset 1");
                }
                else
                {
                    mIntensity = value;
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

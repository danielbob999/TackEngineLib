/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngineLib.Main
{
    /// <summary>
    /// The class that sets global world information for the engine
    /// </summary>
    public static class World
    {
        public static Colour4b AmbientLightColour = Colour4b.Blue;
        public static float AmbientLightIntensity = 0.5f;

        internal static Colour4b GetAmbientColour() {
            byte intensityValue = (byte)(AmbientLightIntensity * 255);
            return new Colour4b(AmbientLightColour.R, AmbientLightColour.G, AmbientLightColour.B, intensityValue);
        }

        
    }
}

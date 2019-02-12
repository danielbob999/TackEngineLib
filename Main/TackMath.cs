using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngineLib.Main
{
    public static class TackMath
    {
        public static int AbsVali(int _val)
        {
            if (_val > 0)
                return _val;
            else
                return _val * -1;
        }

        public static float AbsValf(float _val)
        {
            if (_val > 0)
                return _val;
            else
                return _val * -1.0f;
        }

        /// <summary>
        /// Converts an angle in degrees to and angle in radians
        /// </summary>
        /// <param name="_angle">An angle in degrees</param>
        /// <returns>The converted angle in radians</returns>
        public static double DegToRad(float _angle)
        {
            return _angle * (Math.PI / 180);
        }
    }
}

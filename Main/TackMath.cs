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
    }
}

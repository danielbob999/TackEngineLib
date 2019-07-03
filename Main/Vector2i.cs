using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngineLib.Main
{
    public class Vector2i
    {
        private int mX;
        private int mY;

        public int X
        {
            get { return mX; }
            set { mX = value; }
        }

        public int Y
        {
            get { return mY; }
            set { mY = value; }
        }

        public Vector2i()
        {
            mX = 0;
            mY = 0;
        }

        public Vector2i(int _x, int _y)
        {
            mX = _x;
            mY = _y;
        }

        public static Vector2i operator +(Vector2i _a, Vector2i _b)
        {
            return new Vector2i(_a.mX + _b.mX, _a.mY + _b.mY);
        }

        public static Vector2i operator -(Vector2i _a, Vector2i _b)
        {
            return new Vector2i(_b.mX - _a.mX, _b.mY - _a.mY);
        }

        public static Vector2i operator *(Vector2i _a, Vector2i _b)
        {
            return new Vector2i(_a.mX * _b.mX, _a.mY * _b.mY);
        }

        public static bool operator ==(Vector2i _a, Vector2i _b)
        {
            if (_a.mX == _b.mX)
            {
                if (_a.mY == _b.mY)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool operator !=(Vector2i _a, Vector2i _b)
        {
            if (_a.mX == _b.mX)
            {
                if (_a.mY == _b.mY)
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            string returnStr = "(" + mX + ", " + mY + ")";
            return returnStr;
        }
    }
}

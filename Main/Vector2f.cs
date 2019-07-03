/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngineLib.Main
{
    /// <summary>
    /// 
    /// </summary>
    public struct Vector2f
    {
        private float mX;
        private float mY;

        public float X
        {
            get { return mX; }
            set { mX = value; }
        }

        public float Y
        {
            get { return mY; }
            set { mY = value; }
        }

        public Vector2f(float _x, float _y)
        {
            mX = _x;
            mY = _y;
        }

        public static Vector2f operator+ (Vector2f _a, Vector2f _b)
        {
            return new Vector2f(_a.mX + _b.mX, _a.mY + _b.mY);
        }

        public static Vector2f operator-(Vector2f _a, Vector2f _b)
        {
            return new Vector2f(_b.mX - _a.mX, _b.mY - _a.mY);
        }

        public static Vector2f operator* (Vector2f _a, Vector2f _b)
        {
            return new Vector2f(_a.mX * _b.mX, _a.mY * _b.mY);
        }

        public static bool operator== (Vector2f _a, Vector2f _b)
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

        public static bool operator!= (Vector2f _a, Vector2f _b)
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

        public static Vector2f operator- (Vector2f _a)
        {
            return new Vector2f(_a.X * -1.0f, _a.Y * -1.0f);
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

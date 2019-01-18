using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngineLib.Main
{
    public class Vector2f
    {
        private float m_X;
        private float m_Y;

        public float X
        {
            get { return m_X; }
            set { m_X = value; }
        }

        public float Y
        {
            get { return m_Y; }
            set { m_Y = value; }
        }

        public Vector2f()
        {
            m_X = 0;
            m_Y = 0;
        }

        public Vector2f(float _x, float _y)
        {
            m_X = _x;
            m_Y = _y;
        }

        public static Vector2f operator+ (Vector2f _a, Vector2f _b)
        {
            return new Vector2f(_a.m_X + _b.m_X, _a.m_Y + _b.m_Y);
        }

        public static Vector2f operator-(Vector2f _a, Vector2f _b)
        {
            return new Vector2f(_b.m_X - _a.m_X, _b.m_Y - _a.m_Y);
        }

        public static Vector2f operator* (Vector2f _a, Vector2f _b)
        {
            return new Vector2f(_a.m_X * _b.m_X, _a.m_Y * _b.m_Y);
        }

        public static bool operator== (Vector2f _a, Vector2f _b)
        {
            if (_a.m_X == _b.m_X)
            {
                if (_a.m_Y == _b.m_Y)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool operator!= (Vector2f _a, Vector2f _b)
        {
            if (_a.m_X == _b.m_X)
            {
                if (_a.m_Y == _b.m_Y)
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
            string returnStr = "(" + m_X + ", " + m_Y + ")";
            return returnStr;
        }
    }
}

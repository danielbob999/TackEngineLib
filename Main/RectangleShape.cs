using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngineLib.Main
{
    public class RectangleShape
    {
        private float m_X;
        private float m_Y;
        private float m_Width;
        private float m_Height;

        // Properties
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

        public float Width
        {
            get { return m_Width; }
            set { m_Width = value; }
        }

        public float Height
        {
            get { return m_Height; }
            set { m_Height = value; }
        }

        public RectangleShape()
        {
            X = 0;
            Y = 0;
            Width = 0;
            Height = 0;
        }

        public RectangleShape(float _x, float _y, float _w, float _h)
        {
            X = _x;
            Y = _y;
            Width = _w;
            Height = _h;
        }

        public override string ToString()
        {
            return (string.Format("({0}, {1}, {2}, {3})", m_X, m_Y, m_Width, m_Height));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;

namespace TackEngineLib.GUI
{
    public class GUIBorder
    {
        private int m_Left;
        private int m_Right;
        private int m_Up;
        private int m_Bottom;

        public int Left
        {
            get { return m_Left; }
            set { m_Left = value; }
        }

        public int Right
        {
            get { return m_Right; }
            set { m_Right = value; }
        }

        public int Up
        {
            get { return m_Up; }
            set { m_Up = value; }
        }

        public int Bottom
        {
            get { return m_Bottom; }
            set { m_Bottom = value; }
        }

        public GUIBorder(int _left, int _right, int _up, int _bottom)
        {
            m_Left = _left;
            m_Right = _right;
            m_Up = _up;
            m_Bottom = _bottom;
        }
    }
}

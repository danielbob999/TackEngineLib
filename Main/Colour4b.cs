using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngineLib.Main
{
    public class Colour4b
    {
        private byte m_R;
        private byte m_G;
        private byte m_B;
        private byte m_A;

        public byte R
        {
            get { return m_R; }
            set { m_R = value; }
        }

        public byte G
        {
            get { return m_G; }
            set { m_G = value; }
        }

        public byte B
        {
            get { return m_B; }
            set { m_B = value; }
        }

        public byte A
        {
            get { return m_A; }
            set { m_A = value; }
        }

        public Colour4b()
        {
            m_R = 0;
            m_G = 0;
            m_B = 0;
            m_A = 255;
        }

        public Colour4b(byte _r, byte _g, byte _b)
        {
            m_R = _r;
            m_G = _g;
            m_B = _b;
            m_A = 255;
        }

        public Colour4b(byte _r, byte _g, byte _b, byte _a)
        {
            m_R = _r;
            m_G = _g;
            m_B = _b;
            m_A = _a;
        }

        public static Colour4b operator* (Colour4b _a, Colour4b _b)
        {
            int m_R = _a.m_R * (_b.m_R / 255);
            int m_G = _a.m_G * (_b.m_G / 255);
            int m_B = _a.m_B * (_b.m_B / 255);
            int m_A = _a.m_A * (_b.m_A / 255);

            return new Colour4b((byte)m_R, (byte)m_G, (byte)m_B, (byte)m_A);
        }
    }
}

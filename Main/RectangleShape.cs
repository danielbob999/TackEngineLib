/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngineLib.Main
{
    public class RectangleShape
    {
        private float mX;
        private float mY;
        private float mWidth;
        private float mHeight;

        // Properties
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

        public float Width
        {
            get { return mWidth; }
            set { mWidth = value; }
        }

        public float Height
        {
            get { return mHeight; }
            set { mHeight = value; }
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
            return (string.Format("({0}, {1}, {2}, {3})", mX, mY, mWidth, mHeight));
        }
    }
}

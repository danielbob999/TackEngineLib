/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;

using TackEngineLib.GUI;
using TackEngineLib.Main;

namespace TackEngineLib.GUI
{
    public class BoxStyle
    {
        private Colour4b mColour;
        private Sprite mSprite;
        private GUIBorder mBorder;

        public Colour4b Colour
        {
            get { return mColour; }
            set { mColour = value; }
        }

        public Sprite SpriteTexture
        {
            get { return mSprite; }
            set
            {

                mSprite = value;
            }
        }

        public GUIBorder Border
        {
            get { return mBorder; }
            set { mBorder = value; }
        }

        public BoxStyle()
        {
            mColour = new Colour4b(255, 255, 255, 255);
            mSprite = null;

            mBorder = null;
        }

        public void Destory()
        {
        }
    }
}

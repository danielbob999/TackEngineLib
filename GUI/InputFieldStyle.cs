/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;

namespace TackEngineLib.GUI
{
    public class InputFieldStyle
    {
        private float mFontSize;
        private Colour4b mFontColour;
        private int mFontFamilyId;
        private VerticalAlignment mVerticalAlignment;
        private HorizontalAlignment mHorizontalAlignment;
        private Colour4b mColour;
        private GUIBorder mBorder;
        private Sprite mSpriteTexture;
        private bool mScrollable;
        private uint mCaretPosition;

        public float FontSize
        {
            get { return mFontSize; }
            set { mFontSize = value; }
        }

        public Colour4b FontColour
        {
            get { return mFontColour; }
            set { mFontColour = value; }
        }

        public Colour4b BackgroundColour
        {
            get { return mColour; }
            set { mColour = value; }
        }

        public GUIBorder Border
        {
            get { return mBorder; }
            set { mBorder = value; }
        }

        public Sprite SpriteTexture
        {
            get { return mSpriteTexture; }
            set
            {
                mSpriteTexture = value;
            }
        }

        public int FontFamilyId
        {
            get { return mFontFamilyId; }
            set { mFontFamilyId = value; }
        }

        public bool Scrollable
        {
            get { return mScrollable; }
            set { mScrollable = value; }
        }

        public uint CaretPosition {
            get { return mCaretPosition; }
            set { mCaretPosition = value; }
        }

        /// <summary>
        /// The VerticalAlignment of this text
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get { return mVerticalAlignment; }
            set { mVerticalAlignment = value; }
        }

        /// <summary>
        /// The HorizontalAlignment of this text
        /// </summary>
        public HorizontalAlignment HorizontalAlignment
        {
            get { return mHorizontalAlignment; }
            set { mHorizontalAlignment = value; }
        }

        public InputFieldStyle()
        {
            mSpriteTexture = Sprite.DefaultSprite;

            mFontSize = 6f;
            mFontColour = new Colour4b(0, 0, 0, 255);
            mColour = new Colour4b(255, 255, 255, 255);
            mBorder = new GUIBorder(0, 0, 0, 0, new Colour4b(0, 0, 0, 255));
            mCaretPosition = 0;
        }

        public BoxStyle GetBoxStyle()
        {
            BoxStyle style = new BoxStyle()
            {
                Border = mBorder,
                Colour = mColour,
                SpriteTexture = mSpriteTexture,
            };

            return style;
        }

        public TextAreaStyle GetTextStyle()
        {
            TextAreaStyle style = new TextAreaStyle() {
                BackgroundColour = mColour,
                FontColour = mFontColour,
                FontFamilyId = mFontFamilyId,
                FontSize = mFontSize,
                SpriteTexture = mSpriteTexture,
                HorizontalAlignment = mHorizontalAlignment,
                VerticalAlignment = mVerticalAlignment,
                Scrollable = mScrollable
            };

            return style;
        }

        public void Destory() {
        }
    }
}

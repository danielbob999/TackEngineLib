using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;

namespace TackEngineLib.GUI
{
    public class TextAreaStyle
    {
        private float mFontSize;
        private int mFontFamily;
        private VerticalAlignment mVerticalAlignment;
        private HorizontalAlignment mHorizontalAlignment;
        private Colour4b mFontColour;
        private Colour4b mBackgroundColour;
        private Sprite mSpriteTexture;
        private float mScrollPosition = 0.0f;
        private bool mScrollable;

        public float FontSize
        {
            get { return mFontSize; }
            set
            {
                if (value >= 0)
                    mFontSize = value;
                else
                    TackConsole.EngineLog(Engine.EngineLogType.Message, "Cannot set TextAreaStyle.FontSize to less than or equal to 0.0f");
            }
        }

        public int FontFamilyId
        {
            get { return mFontFamily; }
            set { mFontFamily = value; }
        }

        public Colour4b FontColour
        {
            get { return mFontColour; }
            set { mFontColour = value; }
        }

        public Colour4b BackgroundColour
        {
            get { return mBackgroundColour; }
            set { mBackgroundColour = value; }
        }

        public Sprite SpriteTexture
        {
            get { return mSpriteTexture; }
            set
            {
                mSpriteTexture = value;
            }
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

        /// <summary>
        /// The scroll position of the text box
        /// </summary>
        public float ScrollPosition
        {
            get { return mScrollPosition; }
            set { mScrollPosition = value; }
        }

        /// <summary>
        /// Whether the textbox can be vertically scrolled
        /// </summary>
        public bool Scrollable
        {
            get { return mScrollable; }
            set { mScrollable = value; }
        }

        public TextAreaStyle()
        {
            FontSize = 8f;
            mFontFamily = 0;
            mFontColour = new Colour4b(0, 0, 0, 255);
            mSpriteTexture = Sprite.DefaultSprite;
            mHorizontalAlignment = HorizontalAlignment.Left;
            mVerticalAlignment = VerticalAlignment.Top;
        }

        public TextAreaStyle(float _size, int _family, Colour4b _textColour, Colour4b _backColour, Sprite _sprite)
        {
            FontSize = _size;
            mFontFamily = _family;
            mFontColour = _textColour;
            mBackgroundColour = _backColour;
            mSpriteTexture = _sprite;
        }

        public void Destory()
        {
        }
    }
}

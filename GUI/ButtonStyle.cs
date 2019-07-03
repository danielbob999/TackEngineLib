using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;

namespace TackEngineLib.GUI
{
    public class ButtonStyle
    {
        private float mFontSize;
        private Colour4b mFontColour;
        private int mFontFamilyId;
        private VerticalAlignment mVerticalAlignment;
        private HorizontalAlignment mHorizontalAlignment;
        private Colour4b mColour;
        private GUIBorder mBorder;
        private Sprite mSpriteTexture;
        private RectangleShape mShape;

        public RectangleShape Shape
        {
            get { return mShape; }
            set { mShape = value; }
        }

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
            set {
                mSpriteTexture = value;
            }
        }

        public int FontFamilyId
        {
            get { return mFontFamilyId; }
            set { mFontFamilyId = value; }
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

        public ButtonStyle() {
            mSpriteTexture = Sprite.DefaultSprite;

            mFontSize = 6f;
            mFontColour = new Colour4b(0, 0, 0, 255);
            mColour = new Colour4b(255, 255, 255, 255);
            mBorder = new GUIBorder(0, 0, 0, 0, new Colour4b(0, 0, 0, 255));
        }

        internal TextAreaStyle GetTextAreaStyle() {
            TextAreaStyle style = new TextAreaStyle() {
                FontSize = mFontSize,
                BackgroundColour = mColour,
                FontColour = mFontColour,
                FontFamilyId = mFontFamilyId,
                 HorizontalAlignment = mHorizontalAlignment,
                 VerticalAlignment = mVerticalAlignment,
                 Scrollable = false,
                 ScrollPosition = 0.0f,
                 SpriteTexture = mSpriteTexture
            };

            return style;
        }

        internal BoxStyle GetBoxStyle() {
            BoxStyle style = new BoxStyle() {
                Colour = mColour,
                Border = mBorder,
                SpriteTexture = mSpriteTexture
            };

            return style;
        }
    }
}

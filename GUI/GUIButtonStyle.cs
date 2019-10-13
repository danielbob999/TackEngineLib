/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;

namespace TackEngineLib.GUI
{
    public class GUIButtonStyle
    {
        /// <summary>
        /// The default style of a GUIButton
        /// </summary>
        public static GUIButtonStyle DefaultStyle
        {
            get {
                return new GUIButtonStyle() {
                    BackgroundColour = new Colour4b(200, 200, 200, 255),
                    FontColour = Colour4b.Black,
                    FontFamilyId = 0,
                    FontSize = 7.0f,
                    Border = new GUIBorder(0, 0, 0, 0, new Colour4b(0, 0, 0, 255)),
                    HorizontalTextAlignment = HorizontalAlignment.Middle,
                    VerticalTextAlignment = VerticalAlignment.Middle,
                    SpriteTexture = Sprite.DefaultSprite
                };
            }
        }

        /// <summary>
        /// The default style of a GUIButton when hovered
        /// </summary>
        public static GUIButtonStyle DefaultHoverStyle
        {
            get {
                return new GUIButtonStyle() {
                    BackgroundColour = new Colour4b(190, 190, 190, 255),
                    FontColour = Colour4b.Black,
                    FontFamilyId = 0,
                    FontSize = 7.0f,
                    Border = new GUIBorder(0, 0, 0, 0, new Colour4b(0, 0, 0, 255)),
                    HorizontalTextAlignment = HorizontalAlignment.Middle,
                    VerticalTextAlignment = VerticalAlignment.Middle,
                    SpriteTexture = Sprite.DefaultSprite
                };
            }
        }

        /// <summary>
        /// The default style of a GUIButton when being pressed
        /// </summary>
        public static GUIButtonStyle DefaultPressStyle
        {
            get {
                return new GUIButtonStyle() {
                    BackgroundColour = new Colour4b(170, 170, 170, 255),
                    FontColour = Colour4b.Black,
                    FontFamilyId = 0,
                    FontSize = 7.0f,
                    Border = new GUIBorder(0, 0, 0, 0, new Colour4b(0, 0, 0, 255)),
                    HorizontalTextAlignment = HorizontalAlignment.Middle,
                    VerticalTextAlignment = VerticalAlignment.Middle,
                    SpriteTexture = Sprite.DefaultSprite
                };
            }
        }

        private float mFontSize;
        private Colour4b mFontColour;
        private int mFontFamilyId;
        private VerticalAlignment mVerticalAlignment;
        private HorizontalAlignment mHorizontalAlignment;
        private Colour4b mColour;
        private GUIBorder mBorder;
        private Sprite mSpriteTexture;

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
        public VerticalAlignment VerticalTextAlignment
        {
            get { return mVerticalAlignment; }
            set { mVerticalAlignment = value; }
        }

        /// <summary>
        /// The HorizontalAlignment of this text
        /// </summary>
        public HorizontalAlignment HorizontalTextAlignment
        {
            get { return mHorizontalAlignment; }
            set { mHorizontalAlignment = value; }
        }

        public GUIButtonStyle() {
            mSpriteTexture = Sprite.DefaultSprite;
            mHorizontalAlignment = HorizontalAlignment.Middle;
            mVerticalAlignment = VerticalAlignment.Middle;

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

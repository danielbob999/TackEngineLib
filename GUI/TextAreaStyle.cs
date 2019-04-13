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
        private float m_FontSize;
        private int m_FontFamily;
        private VerticalAlignment m_VerticalAlignment;
        private HorizontalAlignment m_HorizontalAlignment;
        private Colour4b m_FontColour;
        private Colour4b m_BackgroundColour;
        private Sprite m_SpriteTexture;
        private float m_ScrollPosition = 0.0f;
        private bool m_Scrollable;

        public float FontSize
        {
            get { return m_FontSize; }
            set
            {
                if (value >= 0)
                    m_FontSize = value;
                else
                    TackConsole.EngineLog(Engine.EngineLogType.Message, "Cannot set TextAreaStyle.FontSize to less than or equal to 0.0f");
            }
        }

        public int FontFamilyId
        {
            get { return m_FontFamily; }
            set { m_FontFamily = value; }
        }

        public Colour4b FontColour
        {
            get { return m_FontColour; }
            set { m_FontColour = value; }
        }

        public Colour4b BackgroundColour
        {
            get { return m_BackgroundColour; }
            set { m_BackgroundColour = value; }
        }

        public Sprite SpriteTexture
        {
            get { return m_SpriteTexture; }
            set
            {
                m_SpriteTexture = value;
            }
        }

        /// <summary>
        /// The VerticalAlignment of this text
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get { return m_VerticalAlignment; }
            set { m_VerticalAlignment = value; }
        }

        /// <summary>
        /// The HorizontalAlignment of this text
        /// </summary>
        public HorizontalAlignment HorizontalAlignment
        {
            get { return m_HorizontalAlignment; }
            set { m_HorizontalAlignment = value; }
        }

        /// <summary>
        /// The scroll position of the text box
        /// </summary>
        public float ScrollPosition
        {
            get { return m_ScrollPosition; }
            set { m_ScrollPosition = value; }
        }

        /// <summary>
        /// Whether the textbox can be vertically scrolled
        /// </summary>
        public bool Scrollable
        {
            get { return m_Scrollable; }
            set { m_Scrollable = value; }
        }

        public TextAreaStyle()
        {
            FontSize = 8f;
            m_FontFamily = 0;
            m_FontColour = new Colour4b(0, 0, 0, 255);
            m_SpriteTexture = Sprite.DefaultSprite;
            m_HorizontalAlignment = HorizontalAlignment.Left;
            m_VerticalAlignment = VerticalAlignment.Top;
        }

        public TextAreaStyle(float _size, int _family, Colour4b _textColour, Colour4b _backColour, Sprite _sprite)
        {
            FontSize = _size;
            m_FontFamily = _family;
            m_FontColour = _textColour;
            m_BackgroundColour = _backColour;
            m_SpriteTexture = _sprite;
        }

        public void Destory()
        {
        }
    }
}

using System;

using TackEngineLib.GUI;
using TackEngineLib.Main;

namespace TackEngineLib.GUI
{
    public class BoxStyle
    {
        private Colour4b m_Colour;
        private Sprite m_Sprite;
        private GUIBorder m_Border;

        public Colour4b Colour
        {
            get { return m_Colour; }
            set { m_Colour = value; }
        }

        public Sprite SpriteTexture
        {
            get { return m_Sprite; }
            set
            {
                m_Sprite.Destory(false);

                m_Sprite = value;
            }
        }

        public GUIBorder Border
        {
            get { return m_Border; }
            set { m_Border = value; }
        }

        public BoxStyle()
        {
            m_Colour = new Colour4b(255, 255, 255, 255);
            m_Sprite = Sprite.LoadFromBitmap(TackEngineLib.Properties.Resources.DefaultSprite);
            m_Sprite.Create(false);

            m_Border = null;
        }

        public void Destory()
        {
            m_Sprite.Destory(false);
        }
    }
}

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
        private float m_FontSize;
        private Colour4b m_FontColour;
        private Colour4b m_Colour;
        private GUIBorder m_Border;
        private Sprite m_SpriteTexture;

        public InputFieldStyle()
        {
            m_SpriteTexture = Sprite.LoadFromBitmap(TackEngineLib.Properties.Resources.DefaultSprite);
            m_SpriteTexture.Create();

            m_FontSize = 6f;
            m_FontColour = new Colour4b(0, 0, 0, 255);
            m_Colour = new Colour4b(255, 255, 255, 255);
            m_Border = new GUIBorder(0, 0, 0, 0, new Colour4b(0, 0, 0, 255));
        }

        public BoxStyle GetBoxStyle()
        {
            BoxStyle style = new BoxStyle()
            {
                Border = m_Border,
                Colour = m_Colour,
                SpriteTexture = m_SpriteTexture,
            };

            return style;
        }



    }
}

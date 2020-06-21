using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;

namespace TackEngineLib.GUI {
    public class GUIBox : GUIObject {

        /// <summary>
        /// The style of a GUIBox object
        /// </summary>
        public class GUIBoxStyle {

            public Colour4b Colour { get; set; }
            public Sprite Texture { get; set; }
            public GUIBorder Border { get; set; }

            public GUIBoxStyle() {
                Colour = Colour4b.White;
                Texture = Sprite.DefaultSprite;
                Border = new GUIBorder(0, 0, 0, 0, Colour4b.Black);
            }
        }

        private bool m_hovering = false;

        public RectangleShape Bounds { get; set; }
        public GUIBoxStyle NormalStyle { get; set; }
        public GUIBoxStyle HoverStyle { get; set; }

        public GUIBox() {
            Bounds = new RectangleShape(5, 5, 300, 35);
            NormalStyle = new GUIBoxStyle();
            HoverStyle = new GUIBoxStyle();

            TackGUI.RegisterGUIObject(this);
        }

        internal override void OnStart() {

        }

        internal override void OnUpdate() {
            Vector2f mousePosition = Input.TackInput.MousePosition();

            if (mousePosition.X > Bounds.X && mousePosition.X < (Bounds.X + Bounds.Width)) {
                if (mousePosition.Y > Bounds.Y && mousePosition.Y < (Bounds.Y + Bounds.Height)) {
                    m_hovering = true;

                } else {
                    m_hovering = false;
                }
            } else {
                m_hovering = false;
            }
        }

        internal override void OnRender() {
            if (m_hovering) {
                TackGUI.InternalBox(Bounds, HoverStyle);
            } else {
                TackGUI.InternalBox(Bounds, NormalStyle);
            }
        }

        internal override void OnClose() {
            
        }
    }
}

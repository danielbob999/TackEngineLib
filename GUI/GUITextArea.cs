using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;

namespace TackEngineLib.GUI {
    public class GUITextArea : GUIObject {

        /// <summary>
        /// The style of a GUITextArea object
        /// </summary>
        public class GUITextAreaStyle {

            public Colour4b Colour { get; set; }
            public GUIBorder Border { get; set; }
            public int FontFamilyId { get; set; }
            public float FontSize { get; set; }
            public Colour4b FontColour { get; set; }
            public HorizontalAlignment HorizontalAlignment { get; set; }
            public VerticalAlignment VerticalAlignment { get; set; }
            public Sprite Texture { get; set; }
            public float ScrollPosition { get; set; }
            public bool Scrollable { get; set; }

            public GUITextAreaStyle() {
                Colour = Colour4b.White;
                Border = new GUIBorder(0, 0, 0, 0, Colour4b.Black);
                FontSize = 8f;
                FontFamilyId = 0;
                FontColour = Colour4b.Black;
                HorizontalAlignment = HorizontalAlignment.Left;
                VerticalAlignment = VerticalAlignment.Top;
                Texture = Sprite.DefaultSprite;
                ScrollPosition = 0.0f;
                Scrollable = false;
            }
        }

        private bool m_hovering = false;

        public string Text { get; set; }
        public RectangleShape Bounds { get; set; }
        public GUITextAreaStyle NormalStyle { get; set; }
        public GUITextAreaStyle HoverStyle { get; set; }

        public GUITextArea() {
            Text = "";
            NormalStyle = new GUITextAreaStyle();
            HoverStyle = new GUITextAreaStyle();

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
                TackGUI.InternalTextArea(Bounds, Text, HoverStyle);
            } else {
                TackGUI.InternalTextArea(Bounds, Text, NormalStyle);
            }
        }

        internal override void OnClose() {
            
        }
    }
}

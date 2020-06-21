using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;
using TackEngineLib.Input;

namespace TackEngineLib.GUI {
    public class GUIButton : GUIObject {

        /// <summary>
        /// The style of a GUIButton object
        /// </summary>
        public class GUIButtonStyle {

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

            public GUIButtonStyle() {
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

            internal GUITextArea.GUITextAreaStyle ConvertToGUITextAreaStyle() {
                GUITextArea.GUITextAreaStyle style = new GUITextArea.GUITextAreaStyle();
                style.Border = Border;
                style.Colour = Colour;
                style.FontColour = FontColour;
                style.FontFamilyId = FontFamilyId;
                style.FontSize = FontSize;
                style.HorizontalAlignment = HorizontalAlignment;
                style.VerticalAlignment = VerticalAlignment;
                style.Texture = Texture;
                style.Scrollable = Scrollable;
                style.ScrollPosition = ScrollPosition;

                return style;
            }
        }

        private bool m_hovering;
        private bool m_pressing;

        /// <summary>
        /// The regular style of this GUIButton
        /// </summary>
        public GUIButtonStyle NormalStyle { get; set; }

        /// <summary>
        /// The style of this GUIButton when being hovered over
        /// </summary>
        public GUIButtonStyle HoverStyle { get; set; }

        /// <summary>
        /// The style of this GUIButton when it is being pressed
        /// </summary>
        public GUIButtonStyle ClickedStyle { get; set; }

        /// <summary>
        /// The text of this GUIButton
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The shape of this GUIButton
        /// </summary>
        public RectangleShape Bounds { get; set; }

        /// <summary>
        /// The event that is invoked when this GUIButton is pressed 
        /// </summary>
        public event EventHandler OnClickEvent;

        /// <summary>
        /// Intialises a new Button
        /// </summary>
        public GUIButton() {
            Text = "GUIButton";

            NormalStyle = new GUIButtonStyle();

            HoverStyle = new GUIButtonStyle();
            HoverStyle.Colour = new Colour4b(225, 225, 225, 255);

            ClickedStyle = new GUIButtonStyle();
            ClickedStyle.Colour = new Colour4b(195, 195, 195, 255);

            TackGUI.RegisterGUIObject(this);
        }

        internal override void OnStart() {

        }

        internal override void OnUpdate() {
            Vector2f mousePosition = Input.TackInput.MousePosition();

            if (mousePosition.X >= Bounds.X && mousePosition.X <= (Bounds.X + Bounds.Width)) {
                if (mousePosition.Y >= Bounds.Y && mousePosition.Y <= (Bounds.Y + Bounds.Height)) {
                    m_hovering = true;

                    if (TackInput.MouseButtonDown(MouseButtonKey.Left)) {
                        m_pressing = true;

                        if (OnClickEvent != null) {
                            if (OnClickEvent.GetInvocationList().Length > 0) {
                                OnClickEvent.Invoke(this, EventArgs.Empty);
                            }
                        }
                    }

                } else {
                    m_hovering = false;
                }
            } else {
                m_hovering = false;
            }

            if (TackInput.MouseButtonUp(MouseButtonKey.Left)) {
                m_pressing = false;
            }
        }

        internal override void OnRender() {
            if (m_pressing) {
                TackGUI.InternalTextArea(Bounds, Text, ClickedStyle.ConvertToGUITextAreaStyle());
            } else if (m_hovering){
                TackGUI.InternalTextArea(Bounds, Text, HoverStyle.ConvertToGUITextAreaStyle());
            } else {
                TackGUI.InternalTextArea(Bounds, Text, NormalStyle.ConvertToGUITextAreaStyle());
            }
        }

        internal override void OnClose() {

        }
    }
}

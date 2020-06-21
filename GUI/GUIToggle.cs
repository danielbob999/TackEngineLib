using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;
using TackEngineLib.Input;

namespace TackEngineLib.GUI {
    public class GUIToggle : GUIObject {

        /// <summary>
        /// The style of a GUIToggle object
        /// </summary>
        public class GUIToggleStyle {

            public Colour4b Colour { get; set; }
            public Colour4b SelectionColour { get; set; }
            public GUIBorder Border { get; set; }
            public int FontFamilyId { get; set; }
            public float FontSize { get; set; }
            public Colour4b FontColour { get; set; }

            // Static default styles

           // Default style
            public static GUIToggleStyle DefaultNormalStyle { 
                get {
                    return new GUIToggleStyle() {
                        Colour = Colour4b.White,
                        SelectionColour = new Colour4b(67, 160, 48, 255),
                        Border = new GUIBorder(2, 2, 2, 2, new Colour4b(67, 160, 48, 255))
                    };
                } 
            }

            // Default style when the object is being hovered over
            public static GUIToggleStyle DefaultHoverStyle {
                get {
                    return new GUIToggleStyle() {
                        Colour = new Colour4b(235, 235, 235, 255),
                        SelectionColour = new Colour4b(67, 160, 48, 255),
                        Border = new GUIBorder(2, 2, 2, 2, new Colour4b(67, 160, 48, 255))
                    };
                }
            }

            // Default style when the object is selected
            public static GUIToggleStyle DefaultSelectedStyle {
                get {
                    return new GUIToggleStyle() {
                        Colour = Colour4b.White,
                        SelectionColour = new Colour4b(67, 160, 48, 255),
                        Border = new GUIBorder(2, 2, 2, 2, new Colour4b(67, 160, 48, 255))
                    };
                }
            }

            // Default style when the object is being hovered over AND is selected
            public static GUIToggleStyle DefaultSelectedHoverStyle {
                get {
                    return new GUIToggleStyle() {
                        Colour = new Colour4b(235, 235, 235, 255),
                        SelectionColour = new Colour4b(67, 160, 48, 255),
                        Border = new GUIBorder(2, 2, 2, 2, new Colour4b(67, 160, 48, 255))
                    };
                }
            }

            public GUIToggleStyle() {
                Colour = Colour4b.White;
                SelectionColour = new Colour4b(67, 160, 48, 255);
                Border = new GUIBorder(0, 0, 0, 0, Colour4b.Black);
                FontSize = 8f;
                FontFamilyId = 0;
                FontColour = Colour4b.Black;
            }

            internal GUIBox.GUIBoxStyle ConvertToGUIBoxStyle() {
                GUIBox.GUIBoxStyle style = new GUIBox.GUIBoxStyle();
                style.Border = Border;
                style.Colour = Colour;

                return style;
            }

            internal GUITextArea.GUITextAreaStyle ConvertToGUITextStyle() {
                GUITextArea.GUITextAreaStyle style = new GUITextArea.GUITextAreaStyle();
                style.Border = Border;
                style.Colour = new Colour4b(Colour.R, Colour.G, Colour.B, 0);
                style.FontColour = FontColour;
                style.FontFamilyId = FontFamilyId;
                style.FontSize = FontSize;
                style.HorizontalAlignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Middle;
                style.Scrollable = false;
                style.ScrollPosition = 0.0f;

                return style;
            }
        }

        private bool m_hovering;

        public string Text { get; set; }
        public RectangleShape Bounds { get; set; }
        public bool IsSelected { get; set; }
        public GUIToggleStyle NormalStyle { get; set; }
        public GUIToggleStyle HoverStyle { get; set; }
        public GUIToggleStyle SelectedStyle { get; set; }
        public GUIToggleStyle SelectedHoverStyle { get; set; }

        /// <summary>
        /// The event that is invoked when the object is selected/unselected
        /// </summary>
        public event EventHandler OnSelectionChangedEvent;

        public GUIToggle() {
            Text = "GUIToggle";
            IsSelected = false;

            NormalStyle = GUIToggleStyle.DefaultNormalStyle;
            HoverStyle = GUIToggleStyle.DefaultHoverStyle;
            SelectedHoverStyle = GUIToggleStyle.DefaultSelectedHoverStyle;
            SelectedStyle = GUIToggleStyle.DefaultSelectedStyle;

            TackGUI.RegisterGUIObject(this);
        }

        internal override void OnStart() {
            
        }

        internal override void OnUpdate() {
            Vector2f mousePosition = Input.TackInput.MousePosition();
            RectangleShape checkBoxShape = new RectangleShape(Bounds.X + 2, Bounds.Y + 2, Bounds.Height - 4, Bounds.Height - 4);

            if (mousePosition.X >= checkBoxShape.X && mousePosition.X <= (checkBoxShape.X + checkBoxShape.Width)) {
                if (mousePosition.Y >= checkBoxShape.Y && mousePosition.Y <= (checkBoxShape.Y + checkBoxShape.Height)) {
                    m_hovering = true;

                    if (TackInput.MouseButtonUp(MouseButtonKey.Left)) {
                        IsSelected = !IsSelected;

                        if (OnSelectionChangedEvent != null) {
                            if (OnSelectionChangedEvent.GetInvocationList().Length > 0) {
                                OnSelectionChangedEvent.Invoke(this, EventArgs.Empty);
                            }
                        }
                    }
                } else {
                    m_hovering = false;
                }
            } else {
                m_hovering = false;
            }
        }

        internal override void OnRender() {
            if (IsSelected) {
                if (m_hovering) {
                    TackGUI.InternalToggle(Bounds, IsSelected, Text, SelectedHoverStyle);
                } else {
                    TackGUI.InternalToggle(Bounds, IsSelected, Text, SelectedStyle);
                }
            } else if (m_hovering) {
                TackGUI.InternalToggle(Bounds, IsSelected, Text, HoverStyle);
            } else {
                TackGUI.InternalToggle(Bounds, IsSelected, Text, NormalStyle);
            }
        }

        internal override void OnClose() {
            
        }
    }
}

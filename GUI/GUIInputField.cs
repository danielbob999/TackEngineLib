using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;
using TackEngineLib.Input;

namespace TackEngineLib.GUI {
    public class GUIInputField : GUIObject {

        /// <summary>
        /// The style of a GUIInputField object
        /// </summary>
        public class GUIInputFieldStyle {

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

            // Static default styles

            // Default style
            public static GUIInputFieldStyle DefaultNormalStyle {
                get {
                    return new GUIInputFieldStyle() {
                        Colour = Colour4b.White,
                        Border = new GUIBorder(1, 1, 1, 1, new Colour4b(100, 100, 100, 255))
                    };
                }
            }

            // Default style when the object is requiring input
            public static GUIInputFieldStyle DefaultRequiringInputStyle {
                get {
                    return new GUIInputFieldStyle() {
                        Colour = Colour4b.White,
                        Border = new GUIBorder(1, 1, 1, 1, new Colour4b(100, 100, 100, 255))
                    };
                }
            }

            public GUIInputFieldStyle() {
                Colour = Colour4b.White;
                Border = Border = new GUIBorder(0, 0, 0, 0, Colour4b.Black);
                FontSize = 8f;
                FontFamilyId = 0;
                FontColour = Colour4b.Black;
            }

            internal GUITextArea.GUITextAreaStyle ConvertToGUITextStyle() {
                GUITextArea.GUITextAreaStyle style = new GUITextArea.GUITextAreaStyle();
                style.Border = Border;
                style.Colour = Colour;
                style.FontColour = FontColour;
                style.FontFamilyId = FontFamilyId;
                style.FontSize = FontSize;
                style.HorizontalAlignment = HorizontalAlignment;
                style.VerticalAlignment = VerticalAlignment;
                style.Scrollable = Scrollable;
                style.ScrollPosition = ScrollPosition;

                return style;
            }
        }

        private GUIBox m_caretBox;

        public string Text { get; set; }
        public int SelectionStart { get; set; }  // The start of the selection. If the SelectionLength = 0, SelectionStart is the position of the Caret
        public int SelectionLength { get; set; }
        public RectangleShape Bounds { get; set; }
        public bool RequiringInput { get; set; }
        public GUIInputFieldStyle NormalStyle { get; set; }
        public GUIInputFieldStyle RequiringInputStyle { get; set; }

        public GUIInputField() {
            Text = "";
            RequiringInput = false;

            NormalStyle = GUIInputFieldStyle.DefaultNormalStyle;
            RequiringInputStyle = GUIInputFieldStyle.DefaultRequiringInputStyle;

            TackGUI.RegisterGUIObject(this);

            m_caretBox = new GUIBox();
            m_caretBox.NormalStyle = new GUIBox.GUIBoxStyle();
            m_caretBox.NormalStyle.Colour = Colour4b.Black;
        }

        internal override void OnStart() {
        }

        internal override void OnUpdate() {
            m_caretBox.Bounds = new RectangleShape(Bounds.X + 10, Bounds.Y + 10, 0, 0);

            Vector2f mousePosition = TackInput.MousePosition();

            if (TackInput.MouseButtonUp(MouseButtonKey.Left)) {
                if (mousePosition.X >= Bounds.X && mousePosition.X <= (Bounds.X + Bounds.Width)) {
                    if (mousePosition.Y >= Bounds.Y && mousePosition.Y <= (Bounds.Y + Bounds.Height)) {
                        RequiringInput = true;
                        TackInput.GUIInputRequired = true;
                    } else {
                        RequiringInput = false;
                        TackInput.GUIInputRequired = false;
                    }
                } else {
                    RequiringInput = false;
                    TackInput.GUIInputRequired = false;
                }
            }

            KeyboardKey[] bufferOperations = TackInput.GetInputBufferArray();

            for (int i = 0; i < bufferOperations.Length; i++) {
                if (bufferOperations[i] == KeyboardKey.Left) {
                    if (SelectionStart > 0) {
                        SelectionStart -= 1;
                    }
                } else if (bufferOperations[i] == KeyboardKey.Right) {
                    if (SelectionStart < Text.Length) {
                        SelectionStart += 1;
                    }
                } else if (bufferOperations[i] == KeyboardKey.BackSpace) {
                    if (SelectionStart > 0) {
                        Text = Text.Remove((int)SelectionStart - 1, 1);
                    }

                    if (SelectionStart > 0) {
                        SelectionStart -= 1;
                    }
                } else if (bufferOperations[i] == KeyboardKey.Delete) {
                    
                    if (SelectionStart < Text.Length) {
                        Text = Text.Remove((int)SelectionStart, 1);
                    }
                } else if (bufferOperations[i] == KeyboardKey.Space) {
                    Text = Text.Insert((int)SelectionStart, " ");

                    if (SelectionStart < Text.Length) {
                        SelectionStart += 1;
                    }
                } else if (bufferOperations[i] == KeyboardKey.Period) {
                    Text = Text.Insert((int)SelectionStart, ".");

                    if (SelectionStart < Text.Length) {
                        SelectionStart += 1;
                    }
                } else if (bufferOperations[i] == KeyboardKey.Quote) {
                    Text = Text.Insert((int)SelectionStart, "\"");

                    if (SelectionStart < Text.Length) {
                        SelectionStart += 1;
                    }
                } else if (bufferOperations[i] == KeyboardKey.Minus) {
                    if (TackInput.InputBufferShift) {
                        Text = Text.Insert((int)SelectionStart, "_");
                    } else {
                        Text = Text.Insert((int)SelectionStart, "-");
                    }

                    if (SelectionStart < Text.Length) {
                        SelectionStart += 1;
                    }
                }
                
                else if (bufferOperations[i] >= KeyboardKey.Number0 && bufferOperations[i] <= KeyboardKey.Number9) {
                    Text = Text.Insert((int)SelectionStart, ((char)((int)bufferOperations[i] - 61)).ToString());

                    if (SelectionStart < Text.Length) {
                        SelectionStart += 1;
                    }
                } else if (bufferOperations[i] >= KeyboardKey.A && bufferOperations[i] <= KeyboardKey.Z) {
                    if (TackInput.InputBufferCapsLock || TackInput.InputBufferShift) {
                        Text = Text.Insert((int)SelectionStart, ((char)((int)bufferOperations[i] - 18)).ToString());
                    } else {
                        Text = Text.Insert((int)SelectionStart, ((char)((int)bufferOperations[i] + 14)).ToString());
                    }

                    if (SelectionStart < Text.Length) {
                        SelectionStart += 1;
                    }
                }
            }

            TackInput.ClearInputBuffer();
        }

        internal override void OnRender() {
            if (RequiringInput) {
                TackGUI.InternalTextArea(Bounds, Text, RequiringInputStyle.ConvertToGUITextStyle());
            } else {
                TackGUI.InternalTextArea(Bounds, Text, NormalStyle.ConvertToGUITextStyle());
            }
        }

        internal override void OnClose() {
            
        }
    }
}

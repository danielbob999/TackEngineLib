using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

using TackEngineLib.Engine;
using TackEngineLib.Main;
using TackEngineLib.Input;

namespace TackEngineLib.GUI
{
    public class InputField
    {
        private bool m_ReceivingInput;
        private string m_InputString;
        private RectangleShape m_Shape;
        private BoxStyle m_CaretStyle;
        private int m_caretPosition;
        private Stopwatch m_caretDisplayStopwatch;
        private int m_caretDisplaySpeed;
        private bool m_displayCaret;
        private Graphics m_stringMeasurer;

        public event EventHandler SubmitInput;

        /// <summary>
        /// Is this InputField receiving input?
        /// </summary>
        public bool ReceivingInput
        {
            get { return m_ReceivingInput; }
            set {
                m_ReceivingInput = value;
                TackInput.GUIInputRequired = value;

                if (m_ReceivingInput == true)
                {
                    m_displayCaret = true;
                    m_caretDisplayStopwatch.Restart();
                }
                else
                {
                    m_displayCaret = false;
                    m_caretDisplayStopwatch.Stop();
                }
            }
        }

        /// <summary>
        /// The string in this InputField
        /// </summary>
        public string InputString
        {
            get { return m_InputString; }
            set { m_InputString = value; }
        }

        /// <summary>
        /// The shape of this InputField
        /// </summary>
        public RectangleShape Shape
        {
            get { return m_Shape; }
            set { m_Shape = value; }
        }

        /// <summary>
        /// The style of this InputField's caret object
        /// </summary>
        public BoxStyle CaretStyle
        {
            get { return m_CaretStyle; }
            set { m_CaretStyle = value; }
        }

        /// <summary>
        /// The 0-based position of the caret in relation to the current input string
        /// </summary>
        public int CaretPosition
        {
            get { return m_caretPosition; }
            set { m_caretPosition = value; }
        }

        /// <summary>
        /// Intialises a new InputField
        /// </summary>
        public InputField()
        {
            m_ReceivingInput = false;
            m_Shape = new RectangleShape(10, 10, 160, 35);

            m_CaretStyle = new BoxStyle()
            {
                Colour = new Colour4b(0, 0, 0, 255)
            };

            m_caretDisplayStopwatch = new Stopwatch();
            m_caretDisplaySpeed = 750;
            m_displayCaret = false;
            m_caretPosition = 0;
            m_stringMeasurer = Graphics.FromImage(new Bitmap(1, 1));

            TackGUI.inputFields.Add(this);
        }

        /// <summary>
        /// Updates the logic of this InputField
        /// </summary>
        public void Update()
        {
            // Get keyboard keys and deal with them
            KeyboardKey bufferKey;
            if (TackInput.GetKeyFromInputBuffer(out bufferKey))
            {
                if (bufferKey == KeyboardKey.BackSpace)
                {
                    if (m_InputString.Length > 0)
                        m_InputString = m_InputString.Remove(m_InputString.Length - 1, 1);
                } else if (bufferKey == KeyboardKey.Space)
                {
                    m_InputString += " ";
                } else if (bufferKey == KeyboardKey.Period)
                {
                    m_InputString += ".";
                } else if (bufferKey == KeyboardKey.Quote)
                {
                    m_InputString += "\"";
                } else if (bufferKey >= KeyboardKey.Number0 && bufferKey <= KeyboardKey.Number9)
                {
                    m_InputString += (char)((int)bufferKey - 61);
                } else if (bufferKey >= KeyboardKey.A && bufferKey <= KeyboardKey.Z)
                {
                    if (TackInput.InputBufferCapsLock)
                        m_InputString += (char)((int)bufferKey - 18);
                    else
                        m_InputString += (char)((int)bufferKey + 14);
                }
            }

            if (Input.TackInput.InputActiveKeyDown(KeyboardKey.Enter))
            {
                if (SubmitInput != null)
                {
                    SubmitInput.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Renders this InputField to the screen
        /// </summary>
        /// <param name="_value">The string to be rendered in this InputField</param>
        public void Render(InputFieldStyle _style = default(InputFieldStyle))
        {
            if (!string.IsNullOrEmpty(m_InputString))
                m_caretPosition = (m_InputString.Length - 1);

            if (_style == null)
                _style = new InputFieldStyle();

            TackGUI.Box(m_Shape, _style.GetBoxStyle());
            TackGUI.TextArea(m_Shape, m_InputString, _style.GetTextStyle());

            if (m_ReceivingInput)
            {
                if (m_caretDisplayStopwatch.ElapsedMilliseconds >= m_caretDisplaySpeed)
                {
                    m_displayCaret = !m_displayCaret;
                    m_caretDisplayStopwatch.Restart();
                }

                if (m_displayCaret)
                {
                    if (!string.IsNullOrEmpty(m_InputString))
                    {
                        m_caretPosition = m_InputString.Length;
                        string selectedInputStr = m_InputString.Substring(0, m_caretPosition);
                        SizeF stringLengthPx = m_stringMeasurer.MeasureString(selectedInputStr, new Font(TackGUI.GetFontFamily(_style.FontFamilyId), _style.FontSize));

                        int totalPadding = (int)(m_Shape.Height - stringLengthPx.Height);
                        TackGUI.Box(new RectangleShape(stringLengthPx.Width - 3.0f, (TackEngine.ScreenHeight * 0.70f) + (totalPadding / 2), 1, (stringLengthPx.Height)), m_CaretStyle);
                    }
                    else
                    {
                        SizeF stringLengthPx = m_stringMeasurer.MeasureString("l", new Font(TackGUI.GetFontFamily(_style.FontFamilyId), _style.FontSize));

                        int totalPadding = (int)(m_Shape.Height - stringLengthPx.Height);
                        TackGUI.Box(new RectangleShape(3.0f, (TackEngine.ScreenHeight * 0.70f) + (totalPadding / 2), 1, (stringLengthPx.Height)), m_CaretStyle);
                    }
                }

            }
        }

        /// <summary>
        /// Gets the input from the TackInput input buffer
        /// </summary>
        /// <returns></returns>
        public string GetInput()
        {
            return TackInput.GetInputBuffer();
        }

        /// <summary>
        /// Destorys this instance of InputField
        /// </summary>
        /// <returns></returns>
        public void Destory()
        {
            TackGUI.inputFields.Remove(this);
        }

        /// <summary>
        /// Is the mouse in the bounds of the InputField?
        /// </summary>
        /// <returns>Returns true if mouse is in the bounds of the InputField, false otherwise</returns>
        public bool IsMouseInBounds()
        {
            Vector2f mousePos = TackInput.MousePosition();

            if (mousePos.X >= m_Shape.X && mousePos.X <= (m_Shape.X + m_Shape.Width))
            {
                if (mousePos.Y >= m_Shape.Y && mousePos.Y <= (m_Shape.Y + m_Shape.Height))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

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
        private RectangleShape m_Shape;
        private BoxStyle m_CaretStyle;
        private int m_caretPosition;
        private Stopwatch m_caretDisplayStopwatch;
        private int m_caretDisplaySpeed;
        private bool m_displayCaret;
        private Graphics m_stringMeasurer;

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
            m_caretDisplaySpeed = 1000;
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
        }

        /// <summary>
        /// Renders this InputField to the screen
        /// </summary>
        /// <param name="_value">The string to be rendered in this InputField</param>
        public void Render(string _value, InputFieldStyle _style = default(InputFieldStyle))
        {
            if (!string.IsNullOrEmpty(_value))
                m_caretPosition = (_value.Length - 1);

            Console.WriteLine(m_caretPosition);

            if (_style == null)
                _style = new InputFieldStyle();

            TackGUI.Box(m_Shape, _style.GetBoxStyle());
            TackGUI.TextArea(m_Shape, _value, _style.GetTextStyle());

            if (m_ReceivingInput)
            {
                if (m_caretDisplayStopwatch.ElapsedMilliseconds >= m_caretDisplaySpeed)
                {
                    m_displayCaret = !m_displayCaret;
                    m_caretDisplayStopwatch.Restart();
                }

                if (m_displayCaret)
                {
                    m_caretPosition = _value.Length;
                    string selectedInputStr = _value.Substring(0, m_caretPosition);
                    SizeF stringLengthPx = m_stringMeasurer.MeasureString(selectedInputStr, new Font(TackGUI.GetFontFamily(0), _style.FontSize));

                    int totalPadding = (int)(m_Shape.Height - stringLengthPx.Height);
                    TackGUI.Box(new RectangleShape(stringLengthPx.Width - 3.0f, (TackEngine.ScreenHeight * 0.70f) + (totalPadding / 2), 1, (stringLengthPx.Height)), m_CaretStyle);
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

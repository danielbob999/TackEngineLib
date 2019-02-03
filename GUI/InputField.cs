using System;
using System.Collections.Generic;

using TackEngineLib.Engine;
using TackEngineLib.Main;
using TackEngineLib.Input;

namespace TackEngineLib.GUI
{
    public class InputField
    {
        private bool m_ReceivingInput;
        private RectangleShape m_Shape;

        /// <summary>
        /// Is this InputField receiving input?
        /// </summary>
        public bool ReceivingInput
        {
            get { return m_ReceivingInput; }
            set {
                m_ReceivingInput = value;
                TackInput.GUIInputRequired = value;
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
        /// Intialises a new InputField
        /// </summary>
        public InputField()
        {
            m_ReceivingInput = false;
            m_Shape = new RectangleShape(10, 10, 160, 35);

            TackGUI.inputFields.Add(this);
        }

        /// <summary>
        /// Renders this InputField to the screen
        /// </summary>
        /// <param name="_value">The string to be rendered in this InputField</param>
        public void Render(string _value, InputFieldStyle _style = default(InputFieldStyle))
        {
            if (_style == null)
                _style = new InputFieldStyle();

            TackGUI.Box(m_Shape, _style.GetBoxStyle());
            TackGUI.TextArea(m_Shape, _value, _style.GetTextStyle());

            //_style.Destory();
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

/* Copyright (c) 2019 Daniel Phillip Robinson */
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
        private bool mReceivingInput;
        private string mInputString;
        private RectangleShape mShape;
        private BoxStyle mCaretStyle;
        private int mcaretPosition;
        private Stopwatch mcaretDisplayStopwatch;
        private int mcaretDisplaySpeed;
        private bool mdisplayCaret;
        private Graphics mstringMeasurer;

        public event EventHandler SubmitInput;

        /// <summary>
        /// Is this InputField receiving input?
        /// </summary>
        public bool ReceivingInput
        {
            get { return mReceivingInput; }
            set {
                mReceivingInput = value;
                TackInput.GUIInputRequired = value;

                if (mReceivingInput == true)
                {
                    mdisplayCaret = true;
                    mcaretDisplayStopwatch.Restart();
                }
                else
                {
                    mdisplayCaret = false;
                    mcaretDisplayStopwatch.Stop();
                }
            }
        }

        /// <summary>
        /// The string in this InputField
        /// </summary>
        public string InputString
        {
            get { return mInputString; }
            set { mInputString = value; }
        }

        /// <summary>
        /// The shape of this InputField
        /// </summary>
        public RectangleShape Shape
        {
            get { return mShape; }
            set { mShape = value; }
        }

        /// <summary>
        /// The style of this InputField's caret object
        /// </summary>
        public BoxStyle CaretStyle
        {
            get { return mCaretStyle; }
            set { mCaretStyle = value; }
        }

        /// <summary>
        /// The 0-based position of the caret in relation to the current input string
        /// </summary>
        public int CaretPosition
        {
            get { return mcaretPosition; }
            set { mcaretPosition = value; }
        }

        /// <summary>
        /// Intialises a new InputField
        /// </summary>
        public InputField()
        {
            mReceivingInput = false;

            mShape = new RectangleShape(10, 10, 160, 35);

            mCaretStyle = new BoxStyle()
            {
                Colour = new Colour4b(0, 0, 0, 255)
            };

            mcaretDisplayStopwatch = new Stopwatch();
            mcaretDisplaySpeed = 750;
            mdisplayCaret = false;
            mcaretPosition = 0;
            mstringMeasurer = Graphics.FromImage(new Bitmap(1, 1));

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
                    if (mInputString.Length > 0)
                        mInputString = mInputString.Remove(mInputString.Length - 1, 1);
                } else if (bufferKey == KeyboardKey.Space)
                {
                    mInputString += " ";
                } else if (bufferKey == KeyboardKey.Period)
                {
                    mInputString += ".";
                } else if (bufferKey == KeyboardKey.Quote)
                {
                    mInputString += "\"";
                } else if (bufferKey >= KeyboardKey.Number0 && bufferKey <= KeyboardKey.Number9)
                {
                    mInputString += (char)((int)bufferKey - 61);
                } else if (bufferKey >= KeyboardKey.A && bufferKey <= KeyboardKey.Z)
                {
                    if (TackInput.InputBufferCapsLock || TackInput.InputBufferShift)
                        mInputString += (char)((int)bufferKey - 18);
                    else
                        mInputString += (char)((int)bufferKey + 14);
                }
            }

            if (Input.TackInput.InputActiveKeyDown(KeyboardKey.Enter))
            {
                if (SubmitInput != null)
                {
                    if (mReceivingInput)
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
            if (!string.IsNullOrEmpty(mInputString))
                mcaretPosition = (mInputString.Length - 1);

            if (_style == null)
                _style = new InputFieldStyle();

            TackGUI.Box(mShape, _style.GetBoxStyle());
            TackGUI.TextArea(mShape, mInputString, _style.GetTextStyle());

            if (mReceivingInput)
            {
                if (mcaretDisplayStopwatch.ElapsedMilliseconds >= mcaretDisplaySpeed)
                {
                    mdisplayCaret = !mdisplayCaret;
                    mcaretDisplayStopwatch.Restart();
                }

                if (mdisplayCaret)
                {
                    if (!string.IsNullOrEmpty(mInputString))
                    {
                        mcaretPosition = mInputString.Length;
                        string selectedInputStr = mInputString.Substring(0, mcaretPosition);
                        SizeF stringLengthPx = mstringMeasurer.MeasureString(selectedInputStr, new Font(TackGUI.GetFontFamily(_style.FontFamilyId), _style.FontSize));

                        int totalPadding = (int)(mShape.Height - stringLengthPx.Height);
                        TackGUI.Box(new RectangleShape(stringLengthPx.Width - 3.0f, (TackEngine.ScreenHeight * 0.70f) + (totalPadding / 2), 1, (stringLengthPx.Height)), mCaretStyle);
                    }
                    else
                    {
                        SizeF stringLengthPx = mstringMeasurer.MeasureString("l", new Font(TackGUI.GetFontFamily(_style.FontFamilyId), _style.FontSize));

                        int totalPadding = (int)(mShape.Height - stringLengthPx.Height);
                        TackGUI.Box(new RectangleShape(3.0f, (TackEngine.ScreenHeight * 0.70f) + (totalPadding / 2), 1, (stringLengthPx.Height)), mCaretStyle);
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

            if (mousePos.X >= mShape.X && mousePos.X <= (mShape.X + mShape.Width))
            {
                if (mousePos.Y >= mShape.Y && mousePos.Y <= (mShape.Y + mShape.Height))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

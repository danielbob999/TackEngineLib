using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;
using TackEngineLib.Input;

namespace TackEngineLib.GUI
{
    public class GUIButton
    {
        private GUIButtonStyle mStyle;
        private GUIButtonStyle mHoverStyle;
        private GUIButtonStyle mPressStyle;
        private RectangleShape mShape;
        private string mText;
        private bool mHovering = false;
        private bool mPressing = false;
        public event EventHandler Click;
        public event EventHandler OnHover;

        /// <summary>
        /// The regular style of this GUIButton
        /// </summary>
        public GUIButtonStyle Style
        {
            get { return mStyle; }
            set { mStyle = value; }
        }

        /// <summary>
        /// The style of this GUIButton when being hovered over
        /// </summary>
        public GUIButtonStyle HoverStyle
        {
            get { return mHoverStyle; }
            set { mHoverStyle = value; }
        }

        /// <summary>
        /// The style of thi GUIButton when it is being pressed
        /// </summary>
        public GUIButtonStyle PressStyle
        {
            get { return mPressStyle; }
            set { mPressStyle = value; }
        }

        /// <summary>
        /// The text of this GUIButton
        /// </summary>
        public string Text
        {
            get { return mText; }
            set { mText = value; }
        }

        /// <summary>
        /// The shape of this GUIButton
        /// </summary>
        public RectangleShape Shape
        {
            get { return mShape; }
            set { mShape = value; }
        }

        /// <summary>
        /// Intialises a new Button
        /// </summary>
        public GUIButton() {
            mText = "Button";
            mStyle = GUIButtonStyle.DefaultStyle;
            mHoverStyle = GUIButtonStyle.DefaultHoverStyle;
            mPressStyle = GUIButtonStyle.DefaultPressStyle;

            mHovering = false;
            mPressing = false;
        }

        public void Update() {
            Vector2f mouseVec = TackInput.MousePosition();

            if (mouseVec.X > mShape.X && mouseVec.X < (mShape.X + mShape.Width)) {
                if (mouseVec.Y > mShape.Y && mouseVec.Y < (mShape.Y + mShape.Height)) {
                    mHovering = true;

                    if (OnHover != null) {
                        if (OnHover.GetInvocationList().Length > 0) {
                            OnHover.Invoke(this, EventArgs.Empty);
                        }
                    }

                    if (TackInput.MouseButtonDown(MouseButtonKey.Left)) {
                        mPressing = true;

                        if (Click != null) {
                            if (Click.GetInvocationList().Length > 0) {
                                Click.Invoke(this, EventArgs.Empty);
                            }
                        }
                    }

                } else {
                    mHovering = false;
                }
            } else {
                mHovering = false;
            }

            if (TackInput.MouseButtonUp(MouseButtonKey.Left)) {
                mPressing = false;
            }
        }

        public void Render() {
            if (mPressing) {
                // If the button is being pressed
                TackGUI.Box(mShape, mPressStyle.GetBoxStyle());
                TackGUI.TextArea(mShape, mText, mPressStyle.GetTextAreaStyle());
            } else if (mHovering) {
                // If the button is being hovered
                TackGUI.Box(mShape, mHoverStyle.GetBoxStyle());
                TackGUI.TextArea(mShape, mText, mHoverStyle.GetTextAreaStyle());
            } else {
                // Regular style
                TackGUI.Box(mShape, mStyle.GetBoxStyle());
                TackGUI.TextArea(mShape, mText, mStyle.GetTextAreaStyle());
            }
        }
    }
}

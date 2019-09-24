using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngineLib.GUI
{
    public class Button
    {
        private ButtonStyle mButtonStyle;
        private string mText;
        public event EventHandler Click;
        public event EventHandler OnHover;

        public ButtonStyle Style
        {
            get { return mButtonStyle; }
            set { mButtonStyle = value; }
        }

        /// <summary>
        /// The text of this Button
        /// </summary>
        public string Text
        {
            get { return mText; }
            set { mText = value; }
        }

        /// <summary>
        /// Intialises a new Button
        /// </summary>
        public Button() {
            mText = "Button";
            mButtonStyle = new ButtonStyle();
        }

        public void Update() {
            TackEngineLib.Main.Vector2f mouseVec = TackEngineLib.Input.TackInput.MousePosition();

            if (mouseVec.X > mButtonStyle.Shape.X && mouseVec.X < (mButtonStyle.Shape.X + mButtonStyle.Shape.Width)) {
                if (mouseVec.Y > mButtonStyle.Shape.Y && mouseVec.Y < (mButtonStyle.Shape.Y + mButtonStyle.Shape.Height)) {
                    if (OnHover != null) {
                        if (OnHover.GetInvocationList().Length > 0) {
                            OnHover.Invoke(this, EventArgs.Empty);
                        }
                    }

                    if (TackEngineLib.Input.TackInput.MouseButtonDown(Input.MouseButtonKey.Left)) {
                        if (Click != null) {
                            if (Click.GetInvocationList().Length > 0) {
                                Click.Invoke(this, EventArgs.Empty);
                            }
                        }
                    }
                }
            }
        }

        public void Render() {
            TackGUI.Box(mButtonStyle.Shape, mButtonStyle.GetBoxStyle());
            TackGUI.TextArea(mButtonStyle.Shape, mText, mButtonStyle.GetTextAreaStyle());
        }
    }
}

/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;
using TackEngineLib.Engine;

using OpenTK.Input;

namespace TackEngineLib.Input
{
    public static class TackInput
    {
        // Keyboard keys
        private static bool[] mKeysHeld;
        private static bool[] mKeysDownPerFrame;
        private static bool[] mKeysUpPerFrame;
        private static bool[] mLastFramesKeys;

        // Mouse button keys
        private static bool[] mMouseKeysHeld;
        private static bool[] mMouseKeysDownPerFrame;
        private static bool[] mMouseKeysUpPerFrame;
        private static bool[] mLastFrameMouseKeys;

        private static int mMousePositionX = 0;
        private static int mMousePositionY = 0;

        // Key Down/Up Lockers
        private static bool[] locker_mKeysDownPerFrame;

        private static bool[] locker_mMouseKeysDownPerFrame;

        private static List<KeyboardKey> mInputBuffer = new List<KeyboardKey>();

        private static bool mInputBufferCapsLock = false;
        private static bool mInputBufferShift = false;

        // Tells TackInput that there is a TackGUI.InputField active and needs input
        private static bool mGUIInputRequired = false;

        /// <summary>
        /// A bool that tells TackInput that a TackGUI.InputField is active and requires input
        /// </summary>
        internal static bool GUIInputRequired
        {
            get { return mGUIInputRequired; }
            set
            {
                mGUIInputRequired = value;
                //TackConsole.EngineLog(EngineLogType.Message, string.Format("{0} GUI input", mGUIInputRequired ? "Enabled" : "Disabled"));
            }
        }

        internal static bool InputBufferCapsLock
        {
            get { return mInputBufferCapsLock; }
        }

        internal static bool InputBufferShift
        {
            get { return mInputBufferShift; }
        }

        internal static void OnStart()
        {
            if (Console.CapsLock) {
                mInputBufferCapsLock = true;
            }

            // Keyboard keys
            mKeysHeld = new bool[1024];
            mLastFramesKeys = new bool[1024];
            mKeysDownPerFrame = new bool[1024];
            mKeysUpPerFrame = new bool[1024];

            locker_mKeysDownPerFrame = new bool[1024];

            // Mouse keys
            mMouseKeysHeld = new bool[1024];
            mLastFrameMouseKeys = new bool[1024];
            mMouseKeysDownPerFrame = new bool[1024];
            mMouseKeysUpPerFrame = new bool[1024];

            locker_mMouseKeysDownPerFrame = new bool[1024];

            GUIInputRequired = false;
        }

        internal static void OnUpdate()
        {
            mLastFramesKeys = mKeysHeld;
            mLastFrameMouseKeys = mMouseKeysHeld;

            mKeysDownPerFrame = new bool[1024];
            mKeysUpPerFrame = new bool[1024];

            mMouseKeysDownPerFrame = new bool[1024];
            mMouseKeysUpPerFrame = new bool[1024];
        }

        internal static void KeyDownEvent(KeyboardKey _key)
        {
            /*
            // Add character to the input buffer.
            // NOTE: Don't register the key as being pressed
            if (mGUIInputRequired)
            {
                if (FindCharacterFromKeyCode(_key) == 8)
                {
                    if (mInputBuffer.Count > 0)
                        mInputBuffer.RemoveAt(mInputBuffer.Count - 1);
                    return;
                }

                if (FindCharacterFromKeyCode(_key) != 0)
                    mInputBuffer.Add((char)FindCharacterFromKeyCode(_key));

                return;
            }*/

            if (mGUIInputRequired)
            {
                mInputBuffer.Add(_key);
                //Console.WriteLine("Register key down: [{0}]", _key.ToString());

                if (_key == KeyboardKey.CapsLock)
                    mInputBufferCapsLock = !mInputBufferCapsLock;

                if (_key == KeyboardKey.ShiftLeft || _key == KeyboardKey.ShiftRight) {
                    mInputBufferShift = true;
                }

                /*
                if (FindCharacterFromKeyCode(_key) == 8)
                {
                    if (mInputBuffer.Count > 0)
                        mInputBuffer.RemoveAt(mInputBuffer.Count - 1);
                }
                else if (FindCharacterFromKeyCode(_key) != 0)
                {
                    mInputBuffer.Add((char)FindCharacterFromKeyCode(_key));
                }*/
            }

            if (!locker_mKeysDownPerFrame[(int)_key]) // if the down key isn't locked
            {
                mKeysDownPerFrame[(int)_key] = true;
                locker_mKeysDownPerFrame[(int)_key] = true;
            }

            mKeysHeld[(int)_key] = true;
        }

        internal static void KeyUpEvent(KeyboardKey _key)
        {
            if (mGUIInputRequired) {
                if (_key == KeyboardKey.ShiftLeft || _key == KeyboardKey.ShiftRight) {
                    mInputBufferShift = false;
                }
            }

            mKeysUpPerFrame[(int)_key] = true;
            mKeysHeld[(int)_key] = false;

            locker_mKeysDownPerFrame[(int)_key] = false; // Unlock the down key
        }

        internal static void MouseMoveEvent(int _x, int _y)
        {
            mMousePositionX = _x;
            mMousePositionY = _y;
        }

        internal static void MouseDownEvent(MouseButtonKey _key)
        {
            if (!locker_mMouseKeysDownPerFrame[(int)_key]) // if the down key isn't locked
            {
                mMouseKeysDownPerFrame[(int)_key] = true;
                locker_mMouseKeysDownPerFrame[(int)_key] = true;
            }

            mMouseKeysHeld[(int)_key] = true;

            // Register the mouse event to the gui
            GUI.TackGUI.AddMouseEvent(new GUI.GUIMouseEvent(0, new Vector2f(mMousePositionX, mMousePositionY), _key));
        }

        internal static void MouseUpEvent(MouseButtonKey _key)
        {
            mMouseKeysUpPerFrame[(int)_key] = true;
            mMouseKeysHeld[(int)_key] = false;

            locker_mMouseKeysDownPerFrame[(int)_key] = false; // Unlock the down key

            // Register the mouse event to the gui
            GUI.TackGUI.AddMouseEvent(new GUI.GUIMouseEvent(1, new Vector2f(mMousePositionX, mMousePositionY), _key));
        }

        public static bool KeyDown(KeyboardKey _keyCode)
        {
            if (GUIInputRequired)
                return false;

            return mKeysDownPerFrame[(int)_keyCode];
        }

        /// <summary>
        /// Returns true if the specified KeyboardKey was depressed during the last frame.
        ///     Should only be used when the a GUI InputField is getting input
        /// </summary>
        /// <param name="a_keyCode"></param>
        /// <returns></returns>
        internal static bool InputActiveKeyDown(KeyboardKey a_keyCode)
        {
            return mKeysDownPerFrame[(int)a_keyCode];
        }

        public static bool KeyHeld(KeyboardKey _keyCode)
        {
            if (GUIInputRequired)
                return false;

            return mKeysHeld[(int)_keyCode];
        }

        /// <summary>
        /// Returns true if the specified KeyboardKey was held during the last frame.
        ///     Should only be used when the a GUI InputField is getting input
        /// </summary>
        /// <param name="a_keyCode"></param>
        /// <returns></returns>
        internal static bool InputActiveKeyHeld(KeyboardKey a_keyCode)
        { 
            return mKeysHeld[(int)a_keyCode];
        }

        public static bool KeyUp(KeyboardKey _keyCode)
        {
            if (GUIInputRequired)
                return false;

            return mKeysUpPerFrame[(int)_keyCode];
        }

        /// <summary>
        /// Returns true if the specified KeyboardKey was lifted during the last frame.
        ///     Should only be used when the a GUI InputField is getting input
        /// </summary>
        /// <param name="a_keyCode"></param>
        /// <returns></returns>
        internal static bool InputActiveKeyUp(KeyboardKey a_keyCode)
        {
            return mKeysUpPerFrame[(int)a_keyCode];
        }

        public static bool MouseButtonDown(MouseButtonKey _key)
        {
            return mMouseKeysDownPerFrame[(int)_key];
        }

        public static bool MouseButtonHeld(MouseButtonKey _key)
        {
            return mMouseKeysHeld[(int)_key];
        }

        public static bool MouseButtonUp(MouseButtonKey _key)
        {
            return mMouseKeysUpPerFrame[(int)_key];
        }

        public static Vector2f MousePosition()
        {
            /*
            int _oldMaxSizeX = TackEngine.ScreenWidth;

            float oldRangeX = _oldMaxSizeX - 0;
            float newRangeX = 1 - (-1);

            float finalX = (((mMousePositionX - 0) * newRangeX) / oldRangeX) + (-1);

            int _oldMaxSizeY = TackEngine.ScreenHeight;

            float oldRangeY = _oldMaxSizeY - 0;
            float newRangeY = 1 - (-1);

            float finalY = (((mMousePositionY - 0) * newRangeY) / oldRangeY) + (-1);

            return new Vector2f(finalX, -finalY);*/

            return new Vector2f(mMousePositionX, mMousePositionY);
        }

        public static Vector2f MouseCoordsScreenToWorld()
        {
            Vector2f mousePos = MousePosition();
            float xPos = (mousePos.X * (TackEngine.ScreenWidth / 2)) + TackEngine.MainCamera.GetParent().Position.X;
            float yPos = (mousePos.Y * (TackEngine.ScreenHeight / 2)) + TackEngine.MainCamera.GetParent().Position.Y;

            return new Vector2f(xPos, yPos);
        }

        internal static string GetInputBuffer()
        {
            string returnStr = "";

            foreach (char c in mInputBuffer)
            {
                returnStr += c;
            }

            return returnStr;
        }

        internal static KeyboardKey[] GetInputBufferArray() {
            return mInputBuffer.ToArray();
        }

        /// <summary>
        /// Gets a character based on a KeyboardKey code.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>Returns an ASCII keycode that represents the letter than has been pressed.
        /// - Returns 8 if backspace has been pressed
        /// - Returns 0 if useless button was pressed (Caps lock, shift, ect)
        /// </returns>
        internal static int FindCharacterFromKeyCode(KeyboardKey _key)
        {
            if (_key == KeyboardKey.CapsLock)
            {
                mInputBufferCapsLock = !mInputBufferCapsLock;
                return 0; 
            }

            if (_key == KeyboardKey.BackSpace)
                return 8;

            if (_key == KeyboardKey.Space)
                return 32;

            if (_key == KeyboardKey.Period)
                return 46;

            if (_key >= KeyboardKey.Number0 && _key <= KeyboardKey.Number9)
                return ((int)_key - 61);

            if (_key >= KeyboardKey.A && _key <= KeyboardKey.Z)
            {
                if (mInputBufferCapsLock)
                    return ((int)_key - 18);
                else
                    return ((int)_key + 14);
            }

            return 0;
        }

        /// <summary>
        /// Clears the GUI input buffer
        /// </summary>
        public static void ClearInputBuffer()
        {
            mInputBuffer.Clear();
        }

        /// <summary>
        /// Gets the first element in the input buffer, then removes it
        /// </summary>
        /// <param name="a_outKey"></param>
        /// <returns></returns>
        public static bool GetKeyFromInputBuffer(out KeyboardKey a_outKey)
        {
            if (mInputBuffer.Count == 0)
            {
                a_outKey = KeyboardKey.A;
                return false;
            }

            a_outKey = mInputBuffer[0];
            mInputBuffer.RemoveAt(0);
            return true;
        }
    }
}

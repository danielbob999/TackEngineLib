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
        private static bool[] m_KeysHeld;
        private static bool[] m_KeysDownPerFrame;
        private static bool[] m_KeysUpPerFrame;
        private static bool[] m_LastFramesKeys;

        // Mouse button keys
        private static bool[] m_MouseKeysHeld;
        private static bool[] m_MouseKeysDownPerFrame;
        private static bool[] m_MouseKeysUpPerFrame;
        private static bool[] m_LastFrameMouseKeys;

        private static int m_MousePositionX = 0;
        private static int m_MousePositionY = 0;

        // Key Down/Up Lockers
        private static bool[] locker_m_KeysDownPerFrame;

        private static bool[] locker_m_MouseKeysDownPerFrame;

        // An input buffer that is used to get characters from key presses when
        //      a TackGUI.InputField is being used
        private static List<char> m_InputBuffer = new List<char>();

        private static bool m_InputBufferCapsLock = false;

        // Tells TackInput that there is a TackGUI.InputField active and needs input
        private static bool m_GUIInputRequired = false;

        /// <summary>
        /// A bool that tells TackInput that a TackGUI.InputField is active and requires input
        /// </summary>
        internal static bool GUIInputRequired
        {
            get { return m_GUIInputRequired; }
            set
            {
                m_GUIInputRequired = value;
                TackConsole.EngineLog(EngineLogType.Message, string.Format("{0} GUI input", m_GUIInputRequired ? "Enabled" : "Disabled"));
            }
        }

        internal static void OnStart()
        {
            // Keyboard keys
            m_KeysHeld = new bool[1024];
            m_LastFramesKeys = new bool[1024];
            m_KeysDownPerFrame = new bool[1024];
            m_KeysUpPerFrame = new bool[1024];

            locker_m_KeysDownPerFrame = new bool[1024];

            // Mouse keys
            m_MouseKeysHeld = new bool[1024];
            m_LastFrameMouseKeys = new bool[1024];
            m_MouseKeysDownPerFrame = new bool[1024];
            m_MouseKeysUpPerFrame = new bool[1024];

            locker_m_MouseKeysDownPerFrame = new bool[1024];
        }

        internal static void OnUpdate()
        {
            m_LastFramesKeys = m_KeysHeld;
            m_LastFrameMouseKeys = m_MouseKeysHeld;

            m_KeysDownPerFrame = new bool[1024];
            m_KeysUpPerFrame = new bool[1024];

            m_MouseKeysDownPerFrame = new bool[1024];
            m_MouseKeysUpPerFrame = new bool[1024];
        }

        internal static void KeyDownEvent(KeyboardKey _key)
        {
            // Add character to the input buffer.
            // NOTE: Don't register the key as being pressed
            if (m_GUIInputRequired)
            {
                if (FindCharacterFromKeyCode(_key) == 8)
                {
                    if (m_InputBuffer.Count > 0)
                        m_InputBuffer.RemoveAt(m_InputBuffer.Count - 1);
                    return;
                }

                if (FindCharacterFromKeyCode(_key) != 0)
                    m_InputBuffer.Add((char)FindCharacterFromKeyCode(_key));

                return;
            }

            if (!locker_m_KeysDownPerFrame[(int)_key]) // if the down key isn't locked
            {
                m_KeysDownPerFrame[(int)_key] = true;
                locker_m_KeysDownPerFrame[(int)_key] = true;
            }

            m_KeysHeld[(int)_key] = true;
        }

        internal static void KeyUpEvent(KeyboardKey _key)
        {
            m_KeysUpPerFrame[(int)_key] = true;
            m_KeysHeld[(int)_key] = false;

            locker_m_KeysDownPerFrame[(int)_key] = false; // Unlock the down key
        }

        internal static void MouseMoveEvent(int _x, int _y)
        {
            m_MousePositionX = _x;
            m_MousePositionY = _y;
        }

        internal static void MouseDownEvent(MouseButtonKey _key)
        {
            if (!locker_m_MouseKeysDownPerFrame[(int)_key]) // if the down key isn't locked
            {
                m_MouseKeysDownPerFrame[(int)_key] = true;
                locker_m_MouseKeysDownPerFrame[(int)_key] = true;
            }

            m_MouseKeysHeld[(int)_key] = true;
        }

        internal static void MouseUpEvent(MouseButtonKey _key)
        {
            m_MouseKeysUpPerFrame[(int)_key] = true;
            m_MouseKeysHeld[(int)_key] = false;

            locker_m_MouseKeysDownPerFrame[(int)_key] = false; // Unlock the down key
        }

        public static bool KeyDown(KeyboardKey _keyCode)
        {
            /*
            if (m_LastFramesKeys[_keyCode] == false && keys[_keyCode] == true)
                return true;

            return false;*/

            return m_KeysDownPerFrame[(int)_keyCode];
        }

        public static bool KeyHeld(KeyboardKey _keyCode)
        {
            return m_KeysHeld[(int)_keyCode];
        }

        public static bool KeyUp(KeyboardKey _keyCode)
        {
            /*
            if (m_LastFramesKeys[_keyCode] == true && keys[_keyCode] == false)
                return true;

            return false;*/

            return m_KeysUpPerFrame[(int)_keyCode];
        }

        public static bool MouseButtonDown(MouseButtonKey _key)
        {
            return m_MouseKeysDownPerFrame[(int)_key];
        }

        public static bool MouseButtonHeld(MouseButtonKey _key)
        {
            return m_MouseKeysHeld[(int)_key];
        }

        public static bool MouseButtonUp(MouseButtonKey _key)
        {
            return m_MouseKeysUpPerFrame[(int)_key];
        }

        public static Vector2f MousePosition()
        {
            /*
            int _oldMaxSizeX = TackEngine.ScreenWidth;

            float oldRangeX = _oldMaxSizeX - 0;
            float newRangeX = 1 - (-1);

            float finalX = (((m_MousePositionX - 0) * newRangeX) / oldRangeX) + (-1);

            int _oldMaxSizeY = TackEngine.ScreenHeight;

            float oldRangeY = _oldMaxSizeY - 0;
            float newRangeY = 1 - (-1);

            float finalY = (((m_MousePositionY - 0) * newRangeY) / oldRangeY) + (-1);

            return new Vector2f(finalX, -finalY);*/

            return new Vector2f(m_MousePositionX, m_MousePositionY);
        }

        public static Vector2f MouseCoordsScreenToWorld()
        {
            Vector2f mousePos = MousePosition();
            float xPos = (mousePos.X * (TackEngine.ScreenWidth / 2)) + TackEngine.MainCamera.parentObject.Position.X;
            float yPos = (mousePos.Y * (TackEngine.ScreenHeight / 2)) + TackEngine.MainCamera.parentObject.Position.Y;

            return new Vector2f(xPos, yPos);
        }

        internal static string GetInputBuffer()
        {
            string returnStr = "";

            foreach (char c in m_InputBuffer)
            {
                returnStr += c;
            }

            return returnStr;
        }

        /// <summary>
        /// Gets a character based on a KeyboardKey code.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>Returns and ASCII keycode that represents the letter than has been pressed.
        /// - Returns -1 if backspace has been pressed
        /// - Returns 0 if useless button was pressed (Caps lock, shift, ect)
        /// </returns>
        internal static int FindCharacterFromKeyCode(KeyboardKey _key)
        {
            if (_key == KeyboardKey.CapsLock)
            {
                m_InputBufferCapsLock = !m_InputBufferCapsLock;
                return 0; 
            }

            if (_key == KeyboardKey.BackSpace)
                return 8;

            if (_key == KeyboardKey.Space)
                return 32;

            if (_key >= KeyboardKey.Number0 && _key <= KeyboardKey.Number9)
                return ((int)_key - 61);

            if (_key >= KeyboardKey.A && _key <= KeyboardKey.Z)
            {
                if (m_InputBufferCapsLock)
                    return ((int)_key - 18);
                else
                    return ((int)_key + 14);
            }

            return 0;
        }

        public static void ClearGUIInputBuffer()
        {
            m_InputBuffer.Clear();
        }
    }
}

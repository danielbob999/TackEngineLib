using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using TackEngineLib.Input;
using TackEngineLib.Engine;
using TackEngineLib.GUI;
using TackEngineLib.Main;

namespace TackEngineLib.Main
{
    /// <summary>
    /// 
    /// </summary>
    public class TackConsole
    {
        internal static TackConsole ActiveInstance; // The handle to this instance of TackConsole
        internal static List<string> MessageBacklog = new List<string>();

        private bool m_ConsoleGUIActive = false;
        private KeyboardKey m_ActivationKey;

        private List<string> m_Messages = new List<string>();

        private TextAreaStyle m_ConsoleUIStyle;
        private InputField m_InputField;
        private InputFieldStyle m_InputFieldStyle;
        private string m_InputString;

        internal TackConsole()
        {
            // Set the static instance
            ActiveInstance = this;

            m_InputField = new InputField();
            m_InputFieldStyle = new InputFieldStyle();

            m_InputFieldStyle.BackgroundColour = new Colour4b(200, 200, 200, 255);
            m_InputFieldStyle.FontColour = new Colour4b(0, 0, 0, 255);
            m_InputFieldStyle.SpriteTexture = Sprite.DefaultSprite;
            m_InputFieldStyle.FontSize = 9f;
            m_InputFieldStyle.VerticalAlignment = VerticalAlignment.Middle;

            m_InputField.Shape = new RectangleShape(0, (TackEngine.ScreenHeight * 0.66f) - 30, TackEngine.ScreenWidth, 30);
        }

        internal void OnStart()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            m_ActivationKey = KeyboardKey.Tilde;

            m_ConsoleUIStyle = new TextAreaStyle()
            {
                BackgroundColour = new Colour4b(0, 0, 0, 175),
                FontColour = new Colour4b(0, 255, 0, 255),
                FontFamilyId = 0,
                FontSize = 9f,
                VerticalAlignment = VerticalAlignment.Bottom
            };

            EngineLog(EngineLogType.ModuleStart, "", timer.ElapsedMilliseconds);
            timer.Stop();
        }

        internal void OnUpdate()
        {
            // Check to see if user wants to display the TackConsole GUI
            if (TackInput.InputActiveKeyDown(m_ActivationKey))
            {
                m_ConsoleGUIActive = !m_ConsoleGUIActive;
            }

            // Check to see if user has submitted a command [ENTER]
            if (TackInput.InputActiveKeyDown(KeyboardKey.Enter) && m_ConsoleGUIActive)
            {
                //Console.WriteLine("Hello");
                string consoleInput = m_InputString;

                EngineLog(EngineLogType.Message, "Input: " + consoleInput);
            }

            if (TackInput.MouseButtonDown(MouseButtonKey.Left))
            {
                if (m_InputField.IsMouseInBounds())
                {
                    //Console.WriteLine("Enabled TackConsole InputField input");
                    m_InputField.ReceivingInput = true;
                }
                else
                {
                    //Console.WriteLine("Disabled TackConsole InputField input");
                    m_InputField.ReceivingInput = false;
                }
            }
        }

        internal void OnGUIRender()
        {
            // Only display GUI when m_ConsoleGUIActive=true
            if (m_ConsoleGUIActive)
            {
                string consString = "";

                foreach (string str in m_Messages)
                {
                    consString += str + "\n";
                }

                TackGUI.TextArea(new Main.RectangleShape(0, 0, TackEngine.ScreenWidth, (TackEngine.ScreenHeight * 0.98f) - 30), consString, m_ConsoleUIStyle);

                if (m_InputField.ReceivingInput)
                    m_InputString = m_InputField.GetInput();

                m_InputField.Render(m_InputString, m_InputFieldStyle);
            }
        }

        internal void OnClose()
        {

        }

        public static void Log(string _msg)
        {
            if (string.IsNullOrEmpty(_msg))
                return;

            ActiveInstance.m_Messages.Add(string.Format("{0}:{1:00}:{2:00}.{3:000} {4}",
                DateTime.Now.Hour,
                DateTime.Now.Minute,
                DateTime.Now.Second,
                DateTime.Now.Millisecond,
                _msg));
        }

        public static void EngineLog(EngineLogType _type, string _msg, params object[] _params)
        {
            if (ActiveInstance == null)
            {
                MessageBacklog.Add(string.Format("{0}:{1:00}:{2:00}.{3:000} [{4}] {5}",
                DateTime.Now.Hour,
                DateTime.Now.Minute,
                DateTime.Now.Second,
                DateTime.Now.Millisecond,
                _type.ToString(),
                _msg));
                return;
            }

            if (_type == EngineLogType.ModuleStart)
            {
                StackTrace trace = new StackTrace();

                ActiveInstance.m_Messages.Add(string.Format("{0}:{1:00}:{2:00}.{3:000} [{4}] {5}.{6} ({7}ms)",
                    DateTime.Now.Hour,
                    DateTime.Now.Minute,
                    DateTime.Now.Second,
                    DateTime.Now.Millisecond,
                    _type.ToString(),
                    trace.GetFrame(1).GetMethod().DeclaringType.Name,
                    trace.GetFrame(1).GetMethod().Name,
                    _params[0].ToString()));
                return;
            }


            ActiveInstance.m_Messages.Add(string.Format("{0}:{1:00}:{2:00}.{3:000} [{4}] {5}",
                DateTime.Now.Hour,
                DateTime.Now.Minute,
                DateTime.Now.Second,
                DateTime.Now.Millisecond,
                _type.ToString(),
                _msg));

            Console.WriteLine(string.Format("{0}:{1:00}:{2:00}.{3:000} [{4}] {5}",
                DateTime.Now.Hour,
                DateTime.Now.Minute,
                DateTime.Now.Second,
                DateTime.Now.Millisecond,
                _type.ToString(),
                _msg));
        }
    }
}

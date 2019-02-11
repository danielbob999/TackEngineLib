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
    public class TackConsole
    {
        internal static TackConsole ActiveInstance; // The handle to this instance of TackConsole
        internal static List<string> MessageBacklog = new List<string>();

        private bool m_ConsoleGUIActive = false;
        private KeyboardKey m_ActivationKey;

        private List<string> m_Messages = new List<string>();

        private TextAreaStyle m_ConsoleUIStyle;

        internal TackConsole()
        {
            // Set the static instance
            ActiveInstance = this;
        }

        internal void OnStart()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            m_ActivationKey = KeyboardKey.Tilde;

            m_ConsoleUIStyle = new TextAreaStyle()
            {
                BackgroundColour = new Colour4b(255, 255, 255, 255),
                FontColour = new Colour4b(0, 0, 0, 255),
                FontFamilyId = 0,
                FontSize = 8f,
            };

            EngineLog(EngineLogType.ModuleStart, "", timer.ElapsedMilliseconds);
            timer.Stop();
        }

        internal void OnUpdate()
        {
            // Check to see if user wants to display the TackConsole GUI
            if (TackInput.KeyDown(m_ActivationKey))
            {
                m_ConsoleGUIActive = !m_ConsoleGUIActive;
            }

            // Check to see if user has submitted a command [ENTER]
            if (TackInput.KeyDown(KeyboardKey.Enter) && m_ConsoleGUIActive)
            {
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

                TackGUI.TextArea(new Main.RectangleShape(0, 0, TackEngine.ScreenWidth, TackEngine.ScreenHeight - 30), consString, m_ConsoleUIStyle);
            }
        }

        internal void OnClose()
        {

        }

        public static void Log(string _msg)
        {
            if (string.IsNullOrEmpty(_msg))
                return;

            ActiveInstance.m_Messages.Add(string.Format("{0}:{1:00}:{2:00}:{3:000} {4}",
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
                MessageBacklog.Add(string.Format("{0}:{1:00}:{2:00}:{3:000} [{4}] {5}",
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
                ActiveInstance.m_Messages.Add(string.Format("{0}:{1:00}:{2:00}:{3:000} [{4}] {5}. ({6}ms)",
                    DateTime.Now.Hour,
                    DateTime.Now.Minute,
                    DateTime.Now.Second,
                    DateTime.Now.Millisecond,
                    _type.ToString(),
                    _msg,
                    _params[0].ToString()));
                return;
            }


            ActiveInstance.m_Messages.Add(string.Format("{0}:{1:00}:{2:00}:{3:000} [{4}] {5}",
                DateTime.Now.Hour,
                DateTime.Now.Minute,
                DateTime.Now.Second,
                DateTime.Now.Millisecond,
                _type.ToString(),
                _msg));

            Console.WriteLine(string.Format("{0}:{1:00}:{2:00}:{3:000} [{4}] {5}",
                DateTime.Now.Hour,
                DateTime.Now.Minute,
                DateTime.Now.Second,
                DateTime.Now.Millisecond,
                _type.ToString(),
                _msg));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Reflection;

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
        private List<TackCommand> m_ValidCommands = new List<TackCommand>();

        private TextAreaStyle m_ConsoleUIStyle;
        private InputField m_InputField;
        private InputFieldStyle m_InputFieldStyle;
        private BoxStyle m_CaretBoxStyle;
        private string m_InputString;

        internal TackConsole()
        {
            // Set the static instance
            ActiveInstance = this;

            m_InputField = new InputField();
            m_InputFieldStyle = new InputFieldStyle();

            m_InputField.SubmitInput += ProcessCommand;

            m_InputFieldStyle.BackgroundColour = new Colour4b(200, 200, 200, 255);
            m_InputFieldStyle.FontColour = new Colour4b(0, 0, 0, 255);
            m_InputFieldStyle.SpriteTexture = Sprite.DefaultSprite;
            m_InputFieldStyle.FontSize = 10f;
            m_InputFieldStyle.VerticalAlignment = VerticalAlignment.Middle;
            m_InputFieldStyle.FontFamilyId = TackGUI.LoadFontFromFile(Environment.GetFolderPath(Environment.SpecialFolder.Fonts) + "\\cour.ttf");
        }

        internal void OnStart()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            m_ActivationKey = KeyboardKey.Tilde;

            m_ConsoleUIStyle = new TextAreaStyle()
            {
                BackgroundColour = new Colour4b(0, 0, 0, 190),
                FontColour = new Colour4b(0, 255, 0, 255),
                FontFamilyId = TackGUI.GetFontFamilyId("Courier New"),
                FontSize = 10f,
                VerticalAlignment = VerticalAlignment.Middle,
                ScrollPosition = 0,
                Scrollable = false
            };

            m_CaretBoxStyle = new BoxStyle()
            {
                Colour = new Colour4b(255, 0, 0, 255),
            };

            GetCommandsFromAssembly(typeof(TackEngine).Assembly.FullName);

            EngineLog(EngineLogType.ModuleStart, "", timer.ElapsedMilliseconds);
            timer.Stop();
        }

        internal void OnUpdate()
        {
            m_InputField.Shape = new RectangleShape(0, (TackEngine.ScreenHeight * 0.7f), TackEngine.ScreenWidth, 30);
            m_InputField.Update();

            // Check to see if user wants to display the TackConsole GUI
            if (TackInput.InputActiveKeyDown(m_ActivationKey))
            {
                m_ConsoleGUIActive = !m_ConsoleGUIActive;
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

            if (TackInput.KeyDown(KeyboardKey.PageUp))
            {
                if (m_ConsoleUIStyle.ScrollPosition < m_Messages.Count - 1)
                    m_ConsoleUIStyle.ScrollPosition += 1.0f;
            }

            if (TackInput.KeyDown(KeyboardKey.PageDown))
            {
                if (m_ConsoleUIStyle.ScrollPosition > 0)
                    m_ConsoleUIStyle.ScrollPosition -= 1.0f;
            }

            m_InputString = m_InputField.InputString;
        }

        internal void OnGUIRender()
        {
            // Only display GUI when m_ConsoleGUIActive=true
            if (m_ConsoleGUIActive)
            {
                int nextPosition = (int)(TackEngine.ScreenHeight * 0.70f) - 14;

                for (int i = m_Messages.Count - (1 + (int)m_ConsoleUIStyle.ScrollPosition); i > 0; i--)
                {
                    TackGUI.TextArea(new Main.RectangleShape(0, nextPosition, TackEngine.ScreenWidth, 14), m_Messages[i], m_ConsoleUIStyle);

                    nextPosition -= 14;
                }

                while (nextPosition > -14)
                {
                    TackGUI.Box(new RectangleShape(0, nextPosition, TackEngine.ScreenWidth, 14), new BoxStyle() { Colour = new Colour4b(0, 0, 0, 190) });
                    nextPosition -= 14;
                }

                /*
                for (int i = m_Messages.Count - 1; i > m_Messages.Count - (1 + m_ConsoleUIStyle.ScrollPosition); i--)
                {
                    //Console.WriteLine("Rendering message[" + i + "] at PosY: " + nextPosition);
                    TackGUI.TextArea(new Main.RectangleShape(0, nextPosition, TackEngine.ScreenWidth, 14), m_Messages[i], m_ConsoleUIStyle);

                    nextPosition -= 14;
                }*/

                //TackGUI.TextArea(new Main.RectangleShape(0, nextPosition - 14, TackEngine.ScreenWidth, 14), m_Messages[m_Messages.Count - 1], m_ConsoleUIStyle);

                m_InputField.Render(m_InputFieldStyle);
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

        private void GetCommandsFromAssembly(string a_assemblyName)
        {
            Assembly assembly;

            try
            {
                assembly = Assembly.Load(a_assemblyName);
            }
            catch (Exception e)
            {
                EngineLog(EngineLogType.Error, "Failed to load assembly with name: " + a_assemblyName);
                EngineLog(EngineLogType.Error, e.Message);
                return;
            }

            int i = 0;

            EngineLog(EngineLogType.Message, "Looking for ConsoleMethods in Assembly: " + assembly.FullName);
            foreach (Type classType in assembly.GetTypes())
            {
                foreach (MethodInfo methodInfo in classType.GetMethods())
                {
                    foreach (Attribute methodAttribute in methodInfo.GetCustomAttributes())
                    {
                        if (methodAttribute.GetType() == typeof(CommandMethod))
                        {
                            //Console.WriteLine("Class: {0}, Method: {1}, Attribute: {3}", classType.Name, methodInfo.Name, methodAttribute.GetType().Name);
                            m_ValidCommands.Add(new TackCommand(((CommandMethod)methodAttribute).GetCallString(), (EngineDelegates.CommandDelegate)methodInfo.CreateDelegate(typeof(EngineDelegates.CommandDelegate)), ((CommandMethod)methodAttribute).GetArgList().ToList()));
                            i++;
                        }
                    }
                }
            }

            EngineLog(EngineLogType.Message, "Found " + i + " valid CommandMethods in Assembly: " + assembly.FullName);
        }

        private void ProcessCommand(object sender, EventArgs e)
        {
            string commandInput = m_InputString;
            EngineLog(EngineLogType.Message, ">" + commandInput);

            string[] splitCommandBySpaces = commandInput.Split(' ');

            foreach (TackCommand command in m_ValidCommands)
            {
                if (splitCommandBySpaces[0] == command.CommandCallString)
                {
                    command.CommandDelegate.Invoke(splitCommandBySpaces);
                }
            }

            m_InputField.InputString = "";
        }

        [CommandMethod("help", "", "<string:commandName>")]
        public static void Help(string[] a_args)
        {
            if (a_args.Length == 1)
            {
                EngineLog(EngineLogType.Message, "Commands:");

                foreach (TackCommand command in ActiveInstance.m_ValidCommands)
                {
                    Console.WriteLine(command.CommandCallString);
                    EngineLog(EngineLogType.Message, "     " + command.CommandCallString);
                }
            }

            if (a_args.Length == 2)
            {
                TackCommand com = null;

                foreach (TackCommand command in ActiveInstance.m_ValidCommands)
                {
                    if (a_args[1] == command.CommandCallString)
                    {
                        com = command;
                    }
                }

                if (com != null)
                {
                    EngineLog(EngineLogType.Message, com.CommandCallString + ":");

                    foreach (string overloadArgs in com.CommandArgList)
                    {
                        EngineLog(EngineLogType.Message, "     " + overloadArgs);
                    }
                }

                return;
            }
        }

        [CommandMethod("testmeth", "<int>", "<float> <int>")]
        public static void TestMeth(string[] a_args)
        {

        }
    }
}

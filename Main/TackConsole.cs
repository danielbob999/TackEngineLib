/* Copyright (c) 2019 Daniel Phillip Robinson */
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

        private bool mConsoleGUIActive = false;
        private KeyboardKey mActivationKey;

        private List<string> mMessages = new List<string>();
        private List<TackCommand> mValidCommands = new List<TackCommand>();
        private List<string> mPreviousCommands = new List<string>();
        private int mPreviousCommandsIndex = -1;
        private bool mPreviousCommandInputLocker = false;
        private string m_logPath;
        private bool m_allowLoggingToFile = true;

        private TextAreaStyle mConsoleUIStyle; // Style used for the output TextArea
        private InputFieldStyle mInputFieldStyle;
        private BoxStyle mCaretBoxStyle;
        private string mInputString;

        // Implement the new GUI system
        private GUITextArea mConsoleTextArea;
        private GUIInputField mConsoleInputField;

        /// <summary>
        /// Gets or Sets whether the ActiveInstance of TackConsole has logging to file enable. 
        /// If Get is called and ActiveInstance is null, false is returned
        /// </summary>
        public static bool EnableLoggingToFile {
            get {
                if (ActiveInstance != null) {
                    return ActiveInstance.m_allowLoggingToFile;
                } else {
                    return false;
                }
            }

            set {
                if (ActiveInstance != null) {
                    ActiveInstance.m_allowLoggingToFile = value;
                }
            }
        }

        internal TackConsole()
        {
            // Set the static instance
            ActiveInstance = this;

            m_logPath = string.Format("logs/log_{0}_{1}_{2}.txt", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);

            if (!Directory.Exists(Directory.GetCurrentDirectory() + "/logs")) {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/logs");
            }

            mInputFieldStyle = new InputFieldStyle();
            mInputFieldStyle.BackgroundColour = new Colour4b(100, 100, 100, 190);
            mInputFieldStyle.FontColour = new Colour4b(0, 0, 0, 255);
            mInputFieldStyle.SpriteTexture = Sprite.DefaultSprite;
            mInputFieldStyle.FontSize = 10f;
            mInputFieldStyle.VerticalAlignment = VerticalAlignment.Middle;
            mInputFieldStyle.FontFamilyId = 0; //TackGUI.LoadFontFromFile(Environment.GetFolderPath(Environment.SpecialFolder.Fonts) + "\\cour.ttf");
            mInputFieldStyle.Scrollable = false;

            mActivationKey = KeyboardKey.Tilde;

            mConsoleUIStyle = new TextAreaStyle() {
                BackgroundColour = new Colour4b(0, 0, 0, 190),
                FontColour = new Colour4b(0, 255, 0, 255),
                FontFamilyId = TackGUI.GetFontFamilyId("Courier New"),
                FontSize = 10f,
                VerticalAlignment = VerticalAlignment.Top,
                ScrollPosition = 0,
                Scrollable = true
            };

            mCaretBoxStyle = new BoxStyle() {
                Colour = new Colour4b(255, 0, 0, 255),
            };

            mConsoleInputField = null;
            mConsoleTextArea = null;
            mConsoleGUIActive = true;
        }

        internal void OnStart() { 
            //Stopwatch timer = new Stopwatch();
            //timer.Start();

            GetCommandsFromAssembly(typeof(TackEngine).Assembly.FullName);

            //EngineLog(EngineLogType.ModuleStart, "", timer.ElapsedMilliseconds);
            //timer.Stop();
        }

        internal void OnUpdate()
        {
            if (mConsoleTextArea == null) {
                mConsoleTextArea = new GUITextArea() {
                    Bounds = new RectangleShape(5, 5, TackEngine.ScreenWidth - 10, 400),
                    NormalStyle = new GUITextArea.GUITextAreaStyle() {
                        Colour = new Colour4b(0, 0, 0, 255),
                        Texture = Sprite.DefaultSprite,
                        FontColour = new Colour4b(0, 255, 0, 255),
                        FontSize = 9f,
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Left
                    },

                    HoverStyle = new GUITextArea.GUITextAreaStyle() {
                        Colour = new Colour4b(0, 0, 0, 255),
                        Texture = Sprite.DefaultSprite,
                        FontColour = new Colour4b(0, 255, 0, 255),
                        FontSize = 9f,
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Left
                    },
                    Text = "",
                    Active = true
                };
            }

            if (mConsoleInputField == null) {
                mConsoleInputField = new GUIInputField() {
                    Bounds = new RectangleShape(5, 415, TackEngine.ScreenWidth - 10, 25),
                    Text = "",
                    Active = true
                };

                mConsoleInputField.OnSubmit += ProcessCommand;
            }

            mConsoleTextArea.Active = mConsoleGUIActive;
            mConsoleInputField.Active = mConsoleGUIActive;
            mConsoleTextArea.Bounds = new RectangleShape(5, 5, TackEngine.ScreenWidth - 10, 400);
            mConsoleInputField.Bounds = new RectangleShape(5, 410, TackEngine.ScreenWidth - 10, 25);

            // Check to see if user wants to display the TackConsole GUI
            if (TackInput.InputActiveKeyDown(mActivationKey))
            {
                mConsoleGUIActive = !mConsoleGUIActive;
            }

            if (TackInput.KeyDown(KeyboardKey.PageDown))
            {
                if (mConsoleUIStyle.ScrollPosition < mMessages.Count - 1)
                    mConsoleUIStyle.ScrollPosition += 1.0f;
            }

            if (TackInput.KeyDown(KeyboardKey.PageUp))
            {
                if (mConsoleUIStyle.ScrollPosition > 0)
                    mConsoleUIStyle.ScrollPosition -= 1.0f;
            }

            //mInputString = mInputField.InputString;
        }

        internal void OnGUIRender()
        {
            // Only update GUI when mConsoleGUIActive=true
            if (mConsoleGUIActive) {
                string consoleString = "";

                foreach (string str in mMessages) {
                    consoleString += (str + "\n");
                }

                if (mConsoleTextArea != null) {
                    mConsoleTextArea.Text = consoleString;
                }
            }
        }

        internal void OnClose() {
            if (m_allowLoggingToFile) {
                File.AppendAllLines(Directory.GetCurrentDirectory() + "/" + m_logPath, mMessages);
            }
        }

        public static void Log(string _msg)
        {
            if (string.IsNullOrEmpty(_msg))
                return;

            ActiveInstance.mMessages.Add(string.Format("{0}:{1:00}:{2:00}.{3:000} {4}",
                DateTime.Now.Hour,
                DateTime.Now.Minute,
                DateTime.Now.Second,
                DateTime.Now.Millisecond,
                _msg));
        }

        public static void EngineLog(EngineLogType _type, string _msg, params object[] _params)
        {
            string formattedString = string.Format(_msg, _params);

            if (ActiveInstance == null)
            {
                MessageBacklog.Add(string.Format("{0}:{1:00}:{2:00}.{3:000} [{4}] {5}",
                DateTime.Now.Hour,
                DateTime.Now.Minute,
                DateTime.Now.Second,
                DateTime.Now.Millisecond,
                _type.ToString(),
                formattedString));
                return;
            }

            if (_type == EngineLogType.ModuleStart)
            {
                StackTrace trace = new StackTrace();

                ActiveInstance.mMessages.Add(string.Format("{0}:{1:00}:{2:00}.{3:000} [{4}] {5}.{6} ({7}ms)",
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


            ActiveInstance.mMessages.Add(string.Format("{0}:{1:00}:{2:00}.{3:000} [{4}] {5}",
                DateTime.Now.Hour,
                DateTime.Now.Minute,
                DateTime.Now.Second,
                DateTime.Now.Millisecond,
                _type.ToString(),
                formattedString));

            Console.WriteLine(string.Format("{0}:{1:00}:{2:00}.{3:000} [{4}] {5}",
                DateTime.Now.Hour,
                DateTime.Now.Minute,
                DateTime.Now.Second,
                DateTime.Now.Millisecond,
                _type.ToString(),
                formattedString));
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
                            mValidCommands.Add(new TackCommand(((CommandMethod)methodAttribute).GetCallString(), (EngineDelegates.CommandDelegate)methodInfo.CreateDelegate(typeof(EngineDelegates.CommandDelegate)), ((CommandMethod)methodAttribute).GetArgList().ToList()));
                            i++;
                        }
                    }
                }
            }

            EngineLog(EngineLogType.Message, "Found " + i + " valid CommandMethods in Assembly: " + assembly.FullName);
        }

        private void ProcessCommand(object sender, string input)
        {
            if (string.IsNullOrEmpty(input)) {
                EngineLog(EngineLogType.Message, "Command input string is null or empty");
                return;
            }

            string commandInput = input;
            EngineLog(EngineLogType.Message, "> " + commandInput);
            mPreviousCommands.Add(commandInput);
            mPreviousCommandsIndex = -1;

            string[] splitCommandBySpaces = commandInput.Split(' ');

            foreach (TackCommand command in mValidCommands) {
                if (splitCommandBySpaces[0] == command.CommandCallString) {
                    command.CommandDelegate.Invoke(splitCommandBySpaces);
                    return;
                }
            }

            EngineLog(EngineLogType.Message, "No valid TackCommand with call string '" + splitCommandBySpaces[0] + "'");
            EngineLog(EngineLogType.Message, "Use 'help' to get a list of valid commands");

            mConsoleInputField.Text = "";
            mConsoleInputField.SelectionStart = 0;
        }

        internal static List<TackCommand> GetLoadedTackCommands() {
            return ActiveInstance.mValidCommands;
        }
    }
}

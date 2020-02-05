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

        private TextAreaStyle mConsoleUIStyle;
        private InputField mInputField;
        private InputFieldStyle mInputFieldStyle;
        private BoxStyle mCaretBoxStyle;
        private string mInputString;

        internal TackConsole()
        {
            // Set the static instance
            ActiveInstance = this;

            mInputField = new InputField();
            mInputFieldStyle = new InputFieldStyle();

            mInputField.SubmitInput += ProcessCommand;

            mInputFieldStyle.BackgroundColour = new Colour4b(100, 100, 100, 190);
            mInputFieldStyle.FontColour = new Colour4b(0, 0, 0, 255);
            mInputFieldStyle.SpriteTexture = Sprite.DefaultSprite;
            mInputFieldStyle.FontSize = 10f;
            mInputFieldStyle.VerticalAlignment = VerticalAlignment.Middle;
            mInputFieldStyle.FontFamilyId = 0; //TackGUI.LoadFontFromFile(Environment.GetFolderPath(Environment.SpecialFolder.Fonts) + "\\cour.ttf");
            mInputFieldStyle.Scrollable = false;
        }

        internal void OnStart()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            mActivationKey = KeyboardKey.Tilde;

            mConsoleUIStyle = new TextAreaStyle()
            {
                BackgroundColour = new Colour4b(0, 0, 0, 190),
                FontColour = new Colour4b(0, 255, 0, 255),
                FontFamilyId = TackGUI.GetFontFamilyId("Courier New"),
                FontSize = 10f,
                VerticalAlignment = VerticalAlignment.Top,
                ScrollPosition = 0,
                Scrollable = true
            };

            mCaretBoxStyle = new BoxStyle()
            {
                Colour = new Colour4b(255, 0, 0, 255),
            };

            GetCommandsFromAssembly(typeof(TackEngine).Assembly.FullName);

            EngineLog(EngineLogType.ModuleStart, "", timer.ElapsedMilliseconds);
            timer.Stop();
        }

        internal void OnUpdate()
        {
            mInputField.Shape = new RectangleShape(0, (TackEngine.ScreenHeight * 0.7f), TackEngine.ScreenWidth, 30);
            mInputField.Update();

            // Check to see if user wants to display the TackConsole GUI
            if (TackInput.InputActiveKeyDown(mActivationKey))
            {
                mConsoleGUIActive = !mConsoleGUIActive;
            }

            if (TackInput.MouseButtonDown(MouseButtonKey.Left))
            {
                if (mInputField.IsMouseInBounds())
                {
                    //Console.WriteLine("Enabled TackConsole InputField input");
                    mInputField.ReceivingInput = true;
                }
                else
                {
                    //Console.WriteLine("Disabled TackConsole InputField input");
                    mInputField.ReceivingInput = false;
                }
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

            if (TackInput.KeyDown(KeyboardKey.Up)) {
                if (mPreviousCommands.Count > 0) {
                    if (mPreviousCommandsIndex == -1) {
                        mPreviousCommandsIndex = mPreviousCommands.Count - 1;
                        mInputField.InputString = mPreviousCommands[mPreviousCommandsIndex];
                    } else {
                        mPreviousCommandsIndex -= 1;

                        if (mPreviousCommandsIndex < 0)
                            mPreviousCommandsIndex = 0;

                        mInputField.InputString = mPreviousCommands[mPreviousCommandsIndex];
                    }
                }
            }

            if (TackInput.KeyDown(KeyboardKey.Down)) {
                if (mPreviousCommands.Count > 0) {
                    if (mPreviousCommandsIndex == -1) {
                        mPreviousCommandsIndex = 0;
                        mInputField.InputString = mPreviousCommands[mPreviousCommandsIndex];
                    } else {
                        mPreviousCommandsIndex += 1;

                        if (mPreviousCommandsIndex > (mPreviousCommands.Count - 1))
                            mPreviousCommandsIndex = (mPreviousCommands.Count - 1);

                        mInputField.InputString = mPreviousCommands[mPreviousCommandsIndex];
                    }
                }
            }

            mInputString = mInputField.InputString;
        }

        internal void OnGUIRender()
        {
            // Only display GUI when mConsoleGUIActive=true
            if (mConsoleGUIActive)
            {
                int nextPosition = (int)(TackEngine.ScreenHeight * 0.70f) - 14;

                string consoleString = "";

                foreach (string str in mMessages)
                {
                    consoleString += (str + "\n");
                }

                TackGUI.TextArea(new Main.RectangleShape(0, 0, TackEngine.ScreenWidth, (TackEngine.ScreenHeight * 0.70f)), consoleString, mConsoleUIStyle);

                mInputField.Render(mInputFieldStyle);
            }
        }

        internal void OnClose()
        {

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

        private void ProcessCommand(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(mInputString)) {
                EngineLog(EngineLogType.Message, "Command input string is null or empty");
                return;
            }

            string commandInput = mInputString;
            EngineLog(EngineLogType.Message, "> " + commandInput);
            mPreviousCommands.Add(commandInput);
            mPreviousCommandsIndex = -1;

            string[] splitCommandBySpaces = commandInput.Split(' ');

            foreach (TackCommand command in mValidCommands) {
                if (splitCommandBySpaces[0] == command.CommandCallString) {
                    command.CommandDelegate.Invoke(splitCommandBySpaces);

                    mInputField.InputString = "";
                    return;
                }
            }

            EngineLog(EngineLogType.Message, "No valid TackCommand with call string '" + splitCommandBySpaces[0] + "'");
            EngineLog(EngineLogType.Message, "Use 'help' to get a list of valid commands");
            mInputField.InputString = "";
        }

        internal static List<TackCommand> GetLoadedTackCommands() {
            return ActiveInstance.mValidCommands;
        }
    }
}

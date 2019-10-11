/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using TackEngineLib.Main;
using TackEngineLib.Objects;
using TackEngineLib.Objects.Components;

namespace TackEngineLib.Engine
{
    /// <summary>
    /// The main engine class
    /// </summary>
    public class TackEngine
    {
        private const int VERSION_MAJOR = 1;
        private const int VERSION_MINOR = 0;
        private const int VERSION_PATCH = 3;
        private const string VERSION_DESC = "AlphaBuild";

        internal static TackGameWindow currentWindow;

        internal static int mFramesPerSecond;
        internal static int mUpdateCyclesPerSecond;
        internal static TackObject mMainCameraTackObject;

        public static int RenderCyclesPerSecond
        {
            get { return mFramesPerSecond; }
        }

        public static int UpdateCyclesPerSecond
        {
            get { return mUpdateCyclesPerSecond;  }
        }
        
        public static int ScreenWidth {
            get { return currentWindow.Width; }
        }

        public static int ScreenHeight {
            get { return currentWindow.Height; }
        }

        public static Camera MainCamera {
            get {
                if (mMainCameraTackObject == null)
                {
                    TackConsole.EngineLog(EngineLogType.Error, "The active Camera TackObject is currently equal to null. Creating a new TackObject...");

                    mMainCameraTackObject = new TackObject("MainCamera", new Vector2f(0, 0));
                }

                if (mMainCameraTackObject != null && mMainCameraTackObject.GetComponent<Camera>() == null)
                {
                    TackConsole.EngineLog(EngineLogType.Error, "The active Camera TackObject does not have a component of type TackComponent.Camera. Adding one...");

                    Camera newCamera = new Camera();
                    newCamera.CameraScreenHeight = ScreenHeight;
                    newCamera.CameraScreenWidth = ScreenWidth;

                    mMainCameraTackObject.AddComponent(newCamera);
                }

                return mMainCameraTackObject.GetComponent<Camera>();
            }
        }

        /// <summary>
        /// Initialises a new TackEngine instance using the given parameters
        /// </summary>
        /// <param name="_windowWidth">The width of the window</param>
        /// <param name="_windowHeight">The height of the window</param>
        /// <param name="_updatesPerSec">The targeted amount of update cycles per second</param>
        /// <param name="_framesPerSec">The targeted amount of frames rendered per second</param>
        /// <param name="_vsync">Whether v-sync should be enable. If true, will ignore _framesPerSec argument</param>
        /// <param name="_windowName">The title of the window</param>
        /// <param name="_st">The method called on engine startup</param>
        /// <param name="_up">The method called every update cycle</param>
        /// <param name="_guirend">The method called every time a frame is rendered</param>
        /// <param name="_clos">The method called on engine shutdown</param>
        public static void Init(int _windowWidth, int _windowHeight, int _updatesPerSec, int _framesPerSec, bool _vsync, string _windowName, EngineDelegates.OnStart _st, EngineDelegates.OnUpdate _up, EngineDelegates.OnGUIRender _guirend, EngineDelegates.OnClose _clos)
        {
            // Create new window
            NewGameWindow(_windowWidth, _windowHeight, _updatesPerSec, _framesPerSec, _windowName, _st, _up, _guirend, _clos);

            if (_vsync)
                currentWindow.VSync = OpenTK.VSyncMode.On;
            else
                currentWindow.VSync = OpenTK.VSyncMode.Off;

            currentWindow.Run(_updatesPerSec, _framesPerSec);
        }

        private static void NewGameWindow(int _w, int _h, int _u_s, int _f_s, string _n, EngineDelegates.OnStart _s, EngineDelegates.OnUpdate _u, EngineDelegates.OnGUIRender _r, EngineDelegates.OnClose _c) {
            if (currentWindow == null) {
                currentWindow = new TackGameWindow(_w, _h, _n, _s, _u, _r, _c);

                TackConsole.EngineLog(EngineLogType.Message, "Successfully created new CustomGameWindow instance");
                return;
            }

            TackConsole.EngineLog(EngineLogType.Error, "There is already a CustomGameWindow instance running in this session");
        }

        public static TackEngineVersion GetEngineVersion()
        {
            return new TackEngineVersion(VERSION_MAJOR, VERSION_MINOR, VERSION_PATCH, VERSION_DESC, 0);
        }
    }
}

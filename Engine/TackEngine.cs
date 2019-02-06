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
    public class TackEngine
    {
        private const int VERSION_MAJOR = 0;
        private const int VERSION_MINOR = 8;
        private const int VERSION_PATCH = 0;
        private const string VERSION_DESC = "AlphaBuild";
        private const int BUILD_NUMBER = 3;

        internal static TackGameWindow currentWindow;

        internal static int m_FramesPerSecond;
        internal static int m_UpdateCyclesPerSecond;
        internal static TackObject m_MainCameraTackObject;

        // Properties
        public static int RenderCyclesPerSecond
        {
            get { return m_FramesPerSecond; }
        }

        public static int UpdateCyclesPerSecond
        {
            get { return m_UpdateCyclesPerSecond;  }
        }
        
        public static int ScreenWidth {
            get { return currentWindow.Width; }
        }

        public static int ScreenHeight {
            get { return currentWindow.Height; }
        }

        public static Camera MainCamera
        {
            get { return m_MainCameraTackObject.GetComponent<Camera>(); }
        }

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

        private static void NewGameWindow(int _w, int _h, int _u_s, int _f_s, string _n, EngineDelegates.OnStart _s, EngineDelegates.OnUpdate _u, EngineDelegates.OnGUIRender _r, EngineDelegates.OnClose _c)
        {
            if (currentWindow == null)
            {
                currentWindow = new TackGameWindow(_w, _h, _n, _s, _u, _r, _c);
                
                TackConsole.EngineLog(EngineLogType.Message, "Successfully created new CustomGameWindow instance");
                return;
            }

            TackConsole.EngineLog(EngineLogType.Error, "There is already a CustomGameWindow instance running in this session");
        }

        [Obsolete]
        public static string GetEngineVersionStr()
        {
            return string.Format("{0}.{1}.{2} {3}", VERSION_MAJOR, VERSION_MINOR, VERSION_PATCH, VERSION_DESC);
        }

        public static TackEngineVersion GetEngineVersion()
        {
            return new TackEngineVersion(VERSION_MAJOR, VERSION_MINOR, VERSION_PATCH, VERSION_DESC, BUILD_NUMBER);
        }
    }
}

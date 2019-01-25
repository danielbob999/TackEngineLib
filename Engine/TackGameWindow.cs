using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Input;

using TackEngineLib.Main;
using TackEngineLib.Objects;
using TackEngineLib.Input;
using TackEngineLib.Renderer;
using TackEngineLib.Renderer.Shaders;
using TackEngineLib.GUI;
using TackEngineLib.Physics;
using TackEngineLib.Audio;

namespace TackEngineLib.Engine
{
    internal class TackGameWindow : GameWindow
    {
        // Reference to the current GameWindow instance. CANNOT BE CHANGED
        private GameWindow gameWindowRef;

        private EngineDelegates.OnStart onStartFunction;
        private EngineDelegates.OnUpdate onUpdateFunction;
        private EngineDelegates.OnGUIRender onGUIRenderFunction;
        private EngineDelegates.OnClose onCloseFunction;

        // Modules
        private AudioManager m_AudioManager;
        private TackConsole m_TackConsole;
        //private TackPhysics m_TackPhysics;
        private TackObjectManager m_TackObjectManager;
        private TackRenderer m_TackRender;

        private Stopwatch updateTimer;
        private Stopwatch frameTimer;
        //private long elapsedTicks = 0;
        //private long lastElapsedTicks = 0;
        private int updatesPerSec;
        private int framesPerSec;

        private static int colourShaderProgramId;
        private static int imageShaderProgramId;

        // Properties
        public static int ColourShaderProgramInt { get { return colourShaderProgramId; } }
        public static int ImageShaderProgramInt { get { return imageShaderProgramId; } }

        public TackGameWindow(int _width, int _height, string _n, EngineDelegates.OnStart _strtFunc, EngineDelegates.OnUpdate _updtFunc, EngineDelegates.OnGUIRender _guiRendFunc, EngineDelegates.OnClose _onCloseFunc) : base(_width, _height, GraphicsMode.Default, _n)
        {
            onStartFunction = _strtFunc;
            onUpdateFunction = _updtFunc;
            onGUIRenderFunction = _guiRendFunc;
            onCloseFunction = _onCloseFunc;

            MainScreenWindow.Width = _width;
            MainScreenWindow.Height = _height;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            m_TackConsole = new TackConsole();
            m_TackConsole.OnStart();

            TackConsole.EngineLog(EngineLogType.Message, "Starting TackEngine.");
            TackConsole.EngineLog(EngineLogType.Message, string.Format("EngineVersion: {0}", TackEngine.GetEngineVersion().ToString()));

            m_AudioManager = new AudioManager();
            m_AudioManager.OnStart();

            m_TackObjectManager = new TackObjectManager();
            m_TackObjectManager.OnStart();

            m_TackRender = new TackRenderer();
            m_TackRender.OnStart();

            // OpenGL stuffs
            //GL.Viewport(0, 0, Width, Height);

            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadIdentity();
            //GL.Ortho(0, Width, Height, 0, 0, 1);


            //colourShaderProgramId = ShaderFunctions.CompileAndLinkShaders(TackShaderType.ColourShader);
            //imageShaderProgramId = ShaderFunctions.CompileAndLinkShaders(TackShaderType.ImageShader);

            // All OnStart here
            TackInput.OnStart();

            TackGUI.OnStart();

            //testTex = Sprite.LoadFromFile("Resources/DabEmoji.bmp");
            //testTex.Create();

            onStartFunction();

            updateTimer = new Stopwatch();
            updateTimer.Start();

            m_TackObjectManager.RunTackObjectStartMethods();

            // Start the TackPhysics thread
            Thread physicsThread = new Thread(TackPhysics.Init);
            physicsThread.Start(TargetUpdateFrequency);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            onUpdateFunction();

            // All OnUpdate here
            m_TackObjectManager.OnUpdate();
            m_TackObjectManager.RunTackObjectUpdateMethods();

            m_TackConsole.OnUpdate();
            TackInput.OnUpdate();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(new OpenTK.Graphics.Color4(255, 0, 0, 255));

            // All OnRender here
            m_TackRender.OnRender();

            onGUIRenderFunction(); // This function should be called after all rendering. This means text will render above other objects
            m_TackConsole.OnGUIRender(); // TackConsole should be rendered above everything else, including the onGUIRenderFunction

            MainScreenWindow.FramesPerSecond = (int)RenderFrequency;

            this.SwapBuffers();
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);

            onCloseFunction();

            TackPhysics.Stop();

            m_AudioManager.OnClose();
            SpriteManager.OnClose();
            m_TackConsole.OnClose();
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            TackInput.KeyDownEvent((KeyboardKey)e.Key);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);

            TackInput.KeyUpEvent((KeyboardKey)e.Key);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            //Console.WriteLine(e.Button);

            TackInput.MouseDownEvent((MouseButtonKey)e.Button);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            TackInput.MouseUpEvent((MouseButtonKey)e.Button);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);

            TackInput.MouseMoveEvent(e.X, e.Y);
        }

        public void SetGameWindowRef(ref GameWindow _win)
        {
            gameWindowRef = _win;
        }
    }
}

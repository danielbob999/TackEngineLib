/* Copyright (c) 2019 Daniel Phillip Robinson */
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
        private static TackGameWindow ActiveInstance;

        private EngineDelegates.OnStart onStartFunction;
        private EngineDelegates.OnUpdate onUpdateFunction;
        private EngineDelegates.OnGUIRender onGUIRenderFunction;
        private EngineDelegates.OnClose onCloseFunction;

        // Modules
        private AudioManager mAudioManager;
        private TackConsole mTackConsole;
        private TackPhysics mTackPhysics;
        private TackObjectManager mTackObjectManager;
        private TackRenderer mTackRender;

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

        public TackGameWindow(int _width, int _height, string _n, EngineDelegates.OnStart _strtFunc, EngineDelegates.OnUpdate _updtFunc, EngineDelegates.OnGUIRender _guiRendFunc, EngineDelegates.OnClose _onCloseFunc) 
            : base(_width, _height, GraphicsMode.Default, _n)
        {
            onStartFunction = _strtFunc;
            onUpdateFunction = _updtFunc;
            onGUIRenderFunction = _guiRendFunc;
            onCloseFunction = _onCloseFunc;

            ActiveInstance = this;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Sprite.LoadDefaultSprite();

            TackGUI.OnStart();

            mTackConsole = new TackConsole();
            mTackConsole.OnStart();

            TackConsole.EngineLog(EngineLogType.Message, "Starting TackEngine.");
            TackConsole.EngineLog(EngineLogType.Message, string.Format("EngineVersion: {0}", TackEngine.GetEngineVersion().ToString()));

            mAudioManager = new AudioManager();
            mAudioManager.OnStart();

            mTackObjectManager = new TackObjectManager();
            mTackObjectManager.OnStart();

            mTackRender = new TackRenderer();
            mTackRender.OnStart();

            mTackPhysics = new TackPhysics();
            mTackPhysics.Start();

            TackInput.OnStart();

            onStartFunction();

            updateTimer = new Stopwatch();
            updateTimer.Start();

            mTackObjectManager.RunTackObjectStartMethods();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            onUpdateFunction();

            // All OnUpdate here
            mTackPhysics.Update();      // If issues arise, try running this below RunTackObjectUpdateMethods()
            mTackObjectManager.OnUpdate();
            mTackObjectManager.RunTackObjectUpdateMethods();

            mTackConsole.OnUpdate();
            TackInput.OnUpdate();

            TackEngine.mUpdateCyclesPerSecond = (int)UpdateFrequency;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(TackRenderer.BackgroundColour.ConvertToOpenGLColor4());

            // All OnRender here
            mTackRender.OnRender();

            onGUIRenderFunction(); // This function should be called after all rendering. This means text will render above other objects
            mTackConsole.OnGUIRender(); // TackConsole should be rendered above everything else, including the onGUIRenderFunction
            mTackRender.RenderFpsCounter();

           TackEngine.mFramesPerSecond = (int)RenderFrequency;

            this.SwapBuffers();
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);

            onCloseFunction();

            mTackPhysics.Close();

            mAudioManager.OnClose();
            SpriteManager.OnClose();
            mTackConsole.OnClose();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            TackEngine.MainCamera.UpdateCameraDimensions(Width, Height);
            GL.Viewport(0, 0, Width, Height);
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

        public static void RestartModule(string moduleName, bool keepState) {
            if (moduleName == "TackRenderer") {
                ActiveInstance.mTackRender.OnClose();
                ActiveInstance.mTackRender = new TackRenderer();
                ActiveInstance.mTackRender.OnStart();
                return;
            }
        }
    }
}

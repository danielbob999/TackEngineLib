/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;

using OpenTK.Graphics.OpenGL;

using TackEngineLib.Main;
using TackEngineLib.Engine;
using TackEngineLib.Objects;
using TackEngineLib.Objects.Components;
using TackEngineLib.Renderer.Shaders;
using TackEngineLib.GUI;

namespace TackEngineLib.Renderer
{
    internal class TackRenderer
    {
        private static TackRenderer ActiveInstance;

        private List<Shader> m_loadedShaders;
        private Shader m_defaultWorldShader;
        private Shader m_defaultGUIShader;
        private float[] mVertexData;
        private bool mRenderFpsCounter;
        private TextAreaStyle mFpsCounterStyle;
        private Colour4b mBackgroundColour;
        private TackGUI m_guiInstance;

        public static Colour4b BackgroundColour
        {
            get { return ActiveInstance.mBackgroundColour; }
            set { ActiveInstance.mBackgroundColour = value; }
        }

        internal static int MaxTextureUnits { get; private set; }

        internal TackRenderer() {
            ActiveInstance = this;
            mBackgroundColour = new Colour4b(150, 150, 150, 255);

            m_loadedShaders = new List<Shader>();
        }

        public void OnStart() {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            /*
            mShaderProgramIds.Add("shaders.default_object_shader", ShaderFunctions.CompileAndLinkShaders(Properties.Resources.DefaultVertexShader, Properties.Resources.DefaultFragmentShader));
            TackConsole.EngineLog(EngineLogType.Message, "Successfully registered shader with name 'shaders.default_object_shader'");
            mShaderProgramIds.Add("shaders.default_gui_shader", ShaderFunctions.CompileAndLinkShaders(Properties.Resources.DefaultVertexShader, Properties.Resources.GUIFragmentShader));
            TackConsole.EngineLog(EngineLogType.Message, "Successfully registered shader with name 'shaders.default_gui_shader'");
            */

            /*
            m_defaultWorldShader = new Shader("shaders.default_world_shader", TackShaderType.World, System.IO.File.ReadAllText("tackresources/shaders/world/default_world_vertex_shader.vs"), 
                                                                                                    System.IO.File.ReadAllText("tackresources/shaders/world/default_world_fragment_shader.fs"));

            */

            m_defaultWorldShader = new Shader("shaders.default_world_shader", TackShaderType.World, System.IO.File.ReadAllText("tackresources/shaders/world/default_world_vertex_shader.vs"),
                                                                                                    System.IO.File.ReadAllText("tackresources/shaders/world/default_world_fragment_shader.fs"));

            mVertexData = new float[4];
            mRenderFpsCounter = false;

            m_guiInstance = new TackGUI();
            m_guiInstance.OnStart();

            mFpsCounterStyle = new TextAreaStyle() {
                BackgroundColour = new Colour4b(0, 0, 0, 0),
                FontColour = Colour4b.Black,
                FontSize = 7f,
                FontFamilyId = 0,
                Scrollable = false,
                ScrollPosition = 0,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
            };

            timer.Stop();
            TackConsole.EngineLog(EngineLogType.ModuleStart, "", timer.ElapsedMilliseconds);
        }

        public void OnUpdate() {
            m_guiInstance.OnUpdate();
        }

        public void OnRender() {
            // Render everything in world
            RenderQuadRendererComponents();

            // Render GUI
            m_guiInstance.OnGUIRender();
        }

        public void RenderFpsCounter() {
            int width = 100;
            int height = 100;
            if (mRenderFpsCounter) {
                //TackGUI.TextArea(new RectangleShape(TackEngine.MainCamera.CameraScreenWidth - (width + 5), 5, width, height), TackEngine.RenderCyclesPerSecond.ToString(), mFpsCounterStyle);
            }
        }

        public void OnClose() {
            for (int i = 0; i < m_loadedShaders.Count; i++) {
                m_loadedShaders[i].Destroy();
            }

            m_guiInstance.OnClose();
        }

        public static void SetFpsCounterState(bool state) {
            ActiveInstance.mRenderFpsCounter = state;
        }

        public static Shader GetShader(string shaderName, TackShaderType shaderType) {
            if (shaderName == "shaders.default_world_shader") {
                return ActiveInstance.m_defaultWorldShader;
            }

            if (shaderName == "shaders.default_gui_shader") {
                return ActiveInstance.m_defaultGUIShader;
            }

            for (int i = 0; i < ActiveInstance.m_loadedShaders.Count; i++) {
                if (ActiveInstance.m_loadedShaders[i].Name == shaderName && ActiveInstance.m_loadedShaders[i].Type == shaderType) {
                    return ActiveInstance.m_loadedShaders[i];
                }
            }

            if (shaderType == TackShaderType.GUI) {
                return ActiveInstance.m_defaultGUIShader;
            } else {
                return ActiveInstance.m_defaultWorldShader;
            }
        }

         /// <summary>
         /// Converts screen coords e.g(0 - 600) to different coords (-1 - 1)
         /// </summary>
         /// <returns></returns>
        private static float ConvertCoords(int _val, int _oldMaxSize)
        {
            float oldRange = _oldMaxSize - 0;
            float newRange = 1 - (-1);

            return (((_val - 0) * newRange) / oldRange) + (-1);
        }

        /// <summary>
        /// Loops through all active TackObjects and renders all QuadRenderer components
        /// </summary>
        private void RenderQuadRendererComponents()
        {
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);

            GL.EnableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.IndexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            // Tell OpenGL to use teh default object shader
            //GL.UseProgram(GetShader("shaders.default_world_shader", TackShaderType.World).Id);

            TackObject[] allObjects = TackObject.Get();

            List<TackObject> allObjectsWithQRComp = new List<TackObject>(allObjects.Where(x => !x.GetComponent<QuadRenderer>().IsNullComponent()));
            List<TackObject> orderedComponents = new List<TackObject>(allObjectsWithQRComp.OrderBy(x => x.GetComponent<QuadRenderer>().RenderLayer));

            foreach (TackObject currentTackObject in orderedComponents)
            {
                // Continue with the loop if currentTackObject does not contain a QuadRenderer component
                if (currentTackObject.GetComponent<QuadRenderer>().IsNullComponent())
                    continue;

                /*
                // Continue if TackObject is currently inactive
                if (currentTackObject.)
                */

                QuadRenderer quadRenderer = currentTackObject.GetComponent<QuadRenderer>();

                RectangleShape tackObjectBounds = new RectangleShape()
                {
                    X = ((currentTackObject.Position.X - TackEngine.MainCamera.parentObject.Position.X) - (currentTackObject.Scale.X / 2)) / (TackEngine.ScreenWidth / 2),
                    Y = ((currentTackObject.Position.Y - TackEngine.MainCamera.parentObject.Position.Y) + (currentTackObject.Scale.Y / 2)) / (TackEngine.ScreenHeight / 2),
                    Width = (currentTackObject.Scale.X) / (TackEngine.ScreenWidth / 2),
                    Height = (currentTackObject.Scale.Y) / (TackEngine.ScreenHeight / 2)
                };

                /*
                 *    v4 ------ v1
                 *    |         |
                 *    |         |
                 *    |         |
                 *    v3 ------ v2
                 * 
                 */
                
                
                //mVertexData = new float[32]
                //{
                    //       Position (XYZ)                                                                                                      Colours (RGB)                                                                                  TexCoords (XY)
                    /* v1 */ //(tackObjectBounds.X + tackObjectBounds.Width), (tackObjectBounds.Y), 1.0f,                                          (quadRenderer.Colour.R / 255), (quadRenderer.Colour.G / 255), (quadRenderer.Colour.B / 255),   1.0f, 0.0f,
                    /* v2 */ //(tackObjectBounds.X + tackObjectBounds.Width), (tackObjectBounds.Y - tackObjectBounds.Height), 1.0f,                (quadRenderer.Colour.R / 255), (quadRenderer.Colour.G / 255), (quadRenderer.Colour.B / 255),   1.0f, 1.0f,
                    /* v3 */ //(tackObjectBounds.X), (tackObjectBounds.Y - tackObjectBounds.Height), 1.0f,                                         (quadRenderer.Colour.R / 255), (quadRenderer.Colour.G / 255), (quadRenderer.Colour.B / 255),   0.0f, 1.0f,
                    /* v4 */ //(tackObjectBounds.X), (tackObjectBounds.Y), 1.0f,                                                                   (quadRenderer.Colour.R / 255), (quadRenderer.Colour.G / 255), (quadRenderer.Colour.B / 255),   0.0f, 0.0f
                //};

                mVertexData = new float[32]
                {
                    //       Position (XYZ)                                                                                                      Colours (RGB)                                                                                  TexCoords (XY)
                    /* v1 */ FindScreenCoordsFromPosition(quadRenderer.FindVertexPoint(1)).X, FindScreenCoordsFromPosition(quadRenderer.FindVertexPoint(1)).Y, 1.0f,                (quadRenderer.Colour.R / 255.0f), (quadRenderer.Colour.G / 255.0f), (quadRenderer.Colour.B / 255.0f),   1.0f, 0.0f,
                    /* v2 */ FindScreenCoordsFromPosition(quadRenderer.FindVertexPoint(2)).X, FindScreenCoordsFromPosition(quadRenderer.FindVertexPoint(2)).Y, 1.0f,                (quadRenderer.Colour.R / 255.0f), (quadRenderer.Colour.G / 255.0f), (quadRenderer.Colour.B / 255.0f),   1.0f, 1.0f,
                    /* v3 */ FindScreenCoordsFromPosition(quadRenderer.FindVertexPoint(3)).X, FindScreenCoordsFromPosition(quadRenderer.FindVertexPoint(3)).Y, 1.0f,                (quadRenderer.Colour.R / 255.0f), (quadRenderer.Colour.G / 255.0f), (quadRenderer.Colour.B / 255.0f),   0.0f, 1.0f,
                    /* v4 */ FindScreenCoordsFromPosition(quadRenderer.FindVertexPoint(4)).X, FindScreenCoordsFromPosition(quadRenderer.FindVertexPoint(4)).Y, 1.0f,                (quadRenderer.Colour.R / 255.0f), (quadRenderer.Colour.G / 255.0f), (quadRenderer.Colour.B / 255.0f),   0.0f, 0.0f
                };

                int[] indices = new int[]
                {
                    0, 1, 3, // first triangle
                    1, 2, 3  // second triangle
                };

                //Sprite sp = Sprite.LoadFromFile("Resources/background_image.png");
                //sp.Create(false);

                int VAO = GL.GenVertexArray();
                int VBO = GL.GenBuffer();
                int EBO = GL.GenBuffer();

                GL.BindVertexArray(VAO);

                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 32, mVertexData, BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
                GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * 6, indices, BufferUsageHint.StaticDraw);

                // position attribute
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);

                // color attribute
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
                GL.EnableVertexAttribArray(1);

                // texture coord attribute
                GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
                GL.EnableVertexAttribArray(2);

                // Set texture attributes
                GL.ActiveTexture(TextureUnit.Texture0);
                if (quadRenderer.RenderMode == RendererMode.SpriteSheet) {
                    GL.BindTexture(TextureTarget.Texture2D, quadRenderer.SpriteSheet.GetActiveSprite().Id);
                } else {
                    GL.BindTexture(TextureTarget.Texture2D, quadRenderer.Sprite.Id);
                }


                //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, quadRenderer.Sprite.Width, quadRenderer.Sprite.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, quadRenderer.Sprite.SpriteData.Scan0);


                // set texture filtering parameters
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                // set the texture wrapping parameters
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

                

                GL.ActiveTexture(TextureUnit.Texture0);

                if (quadRenderer.RenderMode == RendererMode.SpriteSheet) {
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, quadRenderer.SpriteSheet.SingleSpriteWidth, quadRenderer.SpriteSheet.SingleSpriteHeight, 0, PixelFormat.Bgra, PixelType.UnsignedByte, (IntPtr)0); // changed the end value from sprite.bitmpadata.scan0
                } else {
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, quadRenderer.Sprite.Width, quadRenderer.Sprite.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, quadRenderer.Sprite.Data);
                }
                //GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                m_defaultWorldShader.Use();
                m_defaultWorldShader.SetUniformValue("ourTexture", 0);
                // Set the shader uniform value
                //GL.Uniform1(GL.GetUniformLocation(GetShader("shaders.default_world_shader", TackShaderType.World).Id, "ourTexture"), 0);

                GL.ActiveTexture(TextureUnit.Texture0);
                if (quadRenderer.RenderMode == RendererMode.SpriteSheet) {
                    GL.BindTexture(TextureTarget.Texture2D, quadRenderer.SpriteSheet.GetActiveSprite().Id);
                } else {
                    GL.BindTexture(TextureTarget.Texture2D, quadRenderer.Sprite.Id);
                }

                GL.BindVertexArray(VAO);

                GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, IntPtr.Zero);

                //sp.Destory(false);

                GL.DeleteBuffer(EBO);
                GL.DeleteBuffer(VBO);
                GL.DeleteVertexArray(VAO);
            }
        }

        private Vector2f FindScreenCoordsFromPosition(Vector2f _pos)
        {
            Vector2f vec = new Vector2f()
            {
                X = ((_pos.X - TackEngine.MainCamera.parentObject.Position.X) / (TackEngine.ScreenWidth / 2)),
                Y = ((_pos.Y + TackEngine.MainCamera.parentObject.Position.Y) / (TackEngine.ScreenHeight / 2))
            };

            return vec;
        }

        /*
         * foreach (TackObject tackObject in objects)
            {
                QuadRenderer qr = tackObject.GetComponent<QuadRenderer>();

                if (!qr.IsNullComponent() && qr.Active)
                {
                    float rectX = ((tackObject.Position.X - MainScreenWindow.CameraObject.Position.X) - (tackObject.Scale.X / 2)) / (TackEngine.ScreenWidth / 2);
                    float rectY = ((tackObject.Position.Y - MainScreenWindow.CameraObject.Position.Y) + (tackObject.Scale.Y / 2)) / (TackEngine.ScreenHeight / 2);

                    float scaleX = (tackObject.Scale.X) / (TackEngine.ScreenWidth / 2);
                    float scaleY = (tackObject.Scale.Y) / (TackEngine.ScreenHeight / 2);

                    // inner comment start
                    if (qr.RenderMode != RendererMode.Colour)
                        TackRenderer.RenderQuad(new RectangleShape(rectX, rectY, scaleX, scaleY), qr.Colour, qr.Sprite);
                    else
                        TackRenderer.RenderQuad(new RectangleShape(rectX, rectY, scaleX, scaleY), qr.Colour);
                        // inner comment end

                    if (qr.RenderMode == RendererMode.Sprite)
                    {
                        TackRenderer.RenderQuad(new RectangleShape(rectX, rectY, scaleX, scaleY), qr.Colour, qr.Sprite);
                    }
                    else if (qr.RenderMode == RendererMode.SpriteSheet)
                    {
                        TackRenderer.RenderQuad(new RectangleShape(rectX, rectY, scaleX, scaleY), qr.Colour, qr.SpriteSheet.GetActiveSprite());
                    }
                    else
                    {
                        TackRenderer.RenderQuad(new RectangleShape(rectX, rectY, scaleX, scaleY), qr.Colour);
                    }
                }
            }
    */
    }
}

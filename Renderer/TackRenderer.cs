using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;

using TackEngineLib.Main;
using TackEngineLib.Engine;
using TackEngineLib.Objects;
using TackEngineLib.Objects.Components;

namespace TackEngineLib.Renderer
{
    internal class TackRenderer
    {
        internal TackRenderer()
        {

        }

        public void OnStart()
        {

        }

        public void OnRender()
        {
            try
            {

            }
            catch (Exception e)
            {

                throw;
            }
        }

        public void OnClose()
        {

        }

        public static void RenderCycle()
        {
            int num = 0;

            // Vertex Positions
            // 
            // V1 ------ V2
            // |         |
            // |         |
            // V4 ------ v3

            foreach (RenderObject rendObj in renderQueue)
            {
                num++;

                if (!rendObj.renderSprite)
                {
                    GL.Enable(EnableCap.Blend);
                    GL.Disable(EnableCap.Texture2D);

                    GL.Begin(PrimitiveType.Quads);
                    
                    // Set the colour
                    GL.Color4((byte)rendObj.colour.R, (byte)rendObj.colour.G, (byte)rendObj.colour.B, (byte)rendObj.colour.A);

                    // V1
                    GL.Vertex2(rendObj.rectangle.X, rendObj.rectangle.Y);

                    // V2
                    GL.Vertex2(rendObj.rectangle.X + rendObj.rectangle.Width, rendObj.rectangle.Y);

                    // V3
                    GL.Vertex2(rendObj.rectangle.X + rendObj.rectangle.Width, rendObj.rectangle.Y + -rendObj.rectangle.Height);

                    // V4
                    GL.Vertex2(rendObj.rectangle.X, rendObj.rectangle.Y + -rendObj.rectangle.Height);

                    GL.End();
                }
                else
                {
                    GL.Enable(EnableCap.Texture2D);
                    GL.Enable(EnableCap.Blend);
                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

                    GL.BindTexture(TextureTarget.Texture2D, rendObj.sprite.TextureId);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, rendObj.sprite.Width, rendObj.sprite.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, rendObj.sprite.SpriteData.Scan0);

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

                    GL.BindTexture(TextureTarget.Texture2D, rendObj.sprite.TextureId);

                    GL.Begin(PrimitiveType.Quads);

                    // Set the colour
                    GL.Color4((byte)rendObj.colour.R, (byte)rendObj.colour.G, (byte)rendObj.colour.B, (byte)rendObj.colour.A);

                    // V1
                    GL.TexCoord2(0, 0);
                    GL.Vertex2(rendObj.rectangle.X, rendObj.rectangle.Y);

                    // V2
                    GL.TexCoord2(1, 0);
                    GL.Vertex2(rendObj.rectangle.X + rendObj.rectangle.Width, rendObj.rectangle.Y);

                    // V3
                    GL.TexCoord2(1, 1);
                    GL.Vertex2(rendObj.rectangle.X + rendObj.rectangle.Width, rendObj.rectangle.Y + -rendObj.rectangle.Height);

                    // V4
                    GL.TexCoord2(0, 1);
                    GL.Vertex2(rendObj.rectangle.X, rendObj.rectangle.Y + -rendObj.rectangle.Height);

                    GL.End();
                    //GL.Disable(EnableCap.Blend);
                }
            }

            //GL.End();
            //EngineLog.WriteMessage("Rendered {0} items this cycle", num);
            renderQueue.Clear();
        }

        /// <summary>
        /// Render a quad
        /// </summary>
        /// <param name="rendObj.rectangle">The dimensions and position of the quad</param>
        /// <param name="_col">The colour of the rendered quad</param>
        public static void RenderQuad(RectangleShape _rect, Colour4b _col)
        {
            renderQueue.Add(new RenderObject(_rect, _col, null, false));
        }

        
        public static void RenderQuad(RectangleShape _rect, Colour4b _col, Sprite _tex)
        {
            renderQueue.Add(new RenderObject(_rect, _col, _tex, true));
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

        private void TackObjectSpriteRendering()
        {
            TackObject[] allObjects = TackObject.Get();

            foreach (TackObject currentTackObject in allObjects)
            {
                // Continue with the loop if currentTackObject does not contain a QuadRenderer component
                if (currentTackObject.GetComponent<QuadRenderer>().IsNullComponent())
                    continue;


            }
        }

        /*
         * foreach (TackObject tackObject in objects)
            {
                QuadRenderer qr = tackObject.GetComponent<QuadRenderer>();

                if (!qr.IsNullComponent() && qr.Active)
                {
                    float rectX = ((tackObject.Position.X - MainScreenWindow.CameraObject.Position.X) - (tackObject.Scale.X / 2)) / (MainScreenWindow.Width / 2);
                    float rectY = ((tackObject.Position.Y - MainScreenWindow.CameraObject.Position.Y) + (tackObject.Scale.Y / 2)) / (MainScreenWindow.Height / 2);

                    float scaleX = (tackObject.Scale.X) / (MainScreenWindow.Width / 2);
                    float scaleY = (tackObject.Scale.Y) / (MainScreenWindow.Height / 2);

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

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.IO;

using OpenTK.Graphics.OpenGL;

using TackEngineLib.Main;
using TackEngineLib.Engine;
using TackEngineLib.Renderer;

namespace TackEngineLib.GUI
{
    public static class TackGUI
    {
        private static PrivateFontCollection fontCollection;
        private static FontFamily activeFontFamily;

        public static void OnStart()
        {
            fontCollection = new PrivateFontCollection();
            activeFontFamily = new FontFamily("Arial");
        }

        /// <summary>
        /// Loads a font file into the font collection. Returns the position of the new font family.
        /// </summary>
        /// <param name="_fileName"></param>
        /// <returns></returns>
        public static int LoadFontFromFile(string _fileName)
        {
            if (!File.Exists(_fileName))
            {
                TackConsole.EngineLog(EngineLogType.Error, string.Format("Could not locate file at path: {0}", _fileName));
                return -1;
            }

            fontCollection.AddFontFile(_fileName);

            TackConsole.EngineLog(EngineLogType.Message, string.Format("Added new font with name: {0} to the TackGUI font collection at index: {1}", fontCollection.Families[fontCollection.Families.Length - 1].Name, fontCollection.Families.Length - 1));
            return fontCollection.Families.Length - 1;
        }

        /// <summary>
        /// Sets the active FontFamily
        /// </summary>
        /// <param name="_familyName">the Name of the FontFamily</param>
        public static void SetActiveFontFamily(string _familyName)
        {
            for (int i = 0; i < fontCollection.Families.Length; i++)
            {
                if (fontCollection.Families[i].Name == _familyName)
                {
                    SetActiveFontFamily(i);
                    return;
                }
            }

            TackConsole.EngineLog(EngineLogType.Error, string.Format("No FontFamily with name: {0} was found in the font collection", _familyName));
        }

        /// <summary>
        /// Sets the active FontFamily
        /// </summary>
        /// <param name="_familyIndex">The index of the FontFamily</param>
        public static void SetActiveFontFamily(int _familyIndex)
        {
            if (_familyIndex < fontCollection.Families.Length)
            {
                activeFontFamily = fontCollection.Families[_familyIndex];

                TackConsole.EngineLog(EngineLogType.Message, "Set the active FontFamily. Name: {0}, FamilyIndex: {1}", activeFontFamily.Name, _familyIndex);
            }

            TackConsole.EngineLog(EngineLogType.Error, "The specfied family index is outside the bounds of the font collection Families array");
        }

        public static void Box(RectangleShape _rect)
        {
            Box(_rect, new Colour4b(255, 255, 255, 255));
        }

        public static void Box(RectangleShape _rect, Colour4b _colour)
        {
            Sprite defaultSprite = Sprite.LoadFromBitmap(Properties.Resources.DefaultSprite);
            defaultSprite.Create(false);

            RectangleShape calculatedRect = new RectangleShape()
            {
                X = (_rect.X - (MainScreenWindow.Width / 2)) / (MainScreenWindow.Width / 2),
                Y = ((MainScreenWindow.Height / 2) - _rect.Y) / (MainScreenWindow.Height / 2),
                Width = (_rect.Width / (MainScreenWindow.Width / 2)),
                Height = (_rect.Height / (MainScreenWindow.Height / 2))
            };

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            // Tell OpenGL to use the compiled and linker shader program at m_ShaderProgramId
            GL.UseProgram(TackRenderer.ShaderProgramId);

            float[] vertexData = new float[32]
                {
                    //       Position (XYZ)                                                                                                      Colours (RGB)                                                                                  TexCoords (XY)
                    /* v1 */ (calculatedRect.X + calculatedRect.Width), (calculatedRect.Y), 1.0f,                                        (_colour.R / 255), (_colour.G / 255), (_colour.B / 255),   1.0f, 0.0f,
                    /* v2 */ (calculatedRect.X + calculatedRect.Width), (calculatedRect.Y - calculatedRect.Height), 1.0f,                (_colour.R / 255), (_colour.G / 255), (_colour.B / 255),   1.0f, 1.0f,
                    /* v3 */ (calculatedRect.X), (calculatedRect.Y - calculatedRect.Height), 1.0f,                                       (_colour.R / 255), (_colour.G / 255), (_colour.B / 255),   0.0f, 1.0f,
                    /* v4 */ (calculatedRect.X), (calculatedRect.Y), 1.0f,                                                               (_colour.R / 255), (_colour.G / 255), (_colour.B / 255),   0.0f, 0.0f
                };

            int[] indices = new int[]
            {
                    0, 1, 3, // first triangle
                    1, 2, 3  // second triangle
            };

            int VAO = GL.GenVertexArray();
            int VBO = GL.GenBuffer();
            int EBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 32, vertexData, BufferUsageHint.StaticDraw);

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
            GL.BindTexture(TextureTarget.Texture2D, defaultSprite.TextureId);


            //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, quadRenderer.Sprite.Width, quadRenderer.Sprite.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, quadRenderer.Sprite.SpriteData.Scan0);


            // set texture filtering parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // set the texture wrapping parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);



            GL.ActiveTexture(TextureUnit.Texture0);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, defaultSprite.Width, defaultSprite.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, defaultSprite.SpriteData.Scan0);
            //GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            // Set the shader uniform value
            GL.Uniform1(GL.GetUniformLocation(TackRenderer.ShaderProgramId, "ourTexture"), 0);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, defaultSprite.TextureId);

            GL.BindVertexArray(VAO);

            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, IntPtr.Zero);

            defaultSprite.Destory(false);
        }

        public static void TextArea(RectangleShape _rect, string _text)
        {
            TextArea(_rect, new Colour4b(255, 255, 255, 255), _text, new Colour4b(0, 0, 0, 255));
        }

        public static void TextArea(RectangleShape _rect, Colour4b _backgroundColour, string _text, Colour4b _textColour)
        {
            // Render a box at the back of the TextArea
            Box(_rect, _backgroundColour);

            // Generate Bitmap
            Vector2i size = new Vector2i((int)(_rect.Width), (int)(_rect.Height));
            Bitmap cBmp = new Bitmap(size.X, size.Y);

            // Generate Graphics Object
            Graphics graphics = Graphics.FromImage(cBmp);

            Font myFont = new Font(activeFontFamily, 8f, FontStyle.Regular);

            //Get the perfect Image-Size so that Image-Size = String-Size
            RectangleF rect = new RectangleF(0, 0, cBmp.Width, cBmp.Height);

            //Use this to become better Text-Quality on Bitmap.
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            //Here we draw the string on the Bitmap
            Color customColor = Color.FromArgb(_textColour.A, _textColour.R, _textColour.G, _textColour.B);
            graphics.DrawString(_text, myFont, new SolidBrush(customColor), rect);

            Sprite textTexture = Sprite.LoadFromBitmap(cBmp);
            textTexture.Create(false);

            RectangleShape calculatedRect = new RectangleShape()
            {
                X = (_rect.X - (MainScreenWindow.Width / 2)) / (MainScreenWindow.Width / 2),
                Y = ((MainScreenWindow.Height / 2) - _rect.Y) / (MainScreenWindow.Height / 2),
                Width = (_rect.Width / (MainScreenWindow.Width / 2)),
                Height = (_rect.Height / (MainScreenWindow.Height / 2))
            };

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            // Tell OpenGL to use the compiled and linker shader program at m_ShaderProgramId
            GL.UseProgram(TackRenderer.ShaderProgramId);

            float[] vertexData = new float[32]
                {
                    //       Position (XYZ)                                                                                              Colours (RGB)         TexCoords (XY)
                    /* v1 */ (calculatedRect.X + calculatedRect.Width), (calculatedRect.Y), 1.0f,                                        1f, 1f, 1f,            1.0f, 0.0f,
                    /* v2 */ (calculatedRect.X + calculatedRect.Width), (calculatedRect.Y - calculatedRect.Height), 1.0f,                1f, 1f, 1f,            1.0f, 1.0f,
                    /* v3 */ (calculatedRect.X), (calculatedRect.Y - calculatedRect.Height), 1.0f,                                       1f, 1f, 1f,            0.0f, 1.0f,
                    /* v4 */ (calculatedRect.X), (calculatedRect.Y), 1.0f,                                                               1f, 1f, 1f,            0.0f, 0.0f
                };

            int[] indices = new int[]
            {
                    0, 1, 3, // first triangle
                    1, 2, 3  // second triangle
            };

            int VAO = GL.GenVertexArray();
            int VBO = GL.GenBuffer();
            int EBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 32, vertexData, BufferUsageHint.StaticDraw);

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
            GL.BindTexture(TextureTarget.Texture2D, textTexture.TextureId);


            //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, quadRenderer.Sprite.Width, quadRenderer.Sprite.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, quadRenderer.Sprite.SpriteData.Scan0);


            // set texture filtering parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // set the texture wrapping parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);



            GL.ActiveTexture(TextureUnit.Texture0);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, textTexture.Width, textTexture.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, textTexture.SpriteData.Scan0);
            //GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            // Set the shader uniform value
            GL.Uniform1(GL.GetUniformLocation(TackRenderer.ShaderProgramId, "ourTexture"), 0);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textTexture.TextureId);

            GL.BindVertexArray(VAO);

            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, IntPtr.Zero);

            textTexture.Destory(false);




            /*
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.BindTexture(TextureTarget.Texture2D, textTexture.TextureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, textTexture.Width, textTexture.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, textTexture.SpriteData.Scan0);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.BindTexture(TextureTarget.Texture2D, textTexture.TextureId);

            RectangleShape calculatedRect = new RectangleShape()
            {
                X = (_rect.X - (MainScreenWindow.Width / 2)) / (MainScreenWindow.Width / 2),
                Y = ((MainScreenWindow.Height / 2) - _rect.Y) / (MainScreenWindow.Height / 2),
                Width = (_rect.Width / (MainScreenWindow.Width / 2)),
                Height = (_rect.Height / (MainScreenWindow.Height / 2))
            };

            GL.Begin(PrimitiveType.Quads);

            // Vertex Positions
            // 
            // V1 ------ V2
            // |         |
            // |         |
            // V4 ------ v3

            // V1
            GL.TexCoord2(0, 0);
            GL.Vertex2(calculatedRect.X, calculatedRect.Y);

            // V2
            GL.TexCoord2(1, 0);
            GL.Vertex2(calculatedRect.X + calculatedRect.Width, calculatedRect.Y);

            // V3
            GL.TexCoord2(1, 1);
            GL.Vertex2(calculatedRect.X + calculatedRect.Width, calculatedRect.Y + -calculatedRect.Height);

            // V4
            GL.TexCoord2(0, 1);
            GL.Vertex2(calculatedRect.X, calculatedRect.Y + -calculatedRect.Height);

            GL.End();

            textTexture.Destory(false);

            //Console.WriteLine("Rendered 1 string '{0}'", _text);*/
        }

        public static void InputField(RectangleShape _rect, string _value, out string _returnString)
        {
            Input.TackInput.GUIInputRequired = true;

            Box(_rect);

            string text = string.Format("{0}{1}", _value, Input.TackInput.GetInputBuffer());

            TextArea(_rect, text);

            _returnString = text;
        }
    }
}

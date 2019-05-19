using System;
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
using TackEngineLib.Renderer.Shaders;
using TackEngineLib.Input;

namespace TackEngineLib.GUI
{
    /// <summary>
    /// The main class for rendering GUI elements to the screen
    /// </summary>
    internal static class TackGUI
    {
        private static PrivateFontCollection fontCollection;
        private static FontFamily activeFontFamily;
        private static int uiShaderProgram;

        internal static List<InputField> inputFields = new List<InputField>();

        public static void OnStart()
        {
            fontCollection = new PrivateFontCollection();
            activeFontFamily = new FontFamily("Arial");
            fontCollection.AddFontFile(Environment.GetFolderPath(Environment.SpecialFolder.Fonts) + "\\Arial.ttf");
            TackConsole.EngineLog(EngineLogType.Message, string.Format("Added default font file from: {0}\\Arial.ttf", Environment.GetFolderPath(Environment.SpecialFolder.Fonts)));

            TackConsole.EngineLog(EngineLogType.Message, "Starting GUI shader compilation and linking");
            uiShaderProgram = ShaderFunctions.CompileAndLinkShaders(Properties.Resources.DefaultVertexShader, Properties.Resources.DefaultFragmentShader_GUI);
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

        /// <summary>
        /// Renders a box to the screen
        /// </summary>
        /// <param name="_rect">The shape (Position and size) of the box</param>
        /// <param name="_style">The BoxStyle used to render this box</param>
        public static void Box(RectangleShape _rect, BoxStyle _style = default(BoxStyle))
        {
            if (_style == null)
                _style = new BoxStyle();

            if (_style.Border != null)
            {
                RectangleShape borderShape = new RectangleShape()
                {
                    X = _rect.X - _style.Border.Left,
                    Y = _rect.Y - _style.Border.Up,
                    Width = _rect.Width + _style.Border.Right + _style.Border.Left,
                    Height = _rect.Height + _style.Border.Bottom + _style.Border.Up
                };

                BoxStyle boxStyle = new BoxStyle()
                {
                    Colour = _style.Border.Colour
                };

                Box(borderShape, boxStyle);

                boxStyle.Destory();
            }

            

            //Sprite defaultSprite = Sprite.LoadFromBitmap(Properties.Resources.DefaultSprite);
            //defaultSprite.Create(false);

            RectangleShape calculatedRect = new RectangleShape()
            {
                X = (_rect.X - (TackEngine.ScreenWidth / 2)) / (TackEngine.ScreenWidth / 2),
                Y = ((TackEngine.ScreenHeight / 2) - _rect.Y) / (TackEngine.ScreenHeight / 2),
                Width = (_rect.Width / (TackEngine.ScreenWidth / 2)),
                Height = (_rect.Height / (TackEngine.ScreenHeight / 2))
            };

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            // Tell OpenGL to use the compiled and linker shader program at m_ShaderProgramId
            GL.UseProgram(uiShaderProgram);

            float[] vertexData = new float[32]
                {
                    //       Position (XYZ)                                                                                                      Colours (RGB)                                                                                  TexCoords (XY)
                    /* v1 */ (calculatedRect.X + calculatedRect.Width), (calculatedRect.Y), 1.0f,                                        (_style.Colour.R / 255.0f), (_style.Colour.G / 255.0f), (_style.Colour.B / 255.0f),   1.0f, 0.0f,
                    /* v2 */ (calculatedRect.X + calculatedRect.Width), (calculatedRect.Y - calculatedRect.Height), 1.0f,                (_style.Colour.R / 255.0f), (_style.Colour.G / 255.0f), (_style.Colour.B / 255.0f),   1.0f, 1.0f,
                    /* v3 */ (calculatedRect.X), (calculatedRect.Y - calculatedRect.Height), 1.0f,                                       (_style.Colour.R / 255.0f), (_style.Colour.G / 255.0f), (_style.Colour.B / 255.0f),   0.0f, 1.0f,
                    /* v4 */ (calculatedRect.X), (calculatedRect.Y), 1.0f,                                                               (_style.Colour.R / 255.0f), (_style.Colour.G / 255.0f), (_style.Colour.B / 255.0f),   0.0f, 0.0f
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
            GL.BindTexture(TextureTarget.Texture2D, _style.SpriteTexture.TextureId);


            //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, quadRenderer.Sprite.Width, quadRenderer.Sprite.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, quadRenderer.Sprite.SpriteData.Scan0);


            // set texture filtering parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // set the texture wrapping parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);



            GL.ActiveTexture(TextureUnit.Texture0);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _style.SpriteTexture.Width, _style.SpriteTexture.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, _style.SpriteTexture.SpriteData.Scan0);
            //GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            // Set the shader uniform value
            GL.Uniform1(GL.GetUniformLocation(uiShaderProgram, "ourTexture"), 0);
            GL.Uniform1(GL.GetUniformLocation(uiShaderProgram, "ourOpacity"), (float)(_style.Colour.A / 255.0f));

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _style.SpriteTexture.TextureId);

            GL.BindVertexArray(VAO);

            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, IntPtr.Zero);

            //defaultSprite.Destory(false);

            GL.DeleteBuffer(EBO);
            GL.DeleteBuffer(VBO);
            GL.DeleteVertexArray(VAO);

            //_style.Destory();
        }

        /// <summary>
        /// Renders text to the screen
        /// </summary>
        /// <param name="_rect">The shape of box to render the text in</param>
        /// <param name="_text">The text to render</param>
        /// <param name="_style">The TextAreaStyle used to render this text</param>
        public static void TextArea(RectangleShape _rect, string _text, TextAreaStyle _style = default(TextAreaStyle))
        {
            if (_style == null)
                _style = new TextAreaStyle();

            BoxStyle boxStyle = new BoxStyle()
            {
                Colour = _style.BackgroundColour,
                SpriteTexture = _style.SpriteTexture
            };

            // Render a box at the back of the TextArea
            Box(_rect, boxStyle);

            Graphics graphics = Graphics.FromImage(new Bitmap(1, 1));
            Font myFont = new Font(fontCollection.Families[_style.FontFamilyId], _style.FontSize, FontStyle.Regular);
            Bitmap cBmp;
            SizeF stringSize = graphics.MeasureString(_text, myFont);

            if (_style.Scrollable)
            {

                if (string.IsNullOrEmpty(_text))
                    stringSize = new SizeF(1, 1);

                cBmp = new Bitmap((int)_rect.Width, (int)stringSize.Height + 40);
                //Console.WriteLine("Is Scrollable");
            }
            else
            {
                //Vector2i size = new Vector2i((int)(_rect.Width), (int)(_rect.Height));
                cBmp = new Bitmap((int)_rect.Width, (int)_rect.Height + 40);
            }

            // Generate Graphics Object
            graphics = Graphics.FromImage(cBmp);

            //Get the perfect Image-Size so that Image-Size = String-Size
            RectangleF rect = new RectangleF(1, 1, 1, 1);

            if (_style.VerticalAlignment == VerticalAlignment.Top || _style.VerticalAlignment == VerticalAlignment.Middle)
                rect = new RectangleF(0, 0, cBmp.Width, cBmp.Height + 40);

            if (_style.VerticalAlignment == VerticalAlignment.Bottom)
            {
                rect = new RectangleF(0, 0, cBmp.Width, cBmp.Height + 40);
            }

            //Console.WriteLine("Render text area. RectHeight: " + _rect.Height + ", stringSizeHeight: " + stringSize.Height);

            //Use this to become better Text-Quality on Bitmap.
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            StringFormat format = new StringFormat();
            //format.Alignment = StringAlignment.Center;
            //format.LineAlignment = StringAlignment.Near;

            // Set the horizontal alignment of the text
            switch (_style.HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        format.Alignment = StringAlignment.Near;
                        break;

                    case HorizontalAlignment.Middle:
                        format.Alignment = StringAlignment.Center;
                        break;

                    case HorizontalAlignment.Right:
                        format.Alignment = StringAlignment.Far;
                        break;

                    default:
                        format.Alignment = StringAlignment.Center;
                        break;
                }

            // Set the vertical alignment of the text
            switch (_style.VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    format.LineAlignment = StringAlignment.Near;
                    break;

                case VerticalAlignment.Middle:
                    format.LineAlignment = StringAlignment.Center;
                    break;

                case VerticalAlignment.Bottom:
                    format.LineAlignment = StringAlignment.Far;
                    break;

                default:
                    format.LineAlignment = StringAlignment.Near;
                    break;
            }

            //Here we draw the string on the Bitmap
            Color customColor = Color.FromArgb(_style.FontColour.A, _style.FontColour.R, _style.FontColour.G, _style.FontColour.B);
            graphics.DrawString(_text, myFont, new SolidBrush(customColor), rect, format);

            Sprite textTexture = Sprite.LoadFromBitmap(cBmp);
            textTexture.Create(false);

            RectangleShape calculatedRect = new RectangleShape()
            {
                X = (_rect.X - (TackEngine.ScreenWidth / 2)) / (TackEngine.ScreenWidth / 2),
                Y = ((TackEngine.ScreenHeight / 2) - _rect.Y) / (TackEngine.ScreenHeight / 2),
                Width = (_rect.Width / (TackEngine.ScreenWidth / 2)),
                Height = (_rect.Height / (TackEngine.ScreenHeight / 2))
            };

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            //_style.ScrollPosition = 0.1f;
            float upBoi;

            if (stringSize.Height > _rect.Height)
                upBoi = _rect.Height / stringSize.Height;
            else
                upBoi = 1;

            //Console.WriteLine("UpBoi: " + upBoi);

            // Tell OpenGL to use the compiled and linker shader program at m_ShaderProgramId
            GL.UseProgram(uiShaderProgram);

            float[] vertexData = new float[32]
                {
                    //       Position (XYZ)                                                                                              Colours (RGB)         TexCoords (XY)
                    /* v1 */ (calculatedRect.X + calculatedRect.Width), (calculatedRect.Y), 1.0f,                                        1f, 1f, 1f,            1.0f, (0 + (1 - 1)),                        //1.0f, 0.0f,
                    /* v2 */ (calculatedRect.X + calculatedRect.Width), (calculatedRect.Y - calculatedRect.Height), 1.0f,                1f, 1f, 1f,            1.0f, (1.5f),                       //1.0f, 1.0f,
                    /* v3 */ (calculatedRect.X), (calculatedRect.Y - calculatedRect.Height), 1.0f,                                       1f, 1f, 1f,            0.0f, (1.5f),                        //0.0f, 1.0f,
                    /* v4 */ (calculatedRect.X), (calculatedRect.Y), 1.0f,                                                               1f, 1f, 1f,            0.0f, (0 + ( 1 - 1))                         //0.0f, 0.0f
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
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);



            GL.ActiveTexture(TextureUnit.Texture0);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, textTexture.Width, textTexture.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, textTexture.SpriteData.Scan0);
            //GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            // Set the shader uniform value
            GL.Uniform1(GL.GetUniformLocation(uiShaderProgram, "ourTexture"), 0);
            GL.Uniform1(GL.GetUniformLocation(uiShaderProgram, "ourOpacity"), (float)(_style.FontColour.A / 255.0f));

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textTexture.TextureId);

            GL.BindVertexArray(VAO);

            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, IntPtr.Zero);

            textTexture.Destory(false);

            GL.DeleteBuffer(EBO);
            GL.DeleteBuffer(VBO);
            GL.DeleteVertexArray(VAO);


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
                X = (_rect.X - (TackEngine.ScreenWidth / 2)) / (TackEngine.ScreenWidth / 2),
                Y = ((TackEngine.ScreenHeight / 2) - _rect.Y) / (TackEngine.ScreenHeight / 2),
                Width = (_rect.Width / (TackEngine.ScreenWidth / 2)),
                Height = (_rect.Height / (TackEngine.ScreenHeight / 2))
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

        public static int GetFontFamilyId(string _familyName)
        {
            for (int i = 0; i < fontCollection.Families.Length; i++)
            {
                if (fontCollection.Families[i].Name == _familyName)
                {
                    return i;
                }
            }

            TackConsole.EngineLog(EngineLogType.Error, string.Format("No FontFamily with name: {0} was found in the font collection", _familyName));
            return -1;
        }

        /// <summary>
        /// Gets the FontFamily at a specified index
        /// </summary>
        /// <param name="a_fontId">The index of the FontFamily in the collection</param>
        /// <returns></returns>
        public static FontFamily GetFontFamily(int a_fontId)
        {
            if (a_fontId < fontCollection.Families.Length)
            {
                return fontCollection.Families[a_fontId];
            }

            return fontCollection.Families[0];
        }
    }
}

/* Copyright (c) 2019 Daniel Phillip Robinson */
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
using TackEngineLib.Renderer.Operations;

namespace TackEngineLib.GUI {
    /// <summary>
    /// The main class for rendering GUI elements to the screen
    /// </summary>
    public class TackGUI {
        private static TackGUI ActiveInstance = null;

        private PrivateFontCollection m_fontCollection;
        private FontFamily m_activeFontFamily;
        private List<GUIOperation> m_guiOperations;
        private Shader m_defaultGUIShader;

        internal static List<InputField> inputFields = new List<InputField>();

        internal void OnStart() {
            if (ActiveInstance != null) {
                TackConsole.EngineLog(EngineLogType.Error, "There is already an active instance of TackGUI.");
                return;
            }

            ActiveInstance = this;

            m_fontCollection = new PrivateFontCollection();
            m_activeFontFamily = new FontFamily("Arial");
            m_fontCollection.AddFontFile(Environment.GetFolderPath(Environment.SpecialFolder.Fonts) + "\\Arial.ttf");
            TackConsole.EngineLog(EngineLogType.Message, string.Format("Added default font file from: {0}\\Arial.ttf", Environment.GetFolderPath(Environment.SpecialFolder.Fonts)));

            m_guiOperations = new List<GUIOperation>();

            m_defaultGUIShader = new Shader("shaders.default_gui_shader", TackShaderType.GUI, System.IO.File.ReadAllText("tackresources/shaders/gui/default_gui_vertex_shader.vs"),
                                                                                             System.IO.File.ReadAllText("tackresources/shaders/gui/default_gui_fragment_shader.fs"));
        }

        internal void OnUpdate() {

        }

        internal void OnGUIRender() {
            if (m_guiOperations.Count == 0) {
                return;
            }

            // Generate SpriteAtlas object using the list of GUIOperations
            Renderer.Sprite.SpriteAtlas atlas = new Renderer.Sprite.SpriteAtlas();

            for (int i = 0; i < m_guiOperations.Count; i++) {
                atlas.AddSprite(m_guiOperations[i].Sprite);
            }

            List<float> vertexBuffer = new List<float>();
            List<int> indicies = new List<int>();

            // Loop through all GUIOperations 
            //   - Add vertex postions/colours/texcoords to the dynamic vertex buffer
            int currentIndex = 0;
            int operationCountToRender = 0;

            for (int i = 0; i < m_guiOperations.Count; i++) {
                RectangleShape rectInScreenCoords = new RectangleShape() {
                    X = (m_guiOperations[i].Bounds.X - (TackEngine.ScreenWidth / 2)) / (TackEngine.ScreenWidth / 2),
                    Y = ((TackEngine.ScreenHeight / 2) - m_guiOperations[i].Bounds.Y) / (TackEngine.ScreenHeight / 2),
                    Width = (m_guiOperations[i].Bounds.Width / (TackEngine.ScreenWidth / 2)),
                    Height = (m_guiOperations[i].Bounds.Height / (TackEngine.ScreenHeight / 2))
                };

                // Get the texture coordinates in a tuple format
                Tuple<float, float>[] texCoords = atlas.GetTexCoords(m_guiOperations[i].Sprite.Id);

                //                                  Pos: XYZ                                                                                                                Colour:  RGB                                                                                                               TexCoords: UV
                vertexBuffer.AddRange(new float[] { (rectInScreenCoords.X + rectInScreenCoords.Width),      (rectInScreenCoords.Y),                                 1.0f,   (m_guiOperations[i].Colour.R / 255.0f), (m_guiOperations[i].Colour.G / 255.0f), (m_guiOperations[i].Colour.B / 255.0f),     texCoords[0].Item1, texCoords[0].Item2, }); // v1
                vertexBuffer.AddRange(new float[] { (rectInScreenCoords.X + rectInScreenCoords.Width),      (rectInScreenCoords.Y - rectInScreenCoords.Height),     1.0f,   (m_guiOperations[i].Colour.R / 255.0f), (m_guiOperations[i].Colour.G / 255.0f), (m_guiOperations[i].Colour.B / 255.0f),     texCoords[1].Item1, texCoords[1].Item2, }); // v2
                vertexBuffer.AddRange(new float[] { (rectInScreenCoords.X),                                 (rectInScreenCoords.Y - rectInScreenCoords.Height),     1.0f,   (m_guiOperations[i].Colour.R / 255.0f), (m_guiOperations[i].Colour.G / 255.0f), (m_guiOperations[i].Colour.B / 255.0f),     texCoords[2].Item1, texCoords[2].Item2, }); // v3
                vertexBuffer.AddRange(new float[] { (rectInScreenCoords.X),                                 (rectInScreenCoords.Y),                                 1.0f,   (m_guiOperations[i].Colour.R / 255.0f), (m_guiOperations[i].Colour.G / 255.0f), (m_guiOperations[i].Colour.B / 255.0f),     texCoords[3].Item1, texCoords[3].Item2, }); // v4

                indicies.AddRange(new int[] { currentIndex,       (currentIndex + 1), (currentIndex + 3) });
                indicies.AddRange(new int[] { (currentIndex + 1), (currentIndex + 2), (currentIndex + 3) });

                currentIndex += 4;
                operationCountToRender++;
            }


            // Call 1 or more draw element calls depending on a maxGUIDrawCallAmount variable 

            int VAO = GL.GenVertexArray();
            int VBO = GL.GenBuffer();
            int EBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertexBuffer.Count, vertexBuffer.ToArray(), BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * indicies.Count, indicies.ToArray(), BufferUsageHint.StaticDraw);

            // position attribute
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // color attribute
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            // texture coord attribute
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            // Generate texture from SpriteAtlas
            Sprite atlasTexture = Sprite.LoadFromBitmap(atlas.GetBitmap());
            atlasTexture.Create(false);

            // Set texture attributes
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, atlasTexture.Id);

            // set texture filtering parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // set the texture wrapping parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, atlasTexture.Width, atlasTexture.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, atlasTexture.Data);
            //GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            m_defaultGUIShader.Use();
            m_defaultGUIShader.SetUniformValue("bTexture", 0);
            m_defaultGUIShader.SetUniformValue("ourOpacity", 255.0f);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, atlasTexture.Id);

            GL.BindVertexArray(VAO);

            GL.DrawElements(PrimitiveType.Triangles, indicies.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

            GL.DeleteBuffer(EBO);
            GL.DeleteBuffer(VBO);
            GL.DeleteVertexArray(VAO);

            atlasTexture.Destory(false);

            m_guiOperations.Clear();
        }

        internal void OnClose() {

        }

        /// <summary>
        /// Generates a Brush with a custom colour
        /// </summary>
        /// <param name="colour"></param>
        /// <returns></returns>
        private Brush GetColouredBrush(Colour4b colour) {
            return new SolidBrush(Color.FromArgb(colour.A, colour.R, colour.G, colour.B));
        }

        /// <summary>
        /// Generates a Pen with a custom colour and size
        /// </summary>
        /// <param name="colour"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private Pen GetColouredPen(Colour4b colour, float size) {
            return new Pen(Color.FromArgb(colour.A, colour.R, colour.G, colour.B), size);
        }

        /// <summary>
        /// Generates a StringFormat object based on HorizontalAlignment and VerticalAlignment values
        /// </summary>
        /// <param name="hAlign"></param>
        /// <param name="vAlign"></param>
        /// <returns></returns>
        private StringFormat GenerateTextFormat(HorizontalAlignment hAlign, VerticalAlignment vAlign) {
            StringFormat format = new StringFormat();

            // Set the vertical alignment of the text
            switch (vAlign) {
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

            switch (hAlign) {
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
                    format.Alignment = StringAlignment.Near;
                    break;
            }

            return format;
        }

        /// <summary>
        /// Loads a font file into the font collection. Returns the position of the new font family.
        /// </summary>
        /// <param name="_fileName"></param>
        /// <returns></returns>
        public static int LoadFontFromFile(string _fileName) {
            if (!File.Exists(_fileName)) {
                TackConsole.EngineLog(EngineLogType.Error, string.Format("Could not locate file at path: {0}", _fileName));
                return -1;
            }

            ActiveInstance.m_fontCollection.AddFontFile(_fileName);

            TackConsole.EngineLog(EngineLogType.Message, string.Format("Added new font with name: {0} to the TackGUI font collection at index: {1}", ActiveInstance.m_fontCollection.Families[ActiveInstance.m_fontCollection.Families.Length - 1].Name, ActiveInstance.m_fontCollection.Families.Length - 1));
            return ActiveInstance.m_fontCollection.Families.Length - 1;
        }

        /// <summary>
        /// Sets the active FontFamily
        /// </summary>
        /// <param name="_familyName">the Name of the FontFamily</param>
        public static void SetActiveFontFamily(string _familyName) {
            for (int i = 0; i < ActiveInstance.m_fontCollection.Families.Length; i++) {
                if (ActiveInstance.m_fontCollection.Families[i].Name == _familyName) {
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
        public static void SetActiveFontFamily(int _familyIndex) {
            if (_familyIndex < ActiveInstance.m_fontCollection.Families.Length) {
                ActiveInstance.m_activeFontFamily = ActiveInstance.m_fontCollection.Families[_familyIndex];

                TackConsole.EngineLog(EngineLogType.Message, "Set the active FontFamily. Name: {0}, FamilyIndex: {1}", ActiveInstance.m_activeFontFamily.Name, _familyIndex);
            }

            TackConsole.EngineLog(EngineLogType.Error, "The specfied family index is outside the bounds of the font collection Families array");
        }

        /// <summary>
        /// Draws a box on the screen
        /// </summary>
        /// <param name="rect">The shape (Position and size) of the box</param>
        /// <param name="style">The BoxStyle used to render this box</param>
        public static void Box(RectangleShape rect, BoxStyle style = null) {
            if (style == null) {
                style = new BoxStyle();
            }

            GUIOperation operation = new GUIOperation(0);
            operation.Border = style.Border;
            operation.Bounds = rect;
            operation.DrawLevel = 1;
            operation.Sprite = style.SpriteTexture;
            operation.Colour = style.Colour;

            ActiveInstance.m_guiOperations.Add(operation);
        }

        /// <summary>
        /// Renders text to the screen
        /// </summary>
        /// <param name="rect">The shape of box to render the text in</param>
        /// <param name="text">The text to render</param>
        /// <param name="style">The TextAreaStyle used to render this text</param>
        public static void TextArea(RectangleShape rect, string text, TextAreaStyle style = null) {
            if (style == null) {
                style = new TextAreaStyle();
            }

            Bitmap textBitmap = new Bitmap((int)rect.Width, (int)rect.Height);
            Graphics g = Graphics.FromImage(textBitmap);
            g.FillRectangle(ActiveInstance.GetColouredBrush(style.BackgroundColour), 0, 0, rect.Width, rect.Height);
            g.DrawString(text, new Font(GetFontFamily(style.FontFamilyId), style.FontSize, FontStyle.Regular), ActiveInstance.GetColouredBrush(style.FontColour), new PointF(0, 0), ActiveInstance.GenerateTextFormat(style.HorizontalAlignment, style.VerticalAlignment));


        }

        public static int GetFontFamilyId(string familyName) {
            if (ActiveInstance == null) {
                return 0;
            }
            
            for (int i = 0; i < ActiveInstance.m_fontCollection.Families.Length; i++)
            {
                if (ActiveInstance.m_fontCollection.Families[i].Name == familyName) {
                    return i;
                }
            }

            TackConsole.EngineLog(EngineLogType.Error, string.Format("No FontFamily with name: {0} was found in the font collection", familyName));
            return -1;
        }

        /// <summary>
        /// Gets the FontFamily at a specified index
        /// </summary>
        /// <param name="a_fontId">The index of the FontFamily in the collection</param>
        /// <returns></returns>
        public static FontFamily GetFontFamily(int fontId) {
            if (fontId < ActiveInstance.m_fontCollection.Families.Length) {
                return ActiveInstance.m_fontCollection.Families[fontId];
            }

            return ActiveInstance.m_fontCollection.Families[0];
        }

        private static string GetRenderableString(string aString, float aScrollPos, float aHeightPerLine, float aTextAreaHeight) {
            if (string.IsNullOrEmpty(aString)) {
                return "";
            }

            // Split strings by lines
            string[] splitString = aString.Split(new char[] { '\n', '\r' });

            int maxLines = (int)(aTextAreaHeight / aHeightPerLine);

            Console.WriteLine("Height: {0}, HeightPerLine: {1}, Lines: {2}", aTextAreaHeight, aHeightPerLine, maxLines);

            if (splitString.Length <= maxLines) {
                return aString;
            }

            string returnString = "";

            for (int i = (int)aScrollPos; i < (aScrollPos + maxLines); i++) {
                if (i < splitString.Length) {
                    returnString += splitString[i] + "\n";
                } else {
                    continue;
                }
            }

            return returnString;
        }
    }
}

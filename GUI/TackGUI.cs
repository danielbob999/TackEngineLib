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
        }

        internal void OnUpdate() {

        }

        internal void OnGUIRender() {
            // Generate SpriteAtlas object using the list of GUIOperations

            // Loop through all GUIOperations 
            //   - Add vertex postions/colours/texcoords to the dynamic vertex buffer


            // Call 1 or more draw element calls depending on a maxGUIDrawCallAmount variable 
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
        public static void Box(RectangleShape rect, BoxStyle style = default(BoxStyle)) {
            if (style == null) {
                style = new BoxStyle();
            }

            GUIOperation operation = new GUIOperation(0);
            operation.Border = style.Border;
            operation.Bounds = rect;
            operation.DrawLevel = 1;
            
            if (style.SpriteTexture == null) {
                operation.IsSpriteDrawMode = false;
                operation.Colour = style.Colour;
            } else {
                operation.IsSpriteDrawMode = true;
                operation.Sprite = style.SpriteTexture;
            }

            ActiveInstance.m_guiOperations.Add(operation);
        }

        /// <summary>
        /// Renders text to the screen
        /// </summary>
        /// <param name="rect">The shape of box to render the text in</param>
        /// <param name="text">The text to render</param>
        /// <param name="style">The TextAreaStyle used to render this text</param>
        public static void TextArea(RectangleShape rect, string text, TextAreaStyle style = default(TextAreaStyle)) {
            if (style == null) {
                style = new TextAreaStyle();
            }

            // Add a background operation
            GUIOperation backgroundOperation = new GUIOperation(0);
            backgroundOperation.Border = null;
            backgroundOperation.Bounds = rect;
            backgroundOperation.DrawLevel = 1;

            if (style.SpriteTexture == null) {
                backgroundOperation.IsSpriteDrawMode = false;
                backgroundOperation.Colour = style.BackgroundColour;
            } else {
                backgroundOperation.IsSpriteDrawMode = true;
                backgroundOperation.Sprite = style.SpriteTexture;
            }

            ActiveInstance.m_guiOperations.Add(backgroundOperation);

            // Add a text area operation

            GUIOperation textOperation = new GUIOperation(1);
            textOperation.Text = text;
            textOperation.Font = new Font(GetFontFamily(style.FontFamilyId), style.FontSize, FontStyle.Regular);
            textOperation.FontSize = style.FontSize;
            textOperation.TextColour = style.FontColour;
            textOperation.Bounds = rect;
            textOperation.TextHAlignment = style.HorizontalAlignment;
            textOperation.TextVAlignment = style.VerticalAlignment;
            textOperation.DrawLevel = 1;

            ActiveInstance.m_guiOperations.Add(textOperation);
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

/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using TackEngineLib.Engine;
using OpenTK.Graphics.OpenGL;

namespace TackEngineLib.Main {
    /// <summary>
    /// 
    /// </summary>
    public class Sprite {
        internal static bool mLogMessageOverride = false;

        internal static bool LogMessageOverride {
            get { return mLogMessageOverride; }
            set { mLogMessageOverride = value; }
        }

        /// <summary>
        /// The default sprite.
        /// </summary>
        public static Sprite DefaultSprite { get; internal set; }

        /// <summary>
        /// The width of the Sprite
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// The height of the Sprite
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// The auto-generated texture ID for this Sprite
        /// </summary>
        public int Id { get; private set; }


        /// <summary>
        /// The PixelFormat of the data for this Sprite
        /// </summary>
        public System.Drawing.Imaging.PixelFormat PixelFormat { get; private set; }

        /// <summary>
        /// The RGBA data of this Sprite
        /// </summary>
        public byte[] Data { get; private set; }
        private int m_stride;

        internal Sprite(int w = 0, int h = 0) {
            Width = w;
            Height = h;
        }

        /// <summary>
        /// Loads the sprite into memory.
        /// </summary>
        /// <param name="_logMsgs">True if messages should be logged to console, false otherwise</param>
        public void Create(bool _logMsgs = true) {
            int newId;
            GL.GenTextures(1, out newId);

            Id = newId;

            if (Id <= 0) {
                if (_logMsgs || (mLogMessageOverride)) {
                    TackConsole.EngineLog(EngineLogType.Error, string.Format("Error with generating sprite texture. Sprite. TextureId cannot be set to 0 or below. (Current Id = {0})", Id));
                }
                return;
            }

            GL.Enable(EnableCap.Texture2D);
            GL.ActiveTexture(TextureUnit.Texture0);

            SpriteManager.AddSprite(this, _logMsgs);

            if (_logMsgs || (mLogMessageOverride)) {
                TackConsole.EngineLog(EngineLogType.Message, string.Format("Generated Sprite texture with TextureId '{0}'", Id));
            }
        }

        public void Destory(bool _logMsgs = true) {
            SpriteManager.RemoveSprite(this, _logMsgs);
        }
        
        internal void BindToTextureUnit(uint textureUnit) {

        }

        public Bitmap GetBitmapCopy() {
            Bitmap bmpCopy = new Bitmap(Width, Height, PixelFormat);
            BitmapData bmpCopyData = bmpCopy.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, PixelFormat);
            bmpCopyData.Stride = m_stride;

            // Copy m_data back to the bitmap Scan0 
            System.Runtime.InteropServices.Marshal.Copy(Data, 0, bmpCopyData.Scan0, Data.Length);

            bmpCopy.UnlockBits(bmpCopyData);

            return bmpCopy;
        }

        public static Sprite LoadFromFile(string path) {
            Sprite newSprite = new Sprite();
            Bitmap newBp;

            try {
                newBp = new Bitmap(Directory.GetCurrentDirectory() + "\\" + path);
            } catch (FileNotFoundException) {
                TackConsole.EngineLog(EngineLogType.Error, string.Format("Failed to load image data. No file found at path: '{0}'", path));
                return newSprite;
            } catch (Exception e) {
                TackConsole.EngineLog(EngineLogType.Error, string.Format("'{0}'", e.ToString()));
                return newSprite;
            }

            newSprite.Width = newBp.Width;
            newSprite.Height = newBp.Height;

            BitmapData bmpData = newBp.LockBits(new System.Drawing.Rectangle(0, 0, newBp.Width, newBp.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            newSprite.PixelFormat = bmpData.PixelFormat;
            newSprite.m_stride = bmpData.Stride;
            newSprite.Data = new byte[Math.TackMath.AbsVali(bmpData.Stride) * newBp.Height];

            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, newSprite.Data, 0, newSprite.Data.Length);

            newBp.UnlockBits(bmpData);
            newBp.Dispose();

            return newSprite;
        }

        public static Sprite LoadFromBitmap(Bitmap original) {
            Sprite newSprite = new Sprite();

            newSprite.Width = original.Width;
            newSprite.Height = original.Height;

            BitmapData bmpData = original.LockBits(new System.Drawing.Rectangle(0, 0, original.Width, original.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            newSprite.PixelFormat = bmpData.PixelFormat;
            newSprite.m_stride = bmpData.Stride;
            newSprite.Data = new byte[Math.TackMath.AbsVali(bmpData.Stride) * original.Height];

            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, newSprite.Data, 0, newSprite.Data.Length);

            original.UnlockBits(bmpData);

            return newSprite;
        }

        internal static void LoadDefaultSprite() {
            DefaultSprite = Sprite.LoadFromBitmap(TackEngineLib.Properties.Resources.DefaultSprite);
            DefaultSprite.Create();

            TackConsole.EngineLog(EngineLogType.Message, "Loaded the default sprite into Sprite.DefaultSprite");
        }
    }
}

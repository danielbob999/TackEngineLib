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

namespace TackEngineLib.Main
{
    /// <summary>
    /// 
    /// </summary>
    public class Sprite
    {
        private static Sprite mDefaultSprite;
        internal static bool mLogMessageOverride = false;

        internal static bool LogMessageOverride
        {
            get { return mLogMessageOverride; }
            set { mLogMessageOverride = value; }
        }

        /// <summary>
        /// The default sprite.
        /// </summary>
        public static Sprite DefaultSprite
        {
            get { return mDefaultSprite; }
        }

        private int mTextureId;
        private Bitmap mSpriteBmp;
        private BitmapData mSpriteData;
        private int mWidth = 0, mHeight = 0;

        /// <summary>
        /// The width of the Sprite
        /// </summary>
        public int Width
        {
            get { return mWidth; }
            set
            {
                if (value < 0 || value > 16384)
                {
                    mWidth = 0;
                    TackConsole.EngineLog(EngineLogType.Error, "Sprite.Width cannot be set to less than 0 or more than 16384");
                } else { mWidth = value; }
            }
        }

        /// <summary>
        /// The height of the Sprite
        /// </summary>
        public int Height
        {
            get { return mHeight; }
            set
            {
                if (value < 0 || value > 16384)
                {
                    mHeight = 0;
                    TackConsole.EngineLog(EngineLogType.Error, "Sprite.Height cannot be set to less than 0 or more than 16384");
                } else { mHeight = value; }
            }
        }

        /// <summary>
        /// The auto-generated texture ID for this Sprite
        /// </summary>
        public int TextureId { get { return mTextureId; } }

        /// <summary>
        /// The BitmapData of this Sprite
        /// </summary>
        public BitmapData SpriteData
        {
            get { return mSpriteData; }
        }

        /// <summary>
        /// Initialises a new Sprite
        /// </summary>
        internal Sprite() {

        }

        /// <summary>
        /// Initialises a new Sprite
        /// </summary>
        /// <param name="_w">The width of the Sprite</param>
        /// <param name="_h">The height of the Sprite</param>
        public Sprite(int _w, int _h)
        {
            Width = _w;
            Height = _h;
        }

        /// <summary>
        /// Loads the sprite into memory.
        /// </summary>
        /// <param name="_logMsgs">True if messages should be logged to console, false otherwise</param>
        public void Create(bool _logMsgs = true)
        {
            GL.GenTextures(1, out mTextureId);

            if (mTextureId <= 0) {
                if (_logMsgs || (mLogMessageOverride))
                    TackConsole.EngineLog(EngineLogType.Error, string.Format("Error with generating sprite texture. Sprite. TextureId cannot be set to 0 or below. (Current Id = {0})", mTextureId));
                return;
            }

            GL.Enable(EnableCap.Texture2D);
            GL.ActiveTexture(TextureUnit.Texture0);

            mSpriteData = mSpriteBmp.LockBits(new System.Drawing.Rectangle(0, 0, mSpriteBmp.Width, mSpriteBmp.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            SpriteManager.AddSprite(this, _logMsgs);
            if (_logMsgs || (mLogMessageOverride))
                TackConsole.EngineLog(EngineLogType.Message, string.Format("Generated Sprite texture with TextureId '{0}'", mTextureId));
        }

        public void Destory(bool _logMsgs = true)
        {
            mSpriteBmp.UnlockBits(mSpriteData);
            SpriteManager.RemoveSprite(this, _logMsgs);
        }

        public Bitmap GetBitmap()
        {
            return mSpriteBmp;
        }

        public static Sprite LoadFromFile(string _path)
        {
            Sprite newSprite = new Sprite();
            Bitmap newBp;

            try
            {
                newBp = new Bitmap(Directory.GetCurrentDirectory() + "\\" + _path);
            }
            catch (FileNotFoundException)
            {
                TackConsole.EngineLog(EngineLogType.Error, string.Format("Failed to load image data. No file found at path: '{0}'", _path));
                return newSprite;
            }
            catch (Exception e)
            {
                TackConsole.EngineLog(EngineLogType.Error, string.Format("'{0}'", e.ToString()));
                return newSprite;
            }

            newSprite.mSpriteBmp = newBp;
            newSprite.Width = newBp.Width;
            newSprite.Height = newBp.Height;

            return newSprite;
        }

        public static Sprite LoadFromBitmap(Bitmap _bitmap)
        {
            Sprite newSprite = new Sprite();
            newSprite.mSpriteBmp = _bitmap;
            newSprite.Width = _bitmap.Width;
            newSprite.Height = _bitmap.Height;

            return newSprite;
        }

        internal static void LoadDefaultSprite()
        {
            mDefaultSprite = Sprite.LoadFromBitmap(TackEngineLib.Properties.Resources.DefaultSprite);
            mDefaultSprite.Create();

            TackConsole.EngineLog(EngineLogType.Message, "Loaded the default sprite into Sprite.DefaultSprite");
        }
    }
}

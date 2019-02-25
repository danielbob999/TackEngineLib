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
        private static Sprite m_DefaultSprite;
        private static bool m_LogMessageOverride = true;

        public static bool LogMessageOverride
        {
            get { return m_LogMessageOverride; }
            set { m_LogMessageOverride = value; }
        }

        public static Sprite DefaultSprite
        {
            get { return m_DefaultSprite; }
        }

        private int m_TextureId;
        private Bitmap m_SpriteBmp;
        private BitmapData m_SpriteData;
        private int m_Width = 0, m_Height = 0;

        // Properties
        public int Width
        {
            get { return m_Width; }
            set
            {
                if (value < 0 || value > 16384)
                {
                    m_Width = 0;
                    TackConsole.EngineLog(EngineLogType.Error, "Sprite.Width cannot be set to less than 0 or more than 16384");
                } else { m_Width = value; }
            }
        }

        public int Height
        {
            get { return m_Height; }
            set
            {
                if (value < 0 || value > 16384)
                {
                    m_Height = 0;
                    TackConsole.EngineLog(EngineLogType.Error, "Sprite.Height cannot be set to less than 0 or more than 16384");
                } else { m_Height = value; }
            }
        }

        public int TextureId { get { return m_TextureId; } }

        public BitmapData SpriteData
        {
            get { return m_SpriteData; }
        }

        public Sprite() { }

        public Sprite(int _w, int _h)
        {
            Width = _w;
            Height = _h;
        }

        public void Create(bool _logMsgs = true)
        {
            GL.GenTextures(1, out m_TextureId);

            if (m_TextureId <= 0) {
                if (_logMsgs || (m_LogMessageOverride))
                    TackConsole.EngineLog(EngineLogType.Error, string.Format("Error with generating sprite texture. Sprite. TextureId cannot be set to 0 or below. (Current Id = {0})", m_TextureId));
                return;
            }

            GL.Enable(EnableCap.Texture2D);
            GL.ActiveTexture(TextureUnit.Texture0);

            m_SpriteData = m_SpriteBmp.LockBits(new System.Drawing.Rectangle(0, 0, m_SpriteBmp.Width, m_SpriteBmp.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            SpriteManager.AddSprite(this, _logMsgs);
            if (_logMsgs || (m_LogMessageOverride))
                TackConsole.EngineLog(EngineLogType.Message, string.Format("Generated Sprite texture with TextureId '{0}'", m_TextureId));
        }

        public void Destory(bool _logMsgs = true)
        {
            m_SpriteBmp.UnlockBits(m_SpriteData);
            SpriteManager.RemoveSprite(this, _logMsgs);
        }

        public Bitmap GetBitmap()
        {
            return m_SpriteBmp;
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

            newSprite.m_SpriteBmp = newBp;
            newSprite.Width = newBp.Width;
            newSprite.Height = newBp.Height;

            return newSprite;
        }

        public static Sprite LoadFromBitmap(Bitmap _bitmap)
        {
            Sprite newSprite = new Sprite();
            newSprite.m_SpriteBmp = _bitmap;
            newSprite.Width = _bitmap.Width;
            newSprite.Height = _bitmap.Height;

            return newSprite;
        }

        internal static void LoadDefaultSprite()
        {
            m_DefaultSprite = Sprite.LoadFromBitmap(TackEngineLib.Properties.Resources.DefaultSprite);
            m_DefaultSprite.Create();

            TackConsole.EngineLog(EngineLogType.Message, "Loaded the default sprite into Sprite.DefaultSprite");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using TackEngineLib.Main;
using TackEngineLib.Engine;

namespace TackEngineLib.Main
{
    public class SpriteSheet
    {
        private Sprite m_SpriteSheetTexture;
        private Sprite[] m_SingleSpriteTextures;
        private int m_SpriteRefreshTime = 0;
        private int m_SingleSpriteWidth = 0;
        private int m_SingleSpriteHeight = 0;
        private int m_SpriteCount = 0;
        private int m_CurrentSpriteId = 0;
        private Stopwatch m_StopWatch;
        private bool m_Loop = true;

        // Properties
        public Sprite SpriteSheetTexture
        {
            get { return m_SpriteSheetTexture; }
            set { m_SpriteSheetTexture = value; }
        }

        public int SpriteSheetRefreshTime
        {
            get { return m_SpriteRefreshTime; }
            set { m_SpriteRefreshTime = value; }
        }

        public int SingleSpriteWidth
        {
            get { return m_SingleSpriteWidth; }
            set
            {
                if (value >= 0)
                {
                    m_SingleSpriteWidth = value;
                }
                else
                {
                    TackConsole.EngineLog(EngineLogType.Error, "SingleSpriteWidth cannot be set to less than 0");
                }
            }
        }

        public int SingleSpriteHeight
        {
            get { return m_SingleSpriteHeight; }
            set
            {
                if (value >= 0)
                {
                    m_SingleSpriteHeight = value;
                }
                else
                {
                    TackConsole.EngineLog(EngineLogType.Error, "SingleSpriteHeight cannot be set to less than 0");
                }
            }
        }

        public int CurrentSpriteId
        {
            get { return m_CurrentSpriteId; }
            set { m_CurrentSpriteId = value; } // TODO: Validation, cannot set id to higher than m_SpriteCount
        }

        public bool Loop
        {
            get { return m_Loop; }
            set { m_Loop = value; }
        }

        public SpriteSheet()
        {
            m_SpriteRefreshTime = 0;
            m_SingleSpriteWidth = 0;
            m_SingleSpriteHeight = 0;

            m_CurrentSpriteId = 0;
        }

        public SpriteSheet(Sprite _tex, int _refreshTime, int _w, int _h, int _count)
        {
            m_SpriteSheetTexture = _tex;
            m_SingleSpriteTextures = new Sprite[_count];
            m_SpriteRefreshTime = _refreshTime;
            m_SingleSpriteWidth = _w;
            m_SingleSpriteHeight = _h;
            m_SpriteCount = _count;

            m_CurrentSpriteId = 0;
        }

        public void Create()
        {
            for (int i = 0; i < m_SpriteCount; i++)
            {
                m_SingleSpriteTextures[i] = Sprite.LoadFromBitmap(m_SpriteSheetTexture.GetBitmap().Clone(
                    new System.Drawing.RectangleF(i * m_SingleSpriteWidth, 0, m_SingleSpriteWidth, m_SingleSpriteHeight), m_SpriteSheetTexture.GetBitmap().PixelFormat));

                m_SingleSpriteTextures[i].Width = m_SingleSpriteWidth;
                m_SingleSpriteTextures[i].Height = m_SingleSpriteHeight;

                m_SingleSpriteTextures[i].Create();
            }

            TackConsole.EngineLog(EngineLogType.Message, string.Format("Created {0} Sprites from a SpriteSheet. SingleWidth: {1}, SingleHeight{2}", m_SpriteCount, m_SingleSpriteWidth, m_SingleSpriteHeight));
        }

        public void StartTimer()
        {
            m_StopWatch = new Stopwatch();
            m_StopWatch.Start();
            //EngineLog.WriteMessage("Starting SpriteSheet Stopwatch");
        }

        public Sprite SetFirstSprite()
        {
            Sprite newSprite = new Sprite();
            newSprite = Sprite.LoadFromBitmap(m_SpriteSheetTexture.GetBitmap().Clone(
                    new System.Drawing.RectangleF(0, 0, m_SingleSpriteWidth, m_SingleSpriteHeight), m_SpriteSheetTexture.GetBitmap().PixelFormat));

            newSprite.Width = m_SingleSpriteWidth;
            newSprite.Height = m_SingleSpriteHeight;

            newSprite.Create();

            return newSprite;
        }

        public void SpriteUpdateCheck()
        {
            if (m_StopWatch.ElapsedMilliseconds >= m_SpriteRefreshTime)
            {
                //EngineLog.WriteMessage("{0} >= {1}. Changing current Sprite id. {2} -> {3}", m_StopWatch.ElapsedMilliseconds, m_SpriteRefreshTime, m_CurrentSpriteId, GetNextValidSpriteId());

                m_CurrentSpriteId = GetNextValidSpriteId();

                m_StopWatch.Restart();
            }
        }

        /// <summary>
        /// Gets the next valid sprite id in the spritesheet. If no more data is found and Loop=true, return 0, else return current id
        /// </summary>
        /// <returns></returns>
        public int GetNextValidSpriteId()
        {
            // Is there another sprite to the right?
            if (m_CurrentSpriteId < m_SpriteCount - 1)
            {
                return m_CurrentSpriteId + 1;
            }

            // There is no more sprite data to the right, and if loop=true, set back to index 0
            if (m_Loop)
            {
                return 0;
            }

            return m_CurrentSpriteId;
        }

        public Sprite GetActiveSprite()
        {
            return m_SingleSpriteTextures[m_CurrentSpriteId];
        }
    }
}

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
        private Sprite mSpriteSheetTexture;
        private Sprite[] mSingleSpriteTextures;
        private int mSpriteRefreshTime = 0;
        private int mSingleSpriteWidth = 0;
        private int mSingleSpriteHeight = 0;
        private int mSpriteCount = 0;
        private int mCurrentSpriteId = 0;
        private Stopwatch mStopWatch;
        private bool mLoop = true;

        // Properties
        public Sprite SpriteSheetTexture
        {
            get { return mSpriteSheetTexture; }
            set { mSpriteSheetTexture = value; }
        }

        public int SpriteSheetRefreshTime
        {
            get { return mSpriteRefreshTime; }
            set { mSpriteRefreshTime = value; }
        }

        public int SingleSpriteWidth
        {
            get { return mSingleSpriteWidth; }
            set
            {
                if (value >= 0)
                {
                    mSingleSpriteWidth = value;
                }
                else
                {
                    TackConsole.EngineLog(EngineLogType.Error, "SingleSpriteWidth cannot be set to less than 0");
                }
            }
        }

        public int SingleSpriteHeight
        {
            get { return mSingleSpriteHeight; }
            set
            {
                if (value >= 0)
                {
                    mSingleSpriteHeight = value;
                }
                else
                {
                    TackConsole.EngineLog(EngineLogType.Error, "SingleSpriteHeight cannot be set to less than 0");
                }
            }
        }

        public int CurrentSpriteId
        {
            get { return mCurrentSpriteId; }
            set { mCurrentSpriteId = value; } // TODO: Validation, cannot set id to higher than mSpriteCount
        }

        public bool Loop
        {
            get { return mLoop; }
            set { mLoop = value; }
        }

        public SpriteSheet()
        {
            mSpriteRefreshTime = 0;
            mSingleSpriteWidth = 0;
            mSingleSpriteHeight = 0;

            mCurrentSpriteId = 0;
        }

        public SpriteSheet(Sprite _tex, int _refreshTime, int _w, int _h, int _count)
        {
            mSpriteSheetTexture = _tex;
            mSingleSpriteTextures = new Sprite[_count];
            mSpriteRefreshTime = _refreshTime;
            mSingleSpriteWidth = _w;
            mSingleSpriteHeight = _h;
            mSpriteCount = _count;

            mCurrentSpriteId = 0;
        }

        public void Create()
        {
            for (int i = 0; i < mSpriteCount; i++)
            {
                mSingleSpriteTextures[i] = Sprite.LoadFromBitmap(mSpriteSheetTexture.GetBitmap().Clone(
                    new System.Drawing.RectangleF(i * mSingleSpriteWidth, 0, mSingleSpriteWidth, mSingleSpriteHeight), mSpriteSheetTexture.GetBitmap().PixelFormat));

                mSingleSpriteTextures[i].Width = mSingleSpriteWidth;
                mSingleSpriteTextures[i].Height = mSingleSpriteHeight;

                mSingleSpriteTextures[i].Create();
            }

            TackConsole.EngineLog(EngineLogType.Message, string.Format("Created {0} Sprites from a SpriteSheet. SingleWidth: {1}, SingleHeight{2}", mSpriteCount, mSingleSpriteWidth, mSingleSpriteHeight));
        }

        public void StartTimer()
        {
            mStopWatch = new Stopwatch();
            mStopWatch.Start();
            //EngineLog.WriteMessage("Starting SpriteSheet Stopwatch");
        }

        public Sprite SetFirstSprite()
        {
            Sprite newSprite = new Sprite();
            newSprite = Sprite.LoadFromBitmap(mSpriteSheetTexture.GetBitmap().Clone(
                    new System.Drawing.RectangleF(0, 0, mSingleSpriteWidth, mSingleSpriteHeight), mSpriteSheetTexture.GetBitmap().PixelFormat));

            newSprite.Width = mSingleSpriteWidth;
            newSprite.Height = mSingleSpriteHeight;

            newSprite.Create();

            return newSprite;
        }

        public void SpriteUpdateCheck()
        {
            if (mStopWatch.ElapsedMilliseconds >= mSpriteRefreshTime)
            {
                //EngineLog.WriteMessage("{0} >= {1}. Changing current Sprite id. {2} -> {3}", mStopWatch.ElapsedMilliseconds, mSpriteRefreshTime, mCurrentSpriteId, GetNextValidSpriteId());

                mCurrentSpriteId = GetNextValidSpriteId();

                mStopWatch.Restart();
            }
        }

        /// <summary>
        /// Gets the next valid sprite id in the spritesheet. If no more data is found and Loop=true, return 0, else return current id
        /// </summary>
        /// <returns></returns>
        public int GetNextValidSpriteId()
        {
            // Is there another sprite to the right?
            if (mCurrentSpriteId < mSpriteCount - 1)
            {
                return mCurrentSpriteId + 1;
            }

            // There is no more sprite data to the right, and if loop=true, set back to index 0
            if (mLoop)
            {
                return 0;
            }

            return mCurrentSpriteId;
        }

        public Sprite GetActiveSprite()
        {
            return mSingleSpriteTextures[mCurrentSpriteId];
        }
    }
}

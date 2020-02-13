using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using TackEngineLib.Main;

namespace TackEngineLib.Renderer.Sprite {
    public class SpriteAtlas {
        private List<SpriteAtlasEntry> m_atlasEntries;
        private Bitmap m_bitmap;

        public int Width { get { return m_bitmap.Width; } }
        public int Height { get { return m_bitmap.Height; } }

        public SpriteAtlas() {
            m_bitmap = new Bitmap(1, 1);
            m_atlasEntries = new List<SpriteAtlasEntry>();
        }

        public void AddSprite(Main.Sprite sp) {
            if (sp == null || m_bitmap == null) {
                return;
            }

            for (int i = 0; i < m_atlasEntries.Count; i++) {
                if (m_atlasEntries[i].Sprite.Id == sp.Id) {
                    return; // If the sprite has already been added to the atlas, return without adding again
                }
            }

            // Create new atlas entry
            SpriteAtlasEntry newEntry = new SpriteAtlasEntry();
            newEntry.Sprite = sp;

            // Add it to the list
            m_atlasEntries.Add(newEntry);

            // Redraw the bitmap
            RedrawBitmap();

            // Recalculate all texture coords
            RecalculateTexCoords();
        }

        public void RedrawBitmap() {
            if (m_bitmap == null) {
                return;
            }

            m_bitmap.Dispose();

            if (m_atlasEntries.Count == 0) {
                m_bitmap = new Bitmap(1, 1);
                return;
            }

            int maxWidth = 0;
            int height = 0;

            for (int i = 0; i < m_atlasEntries.Count; i++) {
                if (m_atlasEntries[i].Sprite.Width > maxWidth) {
                    maxWidth = m_atlasEntries[i].Sprite.Width;
                }

                height += m_atlasEntries[i].Sprite.Height;
            }

            m_bitmap = new Bitmap(maxWidth, height);
            Graphics g = Graphics.FromImage(m_bitmap);
            g.Clear(Color.Transparent);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            float currOffset = 0.0f;

            for (int i = 0; i < m_atlasEntries.Count; i++) {
                Bitmap singleBmp = m_atlasEntries[i].Sprite.GetBitmapCopy();
                g.DrawImage(singleBmp, 0, currOffset);
                singleBmp.Dispose();

                currOffset += m_atlasEntries[i].Sprite.Height;
            }

            g.Dispose();
        }

        /// <summary>
        /// Recalculates the texture coordinates for all entries. Needs to be called after RedrawBitmap()
        /// </summary>
        public void RecalculateTexCoords() {
            if (m_bitmap == null) {
                return;
            }

            int currentHeightOffset = 0;
            int maxWidth = m_bitmap.Width;
            int maxHeight = m_bitmap.Height;

            for (int i = 0; i < m_atlasEntries.Count; i++) {
                int spriteWidth = m_atlasEntries[i].Sprite.Width;
                int spriteHeight = m_atlasEntries[i].Sprite.Height;

                // mw = 1628
                // mh = 1128

                float top = Math.TackMath.Clamp(currentHeightOffset / (float)maxHeight, 0.0f, 1.0f); // 0 / 
                float bottom = Math.TackMath.Clamp((currentHeightOffset + spriteHeight) / (float)maxHeight, 0.0f, 1.0f); // 48 / 1128
                float width = Math.TackMath.Clamp(spriteWidth / (float)maxWidth, 0.0f, 1.0f);

                m_atlasEntries[i].TexCoordVert1 = new Tuple<float, float>(width, top);
                m_atlasEntries[i].TexCoordVert2 = new Tuple<float, float>(width, bottom);
                m_atlasEntries[i].TexCoordVert3 = new Tuple<float, float>(0, bottom);
                m_atlasEntries[i].TexCoordVert4 = new Tuple<float, float>(0, top);

                currentHeightOffset += m_atlasEntries[i].Sprite.Height;
            }
        }

        public Tuple<float, float>[] GetTexCoords(int spriteId) {
            for (int i = 0; i < m_atlasEntries.Count; i++) {
                if (m_atlasEntries[i].Sprite.Id == spriteId) {
                    return m_atlasEntries[i].GetTexCoords();
                }
            }

            return new Tuple<float, float>[] { new Tuple<float, float>(0, 0), new Tuple<float, float>(0, 0), new Tuple<float, float>(0, 0), new Tuple<float, float>(0, 0) };
        }

        public Bitmap GetBitmap() {
            return m_bitmap;
        }

        public void SaveToFile(string path) {
            m_bitmap.Save(path);
        }

        public void Destory() {
            m_bitmap.Dispose();
            m_bitmap = null;
        }
    }
}

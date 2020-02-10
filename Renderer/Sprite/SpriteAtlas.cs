using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using TackEngineLib.Main;

namespace TackEngineLib.Renderer.Sprite {
    internal class SpriteAtlas {
        private List<SpriteAtlasEntry> m_atlasEntries;
        private Bitmap m_bitmap;

        public SpriteAtlas() {
            m_bitmap = new Bitmap(1, 1);
            m_atlasEntries = new List<SpriteAtlasEntry>();
        }

        public void AddSprite(Main.Sprite sp) {
            if (sp == null) {
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

            // Calculate it's texcoords
            int heightOffset = 0;

            for (int i = 0; i < m_atlasEntries.Count; i++) {
                heightOffset += m_atlasEntries[i].Sprite.Height;
            }

            float fullWidth = 0.0f;

            // If the new sprite is now the widest one set fullwidth to 1.0f, else calculate the value
            if (sp.Width > m_bitmap.Width) {
                fullWidth = 1.0f;
            } else {
                fullWidth = sp.Width / m_bitmap.Width;
            }

            float top = Math.TackMath.Clamp(heightOffset / (m_bitmap.Height + sp.Height), 0.0f, 1.0f);
            float bottom = Math.TackMath.Clamp((heightOffset + sp.Height) / (m_bitmap.Height + sp.Height), 0.0f, 1.0f);

            newEntry.TexCoordVert1 = new Tuple<float, float>(fullWidth, top);
            newEntry.TexCoordVert2 = new Tuple<float, float>(fullWidth, bottom);
            newEntry.TexCoordVert3 = new Tuple<float, float>(0, bottom);
            newEntry.TexCoordVert4 = new Tuple<float, float>(0, top);

            // Add it to the list
            m_atlasEntries.Add(newEntry);

            // Redraw the bitmap
            RedrawBitmap();
        }

        public void RedrawBitmap() {
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
            float currOffset = 0.0f;

            for (int i = 0; i < m_atlasEntries.Count; i++) {
                Bitmap singleBmp = m_atlasEntries[i].Sprite.GetBitmapCopy();
                g.DrawImage(singleBmp, 0, currOffset);
                singleBmp.Dispose();

                currOffset += m_atlasEntries[i].Sprite.Height;
            }

            g.Dispose();
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
    }
}

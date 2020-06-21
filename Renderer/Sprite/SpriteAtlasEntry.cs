using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;

namespace TackEngineLib.Renderer.Sprite {
    internal class SpriteAtlasEntry {
        public Main.Sprite Sprite { get; set; }
        public Tuple<float, float> TexCoordVert1 { get; set; }
        public Tuple<float, float> TexCoordVert2 { get; set; }
        public Tuple<float, float> TexCoordVert3 { get; set; }
        public Tuple<float, float> TexCoordVert4 { get; set; }

        public SpriteAtlasEntry() {

        }

        public Tuple<float, float>[] GetTexCoords() {
            return new Tuple<float, float>[] { TexCoordVert1, TexCoordVert2, TexCoordVert3, TexCoordVert4 };
        }
    }
}

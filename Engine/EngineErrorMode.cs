using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngineLib.Engine
{
    public enum EngineErrorMode
    {
        EngineUpdateCycle,
        EngineWindowCreation,
        LightComponent,
        NULL,
        OpenGL,
        OutOfBounds,
        QuadRendererComponent,
        Renderer,
        Sprite,
        SpriteSheet,
        SpriteAlreadyAdded,
        SpriteIdNotWithinBounds,
        SpriteManager,
        TackGUI,
        TackLog,
        TackObject
    }
}

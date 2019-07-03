/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngineLib.Main;
using TackEngineLib.Engine;
using OpenTK.Graphics.OpenGL;

namespace TackEngineLib.Engine
{
    internal static class SpriteManager
    {
        private static List<Sprite> loadedSprites = new List<Sprite>();

        public static void OnClose()
        {
            foreach (Sprite sp in loadedSprites)
            {
                GL.DeleteTexture(sp.TextureId);
                TackConsole.EngineLog(EngineLogType.Message, string.Format("Deleted (OpenGL) Sprite texture with id '{0}'", sp.TextureId));
            }

            loadedSprites.Clear();

            if (loadedSprites.Count == 0)
            {
                TackConsole.EngineLog(EngineLogType.Message, "Successfully deleted all Sprites from SpriteManager");
            }
            else
            {
                TackConsole.EngineLog(EngineLogType.Error, string.Format("Failed to delete all Sprites. Number left in SpriteManager: {0}", loadedSprites.Count));
            }
        }

        public static void AddSprite(Sprite _sprite, bool _debugMsgs = true)
        {
            if (_sprite.TextureId <= 0)
            {
                if (_debugMsgs)
                    TackConsole.EngineLog(EngineLogType.Error, "Cannot add Sprite with id lower than 1 to SpriteManager");
                return;
            }

            if (loadedSprites.Contains(_sprite))
            {
                if (_debugMsgs)
                    TackConsole.EngineLog(EngineLogType.Error, string.Format("Sprite with id '{0}' cannot be added because it is already in SpriteManager", _sprite.TextureId));
                return;
            }

            loadedSprites.Add(_sprite);
            if (_debugMsgs)
                TackConsole.EngineLog(EngineLogType.Message, string.Format("Added new Sprite with id '{0}' to SpriteManager", _sprite.TextureId));
        }

        public static void RemoveSprite(Sprite _sprite, bool _debugMsgs = true)
        {
            if (!loadedSprites.Contains(_sprite))
            {
                if (_debugMsgs)
                    TackConsole.EngineLog(EngineLogType.Error, string.Format("Trying to remove Sprite with id '{0}' but it doesn't exist in SpriteManager", _sprite.TextureId));
                return;
            }

            GL.DeleteTexture(_sprite.TextureId);
            loadedSprites.Remove(_sprite);
            if (_debugMsgs)
                TackConsole.EngineLog(EngineLogType.Message, string.Format("Removed Sprite with id '{0}' from SpriteManager", _sprite.TextureId));
        }

    }
}

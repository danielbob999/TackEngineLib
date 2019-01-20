using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngineLib.Main;
using TackEngineLib.Engine;
using TackEngineLib.Renderer;

namespace TackEngineLib.Objects.Components
{
    public class QuadRenderer : TackComponent
    {
        public RectangleShape rectange;
        private Sprite m_Sprite;
        private SpriteSheet m_SpriteSheet;
        private Colour4b m_Colour;

        private RendererMode m_RenderMode = RendererMode.Colour;

        // PROPERTIES

        /// <summary>
        /// The Sprite to be rendered on this quad.
        /// </summary>
        public Sprite Sprite
        {
            get { return m_Sprite; }
            set { m_Sprite = value; }
        }

        /// <summary>
        /// The SpriteSheet that will be used by this QuadRenderer.
        /// </summary>
        public SpriteSheet SpriteSheet
        {
            get { return m_SpriteSheet; }
            set { m_SpriteSheet = value; }
        }

        /// <summary>
        /// The colour of this quad
        /// </summary>
        public Colour4b Colour
        {
            get { return m_Colour; }
            set { m_Colour = value; }
        }

        /// <summary>
        /// The RenderMode of this QuadRenderer. Use this to specify whether to render a Sprite/SpriteSheet/Colour.
        /// </summary>
        public RendererMode RenderMode
        {
            get { return m_RenderMode; }
            set { m_RenderMode = value; }
        }

        public QuadRenderer() { }

        public QuadRenderer(RectangleShape _rect, Colour4b _colour)
        {
            rectange = _rect;
            m_Colour = _colour;

            Sprite = Main.Sprite.LoadFromBitmap(Properties.Resources.DefaultSprite);
            Sprite.Create(false);
        }

        public override void OnStart()
        {
            base.OnStart();

            if (m_RenderMode == RendererMode.SpriteSheet && m_SpriteSheet != null)
            {
                m_SpriteSheet.StartTimer();
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (m_RenderMode == RendererMode.SpriteSheet)
            {
                m_SpriteSheet.SpriteUpdateCheck();
            }
        }

        public override void OnRender()
        {
            base.OnRender();
        }
    }

    public enum RendererMode
    {
        Colour,
        Sprite,
        SpriteSheet
    }
}

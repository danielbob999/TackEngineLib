﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngineLib.Main;
using TackEngineLib.Engine;
using TackEngineLib.Renderer;

namespace TackEngineLib.Objects.Components
{
    /// <summary>
    /// The TackComponent that is used to render a TackObject on screen
    /// </summary>
    public class QuadRenderer : TackComponent
    {
        /* VERTEX LAYOUT
         * 
         *    v4 ------ v1
         *    |         |
         *    |         |
         *    |         |
         *    v3 ------ v2
         * 
         */

        //public RectangleShape rectange;
        private Sprite m_Sprite;
        private SpriteSheet m_SpriteSheet;
        private Colour4b m_Colour;
        private float[] m_ActualVertexPositions; // [0] = v1, [1] = v2, [2] = v3, [3] = v4

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

        /// <summary>
        /// Creates a new QuadRenderer component width default values
        /// </summary>
        public QuadRenderer()
        {
            m_ActualVertexPositions = new float[4];
        }

        public QuadRenderer(RectangleShape _rect, Colour4b _colour)
        {
            //rectange = _rect;
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

            //FindVertexPoint(1);
            //FindVertexPoint(2);
        }

        public override void OnRender()
        {
            base.OnRender();
        }

        /// <summary>
        /// Calculates the actual vertex point with position and rotation factored in
        /// </summary>
        /// <param name="_vertIndex">The index of the vertex of which to find the position of (1 - 4)</param>
        /// <returns>The position, in Vector2f form, of the vertex</returns>
        public Vector2f FindVertexPoint(int _vertIndex)
        {

            /*
             * Multiplying matrix stuff
             * Formulas from: https://math.stackexchange.com/questions/384186/calculate-new-positon-of-rectangle-corners-based-on-angle
             * 
             * x = x0 + (x - x0) * cos(angle) + (y - y0) * sin(angle)
             * y = y0 - (x - x0) * sin(angle) + (y - y0) * cos(angle)
             * 
             * angle = angle of rotation in degrees
             * x/y = the current point if the vertex
             * x0/y0 = the centre point of the rectangle (the rotation point)
             * 
             */

            if (_vertIndex < 1 || _vertIndex > 4)
            {
                TackConsole.EngineLog(EngineLogType.Error, string.Format("Cannot calculate the position of the vertex with index : {0}. Index values must be within the range (1-4, inclusive)", _vertIndex));
                return new Vector2f();
            }

            RectangleShape objectShape = new RectangleShape()
            {
                X = ((parentObject.Position.X) - (parentObject.Scale.X / 2)),
                Y = ((parentObject.Position.Y ) + (parentObject.Scale.Y / 2)),
                Width = (parentObject.Scale.X),
                Height = (parentObject.Scale.Y)
            };

            if (_vertIndex == 1)
            {
                Vector2f vertPos = new Vector2f(objectShape.X + objectShape.Width, objectShape.Y);

                float x = parentObject.Position.X + (vertPos.X - parentObject.Position.X)
                    * (float)Math.Cos(TackMath.DegToRad(parentObject.Rotation)) + (vertPos.Y - parentObject.Position.Y)
                    * (float)Math.Sin(TackMath.DegToRad(parentObject.Rotation));

                float y = parentObject.Position.Y - (vertPos.X - parentObject.Position.X)
                    * (float)Math.Sin(TackMath.DegToRad(parentObject.Rotation)) + (vertPos.Y - parentObject.Position.Y)
                    * (float)Math.Cos(TackMath.DegToRad(parentObject.Rotation));

                return new Vector2f(x, y);
            }

            if (_vertIndex == 2)
            {
                Vector2f vertPos = new Vector2f(objectShape.X + objectShape.Width, objectShape.Y - objectShape.Height);

                float x = parentObject.Position.X + (vertPos.X - parentObject.Position.X)
                    * (float)Math.Cos(TackMath.DegToRad(parentObject.Rotation)) + (vertPos.Y - parentObject.Position.Y)
                    * (float)Math.Sin(TackMath.DegToRad(parentObject.Rotation));

                float y = parentObject.Position.Y - (vertPos.X - parentObject.Position.X)
                    * (float)Math.Sin(TackMath.DegToRad(parentObject.Rotation)) + (vertPos.Y - parentObject.Position.Y)
                    * (float)Math.Cos(TackMath.DegToRad(parentObject.Rotation));

                return new Vector2f(x, y);
            }

            if (_vertIndex == 3)
            {
                Vector2f vertPos = new Vector2f(objectShape.X, objectShape.Y - objectShape.Height);

                float x = parentObject.Position.X + (vertPos.X - parentObject.Position.X)
                    * (float)Math.Cos(TackMath.DegToRad(parentObject.Rotation)) + (vertPos.Y - parentObject.Position.Y)
                    * (float)Math.Sin(TackMath.DegToRad(parentObject.Rotation));

                float y = parentObject.Position.Y - (vertPos.X - parentObject.Position.X)
                    * (float)Math.Sin(TackMath.DegToRad(parentObject.Rotation)) + (vertPos.Y - parentObject.Position.Y)
                    * (float)Math.Cos(TackMath.DegToRad(parentObject.Rotation));

                return new Vector2f(x, y);
            }

            if (_vertIndex == 4)
            {
                Vector2f vertPos = new Vector2f(objectShape.X, objectShape.Y);

                float x = parentObject.Position.X + (vertPos.X - parentObject.Position.X)
                    * (float)Math.Cos(TackMath.DegToRad(parentObject.Rotation)) + (vertPos.Y - parentObject.Position.Y)
                    * (float)Math.Sin(TackMath.DegToRad(parentObject.Rotation));

                float y = parentObject.Position.Y - (vertPos.X - parentObject.Position.X)
                    * (float)Math.Sin(TackMath.DegToRad(parentObject.Rotation)) + (vertPos.Y - parentObject.Position.Y)
                    * (float)Math.Cos(TackMath.DegToRad(parentObject.Rotation));

                return new Vector2f(x, y);
            }

            return new Vector2f(-1, -1);
        }
    }

    public enum RendererMode
    {
        Colour,
        Sprite,
        SpriteSheet
    }
}

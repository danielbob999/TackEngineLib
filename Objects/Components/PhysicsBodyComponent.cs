/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;
using TackEngineLib.Physics;
using tainicom.Aether.Physics2D.Dynamics;

namespace TackEngineLib.Objects.Components
{
    public class PhysicsBodyComponent : TackComponent {

        public enum ShapeType {
            None,
            Rectangle,
            Circle
        }

        private Body m_physicsBody;
        private List<Fixture> m_fixtures;
        private Vector2f m_bodySizeMultiplier; // Size multiplier of the collider compared to the object's scale. 1.0f = collider size is the same as parentObject.Scale
        private Vector2f m_bodyOffset;
        private float m_mass;

        internal Body PhysicsBody {
            get { return m_physicsBody; }
        }

        internal Fixture[] Fixtures {
            get { return m_fixtures.ToArray(); }
        }

        /// <summary>
        /// The size multiplier of the physics body
        /// </summary>
        public Vector2f BodySizeMultiplier {
            get { return m_bodySizeMultiplier; }
            set { m_bodySizeMultiplier = value; }
        }

        /// <summary>
        /// The offset of the physics body
        /// </summary>
        public Vector2f BodyOffset {
            get { return m_bodyOffset; }
            set { m_bodyOffset = value; }
        }

        public float Mass {
            get { return m_physicsBody.Mass; }
            set { m_physicsBody.Mass = value; }
        }

        public PhysicsBodyComponent() {
            m_fixtures = new List<Fixture>();
            m_physicsBody = TackPhysics.GetPhysicsWorld().CreateBody();
        }

        public override void OnStart() {
            base.OnStart();
        }

        public override void OnUpdate() {
            base.OnUpdate();

            parentObject.Position = new Vector2f(m_physicsBody.Position.X, m_physicsBody.Position.Y);
        }

        public override void OnAddedToTackObject() {
            TackPhysics.RegisterPhysicsComponent(this);
            RegenerateFixtures();
        }

        public void AddForce(float forceX, float forceY, TackPhysics.ForceType forceType) {
            AddForce(new Vector2f(forceX, forceY), forceType);
        }

        public void AddForce(Vector2f force, TackPhysics.ForceType forceType) {
            TackPhysics.AddForceToComponent(this, force, forceType);
        }

        public void Destroy() {
            TackPhysics.DeregisterPhysicsComponent(this);
        }

        private void RegenerateFixtures() {
            if (m_physicsBody == null) {
                return;
            }

            for (int i = 0; i < m_fixtures.Count; i++) {
                m_physicsBody.Remove(m_fixtures[i]);
            }

            /*
             * Vertex Positions
             * 
             *     v4 ------ v3
             *      |         |
             *      |         |
             *     v1 ------ v2
             */

            // Final calculated scale
            Vector2f finalScale = new Vector2f(parentObject.Scale.X * m_bodySizeMultiplier.X, parentObject.Scale.Y * m_bodySizeMultiplier.Y);
            Vector2f finalScaleSplit = new Vector2f(finalScale.X / 2.0f, finalScale.Y / 2.0f);
            
            // Vertex: v1 -> v2
            m_fixtures.Add(m_physicsBody.CreateEdge(
                new tainicom.Aether.Physics2D.Common.Vector2(parentObject.Position.X - finalScaleSplit.X, parentObject.Position.Y - finalScaleSplit.Y),     // Start point
                new tainicom.Aether.Physics2D.Common.Vector2(parentObject.Position.X + finalScaleSplit.X, parentObject.Position.Y - finalScaleSplit.Y)));    // End point


            // Vertex: v2 -> v3
            m_fixtures.Add(m_physicsBody.CreateEdge(
                new tainicom.Aether.Physics2D.Common.Vector2(parentObject.Position.X + finalScaleSplit.X, parentObject.Position.Y - finalScaleSplit.Y), 
                new tainicom.Aether.Physics2D.Common.Vector2(parentObject.Position.X + finalScaleSplit.X, parentObject.Position.Y + finalScaleSplit.Y)));

            // Vertex: v3 -> v4
            m_fixtures.Add(m_physicsBody.CreateEdge(
                new tainicom.Aether.Physics2D.Common.Vector2(parentObject.Position.X + finalScaleSplit.X, parentObject.Position.Y + finalScaleSplit.Y), 
                new tainicom.Aether.Physics2D.Common.Vector2(parentObject.Position.X - finalScaleSplit.X, parentObject.Position.Y + finalScaleSplit.Y)));

            // Vertex: v4 -> v1
            m_fixtures.Add(m_physicsBody.CreateEdge(
                new tainicom.Aether.Physics2D.Common.Vector2(parentObject.Position.X - finalScaleSplit.X, parentObject.Position.Y + finalScaleSplit.Y), 
                new tainicom.Aether.Physics2D.Common.Vector2(parentObject.Position.X - finalScaleSplit.X, parentObject.Position.Y - finalScaleSplit.Y)));
        }
    }
}

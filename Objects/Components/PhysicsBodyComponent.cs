/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;
using TackEngineLib.Physics;

namespace TackEngineLib.Objects.Components
{
    public class PhysicsBodyComponent : TackComponent {

        public enum ShapeType {
            None,
            Rectangle,
            Circle
        }

        private Vector2f m_bodySizeMultiplier; // Size multiplier of the collider compared to the object's scale. 1.0f = collider size is the same as parentObject.Scale
        private Vector2f m_bodyOffset;
        private float m_mass;
        private bool m_isStatic;
        private bool m_simulatesGravity;

        /// <summary>
        /// Should the physics body be affected by gravity
        /// </summary>
        public bool SimulatesGravity {
            get { return m_simulatesGravity; }
            set { m_simulatesGravity = value; }
        }

        /// <summary>
        /// Is the physics body static. A static physics body cannot move and will not be affected by other objects/gravity
        /// </summary>
        public bool IsStatic {
            get { return m_isStatic; }
            set { m_isStatic = value; }
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
            get { return m_mass; }
            set { m_mass = value; }
        }

        public PhysicsBodyComponent() {
        }

        public override void OnStart() {
            base.OnStart();
        }

        public override void OnUpdate() {
            base.OnUpdate();
        }

        public override void OnAttachedToTackObject() {
            TackPhysics.GetInstance().RegisterPhysicsComponent(this);
        }

        public override void OnDetachedFromTackObject() {
            TackPhysics.GetInstance().DeregisterPhysicsComponent(this);
        }

        public void AddForce(float forceX, float forceY, TackPhysics.ForceType forceType) {
            AddForce(new Vector2f(forceX, forceY), forceType);
        }

        public void AddForce(Vector2f force, TackPhysics.ForceType forceType) {
        }
    }
}

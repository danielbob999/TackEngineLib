using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;
using TackEngineLib.Physics;

namespace TackEngineLib.Objects.Components {
    public abstract class BasePhysicsComponent : TackComponent {

        private float m_mass;
        private float m_invMass;
        private Vector2f m_velocity;
        private float m_gravityModifier;

        private bool m_affectedByGravity;
        private bool m_isStatic;

        /// <summary>
        /// Gets/Sets the mass of the physics body. If setting the mass, the value must be a number larger than 0 and smaller than Infinity
        /// </summary>
        public float Mass {
            get { return m_mass; }
            set {
                if (value <= 0) {
                    throw new Exception("The mass of a physics body must be larger than 0");
                }

                if (value == float.PositiveInfinity || value == float.NegativeInfinity) {
                    throw new Exception("The mass of a physics body cannot be equal to Inifity");
                }

                m_mass = value;
                m_invMass = 1.0f / value;
            }
        }

        /// <summary>
        /// Gets/Sets the velocity of the physics body
        /// </summary>
        public Vector2f Velocity {
            get { return m_velocity; }
            set { m_velocity = value; }
        }

        /// <summary>
        /// Gets/Sets whether the physics body is static. A static physics body cannot be moved with a force or collisions
        /// </summary>
        public bool IsStatic {
            get { return m_isStatic; }
            set { m_isStatic = value; }
        }

        /// <summary>
        /// Gets/Sets whether the physics body is affected by gravity
        /// </summary>
        public bool IsAffectedByGravity {
            get { return m_affectedByGravity; }
            set { m_affectedByGravity = value; }
        }

        /// <summary>
        /// Gets/Sets the gravity modifier of a physics body. This setting only affects how the body is affected by gravity. The modifier may not be set to 0
        /// </summary>
        public float GravityModifier {
            get { return m_gravityModifier; }
            set { 
                if (value == 0) {
                    throw new Exception("The gravity modifier for a physics body cannot be 0.");
                }

                m_gravityModifier = value; 
            }
        }

        public BasePhysicsComponent() {

        }

        public override void OnStart() {
            base.OnStart();
        }

        public override void OnUpdate() {
            base.OnUpdate();
        }

        public override void OnGUIRender() {
            base.OnGUIRender();
        }

        public override void OnClose() {
            base.OnClose();
        }

        public override void OnAttachedToTackObject() {
            base.OnAttachedToTackObject();

            TackPhysics.GetInstance().RegisterPhysicsComponent(this);
        }

        public override void OnDetachedFromTackObject() {
            base.OnDetachedFromTackObject();

            TackPhysics.GetInstance().DeregisterPhysicsComponent(this);
        }

        public virtual void AddForce(Vector2f force, TackPhysics.ForceType forceType) {
            if (forceType == TackPhysics.ForceType.Set) {
                m_velocity = force * m_invMass;
                return;
            }

            Vector2f finalForce = force * m_invMass;
            m_velocity += finalForce;
        }
    }
}

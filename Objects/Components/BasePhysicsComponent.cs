using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;
using TackEngineLib.Physics;

namespace TackEngineLib.Objects.Components {
    public abstract class BasePhysicsComponent : TackComponent {

        private Type m_finalType;

        private float m_mass;
        private float m_invMass;
        private float m_drag;
        private Vector2f m_velocity;
        private Vector2f m_currentGravityForce;
        private Vector2f m_currentActingForce;
        private float m_gravityModifier;
        private float m_restitution;
        private Vector2f m_previousPosition;

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
            set { 
                m_affectedByGravity = value;
                m_currentGravityForce = new Vector2f(0, 0);
            }
        }

        /// <summary>
        /// Gets/Sets the gravity modifier of the physics body. This setting only affects how the body is affected by gravity. The modifier may not be set to 0
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


        /// <summary>
        /// Gets/Sets the drag of the physics body
        /// </summary>
        public float Drag {
            get { return m_drag; }
            set { m_drag = value; }
        }

        /// <summary>
        /// Gets/Sets the restitution (or bounciness) of the physics body
        /// </summary>
        public float Restitution {
            get { return m_restitution; }
            set { m_restitution = value; }
        }

        internal float InvMass {
            get { return m_invMass; }
        }

        internal Vector2f CurrentActingForce {
            get { return m_currentActingForce; }
            set { m_currentActingForce = value; }
        }

        internal Vector2f CurrentGravityForce {
            get { return m_currentGravityForce; }
            set { m_currentGravityForce = value; }
        }

        internal Type FinalType { 
            get { return m_finalType; }
        }

        public abstract AABB BoundingBox { get; }

        protected BasePhysicsComponent(Type finalType) {
            m_finalType = finalType;
            m_currentActingForce = new Vector2f(0, 0);
            m_currentGravityForce = new Vector2f(0, 0);
        }

        public override void OnStart() {
            base.OnStart();
        }

        public override void OnUpdate() {
            base.OnUpdate();

            m_velocity = m_previousPosition - GetParent().Position;
            m_previousPosition = new Vector2f(GetParent().Position);
        }

        public override void OnGUIRender() {
            base.OnGUIRender();
        }

        public override void OnClose() {
            base.OnClose();
        }

        public override void OnAttachedToTackObject() {
            base.OnAttachedToTackObject();

            m_previousPosition = new Vector2f(GetParent().Position);
            TackPhysics.GetInstance().RegisterPhysicsComponent(this);
        }

        public override void OnDetachedFromTackObject() {
            base.OnDetachedFromTackObject();

            TackPhysics.GetInstance().DeregisterPhysicsComponent(this);
        }

        public virtual void AddForce(Vector2f force, TackPhysics.ForceType forceType) {
            if (forceType == TackPhysics.ForceType.Set) {
                m_currentActingForce = force;
                return;
            }

            Vector2f finalForce = force * m_invMass;
            m_currentActingForce += finalForce;
        }

        internal virtual void AddGravityForce() {
            m_currentGravityForce += TackPhysics.GetInstance().Gravity * m_gravityModifier * (float)Engine.EngineTimer.LastCycleTime;
        }
    }
}

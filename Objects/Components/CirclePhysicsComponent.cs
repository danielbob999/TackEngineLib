using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngineLib.Main;
using TackEngineLib.Engine;
using TackEngineLib.Physics;

namespace TackEngineLib.Objects.Components {
    public class CirclePhysicsComponent : BasePhysicsComponent {

        private float m_radiusMultiplier;

        /// <summary>
        /// Gets/Sets the radius of the circle body
        /// </summary>
        public float RadiusMultiplier {
            get { return m_radiusMultiplier; }
            set { m_radiusMultiplier = value; }
        }

        public override AABB BoundingBox {
            get {
                AABB aabb = new AABB();
                TackObject obj = GetParent();

                aabb.BottomLeft = new Vector2f(obj.Position.X - (obj.Scale.X * m_radiusMultiplier), obj.Position.Y - (obj.Scale.Y * m_radiusMultiplier)); ;
                aabb.TopRight = new Vector2f(obj.Position.X + (obj.Scale.X * m_radiusMultiplier), obj.Position.Y + (obj.Scale.Y * m_radiusMultiplier));

                return aabb;
            }
        }

        public CirclePhysicsComponent() : base(typeof(CirclePhysicsComponent)) {
            m_radiusMultiplier = 1.0f;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Physics;
using TackEngineLib.Main;

namespace TackEngineLib.Objects.Components {
    public class RectanglePhysicsComponent : BasePhysicsComponent {

        private AABB m_aabb;

        private Vector2f m_aabbScale;
        private Vector2f m_aabbOffset;

        public Vector2f BoundingBoxScale {
            get { return m_aabbScale; }
            set { m_aabbScale = value; }
        }

        public Vector2f BoundingBoxOffset {
            get { return m_aabbOffset; }
            set { m_aabbOffset = value; }
        }

        public override AABB BoundingBox {
            get {
                AABB aabb = new AABB();
                TackObject obj = GetParent();

                Vector2f bottomLeftScaled = new Vector2f(obj.Position.X - ((obj.Scale.X / 2.0f) * m_aabbScale.X), obj.Position.Y + ((obj.Scale.Y / 2.0f) * m_aabbScale.Y));
                Vector2f topRightScaled = new Vector2f(obj.Position.X + ((obj.Scale.X / 2.0f) * m_aabbScale.X), obj.Position.Y - ((obj.Scale.Y / 2.0f) * m_aabbScale.Y));

                aabb.BottomLeft = new Vector2f(bottomLeftScaled.X + m_aabbOffset.X, bottomLeftScaled.Y + m_aabbOffset.Y);
                aabb.TopRight = new Vector2f(topRightScaled.X + m_aabbOffset.X, topRightScaled.Y + m_aabbOffset.Y);

                return aabb; 
            }
        }

        public RectanglePhysicsComponent() : base(typeof(RectanglePhysicsComponent)) {
            m_aabbScale = new Vector2f(1, 1);
            m_aabbOffset = new Vector2f(0, 0);
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
    }
}

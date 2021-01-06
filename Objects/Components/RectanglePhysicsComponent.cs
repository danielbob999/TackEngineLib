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

        public RectanglePhysicsComponent() : base() {

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

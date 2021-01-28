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

        public override AABB BoundingBox {
            get {
                AABB aabb = new AABB();
                TackObject obj = GetParent();

                aabb.BottomLeft = new Vector2f(obj.Position.X - ((obj.Scale.X / 2.0f)), obj.Position.Y - ((obj.Scale.Y / 2.0f))); ;
                aabb.TopRight = new Vector2f(obj.Position.X + ((obj.Scale.X / 2.0f)), obj.Position.Y + ((obj.Scale.Y / 2.0f)));

                return aabb;
            }
        }

        public CirclePhysicsComponent() : base(typeof(CirclePhysicsComponent)) {
        }

    }
}

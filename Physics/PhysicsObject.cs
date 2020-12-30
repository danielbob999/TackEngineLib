using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngineLib.Physics
{
    public struct PhysicsObject
    {
        public string mTackObjectHash;
        public TackEngineLib.Objects.Components.PhysicsBodyComponent mPhysicsComponent;
        public TackEngineLib.Main.Vector2f mLeftOverGravityForce;
        public TackEngineLib.Main.Vector2f mLeftOverUserForce;

        public bool IsEqual(PhysicsObject obj) {
            if (mTackObjectHash == obj.mTackObjectHash) {
                if (mPhysicsComponent == obj.mPhysicsComponent) {
                    return true;
                }
            }

            return false;
        }
    }
}

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
        public TackEngineLib.Objects.Components.PhysicsComponent mPhysicsComponent;
        public TackEngineLib.Main.Vector2f mLeftOverForce;

        public bool IsEqual(PhysicsObject obj) {
            if (mTackObjectHash == obj.mTackObjectHash) {
                if (mPhysicsComponent == obj.mPhysicsComponent) {
                    if (mLeftOverForce == obj.mLeftOverForce) {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

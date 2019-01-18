using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;
using TackEngineLib.Physics;

namespace TackEngineLib.Objects.Components
{
    public class PhysicsComponent : TackComponent
    {
        private bool m_CollisionsEnabled = true; // Does the object use a collider
        private bool m_AllowedToMove = true; // Is the object allowed to be moved at runtime
        private Vector2f m_ColliderSizeMultiplier; // Size multiplier of the collider compared to the object's scale. 1.0f = collider size is the same as parentObject.Scale
        private Vector2f m_ColliderOffset;
        private bool m_SimulateGravity;

        // Properties
        public bool CollisionsEnabled
        {
            get { return m_CollisionsEnabled; }
            set { m_CollisionsEnabled = value; }
        }

        public bool AllowedToMove
        {
            get { return m_AllowedToMove; }
            set { m_AllowedToMove = value; }
        }

        public Vector2f ColliderSizeMultiplier
        {
            get { return m_ColliderSizeMultiplier; }
            set { m_ColliderSizeMultiplier = value; }
        }

        public Vector2f ColliderOffset
        {
            get { return m_ColliderOffset; }
            set { m_ColliderOffset = value; }
        }

        public bool SimulateGravity
        {
            get { return m_SimulateGravity; }
            set { m_SimulateGravity = value; }
        }

        public PhysicsComponent()
        {
            m_SimulateGravity = true;
            m_CollisionsEnabled = true;
            m_ColliderSizeMultiplier = new Vector2f(1, 1);
            m_ColliderOffset = new Vector2f(0, 0);
            m_AllowedToMove = true;
        }

        public void Move(float _x, float _y)
        {
            Move(new Vector2f(_x, _y));
        }

        public void Move(Vector2f _vec)
        {
            if (m_AllowedToMove)
            {
                //parentObject.Move(_vec);
                parentObject.Move(TackPhysics.CheckObjectMovementAmount(parentObject, _vec));
            }
        }
    }
}

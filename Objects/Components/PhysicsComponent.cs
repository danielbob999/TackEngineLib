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
    public class PhysicsComponent : TackComponent
    {
        private bool mCollisionsEnabled = true; // Does the object use a collider
        private bool mAllowedToMove = true; // Is the object allowed to be moved at runtime
        private Vector2f mColliderSizeMultiplier; // Size multiplier of the collider compared to the object's scale. 1.0f = collider size is the same as parentObject.Scale
        private Vector2f mColliderOffset;
        private bool mSimulateGravity;
        private float mWeight;
        private bool mModelInertia;

        // Properties
        public bool CollisionsEnabled
        {
            get { return mCollisionsEnabled; }
            set { mCollisionsEnabled = value; }
        }

        public bool AllowedToMove
        {
            get { return mAllowedToMove; }
            set { mAllowedToMove = value; }
        }

        public Vector2f ColliderSizeMultiplier
        {
            get { return mColliderSizeMultiplier; }
            set { mColliderSizeMultiplier = value; }
        }

        public Vector2f ColliderOffset
        {
            get { return mColliderOffset; }
            set { mColliderOffset = value; }
        }

        public bool SimulateGravity
        {
            get { return mSimulateGravity; }
            set { mSimulateGravity = value; }
        }

        public float Weight
        {
            get { return mWeight; }
            set
            {
                if (value > 0)
                    mWeight = value;
                else
                    TackConsole.EngineLog(Engine.EngineLogType.Error, string.Format("Cannot set PhysicsComponent.Weight to a value less than 0. Default weight (1) has been set."));
            }
        }

        public bool ModelInertia
        {
            get { return mModelInertia; }
            set { mModelInertia = value; }
        }

        public PhysicsComponent()
        {
            mSimulateGravity = true;
            mCollisionsEnabled = true;
            mColliderSizeMultiplier = new Vector2f(1, 1);
            mColliderOffset = new Vector2f(0, 0);
            mAllowedToMove = true;
            mWeight = 1;
            mModelInertia = false;
        }

        public override void OnAddedToTackObject() {
            TackPhysics.RegisterPhysicsComponent(this);
        }

        public void AddForce(float forceX, float forceY, TackPhysics.ForceType forceType) {
            AddForce(new Vector2f(forceX, forceY), forceType);
        }

        public void AddForce(Vector2f force, TackPhysics.ForceType forceType) {
            TackPhysics.AddForceToComponent(this, force, forceType);
        }

        public void Move(float _x, float _y)
        {
            Move(new Vector2f(_x, _y));
        }

        public void Move(Vector2f _vec)
        {
            if (mAllowedToMove)
            {
                //parentObject.Move(_vec);
                //parentObject.Move(TackPhysics.CheckObjectMovementAmount(parentObject, _vec));
                Console.WriteLine("Should be setting force");
                //TackPhysics.AddForceToComponent(this, _vec, TackPhysics.ForceType.Set);
            }
        }

        public void Destroy() {
            TackPhysics.DeregisterPhysicsComponent(this);
        }
    }
}

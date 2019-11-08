/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using TackEngineLib.Main;
using TackEngineLib.Objects;
using TackEngineLib.Engine;
using TackEngineLib.Objects.Components;

namespace TackEngineLib.Physics
{
    public class TackPhysics : EngineModule
    {
        public enum ForceType {
            Additive,       // Adds to the left-over force attached the the PhysicsComponent
            Set             // Resets the attached force to the new value
        }

        private static TackPhysics ActiveInstance;

        private Vector2f m_gravityForce;
        private List<PhysicsObject> m_currentPhysicsObjects = new List<PhysicsObject>();

        public Vector2f Gravity
        {
            get { return m_gravityForce; }
            set { m_gravityForce = value; }
        }

        internal TackPhysics() {
            if (ActiveInstance != null) {
                ActiveInstance.Close();
            }

            ActiveInstance = this;
            m_gravityForce = new Vector2f(0, -9.8f);
        }

        /// <summary>
        /// Starts this TackPhysics instance
        /// </summary>
        internal override void Start() {

        }

        internal override void Update() {
            base.Update();

            // Add all gravity forces before calculating collisions
            AddGravityForceToComponents();

            // Move all components using the left-over forces
            MoveObjectsBasedOnForces();

            // Calculate the interia force decrease
            CalculateInertiaForceDecrease();
        }

        internal override void Render() {

        }

        /// <summary>
        /// Closes this TackPhysics instance
        /// </summary>
        internal override void Close() {
        }

        private void AddGravityForceToComponents() {
            for (int i = 0; i < m_currentPhysicsObjects.Count; i++) {
                if (m_currentPhysicsObjects[i].mPhysicsComponent.SimulateGravity) {
                    PhysicsObject obj = m_currentPhysicsObjects[i];
                    obj.mLeftOverGravityForce = m_gravityForce;
                    OverwritePhysicsObject(obj);
                }
            }
        }

        private void MoveObjectsBasedOnForces() {
            for (int i = 0; i < m_currentPhysicsObjects.Count; i++) {
                //Console.WriteLine("TackObject with hash: " + m_currentPhysicsObjects[i].mTackObjectHash + ", " + m_currentPhysicsObjects[i].mLeftOverForce.ToString());
                if (m_currentPhysicsObjects[i].mPhysicsComponent.AllowedToMove) {
                    TackObject.GetUsingHash(m_currentPhysicsObjects[i].mTackObjectHash).Position += (m_currentPhysicsObjects[i].mLeftOverGravityForce + m_currentPhysicsObjects[i].mLeftOverUserForce);
                }
            }
        }

        private void CalculateInertiaForceDecrease() {
            for (int i = 0; i < m_currentPhysicsObjects.Count; i++) {
                if (m_currentPhysicsObjects[i].mPhysicsComponent.ModelInertia) {
                    float totalDecrease = m_currentPhysicsObjects[i].mPhysicsComponent.Weight / 100.0f;
                    PhysicsObject obj = m_currentPhysicsObjects[i];
                    
                    // Decrease X values and clamp
                    if (obj.mLeftOverUserForce.X < 0) {
                        obj.mLeftOverUserForce.X += totalDecrease;
                        if (obj.mLeftOverUserForce.X > 0) {
                            obj.mLeftOverUserForce.X = 0;
                        }
                    } else {
                        //Console.WriteLine("Obj.X = " + obj.mLeftOverUserForce.X);
                        obj.mLeftOverUserForce.X -= totalDecrease;
                        //Console.WriteLine("After decrease: " + obj.mLeftOverUserForce.X);
                        if (obj.mLeftOverUserForce.X < 0) {
                            obj.mLeftOverUserForce.X = 0;
                        }
                    }

                    // Decrease y values and clamp
                    if (obj.mLeftOverUserForce.Y < 0) {
                        obj.mLeftOverUserForce.Y += totalDecrease;
                        if (obj.mLeftOverUserForce.Y > 0) {
                            obj.mLeftOverUserForce.Y = 0;
                        }
                    } else {
                        obj.mLeftOverUserForce.Y -= totalDecrease;
                        if (obj.mLeftOverUserForce.Y < 0) {
                            obj.mLeftOverUserForce.Y = 0;
                        }
                    }

                    OverwritePhysicsObject(obj);
                } else {
                    PhysicsObject obj = m_currentPhysicsObjects[i];
                    obj.mLeftOverUserForce = new Vector2f();
                    OverwritePhysicsObject(obj);
                }
            }
        }

        /// <summary>
        /// Return a PhysicsObject based on it's TackObject's hash value
        /// </summary>
        /// <param name="hash">The hash value of the TackObject</param>
        /// <returns></returns>
        internal PhysicsObject? GetPhysicsObjectByTackObjectHash(string hash) {
            for (int i = 0; i < m_currentPhysicsObjects.Count; i++) {
                if (m_currentPhysicsObjects[i].mTackObjectHash == hash) {
                    return m_currentPhysicsObjects[i];
                }
            }

            return null;
        }

        internal PhysicsObject? GetPhysicsObjectByPhysComp(PhysicsComponent comp) {
            for (int i = 0; i < m_currentPhysicsObjects.Count; i++) {
                if (m_currentPhysicsObjects[i].mPhysicsComponent == comp) {
                    return m_currentPhysicsObjects[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Removed PhysicsObject that is based on the specified PhysicsComponent
        /// </summary>
        /// <param name="comp"></param>
        internal void RemovePhysicsObject(PhysicsComponent comp) {
            for (int i = 0; i < m_currentPhysicsObjects.Count; i++) {
                if (m_currentPhysicsObjects[i].mPhysicsComponent == comp) {
                    m_currentPhysicsObjects.RemoveAt(i);
                }
            }
        }

        internal void OverwritePhysicsObject(PhysicsObject obj) {
            for (int i = 0; i < m_currentPhysicsObjects.Count; i++) {
                if (m_currentPhysicsObjects[i].IsEqual(obj)) {
                    m_currentPhysicsObjects[i] = obj;
                }
            }
        }

        internal Vector2f AddForce(Vector2f startingForce, Vector2f forceToAdd, float clampValue) {
            startingForce.X += forceToAdd.X;
            startingForce.Y += forceToAdd.Y;

            if (startingForce.X > clampValue) {
                startingForce.X = 0;
            }

            if (startingForce.Y > clampValue) {
                startingForce.Y = 0;
            }

            return startingForce;
        }

        internal Vector2f SubtractForce(Vector2f startingForce, Vector2f forceToAdd, float clampValue) {
            startingForce.X -= forceToAdd.X;
            startingForce.Y -= forceToAdd.Y;

            if (startingForce.X < clampValue) {
                startingForce.X = 0;
            }

            if (startingForce.Y < clampValue) {
                startingForce.Y = 0;
            }

            return startingForce;
        }

        internal Vector2f CheckObjectMovementAmount(TackObject _tackObject, Vector2f _movementAmount)
        {
            MovementDirection dirX;
            MovementDirection dirY;

            Vector2f finalMovementAmount = _movementAmount;

            if (_movementAmount.X > 0)
                dirX = MovementDirection.Right;
            else if (_movementAmount.X < 0)
                dirX = MovementDirection.Left;
            else
                dirX = MovementDirection.NULL;

            if (_movementAmount.Y > 0)
                dirY = MovementDirection.Up;
            else if (_movementAmount.Y < 0)
                dirY = MovementDirection.Down;
            else
                dirY = MovementDirection.NULL;

            PhysicsComponent physComp = _tackObject.GetComponent<PhysicsComponent>();

            RectangleShape shape = new RectangleShape(
                _tackObject.Position.X - ((_tackObject.Scale.X / 2) * physComp.ColliderSizeMultiplier.X),
                _tackObject.Position.Y + ((_tackObject.Scale.Y / 2) * physComp.ColliderSizeMultiplier.Y),
                _tackObject.Scale.X * physComp.ColliderSizeMultiplier.X,
                _tackObject.Scale.Y * physComp.ColliderSizeMultiplier.Y);


            if (dirY != MovementDirection.NULL)
            {
                #region DownwardsMovement
                // If Object needs to move down
                if (dirY == MovementDirection.Down)
                {
                    float newBottomPos = (shape.Y - shape.Height) + _movementAmount.Y;

                    TackObject[] tackObjects = TackObject.Get();

                    foreach (TackObject obj in tackObjects)
                    {
                        if (obj == _tackObject)
                            continue;

                        if (obj.GetComponent<PhysicsComponent>().IsNullComponent())
                            continue;

                        RectangleShape objShape = new RectangleShape(
                            obj.Position.X - ((obj.Scale.X / 2)),
                            obj.Position.Y + ((obj.Scale.Y / 2)),
                            obj.Scale.X,
                            obj.Scale.Y);

                        // Check to see if _tackObjects top line is exactly at the position of objs bottom line
                        if (shape.Y == (objShape.Y - objShape.Height))
                            continue;

                        // Check to see if _tackObject is below obj
                        if (shape.Y < (objShape.Y - objShape.Height))
                        {
                            continue;
                        }

                        if (objShape.Y > newBottomPos)
                        {
                            //finalMovementAmount.Y = _movementAmount.Y - (newBottomPos - objShape.Y);
                            //break;

                            if (shape.X < (objShape.X + objShape.Width) && (shape.X + shape.Width) > objShape.X)
                            {
                                if (obj.GetComponent<PhysicsComponent>().AllowedToMove)
                                {
                                    finalMovementAmount = _movementAmount;
                                    obj.GetComponent<PhysicsComponent>().Move(new Vector2f(0, (newBottomPos - objShape.Y)));
                                }
                                else
                                {
                                    finalMovementAmount.Y = _movementAmount.Y - (newBottomPos - objShape.Y);
                                }

                                break;
                            }
                        }
                    }
                }
                #endregion

                #region UpwardsMovement

                // if Object needs to move up
                if (dirY == MovementDirection.Up)
                {
                    float newTopPos = (shape.Y) + _movementAmount.Y;

                    TackObject[] tackObjects = TackObject.Get();

                    foreach (TackObject obj in tackObjects)
                    {
                        // Continue if obj is the same TackObject has the one being evaluated
                        if (obj == _tackObject)
                            continue;

                        // Continue if the TackObject (obj) has no PhysicsComponent
                        if (obj.GetComponent<PhysicsComponent>().IsNullComponent())
                            continue;

                        RectangleShape stationaryObjectShape = new RectangleShape(
                            obj.Position.X - ((obj.Scale.X / 2)),
                            obj.Position.Y + ((obj.Scale.Y / 2)),
                            obj.Scale.X,
                            obj.Scale.Y);


                        // Check to see if _tackObject's bottom is exactly the position of obj's top line
                        if ((shape.Y - shape.Height) == stationaryObjectShape.Y)
                            continue;

                        // Check to see if _tackObject is above obj
                        if ((shape.Y - shape.Height) > (stationaryObjectShape.Y))
                        {
                            continue;
                        }

                        if ((stationaryObjectShape.Y - stationaryObjectShape.Height) < newTopPos) // changed from > to <
                        {
                            if (shape.X < (stationaryObjectShape.X + stationaryObjectShape.Width) && (shape.X + shape.Width) > stationaryObjectShape.X)
                            {
                                if (obj.GetComponent<PhysicsComponent>().AllowedToMove)
                                {
                                    finalMovementAmount = _movementAmount;
                                    obj.GetComponent<PhysicsComponent>().Move(new Vector2f(0, (newTopPos - (stationaryObjectShape.Y - stationaryObjectShape.Height))));
                                }
                                else
                                {
                                    finalMovementAmount.Y = _movementAmount.Y - (newTopPos - (stationaryObjectShape.Y - stationaryObjectShape.Height));
                                }

                                break;
                            }
                        }
                    }
                }
                #endregion
            }

            if (dirX != MovementDirection.NULL)
            {
                #region LeftMovement

                /*
                // If object wants to move left
                if (dirX == MovementDirection.Left)
                {
                    // Calc new left pos
                    float newLeftPosition = shape.X + _movementAmount.X;

                    TackObject[] tackObjects = TackObject.Get();

                    foreach (TackObject stationaryObject in tackObjects)
                    {
                        if (stationaryObject == _tackObject)
                            continue;

                        if (stationaryObject.GetComponent<PhysicsComponent>().IsNullComponent())
                            continue;

                        RectangleShape stationaryObjectShape = new RectangleShape(
                            stationaryObject.Position.X - ((stationaryObject.Scale.X / 2)),
                            stationaryObject.Position.Y + ((stationaryObject.Scale.Y / 2)),
                            stationaryObject.Scale.X,
                            stationaryObject.Scale.Y);

                        // Check if _tackObject is to the left of stationaryObjects right most point
                        if ((shape.X + shape.Width) <= (stationaryObjectShape.X + stationaryObjectShape.Width))
                        {
                            continue;
                        }

                        bool isCollidingXAxis = false;

                        if (
                            ((stationaryObjectShape.Y < shape.Y) && (stationaryObjectShape.Y > (shape.Y - shape.Height))) ||
                            ((stationaryObjectShape.Y - stationaryObjectShape.Height) > (shape.Y - shape.Height)) && (stationaryObjectShape.Y - stationaryObjectShape.Height) < shape.Y)
                        {
                            isCollidingXAxis = true;
                        }

                        if ((shape.Y <= stationaryObjectShape.Y) && ((shape.Y - shape.Height) >= (stationaryObjectShape.Y - stationaryObjectShape.Height)))
                        {
                            isCollidingXAxis = true;
                        }

                        if (isCollidingXAxis)
                        {
                            if (newLeftPosition < (stationaryObjectShape.X + stationaryObjectShape.Width))
                            {
                                if (stationaryObject.GetComponent<PhysicsComponent>().AllowedToMove)
                                {
                                    finalMovementAmount = _movementAmount;
                                    stationaryObject.GetComponent<PhysicsComponent>().Move(new Vector2f((newLeftPosition - (stationaryObjectShape.X + stationaryObjectShape.Width)), 0));
                                }
                                else
                                {
                                    finalMovementAmount.X = _movementAmount.X - (newLeftPosition - (stationaryObjectShape.X + stationaryObjectShape.Width));
                                }

                                break;
                            }
                        }

                        //Console.WriteLine("isCollidingXAxis. Shape: " + shape.ToString() + " , stationaryShape: " + stationaryObjectShape.ToString());
                    }
                }*/

                if (dirX == MovementDirection.Left)
                {
                    // Find the left most vertex position
                    Dictionary<int, Vector2f> movingObjVerts = new Dictionary<int, Vector2f>();
                    movingObjVerts.Add(1, _tackObject.GetComponent<QuadRenderer>().FindVertexPoint(1));
                    movingObjVerts.Add(2, _tackObject.GetComponent<QuadRenderer>().FindVertexPoint(2));
                    movingObjVerts.Add(3, _tackObject.GetComponent<QuadRenderer>().FindVertexPoint(3));
                    movingObjVerts.Add(4, _tackObject.GetComponent<QuadRenderer>().FindVertexPoint(4));

                    TackObject[] tackObjects = TackObject.Get();

                    foreach (TackObject stationaryObject in tackObjects)
                    {
                        if (stationaryObject == _tackObject)
                            continue;

                        if (stationaryObject.GetComponent<PhysicsComponent>().IsNullComponent())
                            continue;

                        Dictionary<int, Vector2f> stationaryObjVerts = new Dictionary<int, Vector2f>();
                        stationaryObjVerts.Add(1, stationaryObject.GetComponent<QuadRenderer>().FindVertexPoint(1));
                        stationaryObjVerts.Add(2, stationaryObject.GetComponent<QuadRenderer>().FindVertexPoint(2));
                        stationaryObjVerts.Add(3, stationaryObject.GetComponent<QuadRenderer>().FindVertexPoint(3));
                        stationaryObjVerts.Add(4, stationaryObject.GetComponent<QuadRenderer>().FindVertexPoint(4));

                        PhysicsMovement.MovementLeft(_movementAmount, movingObjVerts, stationaryObjVerts);

                        /*
                        // Check if _tackObject is to the left of stationaryObjects right most point
                        if ((shape.X + shape.Width) <= (stationaryObjectShape.X + stationaryObjectShape.Width))
                        {
                            continue;
                        }*/

                        //Console.WriteLine("isCollidingXAxis. Shape: " + shape.ToString() + " , stationaryShape: " + stationaryObjectShape.ToString());
                    }

                }

                #endregion

                #region RightMovement

                // If object wants to move left
                if (dirX == MovementDirection.Right)
                {
                    // Calc new right pos
                    float newRightPosition = (shape.X + shape.Width) + _movementAmount.X;

                    TackObject[] tackObjects = TackObject.Get();

                    foreach (TackObject stationaryObject in tackObjects)
                    {
                        if (stationaryObject == _tackObject)
                            continue;

                        if (stationaryObject.GetComponent<PhysicsComponent>().IsNullComponent())
                            continue;

                        RectangleShape stationaryObjectShape = new RectangleShape(
                            stationaryObject.Position.X - ((stationaryObject.Scale.X / 2)),
                            stationaryObject.Position.Y + ((stationaryObject.Scale.Y / 2)),
                            stationaryObject.Scale.X,
                            stationaryObject.Scale.Y);

                        // Check if _tackObject is to the right of stationaryObjects right most point
                        if ((shape.X) >= (stationaryObjectShape.X + stationaryObjectShape.Width))
                        {
                            continue;
                        }

                        bool isCollidingXAxis = false;

                        if (
                            ((stationaryObjectShape.Y < shape.Y) && (stationaryObjectShape.Y > (shape.Y - shape.Height))) ||
                            ((stationaryObjectShape.Y - stationaryObjectShape.Height) > (shape.Y - shape.Height)) && (stationaryObjectShape.Y - stationaryObjectShape.Height) < shape.Y)
                        {
                            isCollidingXAxis = true;
                        }

                        if ((shape.Y <= stationaryObjectShape.Y) && ((shape.Y - shape.Height) >= (stationaryObjectShape.Y - stationaryObjectShape.Height)))
                        {
                            isCollidingXAxis = true;
                        }

                        if (isCollidingXAxis)
                        {
                            if (newRightPosition > (stationaryObjectShape.X))
                            {
                                if (stationaryObject.GetComponent<PhysicsComponent>().AllowedToMove)
                                {
                                    finalMovementAmount = _movementAmount;
                                    stationaryObject.GetComponent<PhysicsComponent>().Move(new Vector2f((newRightPosition - (stationaryObjectShape.X)), 0));
                                }
                                else
                                {
                                    finalMovementAmount.X = _movementAmount.X - (newRightPosition - (stationaryObjectShape.X));
                                }

                                break;
                            }
                        }

                        //Console.WriteLine("isCollidingXAxis. Shape: " + shape.ToString() + " , stationaryShape: " + stationaryObjectShape.ToString());
                    }
                }

                #endregion
            }

            return finalMovementAmount;
        }

        internal static void RegisterPhysicsComponent(PhysicsComponent component) {
            if (ActiveInstance.GetPhysicsObjectByTackObjectHash(component.parentObject.GetHash()) == null) {
                PhysicsObject obj = new PhysicsObject {
                    mLeftOverGravityForce = new Vector2f(),
                    mLeftOverUserForce = new Vector2f(),
                    mPhysicsComponent = component,
                    mTackObjectHash = component.parentObject.GetHash()
                };
                ActiveInstance.m_currentPhysicsObjects.Add(obj);
                return;
            }
        }

        internal static void AddForceToComponent(PhysicsComponent component, Vector2f force, ForceType forceType) {
            PhysicsObject? physObjNullable = ActiveInstance.GetPhysicsObjectByTackObjectHash(component.parentObject.GetHash());
            if (physObjNullable != null) {
                PhysicsObject obj = physObjNullable.Value;
                if (forceType == ForceType.Additive) {
                    obj.mLeftOverUserForce += force;
                } else {
                    obj.mLeftOverUserForce = force;
                }

                // Because of pass-by-value, we need to overwrite the PhysicsObject that
                ActiveInstance.OverwritePhysicsObject(obj);
            }
        }

        internal static void DeregisterPhysicsComponent(PhysicsComponent component) {
            ActiveInstance.RemovePhysicsObject(component);
        }

        public static void Shutdown() {
            ActiveInstance.Close();
        }
    }
}

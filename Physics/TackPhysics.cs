/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using TackEngineLib.Engine;
using TackEngineLib.Main;
using TackEngineLib.Objects;
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
                    Vector2f moveableAmount = (m_currentPhysicsObjects[i].mLeftOverGravityForce + m_currentPhysicsObjects[i].mLeftOverUserForce);

                    /*
                    Vector2f finalMovementAmount = new Vector2f();

                    // If you are wanting to move left
                    if (moveableAmount.X < 0) {
                        //finalMovementAmount.X += CheckMovementAmountLeft(m_currentPhysicsObjects[i].mPhysicsComponent.parentObject, new List<TackObject>() { m_currentPhysicsObjects[i].mPhysicsComponent.parentObject }, moveableAmount.X);
                    } else {
                        Console.WriteLine(moveableAmount.X);
                        finalMovementAmount.X += CheckMovementAmountRight(m_currentPhysicsObjects[i].mPhysicsComponent.parentObject, new List<TackObject>() { m_currentPhysicsObjects[i].mPhysicsComponent.parentObject }, moveableAmount.X);
                    }

                    // If you want to move down
                    if (moveableAmount.Y < 0) {

                    } else {

                    }

                    Console.WriteLine("MovebleAmount: " + moveableAmount.ToString());*/

                    TackObject.GetUsingHash(m_currentPhysicsObjects[i].mTackObjectHash).Position += CheckObjectMovementAmount(m_currentPhysicsObjects[i].mPhysicsComponent.parentObject, new List<TackObject>() { m_currentPhysicsObjects[i].mPhysicsComponent.parentObject }, moveableAmount);
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

        internal static Vector2f CheckObjectMovementAmount(TackObject _tackObject, List<TackObject> _skippableObjects, Vector2f _movementAmount) {
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


            if (dirY != MovementDirection.NULL) {
                #region DownwardsMovement
                // If Object needs to move down
                if (dirY == MovementDirection.Down) {
                    float newBottomPos = (shape.Y - shape.Height) + _movementAmount.Y;

                    TackObject[] tackObjects = TackObject.Get();

                    foreach (TackObject obj in tackObjects) {
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
                        if (shape.Y < (objShape.Y - objShape.Height)) {
                            continue;
                        }

                        if (objShape.Y > newBottomPos) {
                            //finalMovementAmount.Y = _movementAmount.Y - (newBottomPos - objShape.Y);
                            //break;

                            if (shape.X < (objShape.X + objShape.Width) && (shape.X + shape.Width) > objShape.X) {
                                if (obj.GetComponent<PhysicsComponent>().AllowedToMove) {
                                    finalMovementAmount = _movementAmount;
                                    //obj.GetComponent<PhysicsComponent>().Move(new Vector2f(0, (newBottomPos - objShape.Y)));
                                    obj.GetComponent<PhysicsComponent>().AddForce(new Vector2f(0, (newBottomPos - objShape.Y)), ForceType.Set);
                                } else {
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
                if (dirY == MovementDirection.Up) {
                    float newTopPos = (shape.Y) + _movementAmount.Y;

                    TackObject[] tackObjects = TackObject.Get();

                    foreach (TackObject obj in tackObjects) {
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
                        if ((shape.Y - shape.Height) > (stationaryObjectShape.Y)) {
                            continue;
                        }

                        if ((stationaryObjectShape.Y - stationaryObjectShape.Height) < newTopPos) // changed from > to <
                        {
                            if (shape.X < (stationaryObjectShape.X + stationaryObjectShape.Width) && (shape.X + shape.Width) > stationaryObjectShape.X) {
                                if (obj.GetComponent<PhysicsComponent>().AllowedToMove) {
                                    finalMovementAmount = _movementAmount;
                                    //obj.GetComponent<PhysicsComponent>().Move(new Vector2f(0, (newTopPos - (stationaryObjectShape.Y - stationaryObjectShape.Height))));
                                    obj.GetComponent<PhysicsComponent>().AddForce(new Vector2f(0, (newTopPos - (stationaryObjectShape.Y - stationaryObjectShape.Height))), ForceType.Set);
                                } else {
                                    finalMovementAmount.Y = _movementAmount.Y - (newTopPos - (stationaryObjectShape.Y - stationaryObjectShape.Height));
                                }

                                break;
                            }
                        }
                    }
                }
                #endregion
            }

            if (dirX != MovementDirection.NULL) {
                #region LeftMovement

                
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
                                if (stationaryObject.GetComponent<PhysicsComponent>().AllowedToMove) {
                                    finalMovementAmount = _movementAmount;
                                    //stationaryObject.GetComponent<PhysicsComponent>().Move(new Vector2f((newLeftPosition - (stationaryObjectShape.X + stationaryObjectShape.Width)), 0));
                                    stationaryObject.GetComponent<PhysicsComponent>().AddForce(new Vector2f((newLeftPosition - (stationaryObjectShape.X + stationaryObjectShape.Width)), 0), ForceType.Set);
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
                }

                #endregion

                #region RightMovement

                // If object wants to move left
                if (dirX == MovementDirection.Right) {
                    // Calc new right pos
                    float newRightPosition = (shape.X + shape.Width) + _movementAmount.X;

                    TackObject[] tackObjects = TackObject.Get();

                    foreach (TackObject stationaryObject in tackObjects) {
                        if (_skippableObjects.Contains(stationaryObject))
                            continue;

                        if (stationaryObject.GetComponent<PhysicsComponent>().IsNullComponent())
                            continue;

                        RectangleShape stationaryObjectShape = new RectangleShape(
                            stationaryObject.Position.X - ((stationaryObject.Scale.X / 2)),
                            stationaryObject.Position.Y + ((stationaryObject.Scale.Y / 2)),
                            stationaryObject.Scale.X,
                            stationaryObject.Scale.Y);

                        // Check if _tackObject is to the right of stationaryObjects right most point
                        if ((shape.X) >= (stationaryObjectShape.X + stationaryObjectShape.Width)) {
                            continue;
                        }

                        bool isCollidingXAxis = false;

                        if (
                            ((stationaryObjectShape.Y < shape.Y) && (stationaryObjectShape.Y > (shape.Y - shape.Height))) ||
                            ((stationaryObjectShape.Y - stationaryObjectShape.Height) > (shape.Y - shape.Height)) && (stationaryObjectShape.Y - stationaryObjectShape.Height) < shape.Y) {
                            isCollidingXAxis = true;
                        }

                        if ((shape.Y <= stationaryObjectShape.Y) && ((shape.Y - shape.Height) >= (stationaryObjectShape.Y - stationaryObjectShape.Height))) {
                            isCollidingXAxis = true;
                        }

                        if (isCollidingXAxis) {
                            if (newRightPosition > (stationaryObjectShape.X)) {
                                if (stationaryObject.GetComponent<PhysicsComponent>().AllowedToMove) {
                                    finalMovementAmount = _movementAmount;
                                    //stationaryObject.GetComponent<PhysicsComponent>().Move(new Vector2f((newRightPosition - (stationaryObjectShape.X)), 0));
                                    stationaryObject.GetComponent<PhysicsComponent>().AddForce(new Vector2f((newRightPosition - (stationaryObjectShape.X)), 0), ForceType.Set);
                                } else {
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

        internal static float CheckMovementAmountRight(TackObject tackObject, List<TackObject> objectsToSkip, float movementAmount) {
            PhysicsComponent thisObjectsPhysComponent = tackObject.GetComponent<PhysicsComponent>();

            // The final amount of movement to the right
            float finalMovementAmount = movementAmount;

            // Calculate the shape of the current
            RectangleShape shape = new RectangleShape(
                tackObject.Position.X - ((tackObject.Scale.X / 2) * thisObjectsPhysComponent.ColliderSizeMultiplier.X),
                tackObject.Position.Y + ((tackObject.Scale.Y / 2) * thisObjectsPhysComponent.ColliderSizeMultiplier.Y),
                tackObject.Scale.X * thisObjectsPhysComponent.ColliderSizeMultiplier.X,
                tackObject.Scale.Y * thisObjectsPhysComponent.ColliderSizeMultiplier.Y);

            // The new position of the rightmost side of the TackObject
            float newRightPosition = (shape.X + shape.Width) + movementAmount;

            TackObject[] tackObjects = TackObject.Get();

            // Loop through all the TackObjects
            foreach (TackObject secondaryObject in tackObjects) {
                if (objectsToSkip.Contains(secondaryObject)) {
                    continue;
                }

                if (secondaryObject.GetComponent<PhysicsComponent>().IsNullComponent()) {
                    continue;
                }

                if (secondaryObject.GetComponent<PhysicsComponent>().AllowedToMove) {
                    return 0;
                }

                // Get the shape of the secondary object
                RectangleShape secondaryObjectShape = new RectangleShape(
                    secondaryObject.Position.X - ((secondaryObject.Scale.X / 2)),
                    secondaryObject.Position.Y + ((secondaryObject.Scale.Y / 2)),
                    secondaryObject.Scale.X,
                    secondaryObject.Scale.Y);

                // Check if the tackObject is to the right of secondaryObject's right most point
                if ((shape.X) >= (secondaryObjectShape.X + secondaryObjectShape.Width)) {
                    continue;
                }

                bool isColliding = false;

                if (
                    ((secondaryObjectShape.Y < shape.Y) && (secondaryObjectShape.Y > (shape.Y - shape.Height))) ||
                    ((secondaryObjectShape.Y - secondaryObjectShape.Height) > (shape.Y - shape.Height)) && (secondaryObjectShape.Y - secondaryObjectShape.Height) < shape.Y) {
                    isColliding = true;
                }

                if ((shape.Y <= secondaryObjectShape.Y) && ((shape.Y - shape.Height) >= (secondaryObjectShape.Y - secondaryObjectShape.Height))) {
                    isColliding = true;
                }

                
                if (isColliding) {
                    if (newRightPosition > (secondaryObjectShape.X)) {
                        if (secondaryObject.GetComponent<PhysicsComponent>().AllowedToMove) {
                            //finalMovementAmount = movementAmount;
                            List<TackObject> newList = new List<TackObject>(objectsToSkip);
                            newList.Add(secondaryObject);
                            float secondaryObjMoveAmount = CheckMovementAmountRight(secondaryObject, newList, finalMovementAmount);
                            finalMovementAmount = secondaryObjMoveAmount;
                        } else {
                            finalMovementAmount = movementAmount - (newRightPosition - (secondaryObjectShape.X));
                        }
                    }
                }
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

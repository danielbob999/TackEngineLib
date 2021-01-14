﻿/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using TackEngineLib.Engine;
using TackEngineLib.Main;
using TackEngineLib.Objects;
using TackEngineLib.Objects.Components;
using OpenTK.Graphics.OpenGL;

namespace TackEngineLib.Physics {
    public class TackPhysics : EngineModule {
        public enum ForceType {
            Additive,       // Adds to the left-over force attached the the PhysicsComponent
            Set             // Resets the attached force to the new value
        }

        private static TackPhysics s_instance = null;

        private Vector2f m_gravityForce;
        private List<BasePhysicsComponent> m_physicBodyComponents;

        public Vector2f Gravity {
            get { return m_gravityForce; }
            set { m_gravityForce = value; }
        }

        internal TackPhysics() {
            m_gravityForce = new Vector2f(0, -1.2f);
            m_physicBodyComponents = new List<BasePhysicsComponent>();

            s_instance = this;
        }

        /// <summary>
        /// Starts this TackPhysics instance
        /// </summary>
        internal override void Start() {
        }

        internal override void Update() {
            base.Update();

            // Add forces to the physics bodies
            ApplyForces();

            // Run a broadphase function (approximates collisions) that returns a list of physics bodies
            List<PhysicsCollision> broadphaseResults = RunBroadphase( m_physicBodyComponents);

            // Run a detailed collision algorithm to detect collisions.
            //      If a collision is detected, generate a [SOMETHING], store it, and call the event function.
            List<PhysicsCollision> collisionResults = DetectCollisions(broadphaseResults);

            // Resolve collisions
            ResolveCollisions(collisionResults);
        }

        internal override void Render() {
            base.Render();

            DrawPhysicsObjects();
        }

        /// <summary>
        /// Closes this TackPhysics instance
        /// </summary>
        internal override void Close() {
        }

        public static TackPhysics GetInstance() {
            return s_instance;
        }

        internal void RegisterPhysicsComponent(BasePhysicsComponent component) {
            if (!m_physicBodyComponents.Contains(component)) {
                m_physicBodyComponents.Add(component);
            }
        }

        internal void DeregisterPhysicsComponent(BasePhysicsComponent component) {
            m_physicBodyComponents.Remove(component);
        }

        internal void DrawPhysicsObjects() {
            // Vertex layout
            //  V4----V3
            //  |     |
            //  |     |
            //  V1----V2

            for (int i = 0; i < m_physicBodyComponents.Count; i++) {
                Type physCompType = m_physicBodyComponents[i].FinalType;

                if (physCompType == typeof(RectanglePhysicsComponent)) {
                    RectanglePhysicsComponent physComp = (RectanglePhysicsComponent)m_physicBodyComponents[i];
                    AABB aabb = physComp.BoundingBox;
                    AABB aabbScreenSpace = new AABB(Renderer.TackRenderer.FindScreenCoordsFromPosition(aabb.BottomLeft), Renderer.TackRenderer.FindScreenCoordsFromPosition(aabb.TopRight));

                    GL.Begin(PrimitiveType.Lines);

                    // V1->V2
                    GL.Vertex2(aabbScreenSpace.Left, aabbScreenSpace.Bottom);
                    GL.Vertex2(aabbScreenSpace.Right, aabbScreenSpace.Bottom);

                    // V2->V3
                    GL.Vertex2(aabbScreenSpace.Right, aabbScreenSpace.Bottom);
                    GL.Vertex2(aabbScreenSpace.Right, aabbScreenSpace.Top);

                    // V3->V4
                    GL.Vertex2(aabbScreenSpace.Right, aabbScreenSpace.Top);
                    GL.Vertex2(aabbScreenSpace.Left, aabbScreenSpace.Top);

                    // V4->V1
                    GL.Vertex2(aabbScreenSpace.Left, aabbScreenSpace.Top);
                    GL.Vertex2(aabbScreenSpace.Left, aabbScreenSpace.Bottom);

                    // V1->V3 (diagonal line)
                    GL.Vertex2(aabbScreenSpace.Left, aabbScreenSpace.Bottom);
                    GL.Vertex2(aabbScreenSpace.Right, aabbScreenSpace.Top);

                    GL.End();
                } else {
                    // Unsupported phyics component
                }
            }
        }

        internal void ApplyForces() {
            for (int i = 0; i < m_physicBodyComponents.Count; i++) {
                // Add gravity
                if (m_physicBodyComponents[i].IsAffectedByGravity) {
                    m_physicBodyComponents[i].AddGravityForce(1 * (float)EngineTimer.LastCycleTime);
                }

                // Add Forces
                if (!m_physicBodyComponents[i].IsStatic) {
                    // Gravity force
                    Vector2f calcedGravityForce = new Vector2f();
                    calcedGravityForce.X = ((m_physicBodyComponents[i].CurrentGravityForce.X * m_physicBodyComponents[i].GravityModifier) * m_gravityForce.X);
                    calcedGravityForce.Y = ((m_physicBodyComponents[i].CurrentGravityForce.Y * m_physicBodyComponents[i].GravityModifier) * m_gravityForce.Y);

                    // Move body
                    Vector2f moveAmnt = m_physicBodyComponents[i].CurrentActingForce + calcedGravityForce;

                    TackObject obj = m_physicBodyComponents[i].GetParent();
                    obj.Position += moveAmnt;

                    // Calculate a slowdown force based on mass
                    float decreaseMod = ((m_physicBodyComponents[i].Mass / 120.0f) * 0.33f) + ((m_physicBodyComponents[i].Drag / 40.0f) * 0.66f);

                    Vector2f newActingForceVec = m_physicBodyComponents[i].CurrentActingForce;

                    // Add the new forces and clamp to 0.
                    if (m_physicBodyComponents[i].CurrentActingForce.X < 0) {
                        newActingForceVec.X = Math.TackMath.Clamp(newActingForceVec.X + decreaseMod, float.NegativeInfinity, 0);
                    } else {
                        newActingForceVec.X = Math.TackMath.Clamp(newActingForceVec.X - decreaseMod, 0, float.PositiveInfinity);
                    }

                    if (m_physicBodyComponents[i].CurrentActingForce.Y < 0) {
                        newActingForceVec.Y = Math.TackMath.Clamp(newActingForceVec.Y + decreaseMod, float.NegativeInfinity, 0);
                    } else {
                        newActingForceVec.Y = Math.TackMath.Clamp(newActingForceVec.Y - decreaseMod, 0, float.PositiveInfinity);
                    }

                    m_physicBodyComponents[i].CurrentActingForce = newActingForceVec * (float)(EngineTimer.LastCycleTime);
                }
            }
        }

        internal List<PhysicsCollision> RunBroadphase(List<BasePhysicsComponent> components) {
            return null;
        }

        internal List<PhysicsCollision> DetectCollisions(List<PhysicsCollision> broadphaseComps) {
            return null;
        }

        internal void ResolveCollisions(List<PhysicsCollision> collisions) {

        }

        public void Shutdown() {
            s_instance.Close();
        }
    }
}

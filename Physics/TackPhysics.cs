/* Copyright (c) 2019 Daniel Phillip Robinson */
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
        private bool m_runBroadphaseAlgorithm;

        public Vector2f Gravity {
            get { return m_gravityForce; }
            set { m_gravityForce = value; }
        }

        /// <summary>
        /// Gets/Sets whether to run the broadphase algorithm to determine if bodies are potentially colliding
        /// </summary>
        public bool RunBroadphaseAlgorithm {
            get { return m_runBroadphaseAlgorithm; }
            set { m_runBroadphaseAlgorithm = value; }
        }

        internal TackPhysics() {
            m_gravityForce = new Vector2f(0, -1.2f);
            m_physicBodyComponents = new List<BasePhysicsComponent>();
            m_runBroadphaseAlgorithm = true;

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
            List<PhysicsCollision> broadphaseResults = RunBroadphase(m_physicBodyComponents);

            // Run a detailed collision algorithm to detect collisions.
            //      If a collision is detected, generate a [SOMETHING], store it, and call the event function.
            List<PhysicsCollision> collisionResults = DetectCollisions(null);

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
            if (!m_runBroadphaseAlgorithm) {
                // If the user doesn't want to run the broadphase, return null. The DetectCollisions method will know what to do.
                return null;
            }

            List<PhysicsCollision> collisions = new List<PhysicsCollision>();

            // Temporary code that simulates a broadphase algorithm. In this case, 
            for (int i = 0; i < m_physicBodyComponents.Count; i++) {
                BasePhysicsComponent comp = m_physicBodyComponents[i];

                for (int j = 0; j < m_physicBodyComponents.Count; j++) {
                    if (m_physicBodyComponents[j].Equals(comp)) {
                        continue;
                    }

                    // Run broadphase calculations here
                    collisions.Add(new PhysicsCollision(comp, m_physicBodyComponents[j], default, 0.0f));
                }
            }

            return collisions;
        }

        internal List<PhysicsCollision> DetectCollisions(List<PhysicsCollision> broadphaseCollisions) {
            List<PhysicsCollision> detailedCollisions = new List<PhysicsCollision>();

            if (broadphaseCollisions == null) {
                // If the collision list is null, the broadphase has not been run. Loop through ALL active components to check for collisions

                List<Tuple<BasePhysicsComponent, BasePhysicsComponent>> checkedCollisions = new List<Tuple<BasePhysicsComponent, BasePhysicsComponent>>();

                for (int i = 0; i < m_physicBodyComponents.Count; i++) {
                    BasePhysicsComponent comp1 = m_physicBodyComponents[i];

                    for (int j = 0; j < m_physicBodyComponents.Count; j++) {
                        if (m_physicBodyComponents[j].Equals(comp1)) {
                            continue; // We shouldn't be checking if something is colliding with itself
                        }

                        if (checkedCollisions.Contains(new Tuple<BasePhysicsComponent, BasePhysicsComponent>(m_physicBodyComponents[j], comp1))) {
                            continue; // We shouldn't be checking the collision of two physics bodies twice
                        }

                        BasePhysicsComponent comp2 = m_physicBodyComponents[j];

                        // Handle if both physic bodies are rectangle types
                        if (comp1.FinalType == typeof(RectanglePhysicsComponent) && comp2.FinalType == typeof(RectanglePhysicsComponent)) {
                            AABB comp1AABB = comp1.BoundingBox;
                            AABB comp2AABB = comp2.BoundingBox;
                            TackObject obj1 = comp1.GetParent();
                            TackObject obj2 = comp2.GetParent();

                            bool isCollidingXAxis = false;
                            bool isCollidingYAxis = false;
                            Vector2f penetration = new Vector2f(0, 0);
                            Vector2f normal = new Vector2f();

                            if (comp1AABB.Origin.X < comp2AABB.Origin.X) {
                                if (comp1AABB.Right > comp2AABB.Left) {
                                    isCollidingXAxis = true;
                                    penetration.X = Math.TackMath.AbsValf(comp1AABB.Right - comp2AABB.Left);
                                    normal.X = -1;
                                }
                            } else {
                                if (comp1AABB.Left < comp2AABB.Right) {
                                    isCollidingXAxis = true;
                                    penetration.X = Math.TackMath.AbsValf(comp2AABB.Right - comp1AABB.Left);
                                    normal.X = 1;
                                }
                            }

                            
                            if (comp1AABB.Origin.Y > comp2AABB.Origin.Y) {
                                if (comp1AABB.Bottom < comp2AABB.Top) {
                                    isCollidingYAxis = true;
                                    penetration.Y = Math.TackMath.AbsValf(comp1AABB.Bottom - comp2AABB.Top);
                                    normal.Y = 1;
                                }
                            } else {
                                if (comp1AABB.Top > comp2AABB.Bottom) {
                                    isCollidingYAxis = true;
                                    penetration.Y = Math.TackMath.AbsValf(comp1AABB.Top - comp2AABB.Bottom);
                                    normal.Y = -1;
                                }
                            }

                            if (isCollidingXAxis && isCollidingYAxis) {
                                Vector2f distances = new Vector2f(Math.TackMath.AbsValf(comp1AABB.Origin.X - comp2AABB.Origin.X), Math.TackMath.AbsValf(comp1AABB.Origin.Y - comp2AABB.Origin.Y));

                                if (distances.X >= distances.Y) {
                                    normal.Y = 0;
                                    detailedCollisions.Add(new PhysicsCollision(comp1, comp2, normal, penetration.X));
                                } else {
                                    normal.X = 0;
                                    detailedCollisions.Add(new PhysicsCollision(comp1, comp2, normal, penetration.Y));
                                }
                            }

                            checkedCollisions.Add(new Tuple<BasePhysicsComponent, BasePhysicsComponent>(comp1, comp2));
                        }
                    }
                }
            }

            return detailedCollisions;
        }

        internal void ResolveCollisions(List<PhysicsCollision> collisions) {
            //Console.WriteLine(collisions.Count + " collisions");
           for (int i = 0; i < collisions.Count; i++) {
                //TackConsole.EngineLog(EngineLogType.Message, "Resolving collision between {0} and {1}, with penetration {2}", collisions[i].Body1.GetParent().Name, collisions[i].Body2.GetParent().Name, collisions[i].Penetration);
                BasePhysicsComponent comp1 = collisions[i].Body1;
                BasePhysicsComponent comp2 = collisions[i].Body2;

                float massRatio = (comp1.Mass + comp2.Mass) / comp1.Mass;

                if (comp1.IsStatic) {
                    //TackConsole.EngineLog(EngineLogType.Message, "comp2 moveamnt: " + (collisions[i].Normal * collisions[i].Penetration).ToString());
                    comp2.GetParent().Position += (collisions[i].Normal * (collisions[i].Penetration * 1.000001f));
                    comp2.Velocity = new Vector2f(0, 0);
                    comp2.CurrentActingForce = new Vector2f(0, 0);
                } else if (comp2.IsStatic) {
                    //TackConsole.EngineLog(EngineLogType.Message, "comp1 moveamnt: " + (collisions[i].Normal * collisions[i].Penetration).ToString());
                    comp1.GetParent().Position += (collisions[i].Normal * (collisions[i].Penetration * 1.000001f));
                    comp1.Velocity = new Vector2f(0, 0);
                    comp1.CurrentActingForce = new Vector2f(0, 0);
                } else {

                }
            }
        }

        public void Shutdown() {
            s_instance.Close();
        }
    }
}

/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using TackEngineLib.Engine;
using TackEngineLib.Main;
using TackEngineLib.Objects;
using TackEngineLib.Objects.Components;
using OpenTK.Graphics.OpenGL;
using tainicom.Aether.Physics2D.Dynamics;
using TackEngineLib.Renderer;
using tainicom.Aether.Physics2D.Collision.Shapes;

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
        private World m_physicsWorld;
        private SolverIterations m_iterations;
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
            m_physicsWorld = new World(new tainicom.Aether.Physics2D.Common.Vector2(0, -9.8f));
        }

        internal override void Update() {
            base.Update();

            m_physicsWorld.Step(1 / 60.0f, ref m_iterations);
        }

        internal override void Render() {

        }

        /// <summary>
        /// Closes this TackPhysics instance
        /// </summary>
        internal override void Close() {
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

        internal PhysicsObject? GetPhysicsObjectByPhysComp(PhysicsBodyComponent comp) {
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
        internal void RemovePhysicsObject(PhysicsBodyComponent comp) {
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


        internal static void RegisterPhysicsComponent(PhysicsBodyComponent component) {
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

        internal static void AddForceToComponent(PhysicsBodyComponent component, Vector2f force, ForceType forceType) {
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

        internal static void DeregisterPhysicsComponent(PhysicsBodyComponent component) {
            ActiveInstance.RemovePhysicsObject(component);
        }

        internal static void DebugDrawPhysicsWorld() {

            GL.LineWidth(2.0f);

            GL.Begin(BeginMode.Lines);
            GL.Color4(0.0f, 1.0f, 0.0f, 1.0f);

            TackObject[] objects = TackObjectManager.GetAllTackObjects();

            for (int i = 0; i < objects.Length; i++) {
                // Draw each physics object

                PhysicsBodyComponent physComp = objects[i].GetComponent<PhysicsBodyComponent>();

                if (physComp == null || physComp.IsNullComponent()) {
                    continue;
                }

                Body phsyBody = physComp.PhysicsBody;
                Fixture[] fixtures = phsyBody.FixtureList.ToArray();

                Console.WriteLine(fixtures.Length);

                for (int j = 0; j < fixtures.Length; j++) {
                    tainicom.Aether.Physics2D.Collision.Shapes.EdgeShape shape = (tainicom.Aether.Physics2D.Collision.Shapes.EdgeShape)fixtures[j].Shape;

                    Vector2f vert0ScreenSpace = TackRenderer.FindScreenCoordsFromPosition(new Vector2f(shape.Vertex1.X, shape.Vertex1.Y));
                    Vector2f vert1ScreenSpace = TackRenderer.FindScreenCoordsFromPosition(new Vector2f(shape.Vertex2.X, shape.Vertex2.Y));

                    GL.Vertex2(vert0ScreenSpace.X, vert0ScreenSpace.Y);
                    GL.Vertex2(vert1ScreenSpace.X, vert1ScreenSpace.Y);
                }

                if (fixtures.Length == 4) {
                    EdgeShape shape1 = (EdgeShape)fixtures[0].Shape;
                    EdgeShape shape2 = (EdgeShape)fixtures[2].Shape;

                    Vector2f vert0ScreenSpace = TackRenderer.FindScreenCoordsFromPosition(new Vector2f(shape1.Vertex1.X, shape1.Vertex1.Y));
                    Vector2f vert1ScreenSpace = TackRenderer.FindScreenCoordsFromPosition(new Vector2f(shape2.Vertex2.X, shape2.Vertex2.Y));

                    GL.Vertex2(vert0ScreenSpace.X, vert0ScreenSpace.Y);
                    GL.Vertex2(vert1ScreenSpace.X, vert1ScreenSpace.Y);
                }
            }

            GL.End();
        }

        internal static World GetPhysicsWorld() {
            return ActiveInstance.m_physicsWorld;
        }

        public static void Shutdown() {
            ActiveInstance.Close();
        }
    }
}

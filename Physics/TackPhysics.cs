/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using TackEngineLib.Engine;
using TackEngineLib.Main;
using TackEngineLib.Objects.Components;

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
            m_gravityForce = new Vector2f(0, -9.8f);
            m_physicBodyComponents = new List<BasePhysicsComponent>();
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
        }

        /// <summary>
        /// Closes this TackPhysics instance
        /// </summary>
        internal override void Close() {
        }

        public static TackPhysics GetInstance() {
            if (s_instance == null) {
                s_instance = new TackPhysics();
            }

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

        }

        internal void ApplyForces() {
            for (int i = 0; i < m_physicBodyComponents.Count; i++) {
                
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

/* Copyright (c) 2019 Daniel Phillip Robinson */
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

        public Vector2f Gravity {
            get { return m_gravityForce; }
            set { m_gravityForce = value; }
        }

        internal TackPhysics() {
            m_gravityForce = new Vector2f(0, -9.8f);
        }

        /// <summary>
        /// Starts this TackPhysics instance
        /// </summary>
        internal override void Start() {
        }

        internal override void Update() {
            base.Update();

            // k
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
        }

        internal void DeregisterPhysicsComponent(BasePhysicsComponent component) {
        }

        internal void DrawPhysicsObjects() {

        }

        public void Shutdown() {
            s_instance.Close();
        }
    }
}

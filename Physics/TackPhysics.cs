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
    public static class TackPhysics
    {
        private static Vector2f gravityForce;
        private static PhysicsStatus status;
        private static bool shouldLoop = false;
        private static Stopwatch updateRateTimer;
        private static int targetUpdateRate;
        private static int updateCounter = 0;
        private static int updateRate = 0;
        private static int timeTakenLastUpdate;

        private static float m_CycleTimeDelta;

        public static float CycleTimeDelta
        {
            get { return m_CycleTimeDelta; }
            set { m_CycleTimeDelta = value; }
        }

        /// <summary>
        /// Method that initializes the physics loop
        /// </summary>
        /// <param name="_targetUpdateRatePerSec">The target update rate (updates per second)</param>
        internal static void Init(object _targetUpdateRatePerSec)
        {
            gravityForce = new Vector2f(0, -9.8f);

            status = PhysicsStatus.Starting;
            updateRateTimer = new Stopwatch();

            shouldLoop = true;
            int targetUR = (int)((double)_targetUpdateRatePerSec);
            targetUpdateRate = targetUR <= 0 ? 60 : targetUR;
            StartPhysicsLoop(targetUR <= 0 ? 60 : targetUR);
        }

        private static void StartPhysicsLoop(int _ur)
        {
            TackConsole.EngineLog(EngineLogType.Message, string.Format("Starting TackPhysics on a new thread ({0}) with priority: {1}", Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.Priority));

            if (Thread.CurrentThread.ThreadState == System.Threading.ThreadState.Running)
            {
                TackConsole.EngineLog(EngineLogType.Message, "Successfully started TackPhysics");
            }

            Stopwatch timer = new Stopwatch(); // A stopwatch that is used to measure how long it took to complete a cycle/frame
            updateRateTimer.Start();

            // Get the target sleep time. 1000(ms) / targetUpdateRate
            // E.g 1000(ms) / 60 = 16.667ms sleep time per loop run
            float targetSleepTimeMs = 1000 / _ur;

            while (shouldLoop)
            {
                // Calculate actual update rate
                if (updateRateTimer.ElapsedMilliseconds >= 1000)
                {
                    updateRate = updateCounter;
                    updateCounter = 0;
                    updateRateTimer.Restart();
                }

                timer.Restart();
                status = PhysicsStatus.Running;

                // Do physics calculations here
                try
                {
                    OnUpdateGravityMovement();
                }
                catch (Exception e)
                {
                    TackConsole.EngineLog(EngineLogType.Error, "TackPhysics has encountered a serious problem and has crashed.");
                    TackConsole.EngineLog(EngineLogType.Error, string.Format("ErrorCode: {0}, ProblematicMethod: {1}, StackTrace: {2}", e.HResult, e.TargetSite, e.StackTrace));
                    Stop();
                    continue;
                }


                long timeTakenThisRun = timer.ElapsedMilliseconds;
                int timeToSleep = (int)((int)targetSleepTimeMs - timeTakenThisRun);

                if (timeToSleep <= 0)
                {
                    timeTakenLastUpdate = (int)timer.ElapsedMilliseconds;
                    updateCounter++;
                    continue;
                }
                else
                {
                    Thread.Sleep(timeToSleep);
                }

                updateCounter++;
                timeTakenLastUpdate = (int)timer.ElapsedMilliseconds;

                m_CycleTimeDelta = timeTakenLastUpdate / (1000 / targetUpdateRate); 
            }

        }

        internal static void Stop()
        {
            TackConsole.EngineLog(EngineLogType.Message, "Shutting TackPhysics down...");
            status = PhysicsStatus.Stopping;
            shouldLoop = false;
        }

        /// <summary>
        /// 
        /// </summary>
        internal static void OnUpdatePhysicsComponent()
        {
            TackObject[] tackObjects = TackObject.Get();

            foreach (TackObject obj in tackObjects)
            {
                if (!obj.GetComponent<PhysicsComponent>().IsNullComponent())
                {
                    PhysicsComponent physicsComponent = obj.GetComponent<PhysicsComponent>();

                    // Update components affected by gravity
                    if (physicsComponent.SimulateGravity)
                    {
                        Vector2f movementVector = new Vector2f(gravityForce.X * m_CycleTimeDelta, gravityForce.Y * m_CycleTimeDelta);
                        physicsComponent.parentObject.Move(movementVector);
                    }
                }
            }
        }

        /// <summary>
        /// To be run every Physics update cycle. Adds a gravity effect
        ///     to all PhysicsComponents where SimulateGravity=true.
        ///     
        /// </summary>
        internal static void OnUpdateGravityMovement()
        {
            TackObject[] tackObjects = TackObject.Get();

            foreach (TackObject obj in tackObjects)
            {
                if (!obj.GetComponent<PhysicsComponent>().IsNullComponent())
                {
                    PhysicsComponent physicsComponent = obj.GetComponent<PhysicsComponent>();

                    // Update components affected by gravity
                    if (physicsComponent.SimulateGravity)
                    {
                        Vector2f movementVector = new Vector2f(gravityForce.X * m_CycleTimeDelta, gravityForce.Y *m_CycleTimeDelta);
                        obj.Move(CheckObjectMovementAmount(obj, movementVector));
                    }
                }
            }
        }

        internal static Vector2f CheckObjectMovementAmount(TackObject _tackObject, Vector2f _movementAmount)
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
                                finalMovementAmount.Y = _movementAmount.Y - (newBottomPos - objShape.Y);
                                break;
                            }
                        }
                    }
                }
            }

            return finalMovementAmount;
        }

        /// <summary>
        /// Returns the current gravity force value used by the physics thread
        /// </summary>
        /// <returns></returns>
        public static Vector2f GetGravityForce()
        {
            return gravityForce;
        }

        /// <summary>
        /// Returns the current status of the physics thread
        /// </summary>
        /// <returns></returns>
        public static PhysicsStatus GetPhysicsStatus()
        {
            return status;
        }

        /// <summary>
        /// Returns the actual update rate of the physics thread
        /// </summary>
        /// <returns></returns>
        public static int GetUpdateRate()
        {
            return updateRate;
        }

        /// <summary>
        /// Set the gravity force value used by the physics thread
        /// </summary>
        /// <param name="_force">The gravity force value to be set</param>
        public static void SetGravityForce(Vector2f _force)
        {
            gravityForce = _force;
        }
    }
}

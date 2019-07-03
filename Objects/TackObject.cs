/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;
using TackEngineLib.Objects.Components;
using TackEngineLib.Engine;
using TackEngineLib.Math;

namespace TackEngineLib.Objects
{
    /// <summary>
    /// The main class used by the TackEngineLib to represent and object
    /// </summary>
    public class TackObject
    {
        // MEMBERS

        private static Random rnd = new Random();

        private string gameObjectHash;
        private string mName;
        private Vector2f mPosition;
        private Vector2f mScale;
        private float mRotation;
        private Vector2f mUp;
        private Vector2f mRight;

        internal List<object> objectComponents = new List<object>();
        internal List<string> usedHashCodes = new List<string>();

        /// <summary>
        /// The name of this TackObject
        /// </summary>
        /// <datatype>string</datatype>
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// The position of this TackObject
        /// </summary>
        /// <datatype>Vector2f</datatype>
        public Vector2f Position
        {
            get { return mPosition; }
            set { mPosition = value; }
        }

        /// <summary>
        /// The scale of this TackObject
        /// </summary>
        /// <datatype>Vector2f</datatype>
        public Vector2f Scale
        {
            get { return mScale; }
            set { mScale = value; }
        }

        /// <summary>
        /// The rotation value of this TackObject
        /// </summary>
        /// <datatype>float</datatype>
        public float Rotation
        {
            get { return mRotation; }
            set { mRotation = value; }
        }

        /// <summary>
        /// The vector of moving the TackObject directly forward based on rotation
        /// </summary>
        public Vector2f Up
        {
            get
            {
                return new Vector2f(1.0f * (float)System.Math.Sin(TackMath.DegToRad(mRotation)), 1.0f * (float)System.Math.Cos(TackMath.DegToRad(mRotation)));
            }
        }

        /// <summary>
        /// The vector of moving the TackObject directly right based on rotation
        /// </summary>
        public Vector2f Right
        {
            get
            {
                return new Vector2f(1.0f * (float)System.Math.Sin(TackMath.DegToRad(mRotation + 90)), 1.0f * (float)System.Math.Cos(TackMath.DegToRad(mRotation + 90)));
            }
        }

        // CONSTRUCTORS
        
        public TackObject()
        {
            gameObjectHash = CreateTackObjectHash();
            mName = "New GameObject";
            mPosition = new Vector2f();
            mScale = new Vector2f();
            mRotation = 0;

            TackObjectManager.AddTackObject(this);

            TackConsole.EngineLog(EngineLogType.Message, string.Format("Create new TackObject at position ({0}, {1}) with name '{2}' and hash '{3}'", mPosition.X, mPosition.Y, mName, gameObjectHash));
        }

        public TackObject(string _n)
        {
            gameObjectHash = CreateTackObjectHash();
            mName = _n;
            mPosition = new Vector2f();
            mScale = new Vector2f();
            mRotation = 0;

            TackObjectManager.AddTackObject(this);

            TackConsole.EngineLog(EngineLogType.Message, string.Format("Create new TackObject at position ({0}, {1}) with name '{2}' and hash '{3}'", mPosition.X, mPosition.Y, mName, gameObjectHash));
        }
        
        public TackObject(string _n, Vector2f _p)
        {
            gameObjectHash = CreateTackObjectHash();
            mName = _n;
            mPosition = _p;
            mScale = new Vector2f();
            mRotation = 0;

            TackObjectManager.AddTackObject(this);

            TackConsole.EngineLog(EngineLogType.Message, string.Format("Create new TackObject at position ({0}, {1}) with name '{2}' and hash '{3}'", mPosition.X, mPosition.Y, mName, gameObjectHash));
        }

        public void AddComponent(object _component)
        {
            if (!_component.GetType().IsSubclassOf(typeof(TackComponent)))
            {
                TackConsole.EngineLog(EngineLogType.Error, string.Format("'{0}' cannot be added to TackObject with name '{1}' because it does not inherit from '{2}'", _component.GetType(), mName, typeof(TackComponent)));
                return;
            }

            if (_component != null)
            {
                ((TackComponent)_component).parentObject = this;
                objectComponents.Add(_component);

                TackConsole.EngineLog(EngineLogType.Message, string.Format("Added a '{0}' component to TackObject with name '{1}'", _component.GetType(), mName));
                
                if (_component.GetType() == typeof(Camera) && TackEngine.mMainCameraTackObject == null)
                {
                    TackEngine.mMainCameraTackObject = this;
                }
            }
        }

        public T GetComponent<T>()
        {
            foreach (object comp in objectComponents)
            {
                if (comp.GetType() == typeof(T))
                    return (T)comp;
            }

            object newComp = (T)Activator.CreateInstance(typeof(T));
            ((TackComponent)newComp).IsNullComponent(true);

            //EngineLog.WriteError(EngineErrorMode.TackObject, "TackObject with name '{0}' and hash '{1}' does not have a component of type '{2}'", mName, gameObjectHash, typeof(T));
            return (T)newComp;
        }

        public bool IsPointInArea(Vector2f _point)
        {
            Vector2f xConstraints = new Vector2f(mPosition.X - (mScale.X / 2), mPosition.X + (mScale.X / 2));
            Vector2f yConstraints = new Vector2f(mPosition.Y - (mScale.Y / 2), mPosition.Y + (mScale.Y / 2));

            if ((_point.X > xConstraints.X) && (_point.X < xConstraints.Y))
            {

                if ((_point.Y > yConstraints.X) && (_point.Y < yConstraints.Y))
                {
                    return true;
                }
            }

            return false;
        }

        internal void Move(Vector2f _vec)
        {
            mPosition += _vec;
        }

        internal string CreateTackObjectHash()
        {
            string finalString = "";

            for (int i = 0; i < 16; i++)
            {
                char newChar = 'A';

                switch (rnd.Next(0, 3))
                {
                    case 0: // Generate a number between 0-9
                        newChar = (char)rnd.Next(48, 48 + 9);
                        break;
                    case 1: // Generate a letter between a-z
                        newChar = (char)rnd.Next(97, 97 + 26);
                        break;
                    case 2: // Generate a letter between A-Z
                        newChar = (char)rnd.Next(65, 65 + 26);
                        break;
                    default:
                        break;
                }

                finalString += newChar;
            }

            foreach (string hash in usedHashCodes)
            {
                if (hash == finalString)
                    return CreateTackObjectHash();
            }

            usedHashCodes.Add(finalString);

            return finalString;
        }

        public string GetHash()
        {
            return gameObjectHash;
        }

        public static TackObject Get(string _name)
        {
            TackObject[] allTackObjects = Get();

            foreach (TackObject obj in allTackObjects)
            {
                if (obj.Name == _name)
                    return obj;
            }

            //TackConsole.EngineLog(EngineLogType.ErrorMessage
            return null;
        }
        
        public static TackObject[] Get()
        {
            return TackObjectManager.GetAllTackObjects();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngineLib.Main;
using TackEngineLib.Objects.Components;
using TackEngineLib.Engine;

namespace TackEngineLib.Objects
{
    public class TackObject
    {
        // MEMBERS

        private static Random rnd = new Random();

        private string gameObjectHash;
        private string m_Name;
        private Vector2f m_Position;
        private Vector2f m_Scale;

        internal List<object> objectComponents = new List<object>();
        internal List<string> usedHashCodes = new List<string>();

        // PROPERTIES

        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public Vector2f Position
        {
            get { return m_Position; }
            set { m_Position = value; }
        }

        public Vector2f Scale
        {
            get { return m_Scale; }
            set { m_Scale = value; }
        }

        // CONSTRUCTORS
        
        public TackObject()
        {
            gameObjectHash = CreateTackObjectHash();
            m_Name = "New GameObject";
            m_Position = new Vector2f();
            m_Scale = new Vector2f();

            TackObjectManager.AddTackObject(this);

            TackConsole.EngineLog(EngineLogType.Message, string.Format("Create new TackObject at position ({0}, {1}) with name '{2}' and hash '{3}'", m_Position.X, m_Position.Y, m_Name, gameObjectHash));
        }

        public TackObject(string _n)
        {
            gameObjectHash = CreateTackObjectHash();
            m_Name = _n;
            m_Position = new Vector2f();
            m_Scale = new Vector2f();

            TackObjectManager.AddTackObject(this);

            TackConsole.EngineLog(EngineLogType.Message, string.Format("Create new TackObject at position ({0}, {1}) with name '{2}' and hash '{3}'", m_Position.X, m_Position.Y, m_Name, gameObjectHash));
        }
        
        public TackObject(string _n, Vector2f _p)
        {
            gameObjectHash = CreateTackObjectHash();
            m_Name = _n;
            m_Position = _p;
            m_Scale = new Vector2f();

            TackObjectManager.AddTackObject(this);

            TackConsole.EngineLog(EngineLogType.Message, string.Format("Create new TackObject at position ({0}, {1}) with name '{2}' and hash '{3}'", m_Position.X, m_Position.Y, m_Name, gameObjectHash));
        }

        public void AddComponent(object _component)
        {
            if (!_component.GetType().IsSubclassOf(typeof(TackComponent)))
            {
                TackConsole.EngineLog(EngineLogType.Error, string.Format("'{0}' cannot be added to TackObject with name '{1}' because it does not inherit from '{2}'", _component.GetType(), m_Name, typeof(TackComponent)));
                return;
            }

            if (_component != null)
            {
                ((TackComponent)_component).parentObject = this;
                objectComponents.Add(_component);

                TackConsole.EngineLog(EngineLogType.Message, string.Format("Added a '{0}' component to TackObject with name '{1}'", _component.GetType(), m_Name));
                
                if (_component.GetType() == typeof(Camera) && TackEngine.m_MainCameraTackObject == null)
                {
                    TackEngine.m_MainCameraTackObject = this;
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

            //EngineLog.WriteError(EngineErrorMode.TackObject, "TackObject with name '{0}' and hash '{1}' does not have a component of type '{2}'", m_Name, gameObjectHash, typeof(T));
            return (T)newComp;
        }

        public bool IsPointInArea(Vector2f _point)
        {
            Vector2f xConstraints = new Vector2f(m_Position.X - (m_Scale.X / 2), m_Position.X + (m_Scale.X / 2));
            Vector2f yConstraints = new Vector2f(m_Position.Y - (m_Scale.Y / 2), m_Position.Y + (m_Scale.Y / 2));

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
            m_Position += _vec;
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

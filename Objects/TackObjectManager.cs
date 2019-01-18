using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using TackEngineLib.Main;
using TackEngineLib.Engine;
using TackEngineLib.Renderer;
using TackEngineLib.Objects.Components;

namespace TackEngineLib.Objects
{
    internal class TackObjectManager
    {
        public static TackObjectManager ActiveInstance;

        private List<TackObject> m_TackObjects = new List<TackObject>();

        public TackObjectManager()
        {
            ActiveInstance = this;
        }

        public void OnStart()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            // TackConsole.EngineLog(EngineLogType.OnStart, "", timer.ElapsedMilliseconds);
            timer.Stop();
        }

        public void OnUpdate()
        {
        }

        public void RunTackObjectStartMethods()
        {
            foreach (TackObject tackObject in m_TackObjects)
            {
                foreach (object tackComponent in tackObject.objectComponents)
                {
                    TackComponent comp = (TackComponent)tackComponent;
                    comp.OnUpdate();
                }
            }
        }

        public void RunTackObjectUpdateMethods()
        {
            foreach (TackObject tackObject in m_TackObjects)
            {
                foreach (object tackComponent in tackObject.objectComponents)
                {
                    TackComponent comp = (TackComponent)tackComponent;
                    comp.OnUpdate();
                }
            }
        }

        internal static bool AddTackObject(TackObject _obj)
        {
            if (ActiveInstance.m_TackObjects.Contains(_obj))
            {
                TackConsole.EngineLog(EngineLogType.Error, "Could not add TackObject with name '{0}' and hash '{1}' because TackObjectManager already contains this TackObject");
                return false;
            }
            else
            {
                ActiveInstance.m_TackObjects.Add(_obj);
                return true;
            }
        }

        internal static void RemoveTackObject(TackObject _obj)
        {
            if (ActiveInstance.m_TackObjects.Contains(_obj))
            {
                ActiveInstance.m_TackObjects.Remove(_obj);
                TackConsole.EngineLog(EngineLogType.Message, string.Format("Removed TackObject with name '{0}' from TackObjectManager", _obj.Name));
            }
        }

        public static TackObject[] GetAllTackObjects()
        {
            return ActiveInstance.m_TackObjects.ToArray();
        }
    }
}

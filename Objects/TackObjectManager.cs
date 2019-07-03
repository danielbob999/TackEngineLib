/* Copyright (c) 2019 Daniel Phillip Robinson */
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

        private List<TackObject> mTackObjects = new List<TackObject>();

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
            foreach (TackObject tackObject in mTackObjects)
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
            foreach (TackObject tackObject in mTackObjects)
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
            if (ActiveInstance.mTackObjects.Contains(_obj))
            {
                TackConsole.EngineLog(EngineLogType.Error, "Could not add TackObject with name '{0}' and hash '{1}' because TackObjectManager already contains this TackObject");
                return false;
            }
            else
            {
                ActiveInstance.mTackObjects.Add(_obj);
                return true;
            }
        }

        internal static void RemoveTackObject(TackObject _obj)
        {
            if (ActiveInstance.mTackObjects.Contains(_obj))
            {
                ActiveInstance.mTackObjects.Remove(_obj);
                TackConsole.EngineLog(EngineLogType.Message, string.Format("Removed TackObject with name '{0}' from TackObjectManager", _obj.Name));
            }
        }

        public static TackObject[] GetAllTackObjects()
        {
            return ActiveInstance.mTackObjects.ToArray();
        }
    }
}

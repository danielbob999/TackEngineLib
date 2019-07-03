using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngineLib.Objects.Components
{
    public class TackComponent 
    {
        private bool mActive = true;
        private bool mIsNullComponent = false;
        public TackObject parentObject;

        // Properties
        public bool Active { get { return mActive;  } set { mActive = value; } }

        public virtual void OnStart()
        {
            IsNullComponent(false);
        }

        public virtual void OnUpdate()
        {

        }

        
        public virtual void OnRender()
        {

        }
        

        public virtual void OnGUIRender()
        {

        }

        public virtual bool IsNullComponent()
        {
            return mIsNullComponent;
        }

        public virtual void IsNullComponent(bool _b)
        {
            mIsNullComponent = _b;
        }
    }
}

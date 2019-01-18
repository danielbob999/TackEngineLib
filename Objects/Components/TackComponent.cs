using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngineLib.Objects.Components
{
    public class TackComponent 
    {
        private bool m_Active = true;
        private bool m_IsNullComponent = false;
        public TackObject parentObject;

        // Properties
        public bool Active { get { return m_Active;  } set { m_Active = value; } }

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
            return m_IsNullComponent;
        }

        public virtual void IsNullComponent(bool _b)
        {
            m_IsNullComponent = _b;
        }
    }
}

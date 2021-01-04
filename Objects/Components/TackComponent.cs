/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngineLib.Objects.Components
{
    public class TackComponent {
        private static int s_nextId = 0;

        private bool m_active = true;
        private string m_parentObjectHash;
        private int m_componentId;

        /// <summary>
        /// Is the component active on the TackObject
        /// </summary>
        public bool Active { 
            get { return m_active;  } 
            set { m_active = value; } 
        }


        internal TackComponent() {
            m_componentId = s_nextId;
            s_nextId++;
        }

        public virtual void OnStart() {

        }

        public virtual void OnUpdate() {

        }

        
        public virtual void OnRender() {

        }
        

        public virtual void OnGUIRender() {

        }

        public virtual void OnAttachedToTackObject() {

        }

        public virtual void OnDetachedFromTackObject() {

        }

        public TackObject GetParent() {
            return TackObject.GetByHash(m_parentObjectHash);
        }

        internal void SetParent(string hash) {
            m_parentObjectHash = hash;
        }

        public bool Equals(TackComponent comp) {
            if (comp.m_componentId == this.m_componentId) {
                return true;
            }

            return false;
        }
    }
}

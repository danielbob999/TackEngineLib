using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngineLib.GUI {
    public class GUIObject {
        private bool m_active;

        public bool Active {
            get { return m_active; }
            set { m_active = value; }
        }

        internal GUIObject() {
            Active = true;
        }

        internal virtual void OnStart() {

        }

        internal virtual void OnUpdate() {

        }

        internal virtual void OnRender() {

        }

        internal virtual void OnClose() {

        }
    }
}

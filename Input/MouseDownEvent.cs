using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngineLib.Main;

namespace TackEngineLib.Input
{
    public class MouseDownEvent
    {
        private Vector2f m_MousePosition;
        private int m_MouseButtonId = -1;

        // Properties
        public Vector2f MousePosition { get { return m_MousePosition; } set { m_MousePosition = value; } }
        public int MouseButtonId { get { return m_MouseButtonId; } set { m_MouseButtonId = value; } }
    }
}

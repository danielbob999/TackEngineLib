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
        private Vector2f mMousePosition;
        private int mMouseButtonId = -1;

        // Properties
        public Vector2f MousePosition { get { return mMousePosition; } set { mMousePosition = value; } }
        public int MouseButtonId { get { return mMouseButtonId; } set { mMouseButtonId = value; } }
    }
}

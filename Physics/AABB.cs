using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;

namespace TackEngineLib.Physics {
    public class AABB {
        private Vector2f m_bottomLeft;
        private Vector2f m_topRight;

        /// <summary>
        /// The bottom left point of the AABB
        /// </summary>
        public Vector2f BottomLeft {
            get { return m_bottomLeft; }
            set { m_bottomLeft = value; }
        }

        /// <summary>
        /// The top right point of the AABB
        /// </summary>
        public Vector2f TopRight {
            get { return m_topRight; }
            set { m_topRight = value; }
        }

        // Shortcuts to get top/bottom/left/right float values

        public float Top {
            get { return m_topRight.Y; }
        }

        public float Bottom {
            get { return m_bottomLeft.Y; }
        }

        public float Left {
            get { return m_bottomLeft.X; }
        }

        public float Right {
            get { return m_topRight.X; }
        }

        public Vector2f Origin {
            get {
                Vector2f origin = new Vector2f() {
                    X = m_bottomLeft.X + ((m_topRight.X - m_bottomLeft.X) / 2.0f),
                    Y = m_topRight.Y - ((m_topRight.Y - m_bottomLeft.Y) / 2.0f)
                };

                return origin;
            }
        }

        public float Width {
            get {
                return m_topRight.X - m_bottomLeft.X;
            }
        }

        public float Height {
            get {
                return m_topRight.Y - m_bottomLeft.Y;
            }
        }

        internal AABB() {
            m_bottomLeft = new Vector2f(-1, -1);
            m_topRight = new Vector2f(1, 1);
        }

        public AABB(Vector2f bottomLeft, Vector2f topRight) {
            m_bottomLeft = bottomLeft;
            m_topRight = topRight;
        }
    }
}

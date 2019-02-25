using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngineLib.Math
{
    /// <summary>
    /// Representation of a linear equation (straight line)
    /// </summary>
    public struct LinearEquation
    {
        private float m_Gradient;
        private float m_YIntercept;

        /// <summary>
        /// The gradient of this line
        /// </summary>
        public float Gradient
        {
            get { return m_Gradient; }
            set { m_Gradient = value; }
        }

        /// <summary>
        /// The y-intercept of this line
        /// </summary>
        public float YIntercept
        {
            get { return m_YIntercept; }
            set { m_YIntercept = value; }
        }

        /// <summary>
        /// Intializes a new LinearEquation
        /// </summary>
        /// <param name="_gradient">The gradient of the line</param>
        /// <param name="_yIntercept">The y-intercept of the line</param>
        public LinearEquation(float _gradient = 0, float _yIntercept = 0)
        {
            m_Gradient = _gradient;
            m_YIntercept = _yIntercept;
        }

        public override string ToString()
        {
            return string.Format("y={0}x+{1}", m_Gradient, m_YIntercept);
        }
    }
}

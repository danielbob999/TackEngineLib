/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;

namespace TackEngineLib.Math
{
    public static class TackMath
    {
        public static int AbsVali(int _val)
        {
            if (_val > 0)
                return _val;
            else
                return _val * -1;
        }

        public static float AbsValf(float _val)
        {
            if (_val > 0)
                return _val;
            else
                return _val * -1.0f;
        }

        /// <summary>
        /// Converts an angle in degrees to and angle in radians
        /// </summary>
        /// <param name="_angle">An angle in degrees</param>
        /// <returns>The converted angle in radians</returns>
        public static double DegToRad(float _angle)
        {
            return _angle * (System.Math.PI / 180);
        }

        /// <summary>
        /// Finds the intersection point of two lines
        /// </summary>
        /// <param name="_lineA">The linear equation of the first line</param>
        /// <param name="_lineB">The linear equation of the second line</param>
        /// <param name="_intersectPoint">The point at which these two lines intersect. (0, 0) if lines do not intersect</param>
        /// <returns>True if the lines intersect at a point, false if lines do not intersect</returns>
        /// <returntype>bool</returntype>
        public static bool GetLinearIntersectionPoint(LinearEquation _lineA, LinearEquation _lineB, out Vector2f _intersectPoint)
        {
            // Get X value from equations.
            float xValue = (_lineB.YIntercept - _lineA.YIntercept) / (_lineA.Gradient - _lineB.Gradient);

            // Calculate intersection point on the Y axis, using both equations
            float yPointLineA = _lineA.Gradient * xValue + _lineA.YIntercept;
            float yPointLineB = _lineB.Gradient * xValue + _lineB.YIntercept;

            // See if both Y points are equal, if so, _lineA and _lineB intersect. If not equal, lines do not intersect
            if (yPointLineA == yPointLineB)
            {
                _intersectPoint = new Vector2f(xValue, yPointLineA);
                return true;
            }
            else
            {
                _intersectPoint = new Vector2f(-1, -1);
                return false;
            }
        }

        /// <summary>
        /// Calculates the LinearEquation of a line given two points
        /// </summary>
        /// <param name="_pointA">The first point</param>
        /// <param name="_pointB">The second point</param>
        /// <returns>The LinearEquation of the line</returns>
        /// <returntype>LinearEquation</returntype>
        public static LinearEquation GetLinearEquationFromPoints(Vector2f _pointA, Vector2f _pointB)
        {
            LinearEquation equation = new LinearEquation();

            // Calculate the gradient
            equation.Gradient = (_pointB.Y - _pointA.Y) / (_pointB.X - _pointA.X);

            // Calculate the y-intercept using the values if _pointA. The values of _pointB can also be used, the answer will be the same
            equation.YIntercept = _pointA.Y - (equation.Gradient * _pointA.X);

            return equation;
        }
    }
}

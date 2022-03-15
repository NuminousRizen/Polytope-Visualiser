using Util;

namespace Polytope2D.Util.Other
{
    // Holds an inequality in the form ax + by + c >= 0
    /// <summary>
    /// An inequality is described as an inequality in the form ax + by + c >= 0.
    /// </summary>
    public struct Inequality
    {
        private double _a, _b, _c;
    
        public Inequality(double a, double b, double c)
        {
            _a = a;
            _b = b;
            _c = c;
        }

        public double GetA()
        {
            return _a;
        }
    
        public double GetB()
        {
            return _b;
        }

        public double GetC()
        {
            return _c;
        }

        public bool IsWithinBounds(double x, double y)
        {
            return _a * x + _b * y + _c >= -VectorD2D.GetEpsilon();
        }
        
        /// <summary>
        /// Check whether the given point satisfies this inequality.
        /// </summary>
        /// <param name="point">The point that need to be checked.</param>
        /// <returns>Whether the given point satisfies this inequality.</returns>
        public bool IsWithinBounds(VectorD2D point)
        {
            return _a * point.x + _b * point.y + _c >= -VectorD2D.GetEpsilon();
        }

        /// <summary>
        /// Checks if the point is on the inequality line (distance from the line is 0).
        /// </summary>
        /// <param name="point">The point that needs to be checked.</param>
        /// <returns>Whether the point is on the line.</returns>
        public bool IsOnInequalityLine(VectorD2D point)
        {
            return _a * point.x + _b * point.y + _c >= -VectorD2D.GetEpsilon() && 
                   _a * point.x + _b * point.y + _c <= VectorD2D.GetEpsilon();
        }
    
        /// <summary>
        /// Get the intersection point between this and another inequality.
        /// </summary>
        /// <param name="other">The other inequality.</param>
        /// <returns>The point at which these inequalities intersect.</returns>
        public VectorD2D GetIntersection(Inequality other)
        {
            double x = (other.GetC() * _b - _c * other.GetB()) / (_a * other.GetB() - other.GetA() * _b);
            double y = (other.GetC() * _a - _c * other.GetA()) / (_b * other.GetA() - other.GetB() * _a);

            return new VectorD2D(x, y);
        }

        public override string ToString()
        {
            return "(" + _a + ")x + (" + _b + ")y + (" + _c + ") >= 0";
        }

        public string GetPrettyInequality()
        {
            return "(" + _a + ")x + (" + _b + ")y + (" + _c + ") >= 0";
        }

        /// <summary>
        /// Build an inequality from the given points and a reference point.
        /// </summary>
        /// <param name="pointA">The first point.</param>
        /// <param name="pointB">The second point.</param>
        /// <param name="referencePoint">A reference point that is known to satisfy the inequality</param>
        /// <returns>The inequality.</returns>
        public static Inequality GetInequalityFromPoints(VectorD2D pointA, VectorD2D pointB, VectorD2D referencePoint)
        {
            Inequality inequalityA;
            Inequality inequalityB;
            
            if (VectorD2D.YEquals(pointA, pointB))
            {
                inequalityA = new Inequality(0, 1, -pointA.y);
                inequalityB = new Inequality(0, -1, pointA.y);
            }

            else if (VectorD2D.XEquals(pointA, pointB))
            {
                inequalityA = new Inequality(1, 0, -pointA.x);
                inequalityB = new Inequality(-1, 0, pointA.x);
            }

            else
            {
                double slope = (pointB.y - pointA.y) / (pointB.x - pointA.x);
                double yIntercept = pointA.y - slope * pointA.x;

                inequalityA = new Inequality(-slope, 1, -yIntercept);
                inequalityB = new Inequality(slope, -1, yIntercept);
            }

            if (inequalityA.IsWithinBounds(referencePoint)) return inequalityA;
            return inequalityB;
        }
    }
}

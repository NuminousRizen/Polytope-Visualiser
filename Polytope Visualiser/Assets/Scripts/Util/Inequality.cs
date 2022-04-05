using Util;

namespace Polytope2D.Util.Other
{
    /// <summary>
    /// Holds an inequality in the form ax + by + c >= 0.
    /// </summary>
    public struct Inequality
    {
        private double a, b, c;
    
        public Inequality(double _a, double _b, double _c)
        {
            a = _a;
            b = _b;
            c = _c;
        }

        public double GetA()
        {
            return a;
        }
    
        public double GetB()
        {
            return b;
        }

        public double GetC()
        {
            return c;
        }

        public bool IsWithinBounds(double x, double y)
        {
            return a * x + b * y + c >= -VectorD2D.GetEpsilon();
        }
        
        /// <summary>
        /// Check whether the given point satisfies this inequality.
        /// </summary>
        /// <param name="point">The point that need to be checked.</param>
        /// <returns>Whether the given point satisfies this inequality.</returns>
        public bool IsWithinBounds(VectorD2D point)
        {
            return a * point.x + b * point.y + c >= -VectorD2D.GetEpsilon();
        }

        /// <summary>
        /// Checks if the point is on the inequality line (distance from the line is 0).
        /// </summary>
        /// <param name="point">The point that needs to be checked.</param>
        /// <returns>Whether the point is on the line.</returns>
        public bool IsOnInequalityLine(VectorD2D point)
        {
            return a * point.x + b * point.y + c >= -VectorD2D.GetEpsilon() && 
                   a * point.x + b * point.y + c <= VectorD2D.GetEpsilon();
        }
    
        /// <summary>
        /// Get the intersection point between this and another inequality.
        /// </summary>
        /// <param name="other">The other inequality.</param>
        /// <returns>The point at which these inequalities intersect.</returns>
        public VectorD2D GetIntersection(Inequality other)
        {
            double x = (other.GetC() * b - c * other.GetB()) / (a * other.GetB() - other.GetA() * b);
            double y = (other.GetC() * a - c * other.GetA()) / (b * other.GetA() - other.GetB() * a);

            return new VectorD2D(x, y);
        }

        public override string ToString()
        {
            return "(" + a + ")x + (" + b + ")y + (" + c + ") >= 0";
        }

        public string GetPrettyInequality()
        {
            return "(" + a + ")x + (" + b + ")y + (" + c + ") >= 0";
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

using Util;

namespace Polytope2D.Util.Other
{
    // Holds an inequality in the form ax + by + c >= 0
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
        
        public bool IsWithinBounds(VectorD2D point)
        {
            return _a * point.x + _b * point.y + _c >= -VectorD2D.GetEpsilon();
        }

        public bool IsOnInequalityLine(VectorD2D point)
        {
            return _a * point.x + _b * point.y + _c >= -VectorD2D.GetEpsilon() && 
                   _a * point.x + _b * point.y + _c <= VectorD2D.GetEpsilon();
        }
    
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

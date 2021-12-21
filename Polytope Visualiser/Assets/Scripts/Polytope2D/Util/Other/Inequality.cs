using UnityEngine;

namespace Polytope2D.Util.Other
{
    // Holds an inequality in the form ax + by + c >= 0
    public struct Inequality
    {
        private float _a;
        private float _b;
        private float _c;
    
        public Inequality(float a, float b, float c)
        {
            _a = a;
            _b = b;
            _c = c;
        }

        public float GetA()
        {
            return _a;
        }
    
        public float GetB()
        {
            return _b;
        }

        public float GetC()
        {
            return _c;
        }

        public bool IsWithinBounds(float x, float y)
        {
            return _a * x + _b * y + _c >= 0;
        }
        
        public bool IsWithinBounds(Vector2 point)
        {
            return _a * point.x + _b * point.y + _c >= 0;
        }
    
        public Vector2 GetIntersection(Inequality other)
        {
            float x = (other.GetC() * _b - _c * other.GetB()) / (_a * other.GetB() - other.GetA() * _b);
            float y = (other.GetC() * _a - _c * other.GetA()) / (_b * other.GetA() - other.GetB() * _a);

            return new Vector2(x, y);
        }

        public string GetPrettyInequality()
        {
            return "(" + _a + ")x + (" + _b + ")y + (" + _c + ") >= 0";
        }

        public static Inequality GetInequalityFromPoints(Vector2 pointA, Vector2 pointB, Vector2 referencePoint)
        {
            Inequality inequalityA;
            Inequality inequalityB;
            
            if (pointA.y == pointB.y)
            {
                inequalityA = new Inequality(0, 1, -pointA.y);
                inequalityB = new Inequality(0, -1, pointA.y);
            }

            else if (pointA.x == pointB.x)
            {
                inequalityA = new Inequality(1, 0, -pointA.x);
                inequalityB = new Inequality(-1, 0, pointA.x);
            }

            else
            {
                float slope = (pointB.y - pointA.y) / (pointB.x - pointA.x);
                float yIntercept = pointA.y - slope * pointA.x;

                inequalityA = new Inequality(-slope, 1, -yIntercept);
                inequalityB = new Inequality(slope, -1, yIntercept);
            }

            if (inequalityA.IsWithinBounds(referencePoint)) return inequalityA;
            return inequalityB;
        }
    }
}

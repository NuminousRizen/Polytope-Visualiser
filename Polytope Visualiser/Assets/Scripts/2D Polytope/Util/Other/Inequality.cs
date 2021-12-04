using UnityEngine;

namespace _2D_Polytope.Util.Other
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
    
        public Vector2 GetIntersection(Inequality other)
        {
            float x = (other.GetC() * _b - _c * other.GetB()) / (_a * other.GetB() - other.GetA() * _b);
            float y = (other.GetC() * _a - _c * other.GetA()) / (_b * other.GetA() - other.GetB() * _a);

            return new Vector2(x, y);
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public struct VectorD2D
    {
        private const double Epsilon = 1e-10;
        
        public double x;
        public double y;
        
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static double GetEpsilon()
        {
            return Epsilon;
        }

        public static implicit operator VectorD2D(VectorD3D a)
        {
            return new VectorD2D(a.x, a.y);
        }

        public static implicit operator VectorD3D(VectorD2D a)
        {
            return new VectorD3D(a.x, a.y, 0);
        }

        public static VectorD2D operator +(VectorD2D a, VectorD2D b)
        {
            return new VectorD2D(a.x + b.x, a.y + b.y);
        }
        
        public static VectorD2D operator -(VectorD2D a, VectorD2D b)
        {
            return new VectorD2D(a.x - b.x, a.y - b.y);
        }
        
        public static VectorD2D operator *(VectorD2D a, VectorD2D b)
        {
            return new VectorD2D(a.x * b.x, a.y * b.y);
        }
        
        public static VectorD2D operator /(VectorD2D a, VectorD2D b)
        {
            return new VectorD2D(a.x / b.x, a.y / b.y);
        }
        
        public static bool operator ==(VectorD2D a, VectorD2D b)
        {
            if (Math.Abs(a.x - b.x) > Epsilon) return false;
            if (Math.Abs(a.y - b.y) > Epsilon) return false;

            return true;
        }
        
        public static bool operator !=(VectorD2D a, VectorD2D b)
        {
            return !(a == b);
        }

        public static double Dot(VectorD2D a, VectorD2D b)
        {
            return a.x * b.x + a.y * b.y;
        }

        public static double Distance(VectorD2D a, VectorD2D b)
        {
            return (a - b).Magnitude();
        }

        public static bool XEquals(VectorD2D a, VectorD2D b)
        {
            return Math.Abs(a.x - b.x) < Epsilon;
        }
        
        public static bool YEquals(VectorD2D a, VectorD2D b)
        {
            return Math.Abs(a.y - b.y) < Epsilon;
        }

        public static VectorD2D Mean(List<VectorD2D> vectors)
        {
            VectorD2D meanVector = new VectorD2D(0, 0);
            foreach (VectorD2D vector in vectors)
            {
                meanVector = meanVector + vector;
            }

            return new VectorD2D(meanVector.x / vectors.Count, meanVector.y / vectors.Count);
        }

        public VectorD2D(double _x, double _y)
        {
            x = _x;
            y = _y;
        }

        public double this[int i]
        {
            get
            {
                if (i == 0) return x;
                if (i == 1) return y;
                throw new IndexOutOfRangeException();
            }
            set
            {
                if (i == 0)
                {
                    x = value;
                    return;
                }

                if (i == 1)
                {
                    y = value;
                    return;
                }
                throw new IndexOutOfRangeException();
            }
        }

        public double Magnitude()
        {
            return Math.Sqrt(x * x + y * y);
        }

        public double SquareMagnitude()
        {
            return x * x + y * y;
        }

        public Vector2 ToVector2()
        {
            return new Vector2((float) x,(float) y);
        }
    }
}
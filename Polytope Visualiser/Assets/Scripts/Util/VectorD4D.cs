using System;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public struct VectorD4D
    {
        private const double Epsilon = 1e-10;
        
        public double x;
        public double y;
        public double z;
        public double w;

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "x: " + x + "; y: " + y + "; z: " + z + "; w: " + w;
        }

        public static double GetEpsilon()
        {
            return Epsilon;
        }

        public static VectorD4D operator +(VectorD4D a, VectorD4D b)
        {
            return new VectorD4D(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        }
        
        public static VectorD4D operator -(VectorD4D a, VectorD4D b)
        {
            return new VectorD4D(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
        }
        
        public static VectorD4D operator *(VectorD4D a, VectorD4D b)
        {
            return new VectorD4D(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
        }
        
        public static VectorD4D operator /(VectorD4D a, VectorD4D b)
        {
            return new VectorD4D(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);
        }
        
        public static bool operator ==(VectorD4D a, VectorD4D b)
        {
            if (Math.Abs(a.x - b.x) > Epsilon) return false;
            if (Math.Abs(a.y - b.y) > Epsilon) return false;
            if (Math.Abs(a.z - b.z) > Epsilon) return false;
            if (Math.Abs(a.w - b.w) > Epsilon) return false;

            return true;
        }
        
        public static bool operator !=(VectorD4D a, VectorD4D b)
        {
            return !(a == b);
        }

        public static double Distance(VectorD4D a, VectorD4D b)
        {
            return (a - b).Magnitude();
        }

        public static double Dot(VectorD4D a, VectorD4D b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        public static VectorD4D Cross(VectorD4D u, VectorD4D v, VectorD4D w)
        {
            double a = (v.x * w.y) - (v.y * w.x);
            double b = (v.x * w.z) - (v.z * w.x);
            double c = (v.x * w.w) - (v.w * w.x);
            double d = (v.y * w.z) - (v.z * w.y);
            double e = (v.y * w.w) - (v.w * w.y);
            double f = (v.z * w.w) - (v.w * w.z);
            
            VectorD4D result = new VectorD4D(0,0,0,0);
            result.x =   (u.y * f) - (u.z * e) + (u.w * d);
            result.y = - (u.x * f) + (u.z * c) - (u.w * b);
            result.z =   (u.x * e) - (u.y * c) + (u.w * a);
            result.w = - (u.x * d) + (u.y * b) - (u.z * a);
            result *= new VectorD4D(-1, -1, -1, -1);
            return result;
        }

        public static VectorD4D Normalise(VectorD4D a)
        {
            double magnitude = a.Magnitude();
            a.x = a.x / magnitude;
            a.y = a.y / magnitude;
            a.z = a.z / magnitude;
            a.w = a.w / magnitude;

            return a;
        }

        public static VectorD4D Mean(List<VectorD4D> vectors)
        {
            VectorD4D meanVector = new VectorD4D(0, 0, 0, 0);
            foreach (VectorD4D vector in vectors)
            {
                meanVector = meanVector + vector;
            }

            return new VectorD4D(
                meanVector.x / vectors.Count, 
                meanVector.y / vectors.Count, 
                meanVector.z / vectors.Count,
                meanVector.w / vectors.Count
            );
        }

        public VectorD4D(double _x, double _y, double _z, double _w)
        {
            x = _x;
            y = _y;
            z = _z;
            w = _w;
        }

        public double this[int i]
        {
            get
            {
                if (i == 0) return x;
                if (i == 1) return y;
                if (i == 2) return z;
                if (i == 3) return w;
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

                if (i == 2)
                {
                    z = value;
                    return;
                }

                if (i == 3)
                {
                    w = value;
                    return;
                }
                throw new IndexOutOfRangeException();
            }
        }

        public double Magnitude()
        {
            return Math.Sqrt(x * x + y * y + z * z + w * w);
        }

        public double SquareMagnitude()
        {
            return x * x + y * y + z * z + w * w;
        }
    }
}
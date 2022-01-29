using System;

namespace Util
{
    public struct VectorD3D
    {
        private const double Epsilon = 1e-10;
        
        public double x;
        public double y;
        public double z;
        
        public static VectorD3D operator +(VectorD3D a, VectorD3D b)
        {
            return new VectorD3D(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        
        public static VectorD3D operator -(VectorD3D a, VectorD3D b)
        {
            return new VectorD3D(a.x - b.x, a.y - b.y, a.z - b.z);
        }
        
        public static VectorD3D operator *(VectorD3D a, VectorD3D b)
        {
            return new VectorD3D(a.x * b.x, a.y * b.y, a.z * b.z);
        }
        
        public static VectorD3D operator /(VectorD3D a, VectorD3D b)
        {
            return new VectorD3D(a.x / b.x, a.y / b.y, a.z / b.z);
        }
        
        public static bool operator ==(VectorD3D a, VectorD3D b)
        {
            if (Math.Abs(a.x - b.x) > Epsilon) return false;
            if (Math.Abs(a.y - b.y) > Epsilon) return false;
            if (Math.Abs(a.z - b.z) > Epsilon) return false;

            return true;
        }
        
        public static bool operator !=(VectorD3D a, VectorD3D b)
        {
            return !(a == b);
        }

        public static double Distance(VectorD3D a, VectorD3D b)
        {
            return (a - b).Magnitude();
        }

        public static VectorD3D Cross(VectorD3D a, VectorD3D b)
        {
            return new VectorD3D(
                a.y * b.z - a.z * b.y,
                a.z * b.x - a.x * b.z,
                a.x * b.y - a.y * b.x
                );
        }

        public VectorD3D(double _x, double _y, double _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public double this[int i]
        {
            get
            {
                if (i == 0) return x;
                if (i == 1) return y;
                if (i == 2) return z;
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
                throw new IndexOutOfRangeException();
            }
        }

        public double Magnitude()
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }

        public double SquareMagnitude()
        {
            return x * x + y * y + z * z;
        }
    }
}
using System;

namespace Util
{
    public struct PlaneInequality
    {
        public double a, b, c, d;

        public PlaneInequality(double a, double b, double c, double d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        public override string ToString()
        {
            return "(" + a + ")x + (" + b + ")y + (" + c + ")z + " + d + " >= 0";
        }

        public double GetDistance(VectorD3D p)
        {
            return a * p.x + b * p.y + c * p.z + d;
        }

        public bool IsWithinBounds(VectorD3D point)
        {
            return a * point.x + b * point.y + c * point.z + d >= -VectorD2D.GetEpsilon();
        }

        public static PlaneInequality GetPlaneInequalityFromPoints(VectorD3D p1, VectorD3D p2, VectorD3D p3,
            VectorD3D referencePoint)
        {
            VectorD3D v1 = p1 - p2;
            VectorD3D v2 = p2 - p3;
            VectorD3D n = VectorD3D.Normalise(VectorD3D.Cross(v1, v2));

            PlaneInequality inequality = new PlaneInequality(n.x, n.y, n.z, -VectorD3D.Dot(n, p1));

            if (inequality.GetDistance(referencePoint) > VectorD3D.GetEpsilon())
            {
                inequality.a = -inequality.a;
                inequality.b = -inequality.b;
                inequality.c = -inequality.c;
            }

            return inequality;
        }

        private static double? GetZ(PlaneInequality p1, PlaneInequality p2, PlaneInequality p3)
        {
            try
            {
                double top = (
                    ((p3.a * p1.b * p1.d * p2.a) - (p3.a * p1.b * p1.d * p2.a)) / (p1.a * (p2.b * p1.a - p1.b * p2.a)) +
                    ((p3.a * p1.d) / p1.a) +
                    (((-p3.b * p1.d * p2.a) + (p3.b * p2.d * p1.a)) / ((p2.b * p1.a) - (p1.b * p2.a))) -
                    p3.d
                );

                double bot = (
                    ((p3.a * p1.b * p2.c * p1.a) - (p3.a * p1.b * p1.c * p2.a)) /
                    (p1.a * ((p2.b * p1.a) - (p1.b * p2.a))) -
                    ((p3.a * p1.c) / p1.a) +
                    ((p3.b * p1.c * p2.a) - (p3.b * p2.c * p1.a)) / ((p2.b * p1.a) - (p1.b * p2.a)) +
                    p3.c
                );

                return top / bot;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private static double? GetY(double z, PlaneInequality p1, PlaneInequality p2)
        {
            try
            {
                double y = (
                    ((p1.c * p2.a * z) + (p1.d * p2.a) - (p2.c * p1.a * z) - p2.d * p1.a) /
                    ((p2.b * p1.a) - (p1.b * p2.a))
                );

                return y;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private static double? GetX(double z, double y, PlaneInequality p1)
        {
            try
            {
                return (
                    (-p1.b * y - p1.c * z - p1.d) /
                    p1.a
                );
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static VectorD3D? GetIntersection(PlaneInequality p1, PlaneInequality p2, PlaneInequality p3)
        {
            double? z = GetZ(p1, p2, p3);
            if (z == null) return null;
            double? y = GetY(z.Value, p1, p2);
            if (y == null) return null;
            double? x = GetX(z.Value, y.Value, p1);
            if (x == null) return null;

            return new VectorD3D(x.Value, y.Value, z.Value);
        }
    }
}
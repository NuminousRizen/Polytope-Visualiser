using UnityEngine;

namespace Util
{
    /// <summary>
    /// A plane inequality (i.e. a 3D inequality) is described as an inequality in the form ax + by + cz + d >= 0.
    /// </summary>
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

        /// <summary>
        /// Get the distance between the given point and this plane.
        /// </summary>
        /// <param name="p">The point from which to find the distance from.</param>
        /// <returns>The distance from the point to this plane.</returns>
        public double GetDistance(VectorD3D p)
        {
            return a * p.x + b * p.y + c * p.z + d;
        }

        /// <summary>
        /// Check whether the given point satisfies this inequality.
        /// </summary>
        /// <param name="point">The point that needs to be checked.</param>
        /// <returns>Whether the given point satisfies this inequality.</returns>
        public bool IsWithinBounds(VectorD3D point)
        {
            return a * point.x + b * point.y + c * point.z + d >= -VectorD2D.GetEpsilon();
        }

        /// <summary>
        /// Build a plane inequality from the given points an a reference point.
        /// </summary>
        /// <param name="p1">The first point.</param>
        /// <param name="p2">The second point.</param>
        /// <param name="p3">The third point.</param>
        /// <param name="referencePoint">A reference point that is known to satisfy the inequality.</param>
        /// <returns>The plane inequality.</returns>
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

        /// <summary>
        /// Check if any of the given planes are parallel to each other.
        /// </summary>
        /// <param name="p1">The first plane.</param>
        /// <param name="p2">The second plane.</param>
        /// <param name="p3">The third plane.</param>
        /// <returns>True if at least two planes are parallel, false otherwise.</returns>
        public static bool AreParallel(PlaneInequality p1, PlaneInequality p2, PlaneInequality p3)
        {
            VectorD3D n1 = VectorD3D.Normalise(new VectorD3D(p1.a, p1.b, p1.c));
            VectorD3D n2 = VectorD3D.Normalise(new VectorD3D(p2.a, p2.b, p2.c));
            VectorD3D n3 = VectorD3D.Normalise(new VectorD3D(p3.a, p3.b, p3.c));

            return n1 == n2 || n2 == n3 || n1 == n3;
        }

        /// <summary>
        /// Calculates the intersection point between the three planes provided.
        /// </summary>
        /// <param name="p1">The first plane.</param>
        /// <param name="p2">The second plane.</param>
        /// <param name="p3">The third plane.</param>
        /// <returns>The intersection point of the planes or null if the planes do not meet at a point.</returns>
        public static VectorD3D? GetIntersection(PlaneInequality p1, PlaneInequality p2, PlaneInequality p3)
        {
            if (AreParallel(p1, p2, p3)) return null;
            VectorD3D n1 = new VectorD3D(p1.a, p1.b, p1.c);
            VectorD3D n2 = new VectorD3D(p2.a, p2.b, p2.c);
            VectorD3D n3 = new VectorD3D(p3.a, p3.b, p3.c);
            double tripleScalar = VectorD3D.Dot(VectorD3D.Cross(n1, n2), n3);
            if (tripleScalar > -VectorD3D.GetEpsilon() && tripleScalar < VectorD3D.GetEpsilon()) return null;
            
            double[,] matrix = new double[3,4];
            matrix[0, 0] = p1.a;
            matrix[0, 1] = p1.b;
            matrix[0, 2] = p1.c;
            matrix[0, 3] = -p1.d;
                
            matrix[1, 0] = p2.a;
            matrix[1, 1] = p2.b;
            matrix[1, 2] = p2.c;
            matrix[1, 3] = -p2.d;
                
            matrix[2, 0] = p3.a;
            matrix[2, 1] = p3.b;
            matrix[2, 2] = p3.c;
            matrix[2, 3] = -p3.d;
            
            matrix = UtilLib.rref(matrix);

            return new VectorD3D(matrix[0, 3], matrix[1, 3], matrix[2, 3]);
        }
    }
}
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
        /// Calculate the z value of the intersection point between the three planes.
        /// </summary>
        /// <param name="p1">The first inequality.</param>
        /// <param name="p2">The second inequality.</param>
        /// <param name="p3">The third inequality.</param>
        /// <returns>The z value of the intersection point, null if no value could be found (i.e. planes do not intersect at a point).</returns>
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
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Calculate the y value of the intersection point between three planes.
        /// </summary>
        /// <param name="z">The z value of the intersection point.</param>
        /// <param name="p1">The first inequality.</param>
        /// <param name="p2">The second inequality.</param>
        /// <returns>The y value of the intersection point, null if no value could be found (i.e. planes do not intersect at a point).</returns>
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
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Calculate the x value of the intersection point between three planes.
        /// </summary>
        /// <param name="z">The z value of the intersection point.</param>
        /// <param name="y">The y value of the intersection point.</param>
        /// <param name="p1">The first inequality</param>
        /// <returns>The x value of the intersection point, null if no value could be found (i.e. planes do not intersect at a point).</returns>
        private static double? GetX(double z, double y, PlaneInequality p1)
        {
            try
            {
                return (
                    (-p1.b * y - p1.c * z - p1.d) /
                    p1.a
                );
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get the intersection point between three planes.
        /// </summary>
        /// <param name="p1">The first plane.</param>
        /// <param name="p2">The second plane.</param>
        /// <param name="p3">The third plane.</param>
        /// <returns>The intersection point, null if no point could be found (i.e. planes do not intersect at a point).</returns>
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
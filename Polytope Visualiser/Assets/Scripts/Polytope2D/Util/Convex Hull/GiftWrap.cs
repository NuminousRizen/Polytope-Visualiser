using System.Collections.Generic;
using Util;

namespace Polytope2D.Util.Convex_Hull
{
    public static class GiftWrap
    {
        private static int SortFun(VectorD2D a, VectorD2D b)
        {
            if (a.x < b.x) return -1;

            return 1;
        }

        private static bool isOnLeft(VectorD2D a, VectorD2D b, VectorD2D c)
        {
            VectorD2D a_prime = a - b;
            VectorD2D b_prime = b - c;
            VectorD3D crossProduct = VectorD3D.Cross(a_prime, b_prime);
            return crossProduct.z < 0;
        }
        
        public static List<VectorD2D> GetConvexHull(List<VectorD2D> points)
        {
            if (points.Count <= 3) return points;
            
            List<VectorD2D> convexHullPoints = new List<VectorD2D>(); 
            points.Sort(SortFun);
            VectorD2D pointOnHull = points[0];
            bool finished = false;
            while (!finished)
            {
                convexHullPoints.Add(pointOnHull);
                VectorD2D endPoint = points[0];
                for (int j = 0; j < points.Count; j++)
                {
                    if (endPoint == pointOnHull || isOnLeft(pointOnHull, endPoint, points[j]))
                    {
                        endPoint = points[j];
                    }
                }

                pointOnHull = endPoint;

                finished = endPoint == convexHullPoints[0];
            }
            
            return convexHullPoints;
        }
    }
}

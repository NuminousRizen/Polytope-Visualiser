using System.Collections.Generic;
using UnityEngine;

namespace _2D_Polytope.Util.Convex_Hull
{
    public static class GiftWrap
    {
        private static int SortFun(Vector2 a, Vector2 b)
        {
            if (a.x < b.x) return -1;

            return 1;
        }

        private static bool isOnLeft(Vector2 a, Vector2 b, Vector2 c)
        {
            Vector2 a_prime = a - b;
            Vector2 b_prime = b - c;
            Vector3 crossProduct = Vector3.Cross(a_prime, b_prime);
            return crossProduct.z < 0;
        }
        
        public static List<Vector2> FindConvexHull(List<Vector2> points)
        {
            if (points.Count <= 3) return points;
            
            List<Vector2> convexHullPoints = new List<Vector2>(); 
            points.Sort(SortFun);
            Vector2 pointOnHull = points[0];
            bool finished = false;
            while (!finished)
            {
                convexHullPoints.Add(pointOnHull);
                Vector2 endPoint = points[0];
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

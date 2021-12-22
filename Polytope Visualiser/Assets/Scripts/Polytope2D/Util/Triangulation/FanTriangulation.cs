using System.Collections.Generic;
using UnityEngine;
using Util;

namespace Polytope2D.Util.Triangulation
{
    public static class FanTriangulation
    {
        public static int[] Triangulate(List<Vector2> points)
        {
            List<int> triangles = new List<int>();
            if (points.Count < 4)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    triangles.Add(i);
                }

                return triangles.ToArray();
            }

            for (int i = 2; i < points.Count; i++)
            {
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(i - 1);
            }

            return triangles.ToArray();
        }
        
        public static int[] Triangulate(List<VectorD2D> points)
        {
            List<int> triangles = new List<int>();
            if (points.Count < 4)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    triangles.Add(i);
                }

                return triangles.ToArray();
            }

            for (int i = 2; i < points.Count; i++)
            {
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(i - 1);
            }

            return triangles.ToArray();
        }
    }
}
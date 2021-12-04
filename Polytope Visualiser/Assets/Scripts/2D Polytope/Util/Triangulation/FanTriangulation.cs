using System.Collections.Generic;
using UnityEngine;

namespace _2D_Polytope.Util.Triangulation
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
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace _2D_Polytope.Util
{
    public static class GrahamScan
    {
        private static Vector2 SecondTop(Stack<Vector2> pointsStack)
        {
            Vector2 tempP = pointsStack.Pop();
            Vector2 secondTop = pointsStack.Peek();
            pointsStack.Push(tempP);
            return secondTop;
        }

        /*
         * return values:
         * 0 = Collinear (points lie on a straight line)
         * 1 = Clockwise turn
         * -1 = CounterClockwise turn
         */
        private static int TurnDirection(Vector2 a, Vector2 b, Vector2 c)
        {
            float dir = (b.y - a.y) * (c.x - b.x) - (b.x - a.x) * (c.y - b.y);
            if (dir == 0) return 0;
            if (dir > 0) return 1;
            return -1;
        }

        private static int SortByPolarAngle(Vector2 p0, Vector2 a, Vector2 b)
        {
            int turn = TurnDirection(p0, a, b);
            if (turn == 0)
            {
                if (Vector2.Distance(p0, a) < Vector2.Distance(p0, b)) return -1;
                return 1;
            }

            return turn;
        }

        private static List<Vector2> RemoveSameAngle(List<Vector2> points, Vector2 p0)
        {
            List<Vector2> toReturn = new List<Vector2>();

            int i = 0;
            while (i < points.Count)
            {
                while (i < points.Count - 1 && TurnDirection(p0, points[i], points[i + 1]) == 0) i++;
                toReturn.Add(points[i]);
                i++;
            }

            return toReturn;
        }

        public static List<Vector2> GetConvexHull(List<Vector2> pointsIn)
        {
            if (pointsIn.Count < 4) return pointsIn;
            List<Vector2> points = new List<Vector2>(pointsIn);
            Stack<Vector2> convexHullPoints = new Stack<Vector2>();

            Vector2 p0 = points[0];
            for (int i = 1; i < points.Count; i++)
            {
                Vector2 p = points[i];
                if (p0.y == p.y && p.x < p0.x) p0 = p;
                if (p.y < p0.y) p0 = p;
            }

            points.Remove(p0);
            
            points.Sort((a, b) => SortByPolarAngle(p0, a, b));
            points = RemoveSameAngle(points, p0);
            points.Insert(0, p0);
            if (points.Count < 3) return points;
            
            convexHullPoints.Push(points[0]);
            convexHullPoints.Push(points[1]);
            convexHullPoints.Push(points[2]);

            for (int i = 3; i < points.Count; i++)
            {
                while (convexHullPoints.Count > 1 && TurnDirection(SecondTop(convexHullPoints), convexHullPoints.Peek(), points[i]) != -1)
                {
                    convexHullPoints.Pop();
                }
                convexHullPoints.Push(points[i]);
            }

            return new List<Vector2>(convexHullPoints);
        }
    }
}
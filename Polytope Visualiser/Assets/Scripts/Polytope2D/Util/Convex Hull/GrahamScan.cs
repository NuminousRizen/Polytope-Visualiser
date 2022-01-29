using System.Collections.Generic;
using Util;

namespace Polytope2D.Util.Convex_Hull
{
    public static class GrahamScan
    {
        private static VectorD2D SecondTop(Stack<VectorD2D> pointsStack)
        {
            VectorD2D tempP = pointsStack.Pop();
            VectorD2D secondTop = pointsStack.Peek();
            pointsStack.Push(tempP);
            return secondTop;
        }

        /*
         * return values:
         * 0 = Collinear (points lie on a straight line)
         * 1 = Clockwise turn
         * -1 = CounterClockwise turn
         */
        private static int TurnDirection(VectorD2D a, VectorD2D b, VectorD2D c)
        {
            double dir = (b.y - a.y) * (c.x - b.x) - (b.x - a.x) * (c.y - b.y);
            if (dir == 0) return 0;
            if (dir > 0) return 1;
            return -1;
        }

        private static int SortByPolarAngle(VectorD2D p0, VectorD2D a, VectorD2D b)
        {
            int turn = TurnDirection(p0, a, b);
            if (turn == 0)
            {
                if (VectorD2D.Distance(p0, a) < VectorD2D.Distance(p0, b)) return -1;
                return 1;
            }

            return turn;
        }

        private static List<VectorD2D> RemoveSameAngle(List<VectorD2D> points, VectorD2D p0)
        {
            List<VectorD2D> toReturn = new List<VectorD2D>();

            int i = 0;
            while (i < points.Count)
            {
                while (i < points.Count - 1 && TurnDirection(p0, points[i], points[i + 1]) == 0) i++;
                toReturn.Add(points[i]);
                i++;
            }

            return toReturn;
        }

        public static List<VectorD2D> GetConvexHull(List<VectorD2D> pointsIn)
        {
            if (pointsIn.Count < 4) return pointsIn;
            List<VectorD2D> points = new List<VectorD2D>(pointsIn);
            Stack<VectorD2D> convexHullPoints = new Stack<VectorD2D>();

            VectorD2D p0 = points[0];
            for (int i = 1; i < points.Count; i++)
            {
                VectorD2D p = points[i];
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

            return new List<VectorD2D>(convexHullPoints);
        }
    }
}
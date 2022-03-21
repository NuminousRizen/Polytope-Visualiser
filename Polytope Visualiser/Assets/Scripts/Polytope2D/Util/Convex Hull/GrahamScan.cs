using System;
using System.Collections.Generic;
using Util;

namespace Polytope2D.Util.Convex_Hull
{
    public static class GrahamScan
    {
        /// <summary>
        /// Gets the second to top item from a given stack.
        /// </summary>
        /// <param name="pointsStack">The stack.</param>
        /// <returns>The point that is second to top on the stack.</returns>
        private static VectorD2D SecondTop(Stack<VectorD2D> pointsStack)
        {
            VectorD2D tempP = pointsStack.Pop();
            VectorD2D secondTop = pointsStack.Peek();
            pointsStack.Push(tempP);
            return secondTop;
        }

        /// <summary>
        /// Returns the "turn" that the given points make.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <param name="c">The third point.</param>
        /// <returns>
        /// Return values are:
        /// 0 = Collinear (points lie on a straight line)
        /// 1 = Clockwise turn
        /// -1 = CounterClockwise turn
        /// </returns>
        private static int TurnDirection(VectorD2D a, VectorD2D b, VectorD2D c)
        {
            double dir = (b.y - a.y) * (c.x - b.x) - (b.x - a.x) * (c.y - b.y);
            if (dir == 0) return 0;
            if (dir > 0) return 1;
            return -1;
        }

        /// <summary>
        /// A sorting function to sort points by polar angle, i.e. sort points counter-clockwise (with respect to some point p0).
        /// (Also sorts them by distance if the points have the same angle).
        /// </summary>
        /// <param name="p0">The reference point.</param>
        /// <param name="a">The first point being compared.</param>
        /// <param name="b">The second point being compared.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Removes duplicate points that have the same angle to a reference point p0.
        /// </summary>
        /// <param name="points">The list of points.</param>
        /// <param name="p0">The reference point.</param>
        /// <returns></returns>
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

        /// <summary>
        /// The Graham scan convex hull algorithm.
        /// </summary>
        /// <param name="pointsIn">The set on points.</param>
        /// <returns>List of points that make up the convex hull (in counter-clockwise order)</returns>
        /// <exception cref="Exception">Error that explains why a convex hull could not be built.</exception>
        public static List<VectorD2D> GetConvexHull(List<VectorD2D> pointsIn)
        {
            if (pointsIn.Count < 3) throw new Exception("Cannot build a convex hull with less than 3 points.");
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
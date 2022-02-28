using System.Collections.Generic;
using Util;

namespace Polytope2D.Util.Convex_Hull
{
    public static class Incremental
    {
        private static (VectorD2D, VectorD2D) GetExtremeX(List<VectorD2D> points)
        {
            VectorD2D minX = points[0];
            VectorD2D maxX = points[0];

            foreach (VectorD2D point in points)
            {
                if (point.x < minX.x)
                {
                    minX = point;
                }
                
                if (point.x > maxX.x)
                {
                    maxX = point;
                }
            }

            return (minX, maxX);
        }

        private static VectorD2D? GetFarthestPointFromEdge(Edge edge, List<VectorD2D> points)
        {
            double farthestDistance = -1;
            VectorD2D? farthestPoint = null;
            foreach (VectorD2D point in points)
            {
                double distance = edge.DistanceFromEdge(point);
                if (distance > farthestDistance)
                {
                    farthestDistance = distance;
                    farthestPoint = point;
                }
            }

            return farthestPoint;
        }

        private static List<VectorD2D> UpdateOutsidePoints(List<VectorD2D> previousOutside, HashSet<Edge> hull, VectorD2D centrePoint) 
        {
            List<VectorD2D> outsidePoints = new List<VectorD2D>();
            foreach (VectorD2D point in previousOutside)
            {
                foreach (Edge edge in hull)
                {
                    if (edge.IsVisible(point, centrePoint))
                    {
                        outsidePoints.Add(point);
                        break;
                    }
                }
            }

            return outsidePoints;
        }

        public static List<VectorD2D> GetConvexHull(List<VectorD2D> pointsIn)
        {
            if (pointsIn.Count < 3) return new List<VectorD2D>();

            List<VectorD2D> outsidePoints = new List<VectorD2D>(pointsIn);

            HashSet<Edge> convexHullEdges = new HashSet<Edge>();
            
            (VectorD2D, VectorD2D) extremeX = GetExtremeX(outsidePoints);

            Edge tempEdge = new Edge(extremeX.Item1, extremeX.Item2);
            VectorD2D? farthestPoint = GetFarthestPointFromEdge(tempEdge, outsidePoints);
            if (farthestPoint == null) return new List<VectorD2D>();
            
            // The "centre point" will ALWAYS be inside the convex hull
            VectorD2D centrePoint = VectorD2D.Mean(new List<VectorD2D>{extremeX.Item1, extremeX.Item2, farthestPoint.Value});

            convexHullEdges.Add(tempEdge);
            convexHullEdges.Add(new Edge(extremeX.Item1, farthestPoint.Value));
            convexHullEdges.Add(new Edge(extremeX.Item2, farthestPoint.Value));

            outsidePoints.Remove(extremeX.Item1);
            outsidePoints.Remove(extremeX.Item2);
            outsidePoints.Remove(farthestPoint.Value);
            outsidePoints = UpdateOutsidePoints(outsidePoints, convexHullEdges, centrePoint);

            while (outsidePoints.Count > 0)
            {
                HashSet<Edge> visibleEdges = new HashSet<Edge>();
                HashSet<Edge> nonVisibleEdges = new HashSet<Edge>();

                VectorD2D currentPoint = outsidePoints[0];

                foreach (Edge edge in convexHullEdges)
                {
                    if (edge.IsVisible(currentPoint, centrePoint)) visibleEdges.Add(edge);
                    else nonVisibleEdges.Add(edge);
                }

                if (visibleEdges.Count > 0)
                {
                    HashSet<VectorD2D> horizonPoints = new HashSet<VectorD2D>(
                        UtilLib.GetPoints2DFromEdges(visibleEdges)
                    );
                
                    horizonPoints.IntersectWith(new HashSet<VectorD2D>(
                        UtilLib.GetPoints2DFromEdges(nonVisibleEdges)
                    ));
                
                    convexHullEdges.ExceptWith(visibleEdges);

                    foreach (VectorD2D horizonPoint in horizonPoints)
                    {
                        convexHullEdges.Add(new Edge(horizonPoint, currentPoint));
                    }

                    outsidePoints.Remove(currentPoint);
                    outsidePoints = UpdateOutsidePoints(outsidePoints, convexHullEdges, centrePoint);
                }
            }

            return UtilLib.GetPoints2DFromEdges(convexHullEdges);
        }
    }
}
using System;
using System.Collections.Generic;
using Util;

namespace Polytope3D.Util.Convex_Hull
{
    public static class Incremental3D
    {
        private static List<VectorD3D> GetExtremePoints(List<VectorD3D> points)
        {
            VectorD3D minX = points[0];
            VectorD3D maxX = points[0];

            VectorD3D minY = points[0];
            VectorD3D maxY = points[0];

            VectorD3D minZ = points[0];
            VectorD3D maxZ = points[0];

            foreach (VectorD3D point in points)
            {
                if (point.x < minX.x) minX = point;
                if (point.x > maxX.x) maxX = point;
                
                if (point.y < minY.y) minY = point;
                if (point.y > maxY.y) maxY = point;
                
                if (point.z < minZ.z) minZ = point;
                if (point.z > maxZ.z) maxZ = point;
            }

            return new List<VectorD3D> {minX, maxX, minY, maxY, minZ, maxZ};
        }

        private static (VectorD3D, VectorD3D) GetMostDistantPoints(List<VectorD3D> points)
        {
            double maxDistance = -1;
            VectorD3D? p1 = null;
            VectorD3D? p2 = null;

            for (int i = 0; i < points.Count; i++)
            {
                VectorD3D point1 = points[i];
                for (int j = 1; j < points.Count; j++)
                {
                    VectorD3D point2 = points[j];
                    if (point1 != point2)
                    {
                        double distance = Math.Abs(VectorD3D.Distance(point1, point2));
                        if (distance > maxDistance)
                        {
                            p1 = point1;
                            p2 = point2;
                        }
                    }
                }
            }

            if (p1 != null && p2 != null) return (p1.Value, p2.Value);

            throw new Exception("Could not find two most distant points.");
        }

        private static VectorD3D GetFurthestPointFromLine(VectorD3D p1, VectorD3D p2, List<VectorD3D> points)
        {
            double maxDistance = 0;
            VectorD3D? maxPoint = null;
            foreach (VectorD3D point in points)
            {
                if (point != p1 && point != p2)
                {
                    VectorD3D v1 = point - p1;
                    VectorD3D v2 = point - p2;
                    VectorD3D v3 = p2 - p1;
                    VectorD3D v4 = VectorD3D.Cross(v1, v2);
                    double distance = Math.Abs(v4.Magnitude() / v3.Magnitude());

                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        maxPoint = point;
                    }
                }
            }

            if (maxPoint != null) return maxPoint.Value;
            throw new Exception("Could not find a most distant point from line.");
        }

        private static VectorD3D GetFurthestPointFromFace(Face face, List<VectorD3D> points)
        {
            double maxDistance = 0;
            VectorD3D? maxPoint = null;

            foreach (VectorD3D point in points)
            {
                double distance = Math.Abs(face.GetDistanceFromFacePlane(point));

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    maxPoint = point;
                }
            }

            if (maxPoint != null) return maxPoint.Value;
            throw new Exception("Could not find a most distant point from face");
        }

        private static HashSet<VectorD3D> UpdateOutsidePoints(HashSet<VectorD3D> previousOutsidePoints,
            HashSet<Face> faces)
        {
            HashSet<VectorD3D> outsidePoints = new HashSet<VectorD3D>();
            foreach (VectorD3D point in previousOutsidePoints)
            {
                foreach (Face face in faces)
                {
                    if (face.IsVisible(point))
                    {
                        outsidePoints.Add(point);
                    }
                }
            }

            return outsidePoints;
        }

        public static HashSet<Face> GetConvexHull(List<VectorD3D> pointsIn)
        {
            if (pointsIn.Count < 4) throw new Exception("Cannot build a convex hull with less than 4 points.");

            HashSet<VectorD3D> outsidePoints = new HashSet<VectorD3D>(pointsIn);
            HashSet<Face> convexHullFaces = new HashSet<Face>();

            List<VectorD3D> extremePoints = new List<VectorD3D>(new HashSet<VectorD3D>(GetExtremePoints(pointsIn)));
            (VectorD3D, VectorD3D) furthestPoints = GetMostDistantPoints(extremePoints);
            VectorD3D thirdPoint = GetFurthestPointFromLine(furthestPoints.Item1, furthestPoints.Item2, extremePoints);
            VectorD3D fourthPoint =
                GetFurthestPointFromFace(new Face(furthestPoints.Item1, furthestPoints.Item2, thirdPoint),
                    extremePoints);

            List<VectorD3D> initialPoints = new List<VectorD3D>{furthestPoints.Item1, furthestPoints.Item2, thirdPoint, fourthPoint};

            for (int i = 0; i < initialPoints.Count; i++)
            {
                Face newFace = new Face(initialPoints[i],
                    initialPoints[(i+1) % initialPoints.Count],
                    initialPoints[(i+2) % initialPoints.Count]);
                newFace.CorrectNormal(initialPoints);
                convexHullFaces.Add(newFace);
                outsidePoints.Remove(initialPoints[i]);
            }
            
            outsidePoints = UpdateOutsidePoints(outsidePoints, convexHullFaces);

            while (outsidePoints.Count > 0)
            {
                List<VectorD3D> outsidePointsList = new List<VectorD3D>(outsidePoints);
                VectorD3D currentPoint = outsidePointsList[0];

                HashSet<Face> visibleFaces = new HashSet<Face>();
                HashSet<Face> nonVisibleFaces = new HashSet<Face>();

                foreach (Face face in convexHullFaces)
                {
                    if (face.IsVisible(currentPoint)) visibleFaces.Add(face);
                    else nonVisibleFaces.Add(face);
                }

                if (visibleFaces.Count > 0)
                {
                    HashSet<Edge> horizonEdges = new HashSet<Edge>(
                        UtilLib.GetEdgesFromFaces(visibleFaces), new EdgeEqualityComparer()
                    );
                    
                    horizonEdges.IntersectWith(
                         new HashSet<Edge>(UtilLib.GetEdgesFromFaces(nonVisibleFaces), new EdgeEqualityComparer())
                    );

                    foreach (Face face in visibleFaces)
                    {
                        convexHullFaces.Remove(face);
                    }

                    foreach (Edge edge in horizonEdges)
                    {
                        (VectorD3D, VectorD3D) edgePoints = edge.GetPoints();
                        Face newFace = new Face(
                            edgePoints.Item1,
                            currentPoint,
                            edgePoints.Item2
                        );
                        newFace.CorrectNormal(initialPoints);
                        convexHullFaces.Add(newFace);
                    }

                    outsidePoints.Remove(currentPoint);
                }
                
                outsidePoints = UpdateOutsidePoints(outsidePoints, convexHullFaces);
            }

            return convexHullFaces;
        }
    }
}
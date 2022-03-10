using System;
using System.Collections.Generic;
using UnityEngine;
using Util;

namespace Polytope3D.Util.Convex_Hull
{
    /// <summary>
    /// The incremental convex hull algorithm in 3 dimensions.
    /// </summary>
    public static class Incremental3D
    {
        /// <summary>
        /// Finds the extreme points from the given points.
        /// In this context extreme points are points with the minimum and maximum value for each axis.
        /// </summary>
        /// <param name="pointsIn">The list of points</param>
        /// <returns>A list containing the extreme points on each axis</returns>
        private static List<VectorD3D> GetExtremePoints(List<VectorD3D> pointsIn)
        {
            VectorD3D minX, maxX, minY, maxY, minZ, maxZ;
            minX = maxX = minY = maxY = minZ = maxZ = pointsIn[0];

            foreach (VectorD3D point in pointsIn)
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
        
        /// <summary>
        /// Finds the "initial" points from the given points.
        /// These initials points would make up the initial tetrahedron.
        /// </summary>
        /// <param name="pointsIn">The list of points</param>
        /// <returns>List containing the 4 initial points.</returns>
        /// <exception cref="Exception">Error describing why 4 initial points were not found.</exception>
        private static List<VectorD3D> GetInitialPoints(List<VectorD3D> pointsIn)
        {
            List<VectorD3D> points = new List<VectorD3D>(pointsIn);
            double maxDistance = 0;
            VectorD3D? p1, p2;
            p1 = p2 = null;

            foreach (VectorD3D point1 in points)
            {
                foreach (VectorD3D point2 in points)
                {
                    if (point1 != point2)
                    {
                        double distance = Math.Abs(VectorD3D.Distance(point1, point2));
                        if (distance > maxDistance)
                        {
                            maxDistance = distance;
                            p1 = point1;
                            p2 = point2;
                        }
                    }
                }
            }

            if (p1 == null) throw new Exception("Could not find two most distant points.");
            points.Remove(p1.Value);
            points.Remove(p2.Value);

            maxDistance = 0;
            VectorD3D? p3 = null;
            VectorD3D v1 = p2.Value - p1.Value;

            foreach (VectorD3D point in points)
            {
                VectorD3D v2 = VectorD3D.Cross(point - p1.Value, point - p2.Value);

                double distance = Math.Abs(v2.Magnitude() / v1.Magnitude());
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    p3 = point;
                }
            }
            
            if (p3 == null) throw new Exception("Could not find a most distant point from line.");
            points.Remove(p3.Value);
            
            PlaneInequality planeInequality = PlaneInequality.GetPlaneInequalityFromPoints(p1.Value, p2.Value, p3.Value, p1.Value);
            maxDistance = 0;
            VectorD3D? p4 = null;

            foreach (VectorD3D point in points)
            {
                double distance = Math.Abs(planeInequality.GetDistance(point));

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    p4 = point;
                }
            }
            
            if (p4 == null) throw new Exception("Could not find a most distant point from face");

            return new List<VectorD3D> {p1.Value, p2.Value, p3.Value, p4.Value};
        }

        /// <summary>
        /// Updates the list of "outside" points, i.e. points that are outside (not yet contained) the specified convex hull.
        /// </summary>
        /// <param name="previousOutsidePoints">List of points that were considered outside the previous convex hull.</param>
        /// <param name="faces">The current convex hull.</param>
        /// <returns>A new set of "outside" points.</returns>
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

        /// <summary>
        /// Builds and returns a convex hull from the given points.
        /// </summary>
        /// <param name="pointsIn">The list of points from which to build the convex hull</param>
        /// <returns>The set of faces that describe the convex hull.</returns>
        /// <exception cref="Exception">Error that explains why a convex hull could not be built.</exception>
        public static HashSet<Face> GetConvexHull(List<VectorD3D> pointsIn)
        {
            if (pointsIn.Count < 4) throw new Exception("Cannot build a convex hull with less than 4 points.");

            HashSet<VectorD3D> outsidePoints = new HashSet<VectorD3D>(pointsIn);
            HashSet<Face> convexHullFaces = new HashSet<Face>();

            foreach (VectorD3D point in GetExtremePoints(pointsIn))
            {
                Debug.Log(point);
            }

            // These are the initial 4 points that will make the initial tetrahedron.
            List<VectorD3D> initialPoints = GetInitialPoints(pointsIn);

            // Build the initial tetrahedron.
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

                // Check which faces are "visible" from the current point.
                foreach (Face face in convexHullFaces)
                {
                    if (face.IsVisible(currentPoint)) visibleFaces.Add(face);
                    else nonVisibleFaces.Add(face);
                }

                if (visibleFaces.Count > 0)
                {
                    // Horizon edges are ones that are shared between a visible face and a non-visible face.
                    HashSet<Edge> horizonEdges = new HashSet<Edge>(
                        UtilLib.GetEdgesFromFaces(visibleFaces), new EdgeEqualityComparer()
                    );
                    
                    horizonEdges.IntersectWith(
                         new HashSet<Edge>(UtilLib.GetEdgesFromFaces(nonVisibleFaces), new EdgeEqualityComparer())
                    );

                    // Remove all visible faces.
                    foreach (Face face in visibleFaces)
                    {
                        convexHullFaces.Remove(face);
                    }

                    // Add the new faces; these are from the horizon edges to the current point.
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
                }
                
                // Remove the point that we just considered from the outside points set and update the outside points list.
                outsidePoints.Remove(currentPoint);
                outsidePoints = UpdateOutsidePoints(outsidePoints, convexHullFaces);
            }

            return convexHullFaces;
        }
    }
}
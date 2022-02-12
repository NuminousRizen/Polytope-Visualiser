using System;
using System.Collections.Generic;
using Polytope2D.Util.Other;

namespace Util
{
    /// <summary>
    /// An equality comparer for edges.
    /// </summary>
    public class EdgeEqualityComparer : IEqualityComparer<Edge>
    {
        public bool Equals(Edge e1, Edge e2)
        {
            (VectorD3D, VectorD3D) e1Points = e1.GetPoints();
            (VectorD3D, VectorD3D) e2Points = e2.GetPoints();

            return (e1Points.Item1 == e2Points.Item1 && e1Points.Item2 == e2Points.Item2) ||
                   (e1Points.Item1 == e2Points.Item2 && e1Points.Item2 == e2Points.Item1);
        }

        public int GetHashCode(Edge e1)
        {
            (VectorD3D, VectorD3D) edgePoints = e1.GetPoints();
            return (edgePoints.Item1 + edgePoints.Item2).GetHashCode();
        }
    }
    
    /// <summary>
    /// An edge is defined as a segment between two points.
    /// </summary>
    public struct Edge
    {
        private VectorD3D p1, p2;

        public Edge(VectorD3D p1, VectorD3D p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        /// <summary>
        /// Check if this edge is equal to another.
        /// </summary>
        /// <param name="other">The other edge to compare equality with.</param>
        /// <returns>Whether this edge is equal to the other.</returns>
        public bool IsEqual(Edge other)
        {
            (VectorD3D, VectorD3D) otherPoints = other.GetPoints();
            return (otherPoints.Item1 == p1 && otherPoints.Item2 == p2) ||
                   (otherPoints.Item1 == p2 && otherPoints.Item2 == p1);
        }

        /// <summary>
        /// Check if this edge is visible from a given point.
        /// </summary>
        /// <param name="eyePoint">The point to check visibility from.</param>
        /// <param name="referenceNonVisiblePoint">A reference point that is know to be non-visible from this edge.</param>
        /// <returns>Whether this edge is visible from the eye point.</returns>
        public bool IsVisible(VectorD3D eyePoint, VectorD3D referenceNonVisiblePoint)
        {
            Inequality temp = Inequality.GetInequalityFromPoints(p1, p2, referenceNonVisiblePoint);
            return !temp.IsWithinBounds(eyePoint);
        }

        /// <summary>
        /// Gives the distance from this edge and a point.
        /// </summary>
        /// <param name="point">The point from which to calculate the distance from.</param>
        /// <returns>The distance from this edge to the point.</returns>
        public double DistanceFromEdge(VectorD3D point)
        {
            double a = p1.y - p2.y;
            double b = p2.x - p1.x;
            double c = p1.x * p2.y - p2.x * p1.y;

            return Math.Abs(a * point.x + b * point.y + c) / Math.Sqrt(a * a + b * b);
        }

        /// <summary>
        /// Get the points that make up this edge.
        /// </summary>
        /// <returns>The pair of points that make up this edge.</returns>
        public (VectorD3D, VectorD3D) GetPoints()
        {
            return (p1, p2);
        }
    }
}
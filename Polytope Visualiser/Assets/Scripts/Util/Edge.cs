using System;
using System.Collections.Generic;
using Polytope2D.Util.Other;

namespace Util
{
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
    
    public struct Edge
    {
        private VectorD3D _p1, _p2;

        public Edge(VectorD3D p1, VectorD3D p2)
        {
            _p1 = p1;
            _p2 = p2;
        }

        public bool IsEqual(Edge other)
        {
            (VectorD3D, VectorD3D) otherPoints = other.GetPoints();
            return (otherPoints.Item1 == _p1 && otherPoints.Item2 == _p2) ||
                   (otherPoints.Item1 == _p2 && otherPoints.Item2 == _p1);
        }

        public bool IsVisible(VectorD3D eyePoint, VectorD3D referenceNonVisiblePoint)
        {
            Inequality temp = Inequality.GetInequalityFromPoints(_p1, _p2, referenceNonVisiblePoint);
            return !temp.IsWithinBounds(eyePoint);
        }

        public double DistanceFromEdge(VectorD3D point)
        {
            double a = _p1.y - _p2.y;
            double b = _p2.x - _p1.x;
            double c = _p1.x * _p2.y - _p2.x * _p1.y;

            return Math.Abs(a * point.x + b * point.y + c) / Math.Sqrt(a * a + b * b);
        }

        public (VectorD3D, VectorD3D) GetPoints()
        {
            return (_p1, _p2);
        }
    }
}
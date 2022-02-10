using System.Collections.Generic;

namespace Util
{
    public class FaceEqualityComparer : IEqualityComparer<Face>
    {
        public bool Equals(Face f1, Face f2)
        {
            (VectorD3D, VectorD3D, VectorD3D) a = f1.GetPoints();
            (VectorD3D, VectorD3D, VectorD3D) b = f2.GetPoints();

            List<VectorD3D> f1Points = new List<VectorD3D> {a.Item1, a.Item2, a.Item3};
            List<VectorD3D> f2Points = new List<VectorD3D> {b.Item1, b.Item2, b.Item3};

            bool equal = true;
            foreach (VectorD3D v1 in f1Points)
            {
                bool innerEqual = false;
                foreach (VectorD3D v2 in f2Points)
                {
                    if (v1 == v2)
                    {
                        innerEqual = true;
                        break;
                    }
                }

                if (!innerEqual)
                {
                    equal = false;
                }
            }

            return equal;
        }

        public int GetHashCode(Face f1)
        {
            (VectorD3D, VectorD3D, VectorD3D) facePoints = f1.GetPoints();
            return (facePoints.Item1 + facePoints.Item2 + facePoints.Item3).GetHashCode();
        }
    }
    
    public struct Face
    {
        private VectorD3D _p1, _p2, _p3;
        private Edge _edge1, _edge2, _edge3;
        private VectorD3D _normal;

        public Face(VectorD3D p1, VectorD3D p2, VectorD3D p3)
        {
            _p1 = p1;
            _p2 = p2;
            _p3 = p3;

            _edge1 = new Edge(p1, p2);
            _edge2 = new Edge(p2, p3);
            _edge3 = new Edge(p1, p3);

            VectorD3D a = p1 - p2;
            VectorD3D b = p2 - p3;
            _normal = VectorD3D.Cross(a, b);
        }

        public static bool operator ==(Face f1, Face f2)
        {
            (VectorD3D, VectorD3D, VectorD3D) a = f1.GetPoints();
            (VectorD3D, VectorD3D, VectorD3D) b = f2.GetPoints();

            List<VectorD3D> f1Points = new List<VectorD3D> {a.Item1, a.Item2, a.Item3};
            List<VectorD3D> f2Points = new List<VectorD3D> {b.Item1, b.Item2, b.Item3};

            bool equal = true;
            foreach (VectorD3D v1 in f1Points)
            {
                bool innerEqual = false;
                foreach (VectorD3D v2 in f2Points)
                {
                    if (v1 == v2)
                    {
                        innerEqual = true;
                        break;
                    }
                }

                if (!innerEqual)
                {
                    equal = false;
                }
            }

            return equal;
        }

        public static bool operator !=(Face f1, Face f2)
        {
            return !(f1 == f2);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (_p1 + _p2 + _p3).GetHashCode();
        }

        public override string ToString()
        {
            return "Point1: " + _p1 + "\nPoint2" + _p2 + "\nPoint3" + _p3;
        }

        public double GetDistanceFromFacePlane(VectorD3D point)
        {
            return VectorD3D.Dot(_normal, point - _p1);
        }

        public void CorrectNormal(List<VectorD3D> internalPoints)
        {
            foreach (VectorD3D point in internalPoints)
            {
                double distance = GetDistanceFromFacePlane(point);
                if (distance > VectorD3D.GetEpsilon())
                {
                    _normal = new VectorD3D(
                        -1 *_normal.x,
                        -1 *_normal.y,
                        -1 *_normal.z
                    );

                    return;
                }
            }
        }

        public bool IsVisible(VectorD3D eyePoint)
        {
            return GetDistanceFromFacePlane(eyePoint) > VectorD3D.GetEpsilon();
        }

        public (Edge, Edge, Edge) GetEdges()
        {
            return (_edge1, _edge2, _edge3);
        }

        public (VectorD3D, VectorD3D, VectorD3D) GetPoints()
        {
            return (_p1, _p2, _p3);
        }
    }
}
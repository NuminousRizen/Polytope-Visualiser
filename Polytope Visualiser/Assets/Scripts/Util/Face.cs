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
        private VectorD3D p1, p2, p3;
        private Edge edge1, edge2, edge3;
        private PlaneInequality faceInequality;

        public Face(VectorD3D p1, VectorD3D p2, VectorD3D p3)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;

            edge1 = new Edge(p1, p2);
            edge2 = new Edge(p2, p3);
            edge3 = new Edge(p1, p3);

            faceInequality = PlaneInequality.GetPlaneInequalityFromPoints(p1, p2, p3, p1);
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
            return (p1 + p2 + p3).GetHashCode();
        }

        public override string ToString()
        {
            return "Point1: " + p1 + "\nPoint2" + p2 + "\nPoint3" + p3;
        }

        public double GetDistanceFromFacePlane(VectorD3D point)
        {
            return faceInequality.GetDistance(point);
        }

        public void CorrectNormal(List<VectorD3D> internalPoints)
        {
            foreach (VectorD3D point in internalPoints)
            {
                double distance = faceInequality.GetDistance(point);
                if (distance > VectorD3D.GetEpsilon())
                {
                    faceInequality.a = -faceInequality.a;
                    faceInequality.b = -faceInequality.b;
                    faceInequality.c = -faceInequality.c;
                    faceInequality.d = -faceInequality.d;

                    return;
                }
            }
        }

        public bool IsVisible(VectorD3D eyePoint)
        {
            return faceInequality.GetDistance(eyePoint) > VectorD3D.GetEpsilon();
        }

        public string GetInequality()
        {
            return faceInequality.ToString();
        }

        public (Edge, Edge, Edge) GetEdges()
        {
            return (edge1, edge2, edge3);
        }

        public (VectorD3D, VectorD3D, VectorD3D) GetPoints()
        {
            return (p1, p2, p3);
        }
    }
}
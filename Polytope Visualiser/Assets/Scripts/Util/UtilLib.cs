using System.Collections.Generic;

namespace Util
{
    public static class UtilLib
    {
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

        public static List<VectorD3D> GetPoints3DFromFaces(HashSet<Face> faces)
        {
            HashSet<VectorD3D> points = new HashSet<VectorD3D>();
            foreach (Face face in faces)
            {
                (VectorD3D, VectorD3D, VectorD3D) facePoints = face.GetPoints();
                points.Add(facePoints.Item1);
                points.Add(facePoints.Item2);
                points.Add(facePoints.Item3);
            }

            return new List<VectorD3D>(points);
        }

        public static List<Edge> GetEdgesFromFaces(HashSet<Face> faces)
        {
            HashSet<Edge> points = new HashSet<Edge>();
            foreach (Face face in faces)
            {
                (Edge, Edge, Edge) facePoints = face.GetEdges();
                points.Add(facePoints.Item1);
                points.Add(facePoints.Item2);
                points.Add(facePoints.Item3);
            }

            return new List<Edge>(points);
        }

        public static List<VectorD2D> GetPoints2DFromEdges(HashSet<Edge> edges)
        {
            HashSet<VectorD2D> points = new HashSet<VectorD2D>();
            foreach (Edge edge in edges)
            {
                (VectorD3D, VectorD3D) edgePoints = edge.GetPoints();
                points.Add(edgePoints.Item1);
                points.Add(edgePoints.Item2);
            }

            return new List<VectorD2D>(points);
        }
        
        public static List<VectorD3D> GetPoints3DFromEdges(HashSet<Edge> edges)
        {
            HashSet<VectorD3D> points = new HashSet<VectorD3D>();
            foreach (Edge edge in edges)
            {
                (VectorD3D, VectorD3D) edgePoints = edge.GetPoints();
                points.Add(edgePoints.Item1);
                points.Add(edgePoints.Item2);
            }

            return new List<VectorD3D>(points);
        }

        public static List<VectorD2D> SortPoints2DCounterClockwise(List<VectorD2D> points)
        {
            VectorD2D p0 = points[0];
            for (int i = 1; i < points.Count; i++)
            {
                VectorD2D p = points[i];
                if (VectorD2D.YEquals(p0, p) && p.x < p0.x) p0 = p;
                if (p.y < p0.y) p0 = p;
            }

            points.Remove(p0);
            
            points.Sort((a, b) => SortByPolarAngle(p0, a, b));
            points = RemoveSameAngle(points, p0);
            points.Insert(0, p0);

            return points;
        }
    }
}
using System.Collections.Generic;

namespace Util
{
    /// <summary>
    /// A general utility library, contains a bundle of useful auxiliary functions.
    /// </summary>
    public static class UtilLib
    {
        /// <summary>
        /// Gives the "turn" that these three points make.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <param name="c">The third point.</param>
        /// <returns>
        /// 0 = Collinear (points lie on a straight line)
        /// 1 = Clockwise turn
        /// -1 = CounterClockwise turn
        /// </returns>
        public static int TurnDirection(VectorD2D a, VectorD2D b, VectorD2D c)
        {
            double dir = (b.y - a.y) * (c.x - b.x) - (b.x - a.x) * (c.y - b.y);
            if (dir == 0) return 0;
            if (dir > 0) return 1;
            return -1;
        }

        /// <summary>
        /// A sorting function to sort points by polar angle, i.e. sort points counter-clockwise (with respect to some point p0).
        /// </summary>
        /// <param name="p0">The reference point p0.</param>
        /// <param name="a">The first point in the comparison.</param>
        /// <param name="b">The first point in the comparison.</param>
        /// <returns>
        /// -1 = a comes before b
        /// 1 = b comes before a
        /// </returns>
        public static int SortByPolarAngle(VectorD2D p0, VectorD2D a, VectorD2D b)
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
        /// Removes points with the same angle from a reference point p0.
        /// </summary>
        /// <param name="points">The list of points.</param>
        /// <param name="p0">The reference point p0.</param>
        /// <returns>A list of points without ones with the same angle.</returns>
        public static List<VectorD2D> RemoveSameAngle(List<VectorD2D> points, VectorD2D p0)
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
        /// Get the points from a set of faces.
        /// </summary>
        /// <param name="faces">The set of faces.</param>
        /// <returns>All the points that are in the given faces (no duplicates).</returns>
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

        /// <summary>
        /// Get the edges from a set of faces.
        /// </summary>
        /// <param name="faces">The set of faces.</param>
        /// <returns>All the edges that are in the given faces (no duplicates)</returns>
        public static List<Edge> GetEdgesFromFaces(HashSet<Face> faces)
        {
            HashSet<Edge> points = new HashSet<Edge>(new EdgeEqualityComparer());
            foreach (Face face in faces)
            {
                (Edge, Edge, Edge) facePoints = face.GetEdges();
                points.Add(facePoints.Item1);
                points.Add(facePoints.Item2);
                points.Add(facePoints.Item3);
            }

            return new List<Edge>(points);
        }

        /// <summary>
        /// Get the points (2D) from a set of edges.
        /// </summary>
        /// <param name="edges">The set of edges.</param>
        /// <returns>A list of points that are in the given edges (no duplicates)</returns>
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
        
        /// <summary>
        /// Get the points (2D) from a set of edges.
        /// </summary>
        /// <param name="edges">The set of edges.</param>
        /// <returns>A list of points that are in the given edges (no duplicates)</returns>
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
        
        /// <summary>
        /// Reduce a matrix to Reduced Row Echelon Form.
        ///
        /// Note: This code is from RosettaCode: https://rosettacode.org/wiki/Reduced_row_echelon_form.
        /// </summary>
        /// <param name="matrix">Matrix to reduce.</param>
        /// <returns>Matrix in row reduced echelon form.</returns>
        public static double[,] rref(double[,] matrix)
        {            
            int lead = 0, rowCount = matrix.GetLength(0), columnCount = matrix.GetLength(1);
            for (int r = 0; r < rowCount; r++)
            {
                if (columnCount <= lead) break;
                int i = r;
                while (matrix[i, lead] == 0)
                {
                    i++;
                    if (i == rowCount)
                    {
                        i = r;
                        lead++;
                        if (columnCount == lead)
                        {
                            lead--;
                            break;
                        }
                    }
                }
                for (int j = 0; j < columnCount; j++)
                {
                    double temp = matrix[r, j];
                    matrix[r, j] = matrix[i, j];
                    matrix[i, j] = temp;
                }
                double div = matrix[r, lead];
                if(div != 0)
                    for (int j = 0; j < columnCount; j++) matrix[r, j] /= div;                
                for (int j = 0; j < rowCount; j++)
                {
                    if (j != r)
                    {
                        double sub = matrix[j, lead];
                        for (int k = 0; k < columnCount; k++) matrix[j, k] -= (sub * matrix[r, k]);
                    }
                }
                lead++;
            }
            return matrix;
        }
    }
}
using System.Collections.Generic;
using Polytope2D.Util.Other;
using Polytope2D.Util.Triangulation;
using Polytope2D.Util.Convex_Hull;
using Polytope3D.Util.Convex_Hull;
using UI.Tooltip;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace Polytope2D.UI
{
    public class PolytopeUI : MonoBehaviour
    {
        public Theme2D theme;
        public GameObject tooltipDisplay;

        private Transform _convexHullPointsHolder;
        private Transform _otherPointsHolder;
        private Transform _linesHolder;

        private Vector2 _mousePos;

        /// <summary>
        /// Generates some random points (2D).
        /// </summary>
        /// <returns>List of randomly generated points.</returns>
        private static List<VectorD2D> GenerateRandomPoints()
        {
            Camera camera = Camera.main;
            float height = 2f * camera.orthographicSize;
            float width = height * camera.aspect;
            float buffer = height * .05f;

            List<VectorD2D> generatedPoints = new List<VectorD2D>();
            
            int numberOfPoints = Random.Range(3, 51);
            Debug.Log("Generated " + numberOfPoints + " points.");

            for (int i = 0; i < numberOfPoints; i++)
            {
                generatedPoints.Add(new VectorD2D(Random.Range(-(width / 2) + buffer, (width / 2) - buffer),
                    Random.Range(-(height / 2) + buffer, (height / 2) - buffer)));
            }

            return generatedPoints;
        }
        
        /// <summary>
        /// Generates some random points (3D).
        /// </summary>
        /// <returns>List of randomly generated points.</returns>
        private static List<VectorD3D> GenerateRandomPoints3D()
        {
            Camera camera = Camera.main;
            float height = 2f * camera.orthographicSize;
            float buffer = height * .05f;

            List<VectorD3D> generatedPoints = new List<VectorD3D>();
            
            int numberOfPoints = Random.Range(4, 51);
            Debug.Log("Generated " + numberOfPoints + " points.");

            for (int i = 0; i < numberOfPoints; i++)
            {
                generatedPoints.Add(new VectorD3D(Random.Range(-(height / 2) + buffer, (height / 2) - buffer),
                    Random.Range(-(height / 2) + buffer, (height / 2) - buffer),
                    Random.Range(-(height / 2) + buffer, (height / 2) - buffer)));
            }

            return generatedPoints;
        }

        public void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            // Disable tooltip.
            if (Input.GetKeyDown(KeyCode.T))
            {
                tooltipDisplay.SetActive(!tooltipDisplay.activeSelf);
            }
        }

        /// <summary>
        /// Clears/ resets the polytope UI.
        /// </summary>
        public void Clear()
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;

            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }
            
            _convexHullPointsHolder = new GameObject("Convex Hull Points").transform;
            _otherPointsHolder = new GameObject("Other Points").transform;
            _linesHolder = new GameObject("Inequalities").transform;
            _convexHullPointsHolder.parent = _otherPointsHolder.parent = _linesHolder.parent = transform;
        }

        /// <summary>
        /// Builds polytope (2D) from points.
        /// </summary>
        /// <param name="pointsIn">The list of points.</param>
        public void BuildFromPoints2D(List<VectorD3D> pointsIn)
        {
            List<VectorD2D> points = new List<VectorD2D>();
            foreach (VectorD3D point in pointsIn)
            {
                points.Add(point);
            }

            BuildPolytope2D(GrahamScan.GetConvexHull(points), points);
            Vector3 centrePoint = VectorD3D.Mean(pointsIn).ToVector3();
            transform.localPosition = -centrePoint;
        }

        /// <summary>
        /// Builds polytope (3D) from points.
        /// </summary>
        /// <param name="pointsIn">The list of points.</param>
        public void BuildFromPoints3D(List<VectorD3D> pointsIn)
        {
            BuildPolytope3D(Incremental3D.GetConvexHull(pointsIn), pointsIn);
            Vector3 centrePoint = VectorD3D.Mean(pointsIn).ToVector3();
            transform.localPosition = -centrePoint;
        }

        /// <summary>
        /// Builds polytope (2D) from equations.
        /// </summary>
        /// <param name="inequalities">The list of equations.</param>
        public void BuildFromInequalities2D(List<Inequality> inequalities)
        {
            List<VectorD2D> points = new List<VectorD2D>();

            List<VectorD2D> intersectionPoints = new List<VectorD2D>();
            for (int i = 0; i < inequalities.Count; i++)
            {
                Inequality currentInequality = inequalities[i];
                
                for (int j = i + 1; j < inequalities.Count; j++)
                {
                    intersectionPoints.Add(currentInequality.GetIntersection(inequalities[j]));
                }
            }
            
            for (int i = 0; i < intersectionPoints.Count; i++)
            {
                VectorD2D currentPoint = intersectionPoints[i];
                bool satisfiesAll = true;
                for (int j = 0; j < inequalities.Count; j++)
                {
                    if (!inequalities[j].IsWithinBounds(currentPoint))
                    {
                        satisfiesAll = false;
                        break;
                    }
                }
            
                if (satisfiesAll)
                {
                    points.Add(currentPoint);
                }
            }
            BuildPolytope2D(GrahamScan.GetConvexHull(points), points);
            VectorD3D centrePoint = VectorD2D.Mean(points);
            transform.localPosition = -centrePoint.ToVector3();
        }

        /// <summary>
        /// Builds polytope (3D) from equations.
        /// </summary>
        /// <param name="planes">The list of equations.</param>
        public void BuildFromInequalities3D(List<PlaneInequality> planes)
        {
            HashSet<VectorD3D> intersectionPoints = new HashSet<VectorD3D>();

            for (int i = 0; i < planes.Count; i++)
            {
                for (int j = 0; j < planes.Count; j++)
                {
                    if (i != j)
                    {
                        for (int k = 0; k < planes.Count; k++)
                        {
                            if (k != i && k != j)
                            {
                                VectorD3D? intersectionPoint = PlaneInequality.GetIntersection(
                                    planes[i],
                                    planes[j],
                                    planes[k]
                                );
                                if (intersectionPoint != null)
                                {
                                    intersectionPoints.Add(intersectionPoint.Value);
                                }
                            }
                        }
                    }
                }
            }

            List<VectorD3D> points3D = new List<VectorD3D>();
            foreach (VectorD3D point in intersectionPoints)
            {
                bool isValid = true;
                foreach (PlaneInequality plane in planes)
                {
                    if (!plane.IsWithinBounds(point))
                    {
                        isValid = false;
                        break;
                    }
                }
                    
                if (isValid) points3D.Add(point);
            }
            
            HashSet<Face> faces = Incremental3D.GetConvexHull(points3D);
            BuildPolytope3D(faces, points3D);
            Vector3 centrePoint = VectorD3D.Mean(points3D).ToVector3();
            transform.localPosition = -centrePoint;
        }

        /// <summary>
        /// Builds a 3D polytope.
        /// </summary>
        /// <param name="faces">The set of faces.</param>
        /// <param name="allPoints">The set of all points (whether on convex hull or not).</param>
        private void BuildPolytope3D(HashSet<Face> faces, List<VectorD3D> allPoints)
        {
            Clear();
            HashSet<Edge> edges = new HashSet<Edge>(UtilLib.GetEdgesFromFaces(faces), new EdgeEqualityComparer());
            List<VectorD3D> points3D = UtilLib.GetPoints3DFromEdges(edges);
            BuildPoints3D(points3D, allPoints);
            BuildLines3D(edges);
            BuildFaces3D(faces);
        }

        /// <summary>
        /// Builds the points.
        /// </summary>
        /// <param name="points3D">The list of points on the convex hull.</param>
        /// <param name="allPoints">The list of all points.</param>
        private void BuildPoints3D(List<VectorD3D> points3D, List<VectorD3D> allPoints)
        {
            foreach (VectorD3D point in allPoints)
            {
                Transform pointObject = Instantiate(theme.pointPrefab, point.ToVector3(), transform.rotation);
                pointObject.name = "x: " + point.x + " ; y: " + point.y + "; z: " + point.z;
                
                bool isConvexHullPoint = points3D.Contains(point);
                pointObject.parent = isConvexHullPoint
                    ? _convexHullPointsHolder
                    : _otherPointsHolder;
                
                pointObject.GetComponent<Renderer>().material.color = isConvexHullPoint
                    ? theme.convexHullPointColour
                    : theme.pointColour;
                
                pointObject.localScale = Vector3.one * theme.pointSize;

                pointObject.gameObject.AddComponent<BoxCollider>();
                
                TooltipTrigger tooltipTrigger = pointObject.gameObject.AddComponent<TooltipTrigger>();
                tooltipTrigger.toShow = "x: " + point.x + " ; y: " + point.y + "; z: " + point.z;
            }
        }

        /// <summary>
        /// Builds the lines/ edges to be displayed.
        /// </summary>
        /// <param name="edges">The set of edges to be displayed.</param>
        private void BuildLines3D(HashSet<Edge> edges)
        {
            foreach (Edge edge in edges)
            {
                (VectorD3D, VectorD3D) edgePoints = edge.GetPoints();
                LineRenderer lineRenderer =
                    new GameObject(edge.ToString()).AddComponent<LineRenderer>();
                lineRenderer.useWorldSpace = false;
                lineRenderer.transform.parent = _linesHolder;
                lineRenderer.startColor = theme.lineColour;
                lineRenderer.endColor = theme.lineColour;
                lineRenderer.startWidth = theme.lineSize;
                lineRenderer.endWidth = theme.lineSize;
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                lineRenderer.positionCount = 2;
                lineRenderer.SetPositions(new Vector3[] { edgePoints.Item1.ToVector3(), edgePoints.Item2.ToVector3() });
            }
        }

        /// <summary>
        /// Builds the faces of the polytope.
        /// </summary>
        /// <param name="faces">The set of faces.</param>
        private void BuildFaces3D(HashSet<Face> faces)
        {
            foreach (Face face in faces)
            {
                (VectorD3D, VectorD3D, VectorD3D) facePoints = face.GetPoints();
                Mesh polytopeMesh = new Mesh();
                polytopeMesh.vertices = new Vector3[]{facePoints.Item1.ToVector3(), facePoints.Item2.ToVector3(), facePoints.Item3.ToVector3()};
                polytopeMesh.triangles = new int[]{0,1,2};
                polytopeMesh.RecalculateNormals();
            
                GameObject polytopeFace = new GameObject(face.GetInequality());
                polytopeFace.AddComponent<MeshFilter>().mesh = polytopeMesh;
                MeshRenderer polytopeRenderer = polytopeFace.AddComponent<MeshRenderer>();
                polytopeRenderer.sharedMaterial = new Material(Shader.Find("Sprites/Default"));
                polytopeRenderer.sharedMaterial.color = theme.polygonColour;
                polytopeFace.transform.parent = transform;
                
                MeshCollider meshCollider = polytopeFace.gameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = polytopeMesh;

                TooltipTrigger tooltipTrigger = polytopeFace.gameObject.AddComponent<TooltipTrigger>();
                tooltipTrigger.toShow = face.GetInequality();
            }
        }

        /// <summary>
        /// Builds a 2D polytope. 
        /// </summary>
        /// <param name="convexHullPoints">The set of points on the convex hull (extreme points).</param>
        /// <param name="points">The set of all points (whether on convex hull or not).</param>
        private void BuildPolytope2D(List<VectorD2D> convexHullPoints, List<VectorD2D> points)
        {
            Clear();
            BuildPoints2D(convexHullPoints, points);
            BuildLines2D(convexHullPoints);
            BuildPolytopeMesh2D(convexHullPoints);
        }

        /// <summary>
        /// Builds the points.
        /// </summary>
        /// <param name="convexHullPoints">The set of points on the convex hull.</param>
        /// <param name="points">The set of all points (whether on hull or not).</param>
        private void BuildPoints2D(List<VectorD2D> convexHullPoints, List<VectorD2D> points)
        {
            foreach (VectorD2D point in points)
            {
                Transform pointObject = Instantiate(theme.pointPrefab, point.ToVector2(), transform.rotation);
                pointObject.name = "x: " + point.x + " ; y: " + point.y;

                bool isConvexHullPoint = convexHullPoints.Contains(point);
                
                pointObject.parent = isConvexHullPoint
                    ? _convexHullPointsHolder
                    : _otherPointsHolder;
                
                pointObject.GetComponent<Renderer>().material.color = isConvexHullPoint
                    ? theme.convexHullPointColour
                    : theme.pointColour;
                
                pointObject.localScale = Vector3.one * theme.pointSize;

                pointObject.gameObject.AddComponent<BoxCollider>();
                
                TooltipTrigger tooltipTrigger = pointObject.gameObject.AddComponent<TooltipTrigger>();
                tooltipTrigger.toShow = "x: " + point.x + " ; y: " + point.y;
            }
        }

        /// <summary>
        /// Builds the lines/ edges to be displayed.
        /// </summary>
        /// <param name="convexHullPoints">The set of points on the convex hull.</param>
        private void BuildLines2D(List<VectorD2D> convexHullPoints)
        {
            for (int i = 0; i < convexHullPoints.Count; i++)
            {
                VectorD2D pointA = convexHullPoints[i];
                VectorD2D pointB = convexHullPoints[(i + 1) % convexHullPoints.Count];
                VectorD2D referencePoint = convexHullPoints[(i + 2) % convexHullPoints.Count];

                Inequality inequality = Inequality.GetInequalityFromPoints(pointA, pointB, referencePoint);
                LineRenderer lineRenderer =
                    new GameObject(inequality.GetPrettyInequality()).AddComponent<LineRenderer>();
                lineRenderer.transform.parent = _linesHolder;
                lineRenderer.startColor = theme.lineColour;
                lineRenderer.endColor = theme.lineColour;
                lineRenderer.startWidth = theme.lineSize;
                lineRenderer.endWidth = theme.lineSize;
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                lineRenderer.positionCount = 2;
                lineRenderer.SetPositions(new Vector3[] { pointA.ToVector2(), pointB.ToVector2() });

                MeshCollider meshCollider = lineRenderer.gameObject.AddComponent<MeshCollider>();

                Mesh mesh = new Mesh();
                lineRenderer.BakeMesh(mesh, true);
                meshCollider.sharedMesh = mesh;

                TooltipTrigger tooltipTrigger = lineRenderer.gameObject.AddComponent<TooltipTrigger>();
                tooltipTrigger.toShow = inequality.GetPrettyInequality();
                
                lineRenderer.useWorldSpace = false;
            }
        }

        /// <summary>
        /// Builds the mesh of the polytope (2D).
        /// </summary>
        /// <param name="convexHullPoints">The set of points on the convex hull.</param>
        private void BuildPolytopeMesh2D(List<VectorD2D> convexHullPoints)
        {
            List<Vector3> toDisplay = new List<Vector3>();
            foreach (VectorD2D point in convexHullPoints)
            {
                toDisplay.Add(new Vector3((float) point.x, (float) point.y));
            }
            Mesh polytopeMesh = new Mesh();
            polytopeMesh.vertices = toDisplay.ToArray();
            polytopeMesh.triangles = FanTriangulation.Triangulate(convexHullPoints);
            polytopeMesh.RecalculateNormals();
            
            GameObject polytope = new GameObject("Polytope");
            polytope.AddComponent<MeshFilter>().mesh = polytopeMesh;
            MeshRenderer polytopeRenderer = polytope.AddComponent<MeshRenderer>();
            polytopeRenderer.sharedMaterial = new Material(Shader.Find("Sprites/Default"));
            polytopeRenderer.sharedMaterial.color = theme.polygonColour;
            polytope.transform.parent = transform;
        }
    }
}

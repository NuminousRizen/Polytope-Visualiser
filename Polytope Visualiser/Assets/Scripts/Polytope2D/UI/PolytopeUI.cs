using System.Collections.Generic;
using Polytope2D.Util.Other;
using Polytope2D.Util.Triangulation;
using Polytope3D.Util.Convex_Hull;
using Polytope4D;
using UI.Tooltip;
using UnityEngine;
using Util;

namespace Polytope2D.UI
{
    public class PolytopeUI : MonoBehaviour
    {
        public Theme2D theme;
        
        public List<VectorD2D> points;

        private Transform _convexHullPointsHolder;
        private Transform _otherPointsHolder;
        private Transform _linesHolder;

        private Vector2 _mousePos;

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

        private void Update()
        {
            HandleInput();
            //BuildFromPoints();
            BuildFromInequalities();
        }

        private void HandleInput()
        {
            Vector2 mouseScroll = Input.mouseScrollDelta;
            if (mouseScroll.y != 0)
            {
                float change = mouseScroll.y * .1f;
                Vector3 temp = transform.localScale + new Vector3(change, change, change);
                if (temp != Vector3.zero)
                {
                    transform.localScale = temp;
                }
            }
            
            if (Input.GetMouseButton(0))
            {
                Vector3 temp = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                transform.localPosition += temp;
            }
            else if (Input.GetMouseButton(1))
            {
                _mousePos += new Vector2(Input.GetAxis("Mouse X"),-Input.GetAxis("Mouse Y")) * 2f;
                transform.rotation = Quaternion.Euler(_mousePos.y, _mousePos.x, 0);
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                transform.position = Vector3.zero;
                transform.rotation = Quaternion.identity;
                transform.localScale = Vector3.one;
                _mousePos = Vector2.zero;
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                foreach (Transform point in _convexHullPointsHolder.transform)
                {
                    print(point.name);
                }
                
                foreach (Transform point in _otherPointsHolder.transform)
                {
                    print(point.name);
                }
            }
        }

        private void Clear()
        {
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }
            
            _convexHullPointsHolder = new GameObject("Convex Hull Points").transform;
            _otherPointsHolder = new GameObject("Other Points").transform;
            _linesHolder = new GameObject("Inequalities").transform;
            _convexHullPointsHolder.parent = _otherPointsHolder.parent = _linesHolder.parent = transform;
        }

        void BuildFromPoints()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // ------- Testing 3D -------
                List<VectorD3D> points3d = GenerateRandomPoints3D();
                // points3d.Add(new VectorD3D(0, 0, 0));
                // points3d.Add(new VectorD3D(0, 10, 0));
                // points3d.Add(new VectorD3D(0, 10, 10));
                // points3d.Add(new VectorD3D(0, 0, 10));
                //
                // points3d.Add(new VectorD3D(10, 0, 0));
                // points3d.Add(new VectorD3D(10, 10, 0));
                // points3d.Add(new VectorD3D(10, 10, 10));
                // points3d.Add(new VectorD3D(10, 0, 10));

                HashSet<Face> faces = Incremental3D.GetConvexHull(points3d);
                // points3d = UtilLib.GetPoints3DFromFaces(faces);
                // foreach (VectorD3D point in points3d)
                // {
                //     print(point);
                // }
                BuildPolytope3D(faces, points3d);
                // // --------------------------

                // points = GenerateRandomPoints();
                // BuildPolytope(UtilLib.SortPoints2DCounterClockwise(Incremental.GetConvexHull(points)));
            }
        }

        void BuildFromInequalities()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // points = new List<VectorD2D>();
                // List<Inequality> inequalities = new List<Inequality>();
                //
                // // A square
                // // inequalities.Add(new Inequality(0, 1, 2.5f));
                // // inequalities.Add(new Inequality(1, 0,2.5f));
                // // inequalities.Add(new Inequality(0, -1, 2.5f));
                // // inequalities.Add(new Inequality(-1, 0, 2.5f));
                //
                // inequalities.Add(new Inequality(-0.3960672, -1, 2.081373));
                // inequalities.Add(new Inequality(-0.07919004, -1,1.726297));
                // inequalities.Add(new Inequality(0.2678808, 1, -1.393981));
                //
                // List<VectorD2D> intersectionPoints = new List<VectorD2D>();
                // for (int i = 0; i < inequalities.Count; i++)
                // {
                //     Inequality currentInequality = inequalities[i];
                //     
                //     for (int j = i + 1; j < inequalities.Count; j++)
                //     {
                //         intersectionPoints.Add(currentInequality.GetIntersection(inequalities[j]));
                //     }
                // }
                //
                // for (int i = 0; i < intersectionPoints.Count; i++)
                // {
                //     VectorD2D currentPoint = intersectionPoints[i];
                //     bool satisfiesAll = true;
                //     for (int j = 0; j < inequalities.Count; j++)
                //     {
                //         if (!inequalities[j].IsWithinBounds(currentPoint))
                //         {
                //             satisfiesAll = false;
                //             break;
                //         }
                //     }
                //
                //     if (satisfiesAll)
                //     {
                //         points.Add(currentPoint);
                //     }
                // }
                // BuildPolytope(GrahamScan.GetConvexHull(points));
                
                // ------------- Testing 3D ------------- //
                List<PlaneInequality> planes = new List<PlaneInequality>
                {
                    new PlaneInequality(0,0,-1,5),
                    new PlaneInequality(0,-1,0,5),
                    new PlaneInequality(-1,0,0,5),

                    new PlaneInequality(1,0,0,0),
                    new PlaneInequality(0,0,1,0),
                    new PlaneInequality(0,1,0,0)
                };

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
                // ------------------------------------- //
                
                
                List<VectorD4D> points4D = new List<VectorD4D>();
                points4D.Add(new VectorD4D(1,0,0,0));
                points4D.Add(new VectorD4D(0,1,0,0));
                points4D.Add(new VectorD4D(0,1,1,0));
                points4D.Add(new VectorD4D(1,1,1,0));
                points4D.Add(new VectorD4D(1,1,1,1));
                
                points4D.Add(new VectorD4D(0,0,0,0));
                points4D.Add(new VectorD4D(1,1,0,0));
                
                points4D.Add(new VectorD4D(0,0,1,0));
                points4D.Add(new VectorD4D(1,0,1,0));
                
                points4D.Add(new VectorD4D(.5,.5,.5,.5));
                
                points4D.Add(new VectorD4D(0,0,0,1));
                points4D.Add(new VectorD4D(1,0,0,1));
                points4D.Add(new VectorD4D(0,1,0,1));
                points4D.Add(new VectorD4D(1,1,0,1));
                
                points4D.Add(new VectorD4D(0,0,1,1));
                points4D.Add(new VectorD4D(1,0,1,1));
                points4D.Add(new VectorD4D(0,1,1,1));
                
                // points4D.Add(new VectorD4D(0,0,0,0));
                // points4D.Add(new VectorD4D(1,1,0,0));
                // points4D.Add(new VectorD4D(1,0,1,0));
                // points4D.Add(new VectorD4D(1,0,0,0));
                // points4D.Add(new VectorD4D(1,1,1,1));

                HashSet<HyperFacet> hyperFacets = Incremental4D.GetConvexHull(points4D);
                
                print(hyperFacets.Count);

                HashSet<VectorD4D> pointsD4Ds = new HashSet<VectorD4D>();
                foreach (HyperFacet hyperFacet in hyperFacets)
                {
                    foreach (VectorD4D subFacet in hyperFacet.vertices)
                    {
                        pointsD4Ds.Add(subFacet);
                    }
                }

                print(pointsD4Ds.Count);
            }
        }

        private void BuildPolytope3D(HashSet<Face> faces, List<VectorD3D> allPoints)
        {
            Clear();
            HashSet<Edge> edges = new HashSet<Edge>(UtilLib.GetEdgesFromFaces(faces), new EdgeEqualityComparer());
            HashSet<VectorD3D> points3D = new HashSet<VectorD3D>(UtilLib.GetPoints3DFromEdges(edges));

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

        private void BuildPolytope(List<VectorD2D> convexHullPoints)
        {
            Clear();
            BuildPoints(convexHullPoints);
            BuildLines(convexHullPoints);
            BuildPolytopeMesh(convexHullPoints);
        }

        private void BuildPoints(List<VectorD2D> convexHullPoints)
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

        private void BuildLines(List<VectorD2D> convexHullPoints)
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

        private void BuildPolytopeMesh(List<VectorD2D> convexHullPoints)
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

using System.Collections.Generic;
using Polytope2D.Util.Other;
using Polytope2D.Util.Triangulation;
using Polytope2D.Util.Convex_Hull;
using UI.Tooltip;
using UnityEngine;

namespace Polytope2D.UI
{
    public class PolytopeUI : MonoBehaviour
    {
        public Theme2D theme;
        
        public List<Vector2> points;

        private Transform _convexHullPointsHolder;
        private Transform _otherPointsHolder;
        private Transform _linesHolder;

        private static List<Vector2> GenerateRandomPoints()
        {
            Camera camera = Camera.main;
            float height = 2f * camera.orthographicSize;
            float width = height * camera.aspect;
            float buffer = height * .05f;

            List<Vector2> generatedPoints = new List<Vector2>();
            
            int numberOfPoints = Random.Range(3, 51);
            Debug.Log("Generated " + numberOfPoints + " points.");

            for (int i = 0; i < numberOfPoints; i++)
            {
                generatedPoints.Add(new Vector2(Random.Range(-(width / 2) + buffer, (width / 2) - buffer),
                    Random.Range(-(height / 2) + buffer, (height / 2) - buffer)));
            }

            return generatedPoints;
        }

        private void Update()
        {
            //BuildFromPoints();
            BuildFromInequalities();
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
                points = GenerateRandomPoints();
                BuildPolytope(GrahamScan.GetConvexHull(points));
            }
        }

        void BuildFromInequalities()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                points = new List<Vector2>();
                List<Inequality> inequalities = new List<Inequality>();
            
                // A square
                inequalities.Add(new Inequality(0, 1, 2.5f));
                inequalities.Add(new Inequality(1, 0,2.5f));
                inequalities.Add(new Inequality(0, -1, 2.5f));
                inequalities.Add(new Inequality(-1, 0, 2.5f));
                
                // A random inequality that generates a triangle shape
                // inequalities.Add(new Inequality(-5f, 1, 13f));
                // inequalities.Add(new Inequality(2f, 1, -1));
                // inequalities.Add(new Inequality(-1f, -1, 2f));
                
                List<Vector2> intersectionPoints = new List<Vector2>();
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
                    Vector2 currentPoint = intersectionPoints[i];
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
                BuildPolytope(GrahamScan.GetConvexHull(points));
            }
        }

        private void BuildPolytope(List<Vector2> convexHullPoints)
        {
            Clear();
            BuildPoints(convexHullPoints);
            BuildLines(convexHullPoints);
            BuildPolytopeMesh(convexHullPoints);
        }

        private void BuildPoints(List<Vector2> convexHullPoints)
        {
            foreach (Vector2 point in points)
            {
                Transform pointObject = Instantiate(theme.pointPrefab, point, transform.rotation);
                pointObject.name = "x: " + point.x + " ; y: " + point.y;

                bool isConvexHullPoint = convexHullPoints.Contains(point);
                
                pointObject.parent = isConvexHullPoint
                    ? _convexHullPointsHolder
                    : _otherPointsHolder;
                
                pointObject.GetComponent<SpriteRenderer>().color = isConvexHullPoint
                    ? theme.convexHullPointColour
                    : theme.pointColour;
                
                pointObject.localScale = Vector3.one * theme.pointSize;

                pointObject.gameObject.AddComponent<BoxCollider>();
                
                TooltipTrigger tooltipTrigger = pointObject.gameObject.AddComponent<TooltipTrigger>();
                tooltipTrigger.toShow = "x: " + point.x + " ; y: " + point.y;
            }
        }

        private void BuildLines(List<Vector2> convexHullPoints)
        {
            for (int i = 0; i < convexHullPoints.Count; i++)
            {
                Vector2 pointA = convexHullPoints[i];
                Vector2 pointB = convexHullPoints[(i + 1) % convexHullPoints.Count];
                Vector2 referencePoint = convexHullPoints[(i + 2) % convexHullPoints.Count];

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
                lineRenderer.SetPositions(new Vector3[] { pointA, pointB });

                MeshCollider meshCollider = lineRenderer.gameObject.AddComponent<MeshCollider>();

                Mesh mesh = new Mesh();
                lineRenderer.BakeMesh(mesh, true);
                meshCollider.sharedMesh = mesh;

                TooltipTrigger tooltipTrigger = lineRenderer.gameObject.AddComponent<TooltipTrigger>();
                tooltipTrigger.toShow = inequality.GetPrettyInequality();
            }
        }

        private void BuildPolytopeMesh(List<Vector2> convexHullPoints)
        {
            List<Vector3> toDisplay = new List<Vector3>();
            foreach (Vector2 point in convexHullPoints)
            {
                toDisplay.Add(new Vector3(point.x, point.y));
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

using System.Collections.Generic;
using _2D_Polytope.Util.Other;
using _2D_Polytope.Util.Triangulation;
using _2D_Polytope.Util.Convex_Hull;
using UI;
using UnityEngine;

namespace _2D_Polytope.UI
{
    public class PolytopeUI : MonoBehaviour
    {
        public Theme2D theme;
        
        public List<Vector2> points;
        private List<Vector2> convexHullPoints;

        void Update()
        {
            //BuildFromPoints();
            BuildFromInequalities();
        }

        void BuildFromPoints()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }

                Camera camera = Camera.main;
                float height = 2f * camera.orthographicSize;
                float width = height * camera.aspect;
                float buffer = height * .05f;

                points = new List<Vector2>();

                // int numberOfPoints = Random.Range(3, 51);
                int numberOfPoints = 3;
                Debug.Log("Making a polytope with " + numberOfPoints + " points.");

                for (int i = 0; i < numberOfPoints; i++)
                {
                    points.Add(new Vector2(Random.Range(-(width / 2) + buffer, (width / 2) - buffer),
                        Random.Range(-(height / 2) + buffer, (height / 2) - buffer)));
                }

                convexHullPoints = GrahamScan.GetConvexHull(points);
                BuildPolytope();
            }
        }

        void BuildFromInequalities()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                foreach (Transform child in transform) {
                    Destroy(child.gameObject);
                }
            
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
            
                convexHullPoints = GrahamScan.GetConvexHull(points);
                BuildPolytope();
            }
        }

        void BuildPolytope()
        {
            Transform convexHullPointsHolder = new GameObject("Convex Hull Points").transform;
            Transform otherPointsHolder = new GameObject("Other Points").transform;
            Transform inequalitiesHolder = new GameObject("Inequalities").transform;
            convexHullPointsHolder.parent = otherPointsHolder.parent = inequalitiesHolder.parent = transform;

            foreach (Vector2 point in points)
            {
                Transform circle = Instantiate(theme.pointPrefab, point, transform.rotation);
                circle.name = "x: " + point.x + " ; y: " + point.y;
                circle.parent = convexHullPoints.Contains(point)
                    ? convexHullPointsHolder
                    : otherPointsHolder;
                
                circle.GetComponent<SpriteRenderer>().color = convexHullPoints.Contains(point)
                    ? theme.convexHullPointColour
                    : theme.pointColour;
                circle.localScale = Vector3.one * theme.pointSize;

                circle.gameObject.AddComponent<BoxCollider>();
                
                TooltipTrigger tooltipTrigger = circle.gameObject.AddComponent<TooltipTrigger>();
                tooltipTrigger.toShow = "x: " + point.x + " ; y: " + point.y;
            }

            for (int i = 0; i < convexHullPoints.Count; i++)
            {
                Vector2 pointA = convexHullPoints[i];
                Vector2 pointB = convexHullPoints[(i + 1) % convexHullPoints.Count];
                Vector2 referencePoint = convexHullPoints[(i + 2) % convexHullPoints.Count];

                Inequality inequality = Inequality.GetInequalityFromPoints(pointA, pointB, referencePoint);
                LineRenderer lineRenderer =
                    new GameObject(inequality.GetPrettyInequality()).AddComponent<LineRenderer>();
                lineRenderer.transform.parent = inequalitiesHolder;
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
            
            List<Vector3> toDisplay = new List<Vector3>();
            foreach (Vector2 point in convexHullPoints)
            {
                toDisplay.Add(new Vector3(point.x, point.y));
            }
            toDisplay.Add(new Vector3(convexHullPoints[0].x, convexHullPoints[0].y));

            toDisplay.RemoveAt(toDisplay.Count - 1);
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

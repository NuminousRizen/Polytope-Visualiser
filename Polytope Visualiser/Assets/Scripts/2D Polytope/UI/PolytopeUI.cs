using System.Collections.Generic;
using _2D_Polytope.Util;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _2D_Polytope.UI
{
    public class PolytopeUI : MonoBehaviour
    {
        public Transform pointPrefab;

        public Theme2D theme;
        
        public List<Vector2> points;
        private List<Vector2> convexHullPoints;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                foreach (Transform child in transform) {
                    Destroy(child.gameObject);
                }
                
                Camera camera = Camera.main;
                float height = 2f * camera.orthographicSize;
                float width = height * camera.aspect;
                float buffer = height * .05f;
            
                points = new List<Vector2>();

                int numberOfPoints = Random.Range(3, 51);
                Debug.Log("Making a polytope with " + numberOfPoints + " points.");

                for (int i = 0; i < numberOfPoints; i++)
                {
                    points.Add(new Vector2(Random.Range(-(width / 2) + buffer, (width / 2) - buffer), Random.Range(-(height / 2) + buffer, (height / 2) - buffer)));
                }
            
                BuildPolytope();
            }
        }

        void BuildPolytope()
        {
            // convexHullPoints = GiftWrap.FindConvexHull(points);
            convexHullPoints = GrahamScan.GetConvexHull(points);
            
            foreach (Vector2 point in points)
            {
                Transform circle = Instantiate(pointPrefab, point, transform.rotation);
                circle.parent = transform;
                circle.name = ("x: " + point.x + " ; y: " + point.y);
                circle.GetComponent<SpriteRenderer>().color = convexHullPoints.Contains(point)
                    ? theme.convexHullPointColour
                    : theme.pointColour;
                circle.localScale = Vector3.one * theme.pointSize;
            }
            
            LineRenderer lineRenderer = new GameObject("Lines").AddComponent<LineRenderer>();
            lineRenderer.transform.parent = transform;
            lineRenderer.startColor = theme.lineColour;
            lineRenderer.endColor = theme.lineColour;
            lineRenderer.startWidth = theme.lineSize;
            lineRenderer.endWidth = theme.lineSize;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            List<Vector3> toDisplay = new List<Vector3>();
            foreach (Vector2 point in convexHullPoints)
            {
                toDisplay.Add(new Vector3(point.x, point.y));
            }
            toDisplay.Add(new Vector3(convexHullPoints[0].x, convexHullPoints[0].y));
            lineRenderer.positionCount = toDisplay.Count;
            lineRenderer.SetPositions(toDisplay.ToArray());
            
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

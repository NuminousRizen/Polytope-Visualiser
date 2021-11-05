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

        private void Start()
        {
            Camera camera = Camera.main;
            float height = 2f * camera.orthographicSize;
            float width = height * camera.aspect;
            float buffer = height * .05f;
            
            points = new List<Vector2>();

            int numberOfPoints = Random.Range(2, 51);
            Debug.Log("Making a polytope with " + numberOfPoints + " points.");

            for (int i = 0; i < numberOfPoints; i++)
            {
                points.Add(new Vector2(Random.Range(-(width / 2) + buffer, (width / 2) - buffer), Random.Range(-(height / 2) + buffer, (height / 2) - buffer)));
            }
            BuildPolytope();
        }

        void BuildPolytope()
        {
            convexHullPoints = GiftWrap.FindConvexHull(points);
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

            LineRenderer lineRenderer = new GameObject().AddComponent<LineRenderer>();
            lineRenderer.startColor = theme.lineColour;
            lineRenderer.endColor = theme.lineColour;
            lineRenderer.startWidth = theme.lineSize;
            lineRenderer.endWidth = theme.lineSize;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            List<Vector3> toDiplay = new List<Vector3>();
            foreach (Vector2 point in convexHullPoints)
            {
                toDiplay.Add(new Vector3(point.x, point.y));
            }
            toDiplay.Add(new Vector3(convexHullPoints[0].x, convexHullPoints[0].y));
            
            lineRenderer.positionCount = toDiplay.Count;
            lineRenderer.SetPositions(toDiplay.ToArray());
        }
    }
}

using UnityEngine;

namespace Polytope2D.UI
{
    [CreateAssetMenu(fileName = "New2DTheme", menuName = "2D Theme")]
    public class Theme2D : ScriptableObject
    {
        public Color pointColour;
        public Color convexHullPointColour;
        public Color lineColour;
        public Color polygonColour;
        public float pointSize;
        public float lineSize;

        public Transform pointPrefab;
    }
}
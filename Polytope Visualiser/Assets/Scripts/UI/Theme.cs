using UnityEngine;

namespace UI
{
    [CreateAssetMenu(fileName = "NewTheme", menuName = "Theme")]
    public class Theme : ScriptableObject
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
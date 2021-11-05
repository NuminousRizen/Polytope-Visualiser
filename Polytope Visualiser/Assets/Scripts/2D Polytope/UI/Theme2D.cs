using UnityEngine;

namespace _2D_Polytope.UI
{
    [CreateAssetMenu(fileName = "New2DTheme", menuName = "2D Theme")]
    public class Theme2D : ScriptableObject
    {
        public Color pointColour;
        public Color convexHullPointColour;
        public Color lineColour;
        public float pointSize;
        public float lineSize;
    }
}
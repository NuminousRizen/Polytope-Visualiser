using UnityEngine;

namespace UI.Tooltip
{
    public class TooltipSystem : MonoBehaviour
    {
        private static TooltipSystem current;

        public Tooltip tooltip;

        public void Awake()
        {
            current = this;
        }

        public static void Show(string contentText)
        {
            current.tooltip.SetText(contentText);
            current.tooltip.gameObject.SetActive(true);
        }

        public static void Hide()
        {
            current.tooltip.gameObject.SetActive(false);
        }
    }

}
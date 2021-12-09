using UnityEngine;

namespace UI
{
    public class TooltipSystem : MonoBehaviour
    {
        private static TooltipSystem _current;

        public Tooltip tooltip;

        public void Awake()
        {
            _current = this;
        }

        public static void Show(string contentText)
        {
            _current.tooltip.SetText(contentText);
            _current.tooltip.gameObject.SetActive(true);
        }

        public static void Hide()
        {
            _current.tooltip.gameObject.SetActive(false);
        }
    }

}
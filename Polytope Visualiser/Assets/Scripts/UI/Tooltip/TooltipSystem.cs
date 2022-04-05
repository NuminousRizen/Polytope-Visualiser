using UnityEngine;

namespace UI.Tooltip
{
    /// <summary>
    /// Script that handles the communication between any object trying to use the tool-tip and the actual tool-tip.
    /// </summary>
    public class TooltipSystem : MonoBehaviour
    {
        private static TooltipSystem current;

        public Tooltip tooltip;

        /// <summary>
        /// Event function called by Unity at the start of the application.
        ///
        /// Sets the shared instance to the current object.
        /// </summary>
        public void Awake()
        {
            current = this;
        }

        /// <summary>
        /// Sets some given text to be displayed by the tool-tip and displays the tool-tip.
        /// </summary>
        /// <param name="contentText">The text to be displayed by the tool-tip.</param>
        public static void Show(string contentText)
        {
            current.tooltip.SetText(contentText);
            current.tooltip.gameObject.SetActive(true);
        }

        /// <summary>
        /// Hides the tool-tip.
        /// </summary>
        public static void Hide()
        {
            current.tooltip.gameObject.SetActive(false);
        }
    }

}
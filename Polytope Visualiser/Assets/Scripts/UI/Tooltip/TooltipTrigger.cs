using UnityEngine;

namespace UI.Tooltip
{
    /// <summary>
    /// Script that acts as a trigger to show the tool-tip. The game object to which this component is attached must
    /// have a collider attached as well.
    /// </summary>
    public class TooltipTrigger : MonoBehaviour
    {
        public string toShow;

        /// <summary>
        /// Event function called by Unity when the mouse pointer enters the collider of the game object.
        ///
        /// Displays the tool-tip with the message stored in the "toShow" field.
        /// </summary>
        public void OnMouseEnter()
        {
            TooltipSystem.Show(toShow);
        }

        /// <summary>
        /// Event function called by Unity when the mouse pointer exits the collider of the game object.
        ///
        /// Hides the tool-tip.
        /// </summary>
        public void OnMouseExit()
        {
            TooltipSystem.Hide();
        }
    }
}

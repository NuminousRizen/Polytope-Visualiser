using UnityEngine;

namespace UI.Tooltip
{
    public class TooltipTrigger : MonoBehaviour
    {
        public string toShow;

        public void OnMouseEnter()
        {
            TooltipSystem.Show(toShow);
        }

        public void OnMouseExit()
        {
            TooltipSystem.Hide();
        }
    }
}

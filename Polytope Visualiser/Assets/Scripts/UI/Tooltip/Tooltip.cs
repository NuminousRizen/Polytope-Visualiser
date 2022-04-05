using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tooltip
{
    /// <summary>
    /// Script that enables the behaviour of the tool-tip. 
    /// </summary>
    public class Tooltip : MonoBehaviour
    {
        public TextMeshProUGUI content;
        public LayoutElement layoutElement;

        private RectTransform _rectTransform;

        private float _characterWrap;

        /// <summary>
        /// Event function called by Unity at the start of the application.
        ///
        /// Gets the information about the relevant sub-components.
        /// </summary>
        private void Awake()
        {
            _characterWrap = layoutElement.preferredWidth;
            _rectTransform = GetComponent<RectTransform>();
        }

        /// <summary>
        /// Event function called by Unity at every frame.
        ///
        /// If the tool-tip is enabled, moves the tool-tip to the location of the mouse pointer.
        /// </summary>
        private void Update()
        {
            if (!enabled) return;
            Vector2 mousePosition = Input.mousePosition;
            float pivotX = mousePosition.x / Screen.width;
            _rectTransform.pivot = new Vector2(pivotX, 0);
            transform.position = mousePosition;
        }

        /// <summary>
        /// Sets some given text to be displayed by the tool-tip.
        /// </summary>
        /// <param name="contentText">The text to be displayed by the tool-tip.</param>
        public void SetText(string contentText)
        {
            content.text = contentText;
            layoutElement.enabled = content.text.Length > _characterWrap;
        }
    }

}

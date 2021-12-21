using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tooltip
{
    public class Tooltip : MonoBehaviour
    {
        public TextMeshProUGUI content;
        public LayoutElement layoutElement;

        private RectTransform _rectTransform;

        private float _characterWrap;

        private void Awake()
        {
            _characterWrap = layoutElement.preferredWidth;
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            Vector2 mousePosition = Input.mousePosition;
            float pivotX = mousePosition.x / Screen.width;
            _rectTransform.pivot = new Vector2(pivotX, 0);
            transform.position = mousePosition;
        }

        public void SetText(string contentText)
        {
            content.text = contentText;
            layoutElement.enabled = content.text.Length > _characterWrap;
        }
    }

}

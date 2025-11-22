using UnityEngine;

namespace GameDevTV.UI.Components
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField]
        private RectTransform mask;
        private RectTransform maskParentRectTransform;

        [SerializeField]
        private Vector2 padding;

        private void Awake()
        {
            if (mask == null)
            {
                Debug.LogError("mask is missing");
                return;
            }
            maskParentRectTransform = mask.parent.GetComponent<RectTransform>();
        }

        public void SetProgress(float progress)
        {
            Vector2 parentSize = maskParentRectTransform.sizeDelta;
            Vector2 targetSize = parentSize - padding * 2;
            targetSize.x *= Mathf.Clamp01(progress);

            mask.offsetMin = padding;
            mask.offsetMax = new Vector2(padding.x + targetSize.x - parentSize.x, -padding.y);
        }
    }
}

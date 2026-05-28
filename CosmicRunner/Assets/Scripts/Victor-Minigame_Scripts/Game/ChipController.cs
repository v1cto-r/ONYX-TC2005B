using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

namespace AB {
    public class ChipController : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private Canvas canvas;
        public TMP_Text chipLabel;
        private Image borderImage;
        private Transform startParent;
        private Vector2 startAnchoredPosition;

        public int FragmentOrder { get; set; }
        public string FragmentText { get; set; } = string.Empty;
        private bool isPlaced;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            SetupBorderLayer();
            canvas = FindAnyObjectByType<Canvas>();
            startParent = transform.parent;
            startAnchoredPosition = rectTransform.anchoredPosition;
        }

        public void SetFragment(string fragmentText, int fragmentOrder)
        {
            FragmentText = fragmentText ?? string.Empty;
            FragmentOrder = fragmentOrder;
            chipLabel.gameObject.SetActive(true);
            chipLabel.text = FragmentText;
            SetBorderColor(null);
        }

        public void MarkCorrect(Transform slotTransform)
        {
            isPlaced = true;

            if (slotTransform != null)
            {
                transform.SetParent(slotTransform, false);
                rectTransform.anchoredPosition = Vector2.zero;
            }

            SetBorderColor(Color.green);
            RepairController.Instance.ChipPlaced();
        }

        public void MarkIncorrect()
        {
            isPlaced = false;
            ReturnToStartPosition();
            SetBorderColor(Color.red);
        }

        public void ReturnToStartPosition()
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            transform.SetParent(startParent, false);
            rectTransform.anchoredPosition = startAnchoredPosition;
        }

        private void SetupBorderLayer()
        {
            Transform existing = transform.Find("Border");
            if (existing != null)
            {
                borderImage = existing.GetComponent<Image>();
                borderImage.gameObject.SetActive(false);
                return;
            }

            GameObject borderObject = new GameObject("Border", typeof(RectTransform), typeof(Image));
            borderObject.transform.SetParent(transform, false);
            borderObject.transform.SetAsFirstSibling();

            RectTransform borderRect = borderObject.GetComponent<RectTransform>();
            borderRect.anchorMin = Vector2.zero;
            borderRect.anchorMax = Vector2.one;
            borderRect.offsetMin = new Vector2(-6f, -6f);
            borderRect.offsetMax = new Vector2(6f, 6f);

            borderImage = borderObject.GetComponent<Image>();
            borderImage.raycastTarget = false;
            borderImage.color = Color.clear;
            borderImage.gameObject.SetActive(false);
        }

        private void SetBorderColor(Color? color)
        {
            if (borderImage == null)
            {
                return;
            }

            if (color.HasValue)
            {
                borderImage.color = color.Value;
                borderImage.gameObject.SetActive(true);
                return;
            }

            borderImage.gameObject.SetActive(false);
        }

        // Do not remove it errors out //
        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            isPlaced = false;
            canvasGroup.alpha = 0.8f;
            canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (rectTransform == null)
            {
                return;
            }

            float scaleFactor = 1f;
            if (canvas != null && canvas.scaleFactor > 0f)
            {
                scaleFactor = canvas.scaleFactor;
            }

            rectTransform.anchoredPosition += eventData.delta / scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
            }

            if (!isPlaced)
            {
                ReturnToStartPosition();
                RepairController.Instance.ChipPlaced();
            }
        }
    }
}
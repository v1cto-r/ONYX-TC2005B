using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

namespace AB {
    public class ChipController : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        // Display y posicionamiento
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private Canvas canvas;
        public TMP_Text chipLabel;
        public Image feedbackImage;
        private Transform startParent;
        private Vector2 startAnchoredPosition;

        // Los datos del fragmento que representa el chip
        public int FragmentOrder { get; set; }
        public string FragmentText { get; set; } = string.Empty;
        // Si el chip se colocó correctamente en un slot
        private bool isPlaced;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            canvas = FindAnyObjectByType<Canvas>();
            startParent = transform.parent;
            startAnchoredPosition = rectTransform.anchoredPosition;
        }

        // Asigna el fragmento de texto y su orden al chip, y lo muestra
        public void SetFragment(string fragmentText, int fragmentOrder)
        {
            FragmentText = fragmentText ?? string.Empty;
            FragmentOrder = fragmentOrder;
            chipLabel.gameObject.SetActive(true);
            chipLabel.text = FragmentText;
            feedbackImage.gameObject.SetActive(false);
        }

        // Feedback de colocar el chip en el slot correcto
        public void MarkCorrect(Transform slotTransform)
        {
            isPlaced = true;
            transform.SetParent(slotTransform, false);
            rectTransform.anchoredPosition = Vector2.zero;
            feedbackImage.color = new Color(0f, 1f, 0f, 0.1f);
            feedbackImage.gameObject.SetActive(true);
            RepairController.Instance.ChipPlaced();
        }

        // Feedback de colocar el chip en un lugar incorrecto
        public void MarkIncorrect()
        {
            isPlaced = false;
            ReturnToStartPosition();
            feedbackImage.color = new Color(1f, 0f, 0f, 0.1f);
            feedbackImage.gameObject.SetActive(true);
        }

        // Regresa el chip a su posición inicial
        public void ReturnToStartPosition()
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            transform.SetParent(startParent, false);
            rectTransform.anchoredPosition = startAnchoredPosition;
        }

        // Do not remove it errors out //
        public void OnPointerDown(PointerEventData eventData)
        {
        }

        // Feedback de arrastrar el chip
        public void OnBeginDrag(PointerEventData eventData)
        {
            isPlaced = false;
            canvasGroup.alpha = 0.8f;
            canvasGroup.blocksRaycasts = false;
        }

        // Mueve el chip con el mouse
        public void OnDrag(PointerEventData eventData)
        {
            float scaleFactor = canvas != null && canvas.scaleFactor > 0f ? canvas.scaleFactor : 1f;
            rectTransform.anchoredPosition += eventData.delta / scaleFactor;
        }

        // Si no se colocó en un slot, regresa a la posición inicial
        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;

            if (!isPlaced)
            {
                ReturnToStartPosition();
                RepairController.Instance.ChipPlaced();
            }
        }
    }
}

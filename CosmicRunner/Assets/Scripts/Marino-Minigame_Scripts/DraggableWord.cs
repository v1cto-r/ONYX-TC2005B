using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class DraggableWord : MonoBehaviour
{
    [SerializeField] private TMP_Text wordLabel;
    [SerializeField] private Transform dragTarget;
    [SerializeField] private bool snapBackOnRelease = true;
    [SerializeField] private Camera worldCamera;
    [SerializeField] private bool initializeFromLabelText = false;

    private InputAction dragAction;
    private InputAction pointerAction;
    private Collider2D ownCollider;
    private bool isDragging;
    private Vector3 dragStartPosition;
    private Vector3 dragOffset;

    public bool HasWord { get; private set; }
    public string CurrentWord { get; private set; } = string.Empty;

    private void Awake()
    {
        if (wordLabel == null)
        {
            wordLabel = GetComponentInChildren<TMP_Text>();
        }

        if (dragTarget == null)
        {
            dragTarget = transform;
        }

        if (worldCamera == null)
        {
            worldCamera = Camera.main;
        }

        ownCollider = GetComponent<Collider2D>();

        ResolveActions();
    }

    private void OnEnable()
    {
        ResolveActions();

        if (initializeFromLabelText)
        {
            SyncWordFromLabelIfNeeded();
        }

        if (dragAction != null)
        {
            dragAction.Enable();
        }

        if (pointerAction != null)
        {
            pointerAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (dragAction != null)
        {
            dragAction.Disable();
        }

        if (pointerAction != null)
        {
            pointerAction.Disable();
        }

        isDragging = false;
    }

    private void Update()
    {
        if (dragAction == null || pointerAction == null || worldCamera == null)
        {
            return;
        }

        Vector3 pointerWorld = GetPointerWorldPosition();

        if (!isDragging && dragAction.WasPressedThisFrame())
        {
            TryBeginDrag(pointerWorld);
        }

        if (isDragging && dragAction.IsPressed())
        {
            dragTarget.position = pointerWorld + dragOffset;
        }

        if (isDragging && dragAction.WasReleasedThisFrame())
        {
            EndDrag();
        }
    }

    public void SetWord(string word)
    {
        CurrentWord = word;
        HasWord = !string.IsNullOrWhiteSpace(word);

        if (wordLabel != null)
        {
            wordLabel.text = HasWord ? word : string.Empty;
        }

        if (PromptsControl.Instance != null)
        {
            PromptsControl.Instance.NotifyWordSlotsChanged();
        }
    }

    public void ClearWord()
    {
        SetWord(string.Empty);
    }

    private void TryBeginDrag(Vector3 pointerWorld)
    {
        if (!CanStartDragging(out string blockedReason))
        {
            return;
        }

        Collider2D[] hits = Physics2D.OverlapPointAll(pointerWorld);
        if (hits == null || hits.Length == 0)
        {
            return;
        }

        bool ownsAnyHit = false;
        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];
            bool isOwnHit = hit == ownCollider || hit.transform.IsChildOf(transform);

            if (isOwnHit)
            {
                ownsAnyHit = true;
            }
        }

        if (!ownsAnyHit)
        {
            return;
        }

        isDragging = true;
        dragStartPosition = dragTarget.position;
        dragOffset = dragTarget.position - pointerWorld;
    }

    private bool CanStartDragging(out string blockedReason)
    {
        if (!HasWord)
        {
            blockedReason = "HasWord is false (slot is empty).";
            return false;
        }

        if (dragTarget == null || ownCollider == null || worldCamera == null)
        {
            blockedReason = "dragTarget, collider, or camera is missing.";
            return false;
        }

        if (dragAction == null)
        {
            blockedReason = $"Drag action 'Drag' was not found.";
            return false;
        }

        if (!dragAction.IsPressed())
        {
            blockedReason = "Drag action is not currently pressed.";
            return false;
        }

        blockedReason = string.Empty;
        return true;
    }

    private Vector3 GetPointerWorldPosition()
    {
        Vector2 pointerPosition = pointerAction.ReadValue<Vector2>();
        Vector3 screenPosition = new Vector3(pointerPosition.x, pointerPosition.y, 0f);
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = dragTarget.position.z;

        return worldPosition;
    }

    private void EndDrag()
    {
        isDragging = false;

        bool wasDroppedIntoBlank = TryDropIntoBlank();

        if (wasDroppedIntoBlank)
        {
            ClearWord();
        }

        if (snapBackOnRelease || wasDroppedIntoBlank)
        {
            dragTarget.position = dragStartPosition;
        }
    }

    private bool TryDropIntoBlank()
    {
        if (!HasWord)
        {
            return false;
        }

        if (PromptsControl.Instance == null)
        {
            return false;
        }

        Vector2 dropPoint = new Vector2(dragTarget.position.x, dragTarget.position.y);
        return PromptsControl.Instance.TryFillBlankAtWorldPoint(dropPoint, CurrentWord, this);
    }

    private void ResolveActions()
    {
        if (dragAction == null)
        {
            dragAction = InputSystem.actions.FindAction("Drag");

            if (dragAction == null)
            {
                Debug.LogWarning($"Input action 'Drag' was not found.", this);
            }
        }

        if (pointerAction == null)
        {
            pointerAction = InputSystem.actions.FindAction("Point");

            if (pointerAction == null)
            {
                Debug.LogWarning($"Input action 'Point' was not found. Drag will use pointer event data instead.", this);
            }
        }
    }

    private void SyncWordFromLabelIfNeeded()
    {
        if (wordLabel == null || HasWord)
        {
            return;
        }

        string labelText = wordLabel.text;
        if (string.IsNullOrWhiteSpace(labelText))
        {
            return;
        }

        CurrentWord = labelText;
        HasWord = true;
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class ShipMenuMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private string moveActionName = "Move";
    [SerializeField] private float rotationOffsetDegrees = 90f;

    [Header("Bounds")]
    [SerializeField] private Camera worldCamera;

    private Rigidbody2D shipRigidbody;
    private InputAction moveAction;
    private Vector2 moveInput;
    private Vector2 boundsPadding;

    private void Awake()
    {
        shipRigidbody = GetComponent<Rigidbody2D>();

        if (worldCamera == null)
        {
            worldCamera = Camera.main;
        }

        moveAction = InputSystem.actions.FindAction(moveActionName);

        if (moveAction == null)
        {
            Debug.LogError($"Input action '{moveActionName}' was not found.", this);
        }

        if (shipRigidbody != null)
        {
            shipRigidbody.gravityScale = 0f;
            shipRigidbody.freezeRotation = false;
        }

        UpdatePaddingFromCollider();
    }

    private void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.Disable();
        }

        moveInput = Vector2.zero;
    }

    private void Update()
    {
        if (moveAction == null)
        {
            return;
        }

        moveInput = moveAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (shipRigidbody == null)
        {
            return;
        }

        Vector2 nextPosition = shipRigidbody.position + moveInput * moveSpeed * Time.fixedDeltaTime;
        shipRigidbody.MovePosition(ClampToCameraBounds(nextPosition));

        if (moveInput.sqrMagnitude > 0.0001f)
        {
            float rotationAngle = Mathf.Atan2(-moveInput.y, -moveInput.x) * Mathf.Rad2Deg + rotationOffsetDegrees;
            shipRigidbody.MoveRotation(rotationAngle);
        }
    }

    private Vector2 ClampToCameraBounds(Vector2 targetPosition)
    {
        if (worldCamera == null)
        {
            return targetPosition;
        }

        if (!worldCamera.orthographic)
        {
            return targetPosition;
        }

        float cameraHeight = worldCamera.orthographicSize;
        float cameraWidth = cameraHeight * worldCamera.aspect;

        Vector3 cameraCenter = worldCamera.transform.position;

        float minX = cameraCenter.x - cameraWidth + boundsPadding.x;
        float maxX = cameraCenter.x + cameraWidth - boundsPadding.x;
        float minY = cameraCenter.y - cameraHeight + boundsPadding.y;
        float maxY = cameraCenter.y + cameraHeight - boundsPadding.y;

        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        return targetPosition;
    }

    private void UpdatePaddingFromCollider()
    {
        Collider2D shipCollider = GetComponent<Collider2D>();
        if (shipCollider != null)
        {
            boundsPadding = shipCollider.bounds.extents;
            return;
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            boundsPadding = spriteRenderer.bounds.extents;
            return;
        }

        boundsPadding = Vector2.zero;
    }
}

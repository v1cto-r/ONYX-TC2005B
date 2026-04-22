using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap collisionTilemap;
    [SerializeField] private float stepRepeatSeconds = 0.15f;
    [SerializeField] private float inputDeadzone = 0.5f;

    private InputAction moveAction;
    private InputAction pullAction;
    private float timeUntilNextStep;
    private bool isMoveHeld;

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        pullAction = InputSystem.actions.FindAction("Pull");

        if (moveAction == null)
        {
            Debug.LogError("Move action was not found in Input System Actions.", this);
        }

        if (pullAction == null)
        {
            Debug.LogError("Pull action was not found in Input System Actions.", this);
        }
    }

    private void OnEnable()
    {
        if (moveAction == null)
        {
            return;
        }

        moveAction.Enable();

        if (pullAction != null)
        {
            pullAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (moveAction == null)
        {
            return;
        }

        moveAction.Disable();

        if (pullAction != null)
        {
            pullAction.Disable();
        }
    }

    private void Update()
    {
        if (moveAction == null)
        {
            return;
        }

        Vector2 moveVector = moveAction.ReadValue<Vector2>();
        Vector2 moveDirection = GetCardinalDirection(moveVector);
        bool isPullButtonHeld = pullAction != null && pullAction.IsPressed();

        if (moveDirection == Vector2.zero)
        {
            isMoveHeld = false;
            timeUntilNextStep = 0f;
            return;
        }

        if (isPullButtonHeld)
        {
            if (moveAction.WasPressedThisFrame())
            {
                TryMoveOneTile(moveDirection);
            }
            return;
        }

        if (!isMoveHeld)
        {
            TryMoveOneTile(moveDirection);
            isMoveHeld = true;
            timeUntilNextStep = stepRepeatSeconds;
            return;
        }

        timeUntilNextStep -= Time.deltaTime;
        if (timeUntilNextStep <= 0f)
        {
            TryMoveOneTile(moveDirection);
            timeUntilNextStep = stepRepeatSeconds;
        }
    }

    private Vector2 GetCardinalDirection(Vector2 inputVector)
    {
        if (inputVector.magnitude < inputDeadzone)
        {
            return Vector2.zero;
        }

        if (Mathf.Abs(inputVector.x) > Mathf.Abs(inputVector.y))
        {
            return new Vector2(Mathf.Sign(inputVector.x), 0f);
        }

        return new Vector2(0f, Mathf.Sign(inputVector.y));
    }

    private void TryMoveOneTile(Vector2 moveDirection)
    {
        Vector3Int frontCell = groundTilemap.WorldToCell(transform.position + (Vector3)moveDirection);
        BoxControl boxInFront = GetBoxAtCell(frontCell);

        bool isPullButtonHeld = pullAction != null && pullAction.IsPressed();
        Vector3Int rearCell = groundTilemap.WorldToCell(transform.position - (Vector3)moveDirection);
        BoxControl boxBehind = isPullButtonHeld ? GetBoxAtCell(rearCell) : null;

        if (!CanPlayerMove(moveDirection, boxInFront, boxBehind))
        {
            return;
        }

        if (boxInFront != null)
        {
            boxInFront.MoveOneTile(moveDirection, groundTilemap, collisionTilemap);
        }

        transform.position += (Vector3)moveDirection;

        if (boxBehind != null)
        {
            boxBehind.MoveOneTile(moveDirection, groundTilemap, collisionTilemap);
        }
    }

    private bool CanPlayerMove(Vector2 moveDirection, BoxControl boxInFront, BoxControl boxBehind)
    {
        Vector3Int nextPlayerCell = groundTilemap.WorldToCell(transform.position + (Vector3)moveDirection);

        if (!groundTilemap.HasTile(nextPlayerCell) || collisionTilemap.HasTile(nextPlayerCell))
        {
            return false;
        }

        if (boxInFront != null && !boxInFront.CanMoveOneTile(moveDirection, groundTilemap, collisionTilemap))
        {
            return false;
        }

        if (boxBehind != null && !boxBehind.CanMoveOneTile(moveDirection, groundTilemap, collisionTilemap))
        {
            return false;
        }

        return true;
    }

    private BoxControl GetBoxAtCell(Vector3Int cell)
    {
        Vector3 cellCenterWorld = groundTilemap.GetCellCenterWorld(cell);
        Collider2D[] collidersAtPoint = Physics2D.OverlapPointAll(cellCenterWorld);

        foreach (Collider2D colliderAtPoint in collidersAtPoint)
        {
            if (!colliderAtPoint.CompareTag("Box"))
            {
                continue;
            }

            if (colliderAtPoint.TryGetComponent<BoxControl>(out BoxControl boxControl))
            {
                return boxControl;
            }
        }

        return null;
    }
}

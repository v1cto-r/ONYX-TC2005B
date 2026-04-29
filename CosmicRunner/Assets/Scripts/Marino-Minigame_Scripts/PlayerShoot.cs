using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private PlayerControl playerControl;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float muzzleOffset = 0.35f;

    [SerializeField] private string shootActionName = "Shoot";
    private InputAction shootAction;

    private void Awake()
    {
        if (playerControl == null)
        {
            playerControl = GetComponentInParent<PlayerControl>();
        }

        ResolveInputAction();

        if (shootAction == null)
        {
            Debug.LogError($"Shoot action was not found in Input System Actions using '{shootActionName}'.", this);
        }
    }

    private void OnEnable()
    {
        if (shootAction == null)
        {
            ResolveInputAction();
        }

        shootAction?.Enable();
    }

    private void OnDisable()
    {
        shootAction?.Disable();
    }

    private void ResolveInputAction()
    {
        if (shootAction == null)
        {
            shootAction = InputSystem.actions.FindAction(shootActionName);
        }
    }

    private void Update()
    {
        if (shootAction == null || playerControl == null)
        {
            return;
        }

        if (shootAction.WasPressedThisFrame())
        {
            FirePlasmaShot();
        }
    }

    private void LateUpdate()
    {
        if (playerControl == null)
        {
            return;
        }

        Vector2 facingDirection = GetFacingDirection();
        Vector3 offset = (Vector3)facingDirection * muzzleOffset;
        transform.position = playerControl.transform.position + offset;
        transform.rotation = Quaternion.Euler(0f, 0f, GetZRotationForDirection(facingDirection));
    }

    private void FirePlasmaShot()
    {
        Vector2 facingDirection = GetFacingDirection();

        if (bulletPrefab == null)
        {
            Debug.LogWarning("PlayerShoot is missing a Bullet Prefab reference.", this);
            return;
        }

        Instantiate(
            bulletPrefab,
            transform.position,
            Quaternion.Euler(0f, 0f, GetZRotationForDirection(facingDirection))
        );
    }

    private Vector2 GetFacingDirection()
    {
        if (playerControl == null)
        {
            return Vector2.down;
        }

        Vector2 facingDirection = playerControl.FacingDirection;
        return facingDirection == Vector2.zero ? Vector2.down : facingDirection;
    }

    private float GetZRotationForDirection(Vector2 facingDirection)
    {
        if (facingDirection == Vector2.right)
        {
            return 0f;
        }

        if (facingDirection == Vector2.up)
        {
            return 90f;
        }

        if (facingDirection == Vector2.left)
        {
            return 180f;
        }

        return -90f;
    }
}

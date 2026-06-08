using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AB {

    [System.Serializable]
    public struct AngleBounds
    {
        public float lowerBound;
        public float upperBound;
    }


    public class ShipControlller : MonoBehaviour
    {

        private GameObject ship;
        private SpriteRenderer spriteRenderer;
        public GameObject explosion;
        private float lookAtAngle;

        [Header("Positioning")]

        public float radius = 17f;
        public GameObject originObject;
        private Vector2 origin;
        private float angle;
        private float newAngle;
        public float initialAngleRadians = 2.8f;
        public AngleBounds angleLimits = new AngleBounds { lowerBound = 2.5f, upperBound = 3.1416f };
        private Vector2 position;
        private Vector2 previousPosition;


        [Header("Movement")]
        public float maxAngularVelocity = 1f;
        public float angularAcceleration = 1f;
        public float drag = 0.1f;
        private float angularVelocity;
        private InputAction moveAction;
        private Vector2 shipVelocity2D;

        [Header("Shooter")]
        public GameObject bulletStoreContainer;
        public GameObject bulletPrefab;
        public float bulletOffset = 0.5f;
        public float shootCooldown = 0.5f;
        private InputAction shootAction;

        [Header("Shield")]
        public GameObject shieldObject;
        private bool shieldActive = false;

        void Awake()
        {
            ship = this.gameObject;

            origin = originObject.transform.position;

            moveAction = InputSystem.actions.FindAction("Move");
            shootAction = InputSystem.actions.FindAction("Shoot");

        if (moveAction == null)
        {
            Debug.LogError("Move action was not found in Input System Actions.", this);
        }

        if (shootAction == null)
        {
            Debug.LogError("Shoot action was not found in Input System Actions.", this);
        }

            position = CalculateShipPosition(angle);
            ship.transform.position = position;
        }

        private void OnEnable()
        {
            if (moveAction == null)
            {
                moveAction = InputSystem.actions.FindAction("Move");
            }

            if (shootAction == null)
            {
                shootAction = InputSystem.actions.FindAction("Shoot");
            }

            moveAction?.Enable();
            shootAction?.Enable();
        }

        private void OnDisable()
        {
            moveAction?.Disable();
            shootAction?.Disable();
        }

        void Update()
        {
            if (shootAction != null && shootAction.WasPressedThisFrame())
            {
                if (GameController.Instance.GetBullets() > 0)
                {
                    GameController.Instance.SpendBullet();

                    SFXGameController.Instance.PlayShootSound();

                    Instantiate(
                        bulletPrefab, 
                        position, 
                        Quaternion.Euler(0f, 0f, lookAtAngle * Mathf.Rad2Deg), 
                        bulletStoreContainer.transform
                        );
                }
            }
        }

        public float GetShipAngle()
        {
            return angle;
        }

        void FixedUpdate()
        {
            float input = 0f;
        if (moveAction != null)
        {
            input = moveAction.ReadValue<Vector2>().y;
        }

            Debug.Log("Input: " + input);

            if (input != 0f)
            {
                SFXGameController.Instance.PlayShipMoveSound();
                float targetVelocity = input * maxAngularVelocity;
                angularVelocity = Mathf.MoveTowards(angularVelocity, targetVelocity, angularAcceleration * Time.fixedDeltaTime);
            }
            else
            {
                SFXGameController.Instance.StopShipMoveSound();
                angularVelocity = Mathf.MoveTowards(angularVelocity, 0f, drag * Time.fixedDeltaTime);
            }

            float proposedAngle = newAngle - angularVelocity * Time.fixedDeltaTime;
            float clampedAngle = Mathf.Clamp(proposedAngle, angleLimits.lowerBound, angleLimits.upperBound);

            if (!Mathf.Approximately(clampedAngle, proposedAngle))
            {
                if ((clampedAngle <= angleLimits.lowerBound && angularVelocity > 0f) ||
                    (clampedAngle >= angleLimits.upperBound && angularVelocity < 0f))
                {
                    angularVelocity = 0f;
                }
            }

            newAngle = clampedAngle;
            lookAtAngle = newAngle + Mathf.PI / 2f;
            ship.transform.rotation = Quaternion.Euler(0f, 0f, lookAtAngle * Mathf.Rad2Deg);

            if (Mathf.Approximately(newAngle, angle)) return;

            angle = newAngle;
            previousPosition = position;
            position = CalculateShipPosition(angle);
            ship.transform.position = position;
            shipVelocity2D = (position - previousPosition) / Time.fixedDeltaTime;
        }

        Vector2 CalculateShipPosition(float angle)
        {
            Vector2 newPosition;

            newPosition.x = origin.x + Mathf.Cos(angle) * radius;
            newPosition.y = origin.y + Mathf.Sin(angle) * radius;
            return newPosition;
        }

        public void EnableShield()
        {
            shieldActive = true;
            shieldObject.SetActive(true);
        }

        public void DisableShield()
        {
            shieldActive = false;
            shieldObject.SetActive(false);
            GameController.Instance.LooseShield();
        }

        public bool getShieldActive()
        {
            return shieldActive;
        }

        public void TakeDamage()
        {
            if (shieldActive)
            {
                DisableShield();
                return;
            }
            SFXGameController.Instance.PlayShipDestroyedSound();
            explosion.SetActive(true);

            StartCoroutine(DieWithDelay());
        }

        private IEnumerator DieWithDelay()
        {
            yield return new WaitForSeconds(0.5f);
            GameController.Instance.EndGame();
        }

        // Soft damage muestra reparar
        public void TakeSoftDamage()
        {
            if (shieldActive)
            {
                DisableShield();
                return;
            }
            
            GameController.Instance.HandleRepair();
        }
    }
}
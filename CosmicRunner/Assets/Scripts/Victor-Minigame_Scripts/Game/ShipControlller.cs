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

        // Render related
        private GameObject ship;
        private SpriteRenderer spriteRenderer;
        public GameObject explosion;
        private float lookAtAngle;

        // The movement of the ship variables, done in the circumference of a circle
        [Header("Positioning")]

        public float radius = 17f;
        public GameObject originObject;
        private Vector2 origin;
        private float angle;
        private float newAngle;
        public float initialAngleRadians = 2.8f;
        // Upper bound first, then lower bound, in radians
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
            // El gameObject al cual se asignó el script (La nave)
            ship = this.gameObject;

            // El origen de rotación, en este caso se le pasa el planeta y se usa su posición
            origin = originObject.transform.position;

            // Del input system, la acción de movimiento, usada en todos los otros juegos
            moveAction = InputSystem.actions.FindAction("Move");
            shootAction = InputSystem.actions.FindAction("Shoot");

            // Inicializar la rotación
            angle = newAngle = initialAngleRadians;

            // Poner la nave en la posición inicial, usando el ángulo inicial
            position = CalculateShipPosition(angle);
            ship.transform.position = position;
        }

        void Update()
        {
            if (shootAction.WasPressedThisFrame())
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
            // Solo utilizamos el movimiento vertical del input
            float input = moveAction.ReadValue<Vector2>().y;

            // Si hay movimiento, acelerar hacia la velocidad
            if (input != 0f)
            {
                // Input (0-1) * velocidad para sacar la velocidad target
                SFXGameController.Instance.PlayShipMoveSound();
                float targetVelocity = input * maxAngularVelocity;
                // Calcular la nueva velocidad, acelerando hacia la velocidad target
                angularVelocity = Mathf.MoveTowards(angularVelocity, targetVelocity, angularAcceleration * Time.fixedDeltaTime);
            }
            else
            {
                SFXGameController.Instance.StopShipMoveSound();
                // Si no hay input, desacelerar hacia 0, en base al drag
                angularVelocity = Mathf.MoveTowards(angularVelocity, 0f, drag * Time.fixedDeltaTime);
            }

            // El nuevo ángulo calculado en base a la velocidad
            float proposedAngle = newAngle - angularVelocity * Time.fixedDeltaTime;
            // Limitar el ángulo a los límites, para que la nave no se salga de la pantalla
            float clampedAngle = Mathf.Clamp(proposedAngle, angleLimits.lowerBound, angleLimits.upperBound);

            // Si la nave tocó el límite, detener la aceleración
            if (!Mathf.Approximately(clampedAngle, proposedAngle))
            {
                if ((clampedAngle <= angleLimits.lowerBound && angularVelocity > 0f) ||
                    (clampedAngle >= angleLimits.upperBound && angularVelocity < 0f))
                {
                    angularVelocity = 0f;
                }
            }

            // Actualizar el ángulo, y la rotación de la nave para voltear a ver el planeta
            newAngle = clampedAngle;
            lookAtAngle = newAngle + Mathf.PI / 2f;
            ship.transform.rotation = Quaternion.Euler(0f, 0f, lookAtAngle * Mathf.Rad2Deg);

            // Si el ángulo no cambió, no calcular la posición
            if (Mathf.Approximately(newAngle, angle)) return;

            // Actualizar el ángulo y la posición de la nave
            angle = newAngle;
            previousPosition = position;
            position = CalculateShipPosition(angle);
            ship.transform.position = position;
            shipVelocity2D = (position - previousPosition) / Time.fixedDeltaTime;
        }

        // Formula para calcular la posición de la nave en base al ángulo, usando trigonometría
        Vector2 CalculateShipPosition(float angle)
        {
            // Nueva posición
            Vector2 newPosition;
            // Calcular componentes x e y, usando coseno y seno respectivamente
            // Creciendo la distancia con el radio
            // Y usando la posición del origen como offset
            newPosition.x = origin.x + Mathf.Cos(angle) * radius;
            newPosition.y = origin.y + Mathf.Sin(angle) * radius;
            return newPosition;
        }

        // Maneja activar y desactivar el escudo
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

        // Damage termina el juego
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

        // Se espera a morir para mostrar la explosión
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
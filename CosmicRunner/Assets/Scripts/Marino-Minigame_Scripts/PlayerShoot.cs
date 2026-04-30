using UnityEngine;
using UnityEngine.InputSystem;

namespace MECS
{
    // Maneja disparo del jugador con cooldown
    public class PlayerShoot : MonoBehaviour
    {
        // Referencia al control de movimiento para saber hacia donde dispara
        [SerializeField] private PlayerControl playerControl;
        // Prefab de la bala o proyectil que se va a instanciar
        [SerializeField] private GameObject bulletPrefab;
        // Distancia visual del arma respecto al jugador
        [SerializeField] private float muzzleOffset = 0.35f;
        // Tiempo minimo entre disparos
        [SerializeField] private float shootCooldown = 0.5f;

        // Nombre de la accion de input que dispara el arma
        [SerializeField] private string shootActionName = "Shoot";
        // Accion de entrada que detecta el disparo
        private InputAction shootAction;
        // Tiempo a partir del cual se permite disparar otra vez
        private float nextShootTime;

        // Busca referencias y valida que la accion de input exista
        private void Awake()
        {
            // Si no se asigno el control, lo tomamos del padre del arma
            if (playerControl == null)
            {
                playerControl = GetComponentInParent<PlayerControl>();
            }

            // Buscamos la accion de disparo una sola vez
            ResolveInputAction();

            // Si no existe la accion, dejamos un error claro para el editor
            if (shootAction == null)
            {
                Debug.LogError($"Shoot action was not found in Input System Actions using '{shootActionName}'.", this);
            }
        }

        // Habilita la accion cuando el objeto entra en uso
        private void OnEnable()
        {
            // Si aun no esta resuelta, la buscamos otra vez
            if (shootAction == null)
            {
                ResolveInputAction();
            }

            // Activamos el input para poder disparar
            shootAction?.Enable();
        }

        // Desactiva la accion cuando el objeto deja de usarse
        private void OnDisable()
        {
            shootAction?.Disable();
        }

        // Intenta resolver la accion de disparo desde el asset de input
        private void ResolveInputAction()
        {
            // Solo hacemos la busqueda si aun no la tenemos en memoria
            if (shootAction == null)
            {
                shootAction = InputSystem.actions.FindAction(shootActionName);
            }
        }

        // Revisa cada frame si el jugador quiere disparar y si el cooldown lo permite
        private void Update()
        {
            // Sin control de jugador o sin input valido no hacemos nada
            if (shootAction == null || playerControl == null)
            {
                return;
            }

            // Solo disparamos si el boton se presiono este frame y el cooldown ya termino
            if (shootAction.WasPressedThisFrame() && Time.time >= nextShootTime)
            {
                FirePlasmaShot();
                nextShootTime = Time.time + shootCooldown;
            }
        }

        // Reubica el arma en la posicion del jugador en la capa visual correcta
        private void LateUpdate()
        {
            // Si no hay jugador, no podemos seguir su posicion
            if (playerControl == null)
            {
                return;
            }

            // Calculamos la direccion a partir de la que se debe dibujar el arma
            Vector2 facingDirection = GetFacingDirection();
            // Desplazamos el arma un poco para que no quede encima del jugador
            Vector3 offset = (Vector3)facingDirection * muzzleOffset;
            transform.position = playerControl.transform.position + offset;
            transform.rotation = Quaternion.Euler(0f, 0f, GetZRotationForDirection(facingDirection));
        }

        // Crea la bala con la orientacion correcta
        private void FirePlasmaShot()
        {
            // La bala se orienta segun la direccion actual del jugador
            Vector2 facingDirection = GetFacingDirection();

            // Si no hay prefab, no podemos crear el disparo
            if (bulletPrefab == null)
            {
                Debug.LogWarning("PlayerShoot is missing a Bullet Prefab reference.", this);
                return;
            }

            // Instanciamos la bala en la posicion actual del arma
            Instantiate(
                bulletPrefab,
                transform.position,
                Quaternion.Euler(0f, 0f, GetZRotationForDirection(facingDirection))
            );
        }

        // Obtiene la direccion actual del jugador o una direccion segura por defecto
        private Vector2 GetFacingDirection()
        {
            // Si no hay jugador, apuntamos hacia abajo para no romper la rotacion
            if (playerControl == null)
            {
                return Vector2.down;
            }

            // Si la direccion es cero, devolvemos una direccion por defecto valida
            Vector2 facingDirection = playerControl.FacingDirection;
            return facingDirection == Vector2.zero ? Vector2.down : facingDirection;
        }

        // Convierte una direccion cardinal a rotacion Z
        private float GetZRotationForDirection(Vector2 facingDirection)
        {
            // Derecha: rotacion base
            if (facingDirection == Vector2.right)
            {
                return 0f;
            }

            // Arriba: 90 grados
            if (facingDirection == Vector2.up)
            {
                return 90f;
            }

            // Izquierda: 180 grados
            if (facingDirection == Vector2.left)
            {
                return 180f;
            }

            // Abajo: valor por defecto
            return -90f;
        }
    }
}

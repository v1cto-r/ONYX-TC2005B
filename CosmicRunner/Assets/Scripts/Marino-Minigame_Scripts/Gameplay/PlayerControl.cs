using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace MECS
{
    // Maneja input y movimiento del jugador en el tilemap, incluyendo empujar y tirar cajas
    public class PlayerControl : MonoBehaviour
    {
        // Mapa del suelo para validar el paso del jugador
        [SerializeField] private Tilemap groundTilemap;
        // Mapa de colisiones que bloquea el movimiento
        [SerializeField] private Tilemap collisionTilemap;
        // Tiempo entre pasos continuos al mantener input
        [SerializeField] private float stepRepeatSeconds = 0.15f;
        // Zona muerta para ignorar input muy pequeno
        [SerializeField] private float inputDeadzone = 0.5f;

        // Componentes de animacion del jugador
        private Animator animator;
        private SpriteRenderer spriteRenderer;

        // Accion de movimiento del sistema de input
        private InputAction moveAction;
        // Accion para empujar o arrastrar cajas
        private InputAction pullAction;
        // Temporizador entre pasos automáticos al mantener pulsado
        private float timeUntilNextStep;
        // Marca si el boton de movimiento sigue mantenido
        private bool isMoveHeld;
        // Direccion cardinal a la que esta mirando el jugador
        private Vector2 facingDirection = Vector2.down;

        // Direccion actual del jugador para otros sistemas
        public Vector2 FacingDirection => facingDirection;
        // Exponemos el mapa del suelo para otras mecanicas
        public Tilemap GroundTilemap => groundTilemap;
        // Exponemos el mapa de colision para otras mecanicas
        public Tilemap CollisionTilemap => collisionTilemap;

        // Busca acciones de input y valida la configuracion
        private void Awake()
        {
            // Buscamos el Animator del jugador para cambiar animaciones
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            // Cargamos las acciones desde el Input System global
            moveAction = InputSystem.actions.FindAction("Move");
            pullAction = InputSystem.actions.FindAction("Pull");

            // Informamos si falta la accion principal de movimiento
            if (moveAction == null)
            {
                Debug.LogError("Move action was not found in Input System Actions.", this);
            }

            // Informamos si falta la accion de empuje
            if (pullAction == null)
            {
                Debug.LogError("Pull action was not found in Input System Actions.", this);
            }
        }

        // Enciende las acciones cuando el jugador esta activo
        private void OnEnable()
        {
            if (moveAction == null)
            {
                return;
            }

            // Activamos movimiento y, si existe, tambien pull
            moveAction.Enable();

            if (pullAction != null)
            {
                pullAction.Enable();
            }
        }

        // Apaga las acciones para no seguir leyendo input fuera de escena
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

        // Reinicia estado interno e input actions tras pausar/reanudar para evitar input bloqueado
        public void RefreshInputAfterPause()
        {
            isMoveHeld = false;
            timeUntilNextStep = 0f;

            if (moveAction != null)
            {
                moveAction.Disable();
                moveAction.Enable();
            }

            if (pullAction != null)
            {
                pullAction.Disable();
                pullAction.Enable();
            }
        }

        // Lee el input y decide si el jugador puede dar un paso
        private void Update()
        {
            // Sin accion de movimiento no hay nada que procesar
            if (moveAction == null)
            {
                return;
            }

            // Leemos el vector del stick, teclado o cruceta
            Vector2 moveVector = moveAction.ReadValue<Vector2>();
            // Lo reducimos a una direccion cardinal simple
            Vector2 moveDirection = GetCardinalDirection(moveVector);
            // El pull cambia como interpretamos el siguiente movimiento
            bool isPullButtonHeld = pullAction != null && pullAction.IsPressed();

            // La ultima direccion valida tambien actualiza hacia donde mira el jugador
            if (moveDirection != Vector2.zero)
            {
                if (!isPullButtonHeld)
                {
                    facingDirection = moveDirection;
                    UpdateAnimation(moveDirection);
                }
            }

            // Si no hay input, reiniciamos el estado de paso continuo
            if (moveDirection == Vector2.zero)
            {
                isMoveHeld = false;
                timeUntilNextStep = 0f;
                return;
            }

            // Si estamos tirando de cajas, solo damos un paso por pulsacion
            if (isPullButtonHeld)
            {
                if (moveAction.WasPressedThisFrame())
                {
                    TryMoveOneTile(moveDirection);
                }
                return;
            }

            // Primer frame de pulsacion: damos un paso inmediato
            if (!isMoveHeld)
            {
                TryMoveOneTile(moveDirection);
                isMoveHeld = true;
                timeUntilNextStep = stepRepeatSeconds;
                return;
            }

            // Cuando se mantiene pulsado, repetimos pasos con ritmo fijo
            timeUntilNextStep -= Time.deltaTime;
            if (timeUntilNextStep <= 0f)
            {
                TryMoveOneTile(moveDirection);
                timeUntilNextStep = stepRepeatSeconds;
            }
        }

        // Convierte cualquier entrada en una direccion arriba, abajo, izquierda o derecha
        private Vector2 GetCardinalDirection(Vector2 inputVector)
        {
            // Si el input es muy pequeno, lo ignoramos
            if (inputVector.magnitude < inputDeadzone)
            {
                return Vector2.zero;
            }

            // Elegimos el eje dominante para evitar diagonales
            if (Mathf.Abs(inputVector.x) > Mathf.Abs(inputVector.y))
            {
                return new Vector2(Mathf.Sign(inputVector.x), 0f);
            }

            return new Vector2(0f, Mathf.Sign(inputVector.y));
        }

        // Intenta mover al jugador una sola celda y empujar o arrastrar cajas
        private void TryMoveOneTile(Vector2 moveDirection)
        {
            // Miramos la celda justo delante del jugador
            Vector3Int frontCell = groundTilemap.WorldToCell(transform.position + (Vector3)moveDirection);
            BoxControl boxInFront = GetBoxAtCell(frontCell);

            // Si estamos usando pull, tambien revisamos la celda de atras
            bool isPullButtonHeld = pullAction != null && pullAction.IsPressed();
            Vector3Int rearCell = groundTilemap.WorldToCell(transform.position - (Vector3)moveDirection);
            BoxControl boxBehind = isPullButtonHeld ? GetBoxAtCell(rearCell) : null;

            // Si el movimiento no es valido, lo descartamos
            if (!CanPlayerMove(moveDirection, boxInFront, boxBehind))
            {
                return;
            }

            // Primero movemos la caja de delante si existe
            if (boxInFront != null)
            {
                boxInFront.MoveOneTile(moveDirection, groundTilemap, collisionTilemap);
            }

            // Luego avanzamos al jugador a la nueva celda
            transform.position += (Vector3)moveDirection;

            // Si estamos tirando, movemos la caja de atras en la misma direccion
            if (boxBehind != null)
            {
                boxBehind.MoveOneTile(moveDirection, groundTilemap, collisionTilemap);
            }
        }

        // Decide si la celda objetivo y las cajas asociadas permiten el paso
        private bool CanPlayerMove(Vector2 moveDirection, BoxControl boxInFront, BoxControl boxBehind)
        {
            // Calculamos la celda de destino del jugador
            Vector3Int nextPlayerCell = groundTilemap.WorldToCell(transform.position + (Vector3)moveDirection);

            // No se puede avanzar fuera del suelo o dentro de una colision
            if (!groundTilemap.HasTile(nextPlayerCell) || collisionTilemap.HasTile(nextPlayerCell))
            {
                return false;
            }

            // Si la caja frontal no puede moverse, el jugador tampoco
            if (boxInFront != null && !boxInFront.CanMoveOneTile(moveDirection, groundTilemap, collisionTilemap))
            {
                return false;
            }

            // Si la caja trasera no puede moverse, el pull tampoco es valido
            if (boxBehind != null && !boxBehind.CanMoveOneTile(moveDirection, groundTilemap, collisionTilemap))
            {
                return false;
            }

            return true;
        }

        // Busca una caja en la celda dada para poder empujarla o tirarla
        private BoxControl GetBoxAtCell(Vector3Int cell)
        {
            // Tomamos el centro de la celda para revisar colisiones fisicas
            Vector3 cellCenterWorld = groundTilemap.GetCellCenterWorld(cell);
            Collider2D[] collidersAtPoint = Physics2D.OverlapPointAll(cellCenterWorld);

            // Solo devolvemos el componente BoxControl si el collider pertenece a una caja
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

        // Cambia la animacion del jugador segun la direccion a la que mira
        private void UpdateAnimation(Vector2 direction)
        {
            // Sin animador no hay nada que cambiar
            if (animator == null)
            {
                return;
            }

            // Determinamos que animacion reproducir segun la direccion
            if (direction == Vector2.down)
            {
                animator.SetTrigger("idle_down");
                if (spriteRenderer != null)
                {
                    spriteRenderer.flipX = false;
                }
            }
            else if (direction == Vector2.up)
            {
                animator.SetTrigger("idle_up");
                if (spriteRenderer != null)
                {
                    spriteRenderer.flipX = false;
                }
            }
            else if (direction == Vector2.left)
            {
                animator.SetTrigger("idle_side");
                if (spriteRenderer != null)
                {
                    spriteRenderer.flipX = false;
                }
            }
            else if (direction == Vector2.right)
            {
                animator.SetTrigger("idle_side");
                if (spriteRenderer != null)
                {
                    spriteRenderer.flipX = true;
                }
            }
        }
    }
}

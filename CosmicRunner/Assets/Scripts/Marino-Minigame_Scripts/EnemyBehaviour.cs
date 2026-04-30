using UnityEngine;
using UnityEngine.Tilemaps;

namespace MECS
{
    // Enemigo que se mueve hacia jugador en rejilla
    public class EnemyBehaviour : MonoBehaviour
    {
        // Tilemap transitable donde el enemigo puede caminar
        [SerializeField] private Tilemap groundTilemap;
        // Tilemap con paredes o zonas bloqueadas
        [SerializeField] private Tilemap wallsTilemap;
        // Tiempo entre cada intento de movimiento
        [SerializeField] private float stepRepeatSeconds = 0.15f;
        // Referencia al jugador objetivo
        [SerializeField] private GameObject player;
        // Puntos que se restan al chocar con el jugador
        [SerializeField] private int scorePenalty = 10;

        // Cuenta regresiva hasta el siguiente paso
        private float timeUntilNextStep;
        // Direcciones cardinales que se prueban para acercarse al jugador
        private static readonly Vector3Int[] CardinalDirections =
        {
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.left,
            Vector3Int.right
        };

        // Recibe referencias de tilemap cuando el spawner crea este enemigo
        public void Initialize(Tilemap ground, Tilemap walls)
        {
            groundTilemap = ground;
            wallsTilemap = walls;
        }

        // Busca al jugador y arranca el temporizador de movimiento
        private void Start()
        {
            // El objetivo siempre es el objeto con tag Player
            player = GameObject.FindGameObjectWithTag("Player");
            timeUntilNextStep = stepRepeatSeconds;
        }

        // Controla el ritmo de movimiento del enemigo
        private void Update()
        {
            // Reducimos el tiempo hasta el siguiente paso
            timeUntilNextStep -= Time.deltaTime;

            // Si todavia no toca moverse, salimos
            if (timeUntilNextStep > 0f)
            {
                return;
            }

            // Cuando el temporizador llega a cero, damos un paso hacia el jugador
            MoveTowardPlayer();
            timeUntilNextStep = stepRepeatSeconds;
        }

        // Elige el paso valido que mas acerque al enemigo al jugador
        private void MoveTowardPlayer()
        {
            // Sin referencias validas no podemos calcular rutas
            if (player == null || groundTilemap == null || wallsTilemap == null)
            {
                return;
            }

            // Convertimos posiciones reales a celdas del mapa
            Vector3Int playerCell = groundTilemap.WorldToCell(player.transform.position);
            Vector3Int enemyCell = groundTilemap.WorldToCell(transform.position);

            // Empezamos suponiendo que no nos movemos
            Vector3Int bestMove = enemyCell;
            float bestDistance = Vector3Int.Distance(enemyCell, playerCell);

            // Probamos cada direccion cardinal para buscar la mejor opcion
            foreach (Vector3Int direction in CardinalDirections)
            {
                Vector3Int nextCell = enemyCell + direction;

                // Si la celda esta bloqueada, la descartamos
                if (!CanMoveToCell(nextCell))
                {
                    continue;
                }

                // Guardamos el movimiento que deje menor distancia al jugador
                float candidateDistance = Vector3Int.Distance(nextCell, playerCell);
                if (candidateDistance < bestDistance)
                {
                    bestDistance = candidateDistance;
                    bestMove = nextCell;
                }
            }

            // Si ninguna opcion mejora la posicion, no movemos al enemigo
            if (bestMove == enemyCell)
            {
                return;
            }

            // Colocamos el enemigo en el centro de la celda elegida
            transform.position = groundTilemap.GetCellCenterWorld(bestMove);
        }

        // Comprueba si una celda es valida para caminar
        private bool CanMoveToCell(Vector3Int cell)
        {
            // La celda debe existir en el suelo y no puede estar bloqueada por paredes
            if (!groundTilemap.HasTile(cell) || wallsTilemap.HasTile(cell))
            {
                return false;
            }

            // Tampoco puede haber una caja en esa celda
            if (IsBoxAtCell(cell))
            {
                return false;
            }

            return true;
        }

        // Revisa si una caja ocupa la celda indicada
        private bool IsBoxAtCell(Vector3Int cell)
        {
            // Miramos el centro de la celda para comprobar colisiones en ese punto
            Vector3 cellCenterWorld = groundTilemap.GetCellCenterWorld(cell);
            Collider2D[] collidersAtPoint = Physics2D.OverlapPointAll(cellCenterWorld);

            // Si encontramos un objeto con tag Box, bloqueamos la celda
            foreach (Collider2D colliderAtPoint in collidersAtPoint)
            {
                if (colliderAtPoint != null && colliderAtPoint.CompareTag("Box"))
                {
                    return true;
                }
            }

            return false;
        }

        // Si el enemigo toca al jugador, se destruye y aplica penalizacion
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Destroy(gameObject);
                Debug.Log("Enemy collided with player!");
                GameControl.Instance.sfxManager.PlayHurtSound();
                GameControl.Instance.RemoveScore(scorePenalty);
            }
        }
    }
}

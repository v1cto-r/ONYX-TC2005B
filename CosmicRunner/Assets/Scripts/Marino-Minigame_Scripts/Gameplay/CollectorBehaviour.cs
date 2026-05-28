using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace MECS
{
    // Coleccionador que se mueve de forma periodica entre tiles validos
    public class CollectorBehaviour : MonoBehaviour
    {
        // Tilemap base donde se calcula el movimiento del collector
        [SerializeField] private Tilemap groundTilemap;
        // Tilemap con obstaculos que no se pueden pisar
        [SerializeField] private Tilemap wallsTilemap;
        // Tiempo entre un movimiento aleatorio y el siguiente
        [SerializeField] private float moveInterval = 2f;

        // Busca el tilemap si no se asigno manualmente
        private void Awake()
        {
            if (groundTilemap == null)
            {
                groundTilemap = FindAnyObjectByType<Tilemap>();
            }
        }

        // Coloca al collector y programa sus siguientes saltos
        private void Start()
        {
            // La primera posicion se decide al iniciar la escena
            MoveToRandomPosition();
            // Luego repetimos la busqueda de destino cada cierto tiempo
            InvokeRepeating(nameof(MoveToRandomPosition), moveInterval, moveInterval);
        }

        // Deja de programar movimientos cuando el objeto se desactiva
        private void OnDisable()
        {
            CancelInvoke(nameof(MoveToRandomPosition));
        }

        // Cambia al collector a una celda valida al azar
        private void MoveToRandomPosition()
        {
            // Sin tilemap no tenemos base para movernos
            if (groundTilemap == null)
            {
                return;
            }

            // Recolectamos todas las celdas donde si se puede estar
            List<Vector3Int> validTiles = GetValidTiles();
            if (validTiles.Count == 0)
            {
                return;
            }

            // Elegimos una celda y colocamos el objeto en su centro
            Vector3Int targetCell = validTiles[Random.Range(0, validTiles.Count)];
            transform.position = groundTilemap.GetCellCenterWorld(targetCell);
        }

        // Devuelve todas las celdas que el collector puede usar
        private List<Vector3Int> GetValidTiles()
        {
            List<Vector3Int> validTiles = new List<Vector3Int>();

            // Revisamos todo el rango del mapa para filtrar celdas seguras
            foreach (Vector3Int cell in groundTilemap.cellBounds.allPositionsWithin)
            {
                if (!groundTilemap.HasTile(cell))
                {
                    continue;
                }

                // Las celdas con muro se descartan
                if (wallsTilemap != null && wallsTilemap.HasTile(cell))
                {
                    continue;
                }

                validTiles.Add(cell);
            }

            return validTiles;
        }
    }
}

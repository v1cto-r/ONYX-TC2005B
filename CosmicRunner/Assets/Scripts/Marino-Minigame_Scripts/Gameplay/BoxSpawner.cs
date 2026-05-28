using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;

namespace MECS
    {
    // Controla cuantas cajas deben existir y donde aparecen
    public class BoxSpawner : MonoBehaviour
    {
        // Mapa del suelo donde pueden aparecer las cajas
        [SerializeField] private Tilemap groundTilemap;
        // Mapa con paredes o tiles bloqueados
        [SerializeField] private Tilemap wallsTilemap;
        // Prefab de la caja que se va a instanciar
        [SerializeField] private GameObject boxPrefab;
        // Tiempo entre revisiones de cantidad
        [SerializeField] private float refreshSeconds = 1f;

        // Referencia al controlador de prompts para saber cuantas cajas hacen falta
        private PromptsControl promptsControl;
        // Lista de cajas ya creadas por este spawner
        private readonly List<GameObject> spawnedBoxes = new List<GameObject>();

        // Guarda referencias globales al iniciar
        private void Awake()
        {
            promptsControl = PromptsControl.Instance;
        }

        // Genera cajas iniciales y arranca la rutina de refresco
        private void Start()
        {
            SpawnMissingBoxes();
            StartCoroutine(RefreshLoop());
        }

        // Corutina que revisa cada cierto tiempo si faltan cajas
        private IEnumerator RefreshLoop()
        {
            while (true)
            {
                // Esperamos antes de volver a revisar el estado de las cajas
                yield return new WaitForSeconds(refreshSeconds);
                SpawnMissingBoxes();
            }
        }

        // Ajusta la cantidad de cajas visibles segun el estado de las palabras
        private void SpawnMissingBoxes()
        {
            // Si el panel de prompts esta abierto, no metemos ruido visual con cajas nuevas
            if (GameControl.Instance != null && GameControl.Instance.uiControl != null && GameControl.Instance.uiControl.IsPromptsPanelOpen())
            {
                return;
            }

            // Eliminamos referencias muertas antes de contar cuantas cajas quedan
            CleanupSpawnedBoxes();

            // Calculamos cuantas cajas deberian existir segun el espacio libre
            int targetBoxCount = GetTargetBoxCount();
            int boxesToSpawn = targetBoxCount - spawnedBoxes.Count;

            // Creamos cajas hasta llegar a la cantidad objetivo
            while (boxesToSpawn > 0)
            {
                GameObject spawnedBox = SpawnOneBox();
                if (spawnedBox == null)
                {
                    return;
                }

                spawnedBoxes.Add(spawnedBox);
                boxesToSpawn--;
            }
        }

        // Calcula cuantas cajas necesitamos para llenar el espacio libre de palabras
        private int GetTargetBoxCount()
        {
            // Si no tenemos referencia, intentamos recuperarla
            if (promptsControl == null)
            {
                promptsControl = PromptsControl.Instance;
            }

            // Si sigue faltando, no podemos calcular la meta
            if (promptsControl == null)
            {
                return 0;
            }

            // Las cajas ocupan el espacio libre que queda en storage
            int freeWordSlots = promptsControl.wordStorageCapacity - promptsControl.currentWordCount;
            return Mathf.Max(0, freeWordSlots);
        }

        // Intenta crear una sola caja en una celda libre valida
        private GameObject SpawnOneBox()
        {
            // Sin referencias no podemos instanciar de forma segura
            if (boxPrefab == null || groundTilemap == null || wallsTilemap == null)
            {
                Debug.LogWarning("BoxSpawner is missing references.", this);
                return null;
            }

            // Buscamos todas las celdas donde una caja podria aparecer
            List<Vector3Int> validTiles = GetValidSpawnTiles();
            while (validTiles.Count > 0)
            {
                // Elegimos una celda al azar y la quitamos de la lista para no repetirla
                int randomIndex = Random.Range(0, validTiles.Count);
                Vector3Int spawnCell = validTiles[randomIndex];
                validTiles.RemoveAt(randomIndex);

                // Convertimos la celda al centro del mundo para instanciar la caja
                Vector3 spawnWorldPos = groundTilemap.GetCellCenterWorld(spawnCell);

                // Si hay algo ocupando el punto, seguimos probando otras celdas
                if (IsCellOccupied(spawnWorldPos))
                {
                    continue;
                }

                // Instanciamos la caja como hija del spawner para tener la escena ordenada
                return Instantiate(boxPrefab, spawnWorldPos, Quaternion.identity, transform);
            }

            // Si no encontramos ningun punto libre, avisamos en consola
            Debug.LogWarning("BoxSpawner could not find a free tile for a box.", this);
            return null;
        }

        // Lista las celdas aptas para colocar cajas
        private List<Vector3Int> GetValidSpawnTiles()
        {
            List<Vector3Int> validTiles = new List<Vector3Int>();

            // Recorremos todo el mapa para filtrar solo las celdas utilizables
            foreach (Vector3Int cell in groundTilemap.cellBounds.allPositionsWithin)
            {
                if (groundTilemap.HasTile(cell) && !wallsTilemap.HasTile(cell))
                {
                    validTiles.Add(cell);
                }
            }

            return validTiles;
        }

        // Comprueba si ya hay algo ocupando la posicion elegida
        private bool IsCellOccupied(Vector3 worldPosition)
        {
            // Revisamos todos los colliders que tocan el punto
            Collider2D[] collidersAtPoint = Physics2D.OverlapPointAll(worldPosition);

            // Cualquier jugador, enemigo, collector o caja bloquea el spawn
            foreach (Collider2D colliderAtPoint in collidersAtPoint)
            {
                if (colliderAtPoint == null)
                {
                    continue;
                }

                if (colliderAtPoint.CompareTag("Player") || colliderAtPoint.CompareTag("Enemy") || colliderAtPoint.CompareTag("Collector") || colliderAtPoint.CompareTag("Box"))
                {
                    return true;
                }
            }

            return false;
        }

        // Limpia la lista interna de cajas que ya fueron destruidas
        private void CleanupSpawnedBoxes()
        {
            // Recorremos al reves para poder borrar elementos sin romper indices
            for (int i = spawnedBoxes.Count - 1; i >= 0; i--)
            {
                if (spawnedBoxes[i] == null)
                {
                    spawnedBoxes.RemoveAt(i);
                }
            }
        }
    }
}

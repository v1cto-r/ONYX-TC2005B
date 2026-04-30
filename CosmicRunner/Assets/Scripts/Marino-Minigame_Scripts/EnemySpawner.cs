using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;

namespace MECS
{
    // Spawnea enemigos en tiles validos
    public class EnemySpawner : MonoBehaviour
    {
        // Mapa del suelo donde pueden aparecer
        [SerializeField] private Tilemap groundTilemap;
        // Mapa con paredes o zonas prohibidas
        [SerializeField] private Tilemap wallsTilemap;
        // Prefab del enemigo a instanciar
        [SerializeField] private GameObject enemyPrefab;
        // Minimo de enemigos por oleada
        [SerializeField] private int minEnemiesPerWave = 1;
        // Maximo de enemigos por oleada
        [SerializeField] private int maxEnemiesPerWave = 3;
        // Tiempo minimo entre spawns
        [SerializeField] private float timeToSpawnMin = 1f;
        // Tiempo maximo entre spawns
        [SerializeField] private float timeToSpawnMax = 3f;

        // Arranca la corutina que genera oleadas de enemigos
        void Start()
        {
            StartCoroutine(SpawnLoop());
        }

        // Bucle infinito que espera y luego crea una nueva oleada
        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                // Esperamos un tiempo aleatorio antes de volver a spawnear
                yield return new WaitForSeconds(Random.Range(timeToSpawnMin, timeToSpawnMax));
                SpawnWave();
            }
        }

        // Instancia una oleada de enemigos respetando las celdas libres
        private void SpawnWave()
        {
            // Sin referencias no podemos crear enemigos de forma segura
            if (enemyPrefab == null || groundTilemap == null || wallsTilemap == null)
            {
                Debug.LogWarning("EnemySpawner is missing references.", this);
                return;
            }

            // Aseguramos un rango valido de cantidad por oleada
            int clampedMin = Mathf.Max(1, minEnemiesPerWave);
            int clampedMax = Mathf.Max(clampedMin, maxEnemiesPerWave);
            int enemiesToSpawn = Random.Range(clampedMin, clampedMax + 1);

            // Reunimos las celdas donde realmente se puede aparecer
            List<Vector3Int> validTiles = GetValidSpawnTiles();
            int spawnedCount = 0;

            // Vamos consumiendo tiles validos hasta cubrir la cantidad objetivo
            while (spawnedCount < enemiesToSpawn && validTiles.Count > 0)
            {
                // Elegimos una celda libre al azar y la sacamos del pool
                int randomIndex = Random.Range(0, validTiles.Count);
                Vector3Int spawnCell = validTiles[randomIndex];
                validTiles.RemoveAt(randomIndex);

                // Convertimos la celda elegida a posicion de mundo
                Vector3 spawnWorldPos = groundTilemap.GetCellCenterWorld(spawnCell);

                // Si algo ocupa el punto, probamos otra celda
                if (IsCellOccupied(spawnWorldPos))
                {
                    continue;
                }

                // Creamos el enemigo y le damos referencias del mapa
                GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnWorldPos, Quaternion.identity);
                EnemyBehaviour enemyBehaviour = spawnedEnemy.GetComponent<EnemyBehaviour>();
                if (enemyBehaviour != null)
                {
                    enemyBehaviour.Initialize(groundTilemap, wallsTilemap);
                }

                spawnedCount++;
            }

            // Avisamos si no se pudo cubrir toda la oleada por falta de espacio
            if (spawnedCount < enemiesToSpawn)
            {
                Debug.LogWarning("Spawned " + spawnedCount + " out of " + enemiesToSpawn + " enemies because not enough free tiles were available.");
            }
        }

        // Devuelve todas las celdas transitable donde puede aparecer un enemigo
        private List<Vector3Int> GetValidSpawnTiles()
        {
            List<Vector3Int> validTiles = new List<Vector3Int>();

            // Recorremos el mapa completo buscando tiles sin pared
            foreach (Vector3Int cell in groundTilemap.cellBounds.allPositionsWithin)
            {
                if (groundTilemap.HasTile(cell) && !wallsTilemap.HasTile(cell))
                {
                    validTiles.Add(cell);
                }
            }

            return validTiles;
        }

        // Comprueba si ya hay algo ocupando la posicion de spawn
        private bool IsCellOccupied(Vector3 worldPosition)
        {
            // Miramos todos los colliders en el punto elegido
            Collider2D[] collidersAtPoint = Physics2D.OverlapPointAll(worldPosition);

            // Si encontramos jugador, enemigo o caja, cancelamos ese spawn
            foreach (Collider2D colliderAtPoint in collidersAtPoint)
            {
                if (colliderAtPoint == null)
                {
                    continue;
                }

                if (colliderAtPoint.CompareTag("Player") || colliderAtPoint.CompareTag("Enemy") || colliderAtPoint.CompareTag("Box"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
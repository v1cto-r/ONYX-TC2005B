using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class BoxSpawner : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap wallsTilemap;
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private int boxCount = 4;

    void Start()
    {
        SpawnBoxes();
    }

    private void SpawnBoxes()
    {
        List<Vector3Int> validTiles = GetValidSpawnTiles();

        if (validTiles.Count < boxCount)
        {
            Debug.LogWarning("Not enough valid tiles to spawn " + boxCount + " boxes!");
        }

        int spawnedCount = 0;

        while (spawnedCount < boxCount && validTiles.Count > 0)
        {
            int randomIndex = Random.Range(0, validTiles.Count);
            Vector3Int spawnCell = validTiles[randomIndex];
            validTiles.RemoveAt(randomIndex);

            Vector3 spawnWorldPos = groundTilemap.GetCellCenterWorld(spawnCell);

            if (IsCellOccupied(spawnWorldPos))
            {
                continue;
            }

            Instantiate(boxPrefab, spawnWorldPos, Quaternion.identity);
            spawnedCount++;
        }

        if (spawnedCount < boxCount)
        {
            Debug.LogWarning("Spawned " + spawnedCount + " out of " + boxCount + " boxes because not enough free tiles were available.");
        }
    }

    private List<Vector3Int> GetValidSpawnTiles()
    {
        List<Vector3Int> validTiles = new List<Vector3Int>();

        foreach (Vector3Int cell in groundTilemap.cellBounds.allPositionsWithin)
        {
            if (groundTilemap.HasTile(cell) && !wallsTilemap.HasTile(cell))
            {
                validTiles.Add(cell);
            }
        }

        return validTiles;
    }

    private bool IsCellOccupied(Vector3 worldPosition)
    {
        Collider2D[] collidersAtPoint = Physics2D.OverlapPointAll(worldPosition);

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

using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;

public class BoxSpawner : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap wallsTilemap;
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private float refreshSeconds = 1f;

    private PromptsControl promptsControl;
    private readonly List<GameObject> spawnedBoxes = new List<GameObject>();

    private void Awake()
    {
        promptsControl = PromptsControl.Instance;
    }

    private void Start()
    {
        SpawnMissingBoxes();
        StartCoroutine(RefreshLoop());
    }

    private IEnumerator RefreshLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(refreshSeconds);
            SpawnMissingBoxes();
        }
    }

    private void SpawnMissingBoxes()
    {
        if (GameControl.Instance != null && GameControl.Instance.uiControl != null && GameControl.Instance.uiControl.IsPromptsPanelOpen())
        {
            return;
        }

        CleanupSpawnedBoxes();

        int targetBoxCount = GetTargetBoxCount();
        int boxesToSpawn = targetBoxCount - spawnedBoxes.Count;

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

    private int GetTargetBoxCount()
    {
        if (promptsControl == null)
        {
            promptsControl = PromptsControl.Instance;
        }

        if (promptsControl == null)
        {
            return 0;
        }

        int freeWordSlots = promptsControl.wordStorageCapacity - promptsControl.currentWordCount;
        return Mathf.Max(0, freeWordSlots);
    }

    private GameObject SpawnOneBox()
    {
        if (boxPrefab == null || groundTilemap == null || wallsTilemap == null)
        {
            Debug.LogWarning("BoxSpawner is missing references.", this);
            return null;
        }

        List<Vector3Int> validTiles = GetValidSpawnTiles();
        while (validTiles.Count > 0)
        {
            int randomIndex = Random.Range(0, validTiles.Count);
            Vector3Int spawnCell = validTiles[randomIndex];
            validTiles.RemoveAt(randomIndex);

            Vector3 spawnWorldPos = groundTilemap.GetCellCenterWorld(spawnCell);

            if (IsCellOccupied(spawnWorldPos))
            {
                continue;
            }

            return Instantiate(boxPrefab, spawnWorldPos, Quaternion.identity, transform);
        }

        Debug.LogWarning("BoxSpawner could not find a free tile for a box.", this);
        return null;
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

            if (colliderAtPoint.CompareTag("Player") || colliderAtPoint.CompareTag("Enemy") || colliderAtPoint.CompareTag("Collector") || colliderAtPoint.CompareTag("Box"))
            {
                return true;
            }
        }

        return false;
    }

    private void CleanupSpawnedBoxes()
    {
        for (int i = spawnedBoxes.Count - 1; i >= 0; i--)
        {
            if (spawnedBoxes[i] == null)
            {
                spawnedBoxes.RemoveAt(i);
            }
        }
    }
}

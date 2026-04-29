using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class CollectorBehaviour : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap wallsTilemap;
    [SerializeField] private float moveInterval = 2f;

    private void Awake()
    {
        if (groundTilemap == null)
        {
            groundTilemap = FindAnyObjectByType<Tilemap>();
        }
    }

    private void Start()
    {
        MoveToRandomPosition();
        InvokeRepeating(nameof(MoveToRandomPosition), moveInterval, moveInterval);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(MoveToRandomPosition));
    }

    private void MoveToRandomPosition()
    {
        if (groundTilemap == null)
        {
            return;
        }

        List<Vector3Int> validTiles = GetValidTiles();
        if (validTiles.Count == 0)
        {
            return;
        }

        Vector3Int targetCell = validTiles[Random.Range(0, validTiles.Count)];
        transform.position = groundTilemap.GetCellCenterWorld(targetCell);
    }

    private List<Vector3Int> GetValidTiles()
    {
        List<Vector3Int> validTiles = new List<Vector3Int>();

        foreach (Vector3Int cell in groundTilemap.cellBounds.allPositionsWithin)
        {
            if (!groundTilemap.HasTile(cell))
            {
                continue;
            }

            if (wallsTilemap != null && wallsTilemap.HasTile(cell))
            {
                continue;
            }

            validTiles.Add(cell);
        }

        return validTiles;
    }
}

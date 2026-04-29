using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap wallsTilemap;
    [SerializeField] private float stepRepeatSeconds = 0.15f;
    [SerializeField] private GameObject player;
    [SerializeField] private int scorePenalty = 10;

    private float timeUntilNextStep;
    private static readonly Vector3Int[] CardinalDirections =
    {
        Vector3Int.up,
        Vector3Int.down,
        Vector3Int.left,
        Vector3Int.right
    };

    public void Initialize(Tilemap ground, Tilemap walls)
    {
        groundTilemap = ground;
        wallsTilemap = walls;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        timeUntilNextStep = stepRepeatSeconds;
    }

    private void Update()
    {
        timeUntilNextStep -= Time.deltaTime;

        if (timeUntilNextStep > 0f)
        {
            return;
        }

        MoveTowardPlayer();
        timeUntilNextStep = stepRepeatSeconds;
    }

    private void MoveTowardPlayer()
    {
        if (player == null || groundTilemap == null || wallsTilemap == null)
        {
            return;
        }

        Vector3Int playerCell = groundTilemap.WorldToCell(player.transform.position);
        Vector3Int enemyCell = groundTilemap.WorldToCell(transform.position);

        Vector3Int bestMove = enemyCell;
        float bestDistance = Vector3Int.Distance(enemyCell, playerCell);

        foreach (Vector3Int direction in CardinalDirections)
        {
            Vector3Int nextCell = enemyCell + direction;

            if (!CanMoveToCell(nextCell))
            {
                continue;
            }

            float candidateDistance = Vector3Int.Distance(nextCell, playerCell);
            if (candidateDistance < bestDistance)
            {
                bestDistance = candidateDistance;
                bestMove = nextCell;
            }
        }

        if (bestMove == enemyCell)
        {
            return;
        }

        transform.position = groundTilemap.GetCellCenterWorld(bestMove);
    }

    private bool CanMoveToCell(Vector3Int cell)
    {
        if (!groundTilemap.HasTile(cell) || wallsTilemap.HasTile(cell))
        {
            return false;
        }

        if (IsBoxAtCell(cell))
        {
            return false;
        }

        return true;
    }

    private bool IsBoxAtCell(Vector3Int cell)
    {
        Vector3 cellCenterWorld = groundTilemap.GetCellCenterWorld(cell);
        Collider2D[] collidersAtPoint = Physics2D.OverlapPointAll(cellCenterWorld);

        foreach (Collider2D colliderAtPoint in collidersAtPoint)
        {
            if (colliderAtPoint != null && colliderAtPoint.CompareTag("Box"))
            {
                return true;
            }
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            Debug.Log("Enemy collided with player!");
            GameControl.Instance.RemoveScore(scorePenalty);
        }
    }
}

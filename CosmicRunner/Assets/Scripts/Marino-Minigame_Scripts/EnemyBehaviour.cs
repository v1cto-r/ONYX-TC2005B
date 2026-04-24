using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap wallsTilemap;
    [SerializeField] private float stepRepeatSeconds = 0.15f;
    [SerializeField] private GameObject player;

    private float timeUntilNextStep;
    private Vector3Int[] cardinalDirections = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

    public void Initialize(Tilemap ground, Tilemap walls)
    {
        groundTilemap = ground;
        wallsTilemap = walls;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        timeUntilNextStep = stepRepeatSeconds;
    }

    void Update()
    {
        timeUntilNextStep -= Time.deltaTime;

        if (timeUntilNextStep <= 0f)
        {
            MoveTowardPlayer();
            timeUntilNextStep = stepRepeatSeconds;
        }
    }

    private void MoveTowardPlayer()
    {
        if (player == null)
            return;

        Vector3Int playerCell = groundTilemap.WorldToCell(player.transform.position);
        Vector3Int enemyCell = groundTilemap.WorldToCell(transform.position);

        Vector3Int bestMove = enemyCell;
        float closestDistToPlayer = Vector3Int.Distance(enemyCell, playerCell);

        foreach (Vector3Int direction in cardinalDirections)
        {
            Vector3Int nextCell = enemyCell + direction;

            if (CanMoveToCell(nextCell))
            {
                float distToPlayer = Vector3Int.Distance(nextCell, playerCell);
                if (distToPlayer < closestDistToPlayer)
                {
                    closestDistToPlayer = distToPlayer;
                    bestMove = nextCell;
                }
            }
        }

        if (bestMove != enemyCell)
        {
            Vector3 nextWorldPos = groundTilemap.GetCellCenterWorld(bestMove);
            transform.position = nextWorldPos;
        }
    }

    private bool CanMoveToCell(Vector3Int cell)
    {
        if (!groundTilemap.HasTile(cell) || wallsTilemap.HasTile(cell))
        {
            return false;
        }

        return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            Debug.Log("Enemy collided with player!");
            // TODO: Remove points from player
        }
    }
}

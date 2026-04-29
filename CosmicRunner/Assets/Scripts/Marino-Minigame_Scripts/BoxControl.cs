using UnityEngine;
using UnityEngine.Tilemaps;

public class BoxControl : MonoBehaviour
{
    public bool CanMoveOneTile(Vector2 moveDirection, Tilemap groundTilemap, Tilemap collisionTilemap)
    {
        Vector3Int targetCell = groundTilemap.WorldToCell(transform.position + (Vector3)moveDirection);

        if (!groundTilemap.HasTile(targetCell) || collisionTilemap.HasTile(targetCell))
        {
            return false;
        }

        Vector3 targetWorld = groundTilemap.GetCellCenterWorld(targetCell);
        Collider2D[] collidersAtTarget = Physics2D.OverlapPointAll(targetWorld);

        foreach (Collider2D colliderAtTarget in collidersAtTarget)
        {
            if (colliderAtTarget == null || colliderAtTarget.transform == transform)
            {
                continue;
            }

            if (colliderAtTarget.CompareTag("Box"))
            {
                return false;
            }
        }

        return true;
    }

    public bool MoveOneTile(Vector2 moveDirection, Tilemap groundTilemap, Tilemap collisionTilemap)
    {
        if (!CanMoveOneTile(moveDirection, groundTilemap, collisionTilemap))
        {
            return false;
        }

        Vector3Int targetCell = groundTilemap.WorldToCell(transform.position + (Vector3)moveDirection);
        Vector3 targetWorld = groundTilemap.GetCellCenterWorld(targetCell);

        transform.position = targetWorld;
        return true;
    }

    public bool TryPush(Vector2 moveDirection, Tilemap groundTilemap, Tilemap collisionTilemap)
    {
        return MoveOneTile(moveDirection, groundTilemap, collisionTilemap);
    }
}

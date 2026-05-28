using UnityEngine;
using UnityEngine.Tilemaps;

namespace MECS
{
    // Control de cajas: movimiento y empuje
    public class BoxControl : MonoBehaviour
    {
        // Comprueba si la caja puede avanzar una celda en una direccion dada
        public bool CanMoveOneTile(Vector2 moveDirection, Tilemap groundTilemap, Tilemap collisionTilemap)
        {
            // Calculamos la celda objetivo segun la direccion solicitada
            Vector3Int targetCell = groundTilemap.WorldToCell(transform.position + (Vector3)moveDirection);

            // La celda debe existir y no estar bloqueada por el mapa de colision
            if (!groundTilemap.HasTile(targetCell) || collisionTilemap.HasTile(targetCell))
            {
                return false;
            }

            // Tambien revisamos que no haya otra caja ocupando el punto objetivo
            Vector3 targetWorld = groundTilemap.GetCellCenterWorld(targetCell);
            Collider2D[] collidersAtTarget = Physics2D.OverlapPointAll(targetWorld);

            // Si encontramos otra caja distinta a esta, bloqueamos el movimiento
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

        // Mueve la caja una sola celda si el movimiento es legal
        public bool MoveOneTile(Vector2 moveDirection, Tilemap groundTilemap, Tilemap collisionTilemap)
        {
            // Si no puede moverse, devolvemos false sin cambiar la posicion
            if (!CanMoveOneTile(moveDirection, groundTilemap, collisionTilemap))
            {
                return false;
            }

            // Recalculamos la celda y la posicion de mundo exacta para centrarla
            Vector3Int targetCell = groundTilemap.WorldToCell(transform.position + (Vector3)moveDirection);
            Vector3 targetWorld = groundTilemap.GetCellCenterWorld(targetCell);

            // Movemos la caja directamente al centro de la nueva celda
            transform.position = targetWorld;

            // Reproducimos el sonido de empujar si tenemos referencia al SFXManager
            if (GameControl.Instance != null && GameControl.Instance.sfxManager != null)
            {
                GameControl.Instance.sfxManager.PlayPushBlockSound();
            }

            return true;
        }

        // Punto de entrada para empujar una caja desde otra mecanica
        public bool TryPush(Vector2 moveDirection, Tilemap groundTilemap, Tilemap collisionTilemap)
        {
            // En esta implementacion empujar equivale a moverse una celda
            return MoveOneTile(moveDirection, groundTilemap, collisionTilemap);
        }
    }
}

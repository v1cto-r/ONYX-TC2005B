using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Algo entró");

        if (collision.CompareTag("Player"))
        {
            Debug.Log("Jugador detectado");

            CheckpointManager.instance.respawnPoint = transform.position;

            Debug.Log("Checkpoint activado");
        }
    }
}
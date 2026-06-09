using UnityEngine;

namespace JorgeGame
{
    public class DeadZone : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            
            if (collision.CompareTag("Player"))
            {
                GameControl.Instance.SpendLives();
                collision.transform.position = SpawnPoint.instance.respawnPoint;
            }
        }
    }
}
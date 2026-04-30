using UnityEngine;

namespace MECS
{
    public class BoxBehaviour : MonoBehaviour
    {
        // Detecta cuando una caja entra en contacto con el collector
        private void OnTriggerEnter2D(Collider2D trigger)
        {
            // Solo reaccionamos si el objeto que toca tiene el tag correcto
            if (trigger.gameObject.CompareTag("Collector"))
            {
                // Buscamos el controlador de prompts para avisar que se recogio una caja
                PromptsControl promptsControl = PromptsControl.Instance;
                if (promptsControl == null)
                {
                    // Si la instancia no existe aun, la buscamos en escena, incluso si esta inactiva
                    promptsControl = FindAnyObjectByType<PromptsControl>(FindObjectsInactive.Include);
                }

                // Si encontramos el controlador, sumamos la palabra al sistema
                if (promptsControl != null)
                {
                    promptsControl.HandleBoxCollected();
                }

                // La caja ya cumplio su trabajo, asi que la destruimos
                if (GameControl.Instance != null && GameControl.Instance.sfxManager != null)
                {
                    GameControl.Instance.sfxManager.PlayBoxCollectSound();
                }
                Destroy(gameObject);
            }
        }
    }
}
using UnityEngine;

namespace MECS
{
    // Controla la reproduccion de efectos de sonido en el juego
    public class SFXManager : MonoBehaviour
    {
        // Referencia al componente AudioSource para reproducir sonidos
        [SerializeField] private AudioSource audioSource;
        
        // Referencia al clip de sonido para el spawn de enemigos
        public AudioClip enemySpawnSound;

        // Referencia al clip de sonido para el disparo
        public AudioClip shootSound;

        // Referencia al clip de sonido para empujar bloques y recoger cajas
        public AudioClip pushBlockSound;
        public AudioClip boxCollectSound;

        // Referencia al clip de sonido para: completar un prompt, obtener puntos, y otros eventos positivos
        public AudioClip goodSound;

        // Referencia al clip de sonido para cuando el jugador es golpeado
        public AudioClip hurtSound;

        // Referencia al clip de sonido para los distintos resultados del nivel
        public AudioClip loseSound;
        public AudioClip winSound;

        // Metodos para reproducir los sonidos en base a la accion del juego
        public void PlayEnemySpawnSound()
        {
            audioSource.PlayOneShot(enemySpawnSound, 0.55f);
        }

        public void PlayShootSound()
        {
            audioSource.PlayOneShot(shootSound, 0.75f);
        }

        public void PlayHurtSound()
        {
            audioSource.PlayOneShot(hurtSound, 0.6f);
        }

        public void PlayPushBlockSound()
        {
            audioSource.PlayOneShot(pushBlockSound, 0.5f);
        }

        public void PlayBoxCollectSound()
        {
            audioSource.PlayOneShot(boxCollectSound, 0.5f);
        }

        public void PlayGoodSound()
        {
            audioSource.PlayOneShot(goodSound, 0.6f);
        }

        public void PlayLoseSound()
        {
            audioSource.PlayOneShot(loseSound, 0.35f);
        }

        public void PlayWinSound()
        {
            audioSource.PlayOneShot(winSound, 0.7f);
        }
    }
}

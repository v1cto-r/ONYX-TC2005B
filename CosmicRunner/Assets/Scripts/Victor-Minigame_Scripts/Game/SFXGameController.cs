using UnityEngine;

namespace AB
{
    public class SFXGameController : MonoBehaviour
    {
        public AudioSource audioSource;
        public AudioClip shootSound;
        public AudioClip shipDestroyedSound;
        public AudioClip shipHitSound;
        public AudioClip shipMoveSound;
        public AudioClip boosterCollectSound;
        public AudioClip chipDropSound;

        public static SFXGameController Instance;

        void Awake()
        {
            Instance = this;
        }

        public void PlayShootSound()
        {
            audioSource.PlayOneShot(shootSound, 0.6f);
        }

        public void PlayShipDestroyedSound()
        {
            audioSource.PlayOneShot(shipDestroyedSound, 0.6f);
        }

        public void PlayShipHitSound()
        {
            audioSource.PlayOneShot(shipHitSound, 0.6f);
        }

        public void PlayShipMoveSound()
        {
            audioSource.clip = shipMoveSound;
            audioSource.loop = true;
            audioSource.volume = 0.4f;
            if (!audioSource.isPlaying) {
                audioSource.Play();
            }
        }

        public void StopShipMoveSound()
        {
            if (audioSource.clip == shipMoveSound) {
                audioSource.Stop();
                audioSource.clip = null;
            }
        }

        public void PlayBoosterCollectSound()
        {
            audioSource.PlayOneShot(boosterCollectSound, 1f);
        }

        public void PlayChipPlaceSound()
        {
            audioSource.PlayOneShot(chipDropSound, 0.6f);
        }
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AB
{
    // Tipo de booster que se puede recoger
    public enum BoosterType { Credit, Shield, Bullet }

    // Va a juntar todos los componentes principales
    public class GameController : MonoBehaviour
    {
        [Header("Game Components")]
        static public GameController Instance;

        public EnemySpawner enemySpawner;
        public BoosterSpawner boosterSpawner;
        public ShipControlller shipController;
        public UIController uiController;
        public RepairController repairController;

        public ChipController[] chips;

        [Header("Game Values")]
        public float gameDuration = 60f;
        public float shieldDuration = 10f;
        private float elapsedTime = 0f;
        private float elapsedShieldTime = 0f;
        private int credits = 0;
        public int bullets = 5;
        
        
        void Awake()
        {
            Instance = this;
            Time.timeScale = 1f;
            uiController.UpdateBullets(bullets);
            StartCoroutine(MatchTime());
        }
        
        public float GetElapsedTime()
        {
            return elapsedTime;
        }

        IEnumerator MatchTime()
        {
            yield return new WaitForSeconds(1);
            elapsedTime += 1;
            uiController.UpdateTimeText(elapsedTime, gameDuration);

            if (elapsedTime >= gameDuration)
            {
                WinGame();
            } else
            {
                StartCoroutine(MatchTime());
            }
        }

        // Timer del shield
        IEnumerator ShieldTime()
        {
            yield return new WaitForSeconds(1);
            elapsedShieldTime += 1;
            uiController.UpdateShieldTime(elapsedShieldTime, shieldDuration);

            if (elapsedShieldTime >= shieldDuration)
            {
                elapsedShieldTime = 0;
                shipController.DisableShield();
            } else
            {
                StartCoroutine(ShieldTime());
            }
        }

        public void LooseShield()
        {
            elapsedShieldTime = 0;
        }

        // Dependiendo del tipo de booster, se llama a la función correspondiente
        public void CollectBooster(BoosterType boosterType)
        {
            switch (boosterType)
            {
                case BoosterType.Credit:
                    CollectCredit();
                    break;
                case BoosterType.Shield:
                    CollectShield();
                    break;
                case BoosterType.Bullet:
                    CollectBullet();
                    break;
            }
        }

        public void CollectCredit()
        {
            credits += 15;
            uiController.ModifyCreditsText(credits);
        }

        public void CollectShield()
        {
            shipController.EnableShield();
            StartCoroutine(ShieldTime());
        }

        public void CollectBullet()
        {
            bullets = Mathf.Clamp(bullets + 3, 0, 6);
            uiController.UpdateBullets(bullets);
        }

        public void SpendBullet()
        {
            if (bullets > 0)
            {
                bullets--;
                uiController.UpdateBullets(bullets);
            }
        }

        public int GetBullets()
        {
            return bullets;
        }

        public void WinGame()
        {
            PlayerPrefs.SetInt("collected_credits", credits);
            PlayerPrefs.SetInt("result", 1);
            SceneManager.LoadScene("WinScene_AB");
        }

        public void EndGame()
        {
            PlayerPrefs.SetInt("collected_credits", credits);
            PlayerPrefs.SetInt("result", -1);
            SceneManager.LoadScene("EndScene_AB");
        }

        // Activa la reparacion, que pausa el juego y muestra el panel de reparación
        public void HandleRepair()
        {
            Time.timeScale = 0f;
            repairController.gameObject.SetActive(true);
        }

        // Termina la reparación, que reanuda el juego y oculta el panel de reparación
        public void FinishRepair()
        {
            Time.timeScale = 1f;
            repairController.gameObject.SetActive(false);
            shipController.EnableShield();
            StartCoroutine(ShieldTime());
        }
    }
}

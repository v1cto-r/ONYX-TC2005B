using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AB
{
    public enum BoosterType { Credit, Shield, Bullet }

    public class GameController : MonoBehaviour
    {
        [Header("Game Components")]
        static public GameController Instance;

        public EnemySpawner enemySpawner;
        public BoosterSpawner boosterSpawner;
        public ShipControlller shipController;
        public UIController uiController;

        [Header("Game Values")]
        public float gameDuration = 60f;
        private float elapsedTime = 0f;
        private int credits = 0;
        public int bullets = 5;
        
        
        void Awake()
        {
            Instance = this;
            Time.timeScale = 1;
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
    }
}

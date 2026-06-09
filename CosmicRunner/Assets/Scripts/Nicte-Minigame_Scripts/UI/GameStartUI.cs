using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

namespace Nicte.Minigame
{

    [System.Serializable]
    public class RankResult
    {
        public string name;
        public string lastname;
        public int posicion;
    }

    [System.Serializable]
    public class RankResultArray
    {
        public RankResult[] items;
    }

    public class GameStartUI : MonoBehaviour
    {
        public GameObject startScreen;
        public GameObject controlsScreen1;
        public GameObject controlsScreen2;
        public GameObject benefitScreen;
        public TextMeshProUGUI messageText;
        public TextMeshProUGUI subtitleText;

        RankResult rank;

        bool benefit = false;

        //private string apiUrl = "https://127.0.0.1:12005/clasificacion/ataqueEstelar/";
        private string apiUrl = "https://nicte.onyx.14082006.xyz/clasificacion/ataqueEstelar/";

        void Start()
        {
            PlayerPrefs.DeleteKey("rank_position");
            PlayerPrefs.DeleteKey("rank_name");
            if (startScreen != null)
                startScreen.SetActive(true);
            if (controlsScreen1 != null)
                controlsScreen1.SetActive(false);
            if (controlsScreen2 != null)
                controlsScreen2.SetActive(false);
            if (benefitScreen != null)
                benefitScreen.SetActive(false);
        }

        IEnumerator CheckRank(int userId)
        {
            string url = apiUrl + userId;
            UnityWebRequest web = UnityWebRequest.Get(url);
            web.certificateHandler = new ForceAcceptAll();
            yield return web.SendWebRequest();

            if (web.result != UnityWebRequest.Result.Success)
            {
                SceneManager.LoadScene("AtaqueEstelarGame");
                yield break;
            }

            string Array = "{\"items\":" + web.downloadHandler.text + "}";
            RankResultArray wrapper = JsonUtility.FromJson<RankResultArray>(Array);
            rank = wrapper.items[0];

            Debug.Log($"Player rank: {rank.posicion}, Name: {rank.name} {rank.lastname}");
            PlayerPrefs.SetString("rank_name", rank.name + " " + rank.lastname);
            PlayerPrefs.SetInt("rank_position", rank.posicion);

            if (rank != null && rank.posicion <= 3)
            {
                benefit = true;
            }
            else
            {
                benefit = false;
            }

            if (benefit)
            {
                ShowBenefitScreen(rank);
            }
            else
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene("AtaqueEstelarGame");
            }
        }

        public void playGame()
        {
            int userId = PlayerPrefs.GetInt("UserId", 1);
            Debug.Log($"UserId: {userId}");
            StartCoroutine(CheckRank(userId));

        }

        public void playGameBenefitScreen()
        {
            SceneManager.LoadScene("AtaqueEstelarGame");
        }


        void ShowBenefitScreen(RankResult rank)
        {
            if (startScreen != null)
            {
                startScreen.SetActive(false);
            }
            if (benefitScreen != null)
            {
                benefitScreen.SetActive(true);
            }

            if (messageText != null)
                messageText.text = $"¡Felicidades, {rank.name}!";

            if (subtitleText != null)
                subtitleText.text = $"Te encuentras en la posición #{rank.posicion} global.\nPor ello, iniciarás este combate con:";
        }

        public void controlScreen1()
        {
            if (startScreen != null)
                startScreen.SetActive(false);
            if (controlsScreen1 != null)
                controlsScreen1.SetActive(true);
            if (controlsScreen2 != null)
                controlsScreen2.SetActive(false);
        }

        public void controlScreen2()
        {
            if (controlsScreen1 != null)
                controlsScreen1.SetActive(false);
            if (controlsScreen2 != null)
                controlsScreen2.SetActive(true);
            if (startScreen != null)
                startScreen.SetActive(false);
        }

        public void closeControls()
        {
            if (controlsScreen2 != null)
                controlsScreen2.SetActive(false);
            if (startScreen != null)
                startScreen.SetActive(true);
        }

        public void exitGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("GameSelectScene");
        }
    }
}
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace JorgeGame
{
    public class JorgeCreditsApi : MonoBehaviour
    {
        [Header("Api")]
        public string apiUrl = "https://jorge.onyx.14082006.xyz/api/jorge/minigame/creditos";

        [Header("Usuario")]
        public int userId;

        private bool creditsSent;

        public void SendCredits(int creditsEarned)
        {
            if (creditsSent || creditsEarned <= 0)
            {
                return;
            }

            creditsSent = true;
            userId = PlayerPrefs.GetInt("UserId", 1);
            StartCoroutine(PostCredits(creditsEarned));
        }

        private IEnumerator PostCredits(int creditsEarned)
        {
            // Creditos ganados
            CreditRequest data = new CreditRequest
            {
                UserId = userId,
                CreditsEarned = creditsEarned
            };

            string json = JsonUtility.ToJson(data);
            byte[] body = Encoding.UTF8.GetBytes(json);

            UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.certificateHandler = new ForceAcceptAll();

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error al agregar creditos: " + request.error);
                Debug.LogError("Respuesta API: " + request.downloadHandler.text);
            }
            else
            {
                Debug.Log("Creditos agregados correctamente: " + request.downloadHandler.text);
            }
        }

        [System.Serializable]
        public class CreditRequest
        {
            public int UserId;
            public int CreditsEarned;
        }
    }
}
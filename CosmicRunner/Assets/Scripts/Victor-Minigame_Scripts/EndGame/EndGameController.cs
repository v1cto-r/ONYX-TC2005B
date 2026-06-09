using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Text;
using UnityEngine.Networking;

public class EndGameController : MonoBehaviour
{
    public TMP_Text resultText;
    public TMP_Text creditsText;
    private string creditsURL = "https://victor.onyx.14082006.xyz/api/game/credits";


    void Start()
    {
        StartCoroutine(AddCredits());
        
        if (PlayerPrefs.GetInt("result") == 1)
        {
            resultText.text = "VICTORIA";
        }
        else
        {
            resultText.text = "DERROTA";
        }
        creditsText.text = "+" + PlayerPrefs.GetInt("collected_credits");
    }

    private IEnumerator AddCredits()
    {
        int userId = PlayerPrefs.GetInt("userId");
        int credits = PlayerPrefs.GetInt("collected_credits");

        string body = $"{{\"user_id\":\"{userId}\",\"credits\":{credits}}}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(body);

        UnityWebRequest request = new UnityWebRequest(creditsURL, "PATCH");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.certificateHandler = new ForceAcceptAll();

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error API: " + request.error);
            yield break;
        }

        string json = request.downloadHandler.text;
    }

    public void RestartGame()
    {
        PlayerPrefs.DeleteKey("collected_credits");
        PlayerPrefs.DeleteKey("result");
        SceneManager.LoadScene("GameScene_AB");
    }

    public void ExitToMenu()
    {
        PlayerPrefs.DeleteKey("collected_credits");
        PlayerPrefs.DeleteKey("result");
        SceneManager.LoadScene("MainMenuScene_AB");
    }
}

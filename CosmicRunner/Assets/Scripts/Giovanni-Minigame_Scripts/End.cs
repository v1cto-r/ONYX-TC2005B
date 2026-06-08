using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

namespace Gio.Minigame
{

[System.Serializable]
class CreditsRequest
{
    public int UserId;
    public int CreditsEarned;
}
public class End : MonoBehaviour
{
    public TMP_Text resultado;
    public TMP_Text creditsText;
    public string apiUrl = "https://127.0.0.1:12003/minigame/creditos";

    void Start()
    {
        if (PlayerPrefs.GetInt("resultado") == 1)
        {
            resultado.text = "VICTORIA";
            if (SFXManager.Instance != null) SFXManager.Instance.PlayWinSound();
        }
        else
        {
            resultado.text = "DERROTA";
            if (SFXManager.Instance != null) SFXManager.Instance.PlayLoseSound();
        }
        int creditosObtenidos = PlayerPrefs.GetInt("credits");
        creditsText.text = "+" + PlayerPrefs.GetInt("credits");

        int userId = PlayerPrefs.GetInt("UserId");
        StartCoroutine(AddCredits(userId, creditosObtenidos));
    }

    private IEnumerator AddCredits(int userId, int credits)
    {
        if (credits <= 0) yield break;

        CreditsRequest body = new CreditsRequest();
        body.UserId = userId;
        body.CreditsEarned = credits;

        string json = JsonUtility.ToJson(body);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

        UnityWebRequest web = new UnityWebRequest(apiUrl, "POST");

        web.uploadHandler = new UploadHandlerRaw(jsonBytes);
        web.downloadHandler = new DownloadHandlerBuffer();
        
        web.certificateHandler = new ForceAcceptAll();
        web.SetRequestHeader("Content-Type", "application/json");

        yield return web.SendWebRequest();

        if (web.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error en la petición: " + web.error);
        }
        else
        {
            Debug.Log("Créditos guardados exitosamente: " + web.downloadHandler.text);
        }
    }

    public void RestartGame()
    {
        PlayerPrefs.DeleteKey("credits");
        PlayerPrefs.DeleteKey("resultado");
        SceneManager.LoadScene("Invasores del Espacio");
    }

    public void ExitToMenu()
    {
        PlayerPrefs.DeleteKey("credits");
        PlayerPrefs.DeleteKey("resultado");
        SceneManager.LoadScene("IEGameStart");
    }
}
}

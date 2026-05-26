using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

namespace Nicte.Minigame{
public class WinUI : MonoBehaviour
{
    public TextMeshProUGUI finalWinScoreText;
    private string apiUrl = "https://127.0.0.1:5000/clasificacion/ataqueEstelar/agregar";

    [System.Serializable]
    class CreditsRequest
    {
        public int p_user_id;
        public int p_credits;
    }

    void Start()
    {
        SFXManager.Instance.WinSound();
        finalWinScoreText.text = GeneralUI.currentCredits.ToString();
        int userId = PlayerPrefs.GetInt("UserId");
        StartCoroutine(AddCredits(userId, GeneralUI.currentCredits));
    }

    IEnumerator AddCredits(int userId,int credits)
    {
        CreditsRequest body = new CreditsRequest();

        body.p_user_id =userId;
        body.p_credits =credits;

        string json =JsonUtility.ToJson(body);

        byte[] jsonBytes =Encoding.UTF8.GetBytes(json);

        UnityWebRequest web =new UnityWebRequest(apiUrl,"POST");

        web.uploadHandler =new UploadHandlerRaw(jsonBytes);
        web.downloadHandler =new DownloadHandlerBuffer();
        web.certificateHandler =new ForceAcceptAll();
        web.SetRequestHeader("Content-Type","application/json");

        yield return web.SendWebRequest();


        if(web.result !=UnityWebRequest.Result.Success)
        {
            Debug.LogError(
                web.error);
            yield break;
        }
    }

    public void playGame()
    {
            SceneManager.LoadScene("AtaqueEstelarGame");
    }
    
    public void backToMenu()
    {
            SceneManager.LoadScene("AtaqueEstelarGameStart");
    }
}
}
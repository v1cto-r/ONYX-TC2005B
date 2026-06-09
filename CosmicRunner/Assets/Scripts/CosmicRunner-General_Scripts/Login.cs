using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

namespace Nicte.Minigame
{
    [System.Serializable]
    public class LoginResponse
    {public int UserId;}

    [System.Serializable]
    public class LoginResponseArray
    {public LoginResponse[] items;}

    public class Login: MonoBehaviour
    {
        public TMP_InputField usernameInput;
        public TMP_InputField passwordInput;
        public TextMeshProUGUI errorText;
        public Toggle rememberToggle;

        private string apiUrl = "https://nicte.onyx.14082006.xyz/clasificacion/ataqueEstelar/";

        void Start()
        {
            if(PlayerPrefs.HasKey("savedUsername"))
            {
                usernameInput.text = PlayerPrefs.GetString("savedUsername");
                rememberToggle.isOn = true;
            }
        }
        public void OnLoginButton()
        {
            StartCoroutine(LoginU(usernameInput.text, passwordInput.text));
        }

        IEnumerator LoginU(string username, string password)
        {
            string url = apiUrl + username + "/" + password;

            UnityWebRequest web = UnityWebRequest.Get(url);
            web.certificateHandler = new ForceAcceptAll();

            yield return web.SendWebRequest();

            if (web.result != UnityWebRequest.Result.Success)
            {
                errorText.text = "Error de conexión.";
                yield break;
            }

            string wrapped = "{\"items\":" + web.downloadHandler.text + "}";
            LoginResponseArray response = JsonUtility.FromJson<LoginResponseArray>(wrapped);
            Debug.Log($"Login response: {response.items[0].UserId}");

            if (response.items.Length > 0 && response.items[0].UserId > 0)
            {
                if (rememberToggle.isOn)
                {
                    PlayerPrefs.SetString("savedUsername", username);
                }
                else
                {
                    PlayerPrefs.DeleteKey("savedUsername");
                }
                PlayerPrefs.SetInt("UserId", response.items[0].UserId);
                SceneManager.LoadScene("MainMenuScene");
            }
            else
            {
                errorText.text = "Usuario o contraseña incorrectos.";
            }
        }
    }
}
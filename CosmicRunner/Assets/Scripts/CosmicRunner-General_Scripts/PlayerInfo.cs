using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class PlayerInfo : MonoBehaviour
{
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI creditsText;

    // Ajustamos la URL base para el endpoint de perfiles
    public string APIBaseUrl = "https://marino.onyx.14082006.xyz/usuario/perfil/";
    
    // ID del usuario que deseas consultar (puedes cambiarlo dinámicamente)
    private int userId;

    private void Start()
    {
        userId = PlayerPrefs.GetInt("UserId", 1); // Aseguramos que el ID del usuario se obtenga al iniciar
        // Iniciamos la corrutina para obtener los datos desde la API
        StartCoroutine(FetchPlayerInfo(userId));
    }

    private IEnumerator FetchPlayerInfo(int id)
    {
        string url = APIBaseUrl + id;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Enviamos la solicitud a la API y esperamos la respuesta
            yield return webRequest.SendWebRequest();

            // Validamos si ocurrió algún error de red o de protocolo
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error al conectar con la API: " + webRequest.error);
                // Si la red falla, cargamos los últimos datos guardados localmente como respaldo
                LoadLocalPlayerInfo();
            }
            else
            {
                // Obtenemos el texto en formato JSON
                string jsonResponse = webRequest.downloadHandler.text;

                // Deserializamos el JSON mapeándolo con las clases correspondientes
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(jsonResponse);

                if (response != null && response.perfil != null)
                {
                    // Asignamos el nombre del usuario (puedes cambiarlo por response.perfil.Username si prefieres el apodo)
                    usernameText.text = response.perfil.Nombre;
                    
                    // Asignamos la cantidad de créditos obtenidos
                    creditsText.text = response.perfil.CreditosTiendita.ToString();

                    // Opcional: Guardamos los datos actualizados de forma local
                    PlayerPrefs.SetString("Username", response.perfil.Nombre);
                    PlayerPrefs.SetInt("Credits", response.perfil.CreditosTiendita);
                    PlayerPrefs.Save();
                }
            }
        }
    }

    // Método de respaldo por si el usuario no tiene conexión a internet
    private void LoadLocalPlayerInfo()
    {
        string username = PlayerPrefs.GetString("Username", "Player");
        int credits = PlayerPrefs.GetInt("Credits", 0);

        usernameText.text = username;
        creditsText.text = credits.ToString();
    }
}

[System.Serializable]
public class PerfilData
{
    public string Username;
    public string Nombre;
    public int CreditosTiendita;
}

[System.Serializable]
public class ApiResponse
{
    public PerfilData perfil;
}
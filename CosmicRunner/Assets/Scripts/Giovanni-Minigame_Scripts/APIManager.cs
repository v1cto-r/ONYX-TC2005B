using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class APIAttackManager : MonoBehaviour
{
    public static APIAttackManager Instance;
    public List<Prompt> listaPrompts = new List<Prompt>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CargarPrompts();
    }

    public void CargarPrompts()
    {
        StartCoroutine(GetPromptsRoutine());
    }

    private IEnumerator GetPromptsRoutine()
    {
        UnityWebRequest web = UnityWebRequest.Get("https://10.22.228.205:8443/prompts");
        
        web.certificateHandler = new ForceAceptAll(); 

        yield return web.SendWebRequest();

        if (web.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error de conexión API: " + web.error);
        }
        else
        {
            try
            {
                listaPrompts = JsonConvert.DeserializeObject<List<Prompt>>(web.downloadHandler.text);
                Debug.Log($"Prompts: {listaPrompts.Count}");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error: " + e.Message);
            }
        }
    }
}
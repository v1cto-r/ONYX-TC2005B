using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
namespace Gio.Minigame{
public class APIAttackManager : MonoBehaviour
{
    public static APIAttackManager Instance;
    public List<Prompt> listaPrompts = new List<Prompt>();

    private void Awake()
    {
        Instance = this;
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
        UnityWebRequest web = UnityWebRequest.Get("https://gio.onyx.14082006.xyz/api/prompts");
        
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
}
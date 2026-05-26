using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace AB
{
    [System.Serializable]
    public class PromptPart
    {
        public int id;
        public string texto;
        public int orden;
    }

    public class RepairController : MonoBehaviour
    {
        private string promptsUrl = "https://127.0.0.1:12002/api/AB/game/prompts";

        public List<PromptPart> prompts = new List<PromptPart>();

        private void Awake()
        {
            StartCoroutine(LoadPrompts());
        }

        private IEnumerator LoadPrompts()
        {
            UnityWebRequest request = UnityWebRequest.Get(promptsUrl);
            request.certificateHandler = new ForceAcceptAll();

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error API: " + request.error);
                yield break;
            }

            string json = request.downloadHandler.text;
            Debug.Log("Response: " + json);

            prompts = JsonConvert.DeserializeObject<List<PromptPart>>(json);
        }

        public void RenewPrompt()
        {
            StartCoroutine(LoadPrompts());
        }

        
    }
}

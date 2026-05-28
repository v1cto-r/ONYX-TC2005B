using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace AB
{
    // Parte del prompt como viene del API
    [System.Serializable]
    public class PromptPart
    {
        public int part_id;
        public string fragment;
        public int fragment_order;
        public int prompt;
    }

    public class RepairController : MonoBehaviour
    {
        public static RepairController Instance { get; set; }

        // URL del API para obtener los prompts
        private string promptsUrl = "https://127.0.0.1:12002/api/game/prompt";
        // Lista de partes del prompt obtenidas del API
        private List<PromptPart> prompts = new List<PromptPart>();
        // Si se reparó la nave
        private bool finished;
        // Chips para mostrar los fragmentos del prompt
        public List<ChipController> chips;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            finished = false;
            StartCoroutine(LoadPrompts());
        }

        // Consigue los prompts del API
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
            prompts = JsonConvert.DeserializeObject<List<PromptPart>>(json);
            AssignPromptsToChips(); // Asigna los fragmentos a los chips para mostrarlos
        }

        private void AssignPromptsToChips()
        {
            for (int i = 0; i < chips.Count; i++)
            {
                // Asegura que el chip vuelva a su posición inicial
                chips[i].ReturnToStartPosition();
                // Asigna el fragmento del prompt al chip correspondiente
                chips[i].SetFragment(prompts[i].fragment, prompts[i].fragment_order);
            }
        }

        // Permite renovar los prompts, al usar reparar varias veces
        public void RenewPrompt()
        {
            StartCoroutine(LoadPrompts());
        }

        // Se activa cuando un chip es colocado 
        public void ChipPlaced()
        {
            RecalculatePlacedCount();
        }

        // Revisa cuantos chips están colocados correctamente, y si todos lo están, se llama a Finished()
        private void RecalculatePlacedCount()
        {
            if (chips.Count == 0) return;
            int correctCount = 0;
            for (int i = 0; i < chips.Count; i++)
            {
                ChipSlotController slot = chips[i].transform.parent?.GetComponent<ChipSlotController>();
                if (slot != null && chips[i].FragmentOrder == slot.slotOrder)
                    correctCount++;
            }

            if (correctCount < chips.Count)
                finished = false;

            if (!finished && correctCount >= chips.Count)
            {
                finished = true;
                Finished();
            }
        }

        // Se llama cuando se han colocado todos los chips correctamente
        public void Finished()
        {
            GameController.Instance.FinishRepair();
        }
    }
}

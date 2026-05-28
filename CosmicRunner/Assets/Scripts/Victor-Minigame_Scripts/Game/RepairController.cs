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
        public int part_id;
        public string fragment;
        public int fragment_order;
        public int prompt;
    }

    public class RepairController : MonoBehaviour
    {
        public static RepairController Instance { get; set; }

        private string promptsUrl = "https://127.0.0.1:12002/api/game/prompt";
        private List<PromptPart> prompts = new List<PromptPart>();
        private bool finished;
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
            AssignPromptsToChips();
        }

        private void AssignPromptsToChips()
        {
            for (int i = 0; i < chips.Count; i++)
            {
                chips[i].ReturnToStartPosition();
                chips[i].SetFragment(prompts[i].fragment, prompts[i].fragment_order);
            }
        }

        public void RenewPrompt()
        {
            StartCoroutine(LoadPrompts());
        }

        public void ChipPlaced()
        {
            RecalculatePlacedCount();
        }

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

        public void Finished()
        {
            GameController.Instance.FinishRepair();
        }
    }
}

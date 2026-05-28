using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

namespace JorgeGame
{
    public class UIQuestionManager : MonoBehaviour
    {
        [Header("Api")]
        public string apiUrl = "https://127.0.0.1:5057/api/jorge/pregunta-aleatoria";

        [Header("UI")]
        public GameObject questionPanel;
        public TextMeshProUGUI questionText;
        public Button[] answerButtons;
        public TextMeshProUGUI[] answerTexts;

        [Header("Mensaje opcional")]
        public TextMeshProUGUI statusText;

        private Vector3 pendingCheckpoint;
        private Checkpoint currentCheckpoint;
        private JorgeQuestion currentQuestion;

        void Start()
        {
            if (questionPanel != null)
                questionPanel.SetActive(false);

            ClearButtons();
        }

        public void ShowQuestion(Vector3 checkpointPos, Checkpoint checkpoint)
        {
            pendingCheckpoint = checkpointPos;
            currentCheckpoint = checkpoint;

            if (questionPanel != null)
                questionPanel.SetActive(true);

            if (questionText != null)
                questionText.text = "Cargando pregunta...";

            if (statusText != null)
                statusText.text = "";

            ClearButtons();

            Time.timeScale = 0f;

            StartCoroutine(GetRandomQuestion());
        }

        private IEnumerator GetRandomQuestion()
        {
            UnityWebRequest web = UnityWebRequest.Get(apiUrl);
            web.certificateHandler = new ForceAcceptAll();
            web.timeout = 10;

            yield return web.SendWebRequest();

            if (web.result != UnityWebRequest.Result.Success)
            {
                ShowError("No se pudo cargar la pregunta desde la API.");
                Debug.LogError(web.error);
                yield break;
            }

            string json = web.downloadHandler.text;
            JorgeQuestionResponse response = JsonUtility.FromJson<JorgeQuestionResponse>(json);

            if (response == null || !response.exito || response.pregunta == null || response.pregunta.answers == null)
            {
                ShowError("La API respondio, pero los datos no llegaron completos.");
                Debug.LogError(json);
                yield break;
            }

            currentQuestion = response.pregunta;
            ShowQuestionData();
        }

        private void ShowQuestionData()
        {
            if (currentQuestion == null)
                return;

            if (questionText != null)
                questionText.text = currentQuestion.question_text;

            int totalAnswers = Mathf.Min(answerButtons.Length, answerTexts.Length, currentQuestion.answers.Count);

            for (int i = 0; i < totalAnswers; i++)
            {
                int index = i;

                answerTexts[i].text = GetLetter(i) + ") " + currentQuestion.answers[i].answer_text;

                answerButtons[i].interactable = true;
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => SelectAnswer(index));
            }

            if (statusText != null)
                statusText.text = "";
        }

        private void SelectAnswer(int answerIndex)
        {
            if (currentQuestion == null)
                return;

            if (answerIndex < 0 || answerIndex >= currentQuestion.answers.Count)
                return;

            bool isCorrect = currentQuestion.answers[answerIndex].is_correct;

            if (isCorrect)
                CorrectAnswer();
            else
                WrongAnswer();
        }

        private void CorrectAnswer()
        {
            if (SFXManager.instance != null)
                SFXManager.instance.PlaySFX(SFXManager.instance.checkpointSound, 0.5f);

            SpawnPoint.instance.respawnPoint = pendingCheckpoint;

            if (currentCheckpoint != null)
                currentCheckpoint.SetCorrect();

            CloseQuestion();
        }

        private void WrongAnswer()
        {
            GameControl.Instance.SpendLives();

            if (currentCheckpoint != null)
                currentCheckpoint.SetWrong();

            CloseQuestion();
        }

        private void CloseQuestion()
        {
            if (questionPanel != null)
                questionPanel.SetActive(false);

            ClearButtons();

            currentQuestion = null;
            currentCheckpoint = null;

            Time.timeScale = 1f;
        }

        private void ClearButtons()
        {
            if (answerButtons != null)
            {
                for (int i = 0; i < answerButtons.Length; i++)
                {
                    if (answerButtons[i] != null)
                    {
                        answerButtons[i].interactable = false;
                        answerButtons[i].onClick.RemoveAllListeners();
                    }
                }
            }

            if (answerTexts != null)
            {
                for (int i = 0; i < answerTexts.Length; i++)
                {
                    if (answerTexts[i] != null)
                        answerTexts[i].text = "";
                }
            }
        }

        private void ShowError(string message)
        {
            if (questionText != null)
                questionText.text = message;

            if (statusText != null)
                statusText.text = "Revisa que la API este encendida.";

            Debug.LogWarning(message);
        }

        private string GetLetter(int index)
        {
            switch (index)
            {
                case 0:
                    return "A";
                case 1:
                    return "B";
                case 2:
                    return "C";
                default:
                    return "";
            }
        }
    }

    [System.Serializable]
    public class JorgeQuestionResponse
    {
        public bool exito;
        public JorgeQuestion pregunta;
    }

    [System.Serializable]
    public class JorgeQuestion
    {
        public int question_id;
        public string question_text;
        public List<JorgeAnswer> answers;
    }

    [System.Serializable]
    public class JorgeAnswer
    {
        public int answer_id;
        public string answer_text;
        public bool is_correct;
    }
}
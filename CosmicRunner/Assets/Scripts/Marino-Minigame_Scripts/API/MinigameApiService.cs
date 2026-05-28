using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace MECS
{
    public class MinigameApiService
    {
        private readonly string baseUrl;

        public MinigameApiService(string baseUrl)
        {
            this.baseUrl = baseUrl.TrimEnd('/');
        }

        public IEnumerator GetWords(Action<List<string>> onSuccess)
        {
            using (UnityWebRequest web = UnityWebRequest.Get($"{baseUrl}/palabras"))
            {
                web.certificateHandler = new ForceAcceptAll();
                yield return web.SendWebRequest();

                if (web.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"GET /minigame/palabras failed. Status: {web.responseCode}. Body: {web.downloadHandler.text}");
                    yield break;
                }

                string rawBody = web.downloadHandler.text;

                List<string> words = new List<string>();

                List<string> wordObjects = MinigameApiServiceHelpers.ExtractTopLevelObjects(rawBody);

                for (int i = 0; i < wordObjects.Count; i++)
                {
                    string wordJson = wordObjects[i];
                    string wordText = MinigameApiServiceHelpers.GetFirstStringValue(wordJson,
                        "WordText", "wordText", "Word", "word", "palabra", "texto", "name", "value");

                    if (!string.IsNullOrWhiteSpace(wordText))
                    {
                        words.Add(wordText);
                    }
                }

                Debug.Log($"GET /minigame/palabras returned {words.Count} usable words.");

                onSuccess?.Invoke(words);
            }
        }

        public IEnumerator GetPrompts(Action<List<PromptEntry>> onSuccess)
        {
            using (UnityWebRequest web = UnityWebRequest.Get($"{baseUrl}/prompts"))
            {
                web.certificateHandler = new ForceAcceptAll();
                yield return web.SendWebRequest();

                if (web.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"GET /minigame/prompts failed. Status: {web.responseCode}. Body: {web.downloadHandler.text}");
                    yield break;
                }

                string rawBody = web.downloadHandler.text;

                List<PromptEntry> prompts = new List<PromptEntry>();

                List<string> promptObjects = MinigameApiServiceHelpers.ExtractTopLevelObjects(rawBody);

                for (int i = 0; i < promptObjects.Count; i++)
                {
                    string promptJson = promptObjects[i];
                    PromptEntry prompt = new PromptEntry
                    {
                        incompletePrompt = MinigameApiServiceHelpers.GetFirstStringValue(promptJson,
                            "PromptText", "promptText", "incompletePrompt", "Prompt", "prompt", "frase", "enunciado", "texto") ?? string.Empty,
                        goodAnswers = MinigameApiServiceHelpers.GetAnswers(promptJson,
                            "GoodAnswers", "goodAnswers", "good_answers", "answersGood", "buenasRespuestas", "respuestasBuenas"),
                        okAnswers = MinigameApiServiceHelpers.GetAnswers(promptJson,
                            "OkAnswers", "okAnswers", "ok_answers", "answersOk", "respuestasOk"),
                        mehAnswers = MinigameApiServiceHelpers.GetAnswers(promptJson,
                            "MehAnswers", "mehAnswers", "meh_answers", "answersMeh", "respuestasMeh")
                    };

                    bool hasAnyAnswers = (prompt.goodAnswers != null && prompt.goodAnswers.Length > 0)
                        || (prompt.okAnswers != null && prompt.okAnswers.Length > 0)
                        || (prompt.mehAnswers != null && prompt.mehAnswers.Length > 0);

                    if (!hasAnyAnswers)
                    {
                        Debug.LogWarning($"GET /minigame/prompts item {i} skipped because it has no usable answers.");
                        continue;
                    }

                    prompts.Add(prompt);
                }

                Debug.Log($"GET /minigame/prompts returned {prompts.Count} usable prompts.");

                onSuccess?.Invoke(prompts);
            }
        }

        public IEnumerator PostCredits(int userId, int creditsEarned)
        {
            MinigameCreditsRequest payload = new MinigameCreditsRequest
            {
                UserId = userId,
                CreditsEarned = creditsEarned
            };

            string json = JsonUtility.ToJson(payload);
            byte[] body = Encoding.UTF8.GetBytes(json);

            using (UnityWebRequest web = new UnityWebRequest($"{baseUrl}/creditos", UnityWebRequest.kHttpVerbPOST))
            {
                web.certificateHandler = new ForceAcceptAll();
                web.uploadHandler = new UploadHandlerRaw(body);
                web.downloadHandler = new DownloadHandlerBuffer();
                web.SetRequestHeader("Content-Type", "application/json");

                yield return web.SendWebRequest();

                if (web.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"POST /minigame/creditos failed. Status: {web.responseCode}. Body: {web.downloadHandler.text}");
                    yield break;
                }
            }
        }
    }

    [Serializable]
    internal class MinigameCreditsRequest
    {
        public int UserId;
        public int CreditsEarned;
    }

    internal static class MinigameApiServiceHelpers
    {
        public static string[] SplitAnswers(string answerList)
        {
            if (string.IsNullOrWhiteSpace(answerList))
            {
                return Array.Empty<string>();
            }

            return answerList.Split('|', StringSplitOptions.RemoveEmptyEntries);
        }

        public static string GetFirstStringValue(string jsonObject, params string[] keys)
        {
            if (string.IsNullOrWhiteSpace(jsonObject) || keys == null)
            {
                return null;
            }

            foreach (string key in keys)
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                string pattern = "\\\"" + Regex.Escape(key) + "\\\"\\s*:\\s*\\\"((?:\\\\.|[^\\\"])*)\\\"";
                Match match = Regex.Match(jsonObject, pattern);
                if (match.Success)
                {
                    return Regex.Unescape(match.Groups[1].Value);
                }
            }

            return null;
        }

        public static string[] GetAnswers(string jsonObject, params string[] keys)
        {
            string answerText = GetFirstStringValue(jsonObject, keys);
            if (!string.IsNullOrWhiteSpace(answerText))
            {
                return SplitAnswers(answerText);
            }

            if (string.IsNullOrWhiteSpace(jsonObject) || keys == null)
            {
                return Array.Empty<string>();
            }

            foreach (string key in keys)
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                string pattern = "\\\"" + Regex.Escape(key) + "\\\"\\s*:\\s*\\[(.*?)\\]";
                Match match = Regex.Match(jsonObject, pattern, RegexOptions.Singleline);
                if (!match.Success)
                {
                    continue;
                }

                List<string> answers = new List<string>();
                MatchCollection valueMatches = Regex.Matches(match.Groups[1].Value, "\\\"((?:\\\\.|[^\\\"])*)\\\"");
                foreach (Match valueMatch in valueMatches)
                {
                    string value = Regex.Unescape(valueMatch.Groups[1].Value);
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        answers.Add(value);
                    }
                }

                return answers.ToArray();
            }

            return Array.Empty<string>();
        }

        public static List<string> ExtractTopLevelObjects(string jsonArray)
        {
            List<string> objects = new List<string>();

            if (string.IsNullOrWhiteSpace(jsonArray))
            {
                return objects;
            }

            bool inString = false;
            bool isEscaped = false;
            int depth = 0;
            int objectStart = -1;

            for (int i = 0; i < jsonArray.Length; i++)
            {
                char current = jsonArray[i];

                if (inString)
                {
                    if (isEscaped)
                    {
                        isEscaped = false;
                    }
                    else if (current == '\\')
                    {
                        isEscaped = true;
                    }
                    else if (current == '"')
                    {
                        inString = false;
                    }

                    continue;
                }

                if (current == '"')
                {
                    inString = true;
                    continue;
                }

                if (current == '{')
                {
                    if (depth == 0)
                    {
                        objectStart = i;
                    }

                    depth++;
                    continue;
                }

                if (current == '}')
                {
                    depth--;
                    if (depth == 0 && objectStart >= 0)
                    {
                        objects.Add(jsonArray.Substring(objectStart, i - objectStart + 1));
                        objectStart = -1;
                    }
                }
            }

            return objects;
        }
    }
}

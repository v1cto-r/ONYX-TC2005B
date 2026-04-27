using UnityEngine;

public enum PromptQuality
{
    Meh,
    Ok,
    Good
}

[System.Serializable]
public class PromptEntry
{
    [TextArea(2, 4)] public string incompletePrompt;
    
    [Tooltip("Best answers for this prompt")]
    public string[] goodAnswers = new string[] { };
    
    [Tooltip("Acceptable answers for this prompt")]
    public string[] okAnswers = new string[] { };
    
    [Tooltip("Mediocre answers for this prompt")]
    public string[] mehAnswers = new string[] { };
}

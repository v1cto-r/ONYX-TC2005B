using TMPro;
using UnityEngine;

[System.Serializable]
public class PromptSlot
{
    public Collider2D promptCollider;
    public Collider2D submitCollider;
    public Collider2D cancelCollider;
    public TMP_Text promptLabel;
    public string blankToken = "_____";

    [HideInInspector] public PromptEntry assignedPrompt;
    [HideInInspector] public string droppedWord = string.Empty;
    [HideInInspector] public DraggableWord sourceWord;
    [HideInInspector] public bool isFilled;
    [HideInInspector] public bool isSubmitted;

    public void Clear()
    {
        assignedPrompt = null;
        droppedWord = string.Empty;
        sourceWord = null;
        isFilled = false;
        isSubmitted = false;

        if (promptLabel != null)
        {
            promptLabel.text = string.Empty;
        }
    }

    public void SetPrompt(PromptEntry prompt)
    {
        assignedPrompt = prompt;
        droppedWord = string.Empty;
        sourceWord = null;
        isFilled = false;
        isSubmitted = false;

        if (promptLabel != null)
        {
            promptLabel.text = prompt != null ? prompt.incompletePrompt : string.Empty;
        }
    }

    public bool ContainsPoint(Vector2 worldPoint)
    {
        return promptCollider != null && promptCollider.OverlapPoint(worldPoint);
    }

    public bool ContainsSubmitPoint(Vector2 worldPoint)
    {
        return submitCollider != null && submitCollider.OverlapPoint(worldPoint);
    }

    public bool ContainsCancelPoint(Vector2 worldPoint)
    {
        return cancelCollider != null && cancelCollider.OverlapPoint(worldPoint);
    }

    public bool TryFillWithWord(string word, DraggableWord source = null)
    {
        if (isSubmitted || isFilled || string.IsNullOrWhiteSpace(word) || promptLabel == null)
        {
            return false;
        }

        string sourceText = assignedPrompt != null && !string.IsNullOrWhiteSpace(assignedPrompt.incompletePrompt)
            ? assignedPrompt.incompletePrompt
            : promptLabel.text;

        if (string.IsNullOrWhiteSpace(sourceText))
        {
            return false;
        }

        string completedText = ReplaceFirstBlank(sourceText, blankToken, word);
        if (completedText == sourceText)
        {
            return false;
        }

        promptLabel.text = completedText;
        droppedWord = word;
        sourceWord = source;
        isFilled = true;
        return true;
    }

    public void CancelFill()
    {
        if (assignedPrompt == null || isSubmitted)
        {
            return;
        }

        if (sourceWord != null && !string.IsNullOrWhiteSpace(droppedWord))
        {
            sourceWord.SetWord(droppedWord);
        }

        droppedWord = string.Empty;
        sourceWord = null;
        isFilled = false;

        if (promptLabel != null)
        {
            promptLabel.text = assignedPrompt.incompletePrompt;
        }
    }

    private static string ReplaceFirstBlank(string sourceText, string token, string replacement)
    {
        if (string.IsNullOrEmpty(sourceText) || string.IsNullOrEmpty(token))
        {
            return sourceText;
        }

        int tokenIndex = sourceText.IndexOf(token, System.StringComparison.Ordinal);
        if (tokenIndex < 0)
        {
            return sourceText;
        }

        return sourceText.Substring(0, tokenIndex)
            + replacement
            + sourceText.Substring(tokenIndex + token.Length);
    }
}

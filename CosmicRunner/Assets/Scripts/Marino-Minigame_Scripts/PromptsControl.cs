using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PromptsControl : MonoBehaviour
{
    [SerializeField] private string[] boxWords;
    [SerializeField] private DraggableWord[] wordSlots;
    [SerializeField] private PromptEntry[] promptPool;
    [SerializeField] private PromptSlot[] promptSlots;
    [SerializeField] private bool clearSlotsOnStart = true;
    [SerializeField] private bool assignRandomPromptsOnStart = true;
    [SerializeField] private int mehPoints = 5;
    [SerializeField] private int okPoints = 10;
    [SerializeField] private int goodPoints = 15;
    [SerializeField] private string clickActionName = "Drag";
    [SerializeField] private string pointerActionName = "Point";
    [SerializeField] private Camera worldCamera;

    private InputAction clickAction;
    private InputAction pointerAction;
    private int currentWordCount;

    public int TotalScore { get; private set; }
    public int CurrentWordCount => currentWordCount;
    public int WordStorageCapacity => wordSlots != null ? wordSlots.Length : 0;

    public static PromptsControl Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (worldCamera == null)
        {
            worldCamera = Camera.main;
        }

        ResolveInputActions();
        InitializeGameplayState();
    }

    private void OnEnable()
    {
        ResolveInputActions();

        if (clickAction != null)
        {
            clickAction.Enable();
        }

        if (pointerAction != null)
        {
            pointerAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (clickAction != null)
        {
            clickAction.Disable();
        }

        if (pointerAction != null)
        {
            pointerAction.Disable();
        }
    }

    private void Update()
    {
        if (clickAction == null || pointerAction == null || worldCamera == null)
        {
            return;
        }

        if (!clickAction.WasPressedThisFrame())
        {
            return;
        }

        Vector2 pointerScreen = pointerAction.ReadValue<Vector2>();
        Vector3 pointerWorld3 = worldCamera.ScreenToWorldPoint(new Vector3(pointerScreen.x, pointerScreen.y, 0f));
        Vector2 pointerWorld = new Vector2(pointerWorld3.x, pointerWorld3.y);

        HandlePromptActionClick(pointerWorld);
    }

    public void HandleBoxCollected()
    {
        if (boxWords == null || boxWords.Length == 0)
        {
            Debug.LogWarning("No words configured in PromptsControl.", this);
            return;
        }

        if (wordSlots == null || wordSlots.Length == 0)
        {
            Debug.LogWarning("No word slots configured in PromptsControl.", this);
            return;
        }

        string randomWord = boxWords[Random.Range(0, boxWords.Length)];
        bool wasAssigned = AssignToFirstEmptySlot(randomWord);

        if (!wasAssigned)
        {
            Debug.Log("All word slots are full. Collected word was ignored.", this);
        }

        RefreshWordCount();
    }

    public void ClearAllWordSlots()
    {
        if (wordSlots == null)
        {
            return;
        }

        foreach (DraggableWord slot in wordSlots)
        {
            if (slot == null)
            {
                continue;
            }

            slot.ClearWord();
        }

        RefreshWordCount();
    }

    public void NotifyWordSlotsChanged()
    {
        RefreshWordCount();
    }

    private void InitializeGameplayState()
    {
        if (clearSlotsOnStart)
        {
            ClearAllWordSlots();
        }

        if (assignRandomPromptsOnStart)
        {
            ClearAllPromptSlots();
            AssignRandomPromptsToSlots();
        }

        RefreshWordCount();
    }

    public bool TryFillBlankAtWorldPoint(Vector2 worldPoint, string word, DraggableWord source = null)
    {
        if (promptSlots == null || promptSlots.Length == 0)
        {
            return false;
        }

        foreach (PromptSlot slot in promptSlots)
        {
            if (slot == null || !slot.ContainsPoint(worldPoint))
            {
                continue;
            }

            if (slot.TryFillWithWord(word, source))
            {
                return true;
            }
        }

        return false;
    }

    public void SubmitPrompt(int slotIndex)
    {
        if (!IsValidSlotIndex(slotIndex))
        {
            return;
        }

        PromptSlot slot = promptSlots[slotIndex];
        if (slot == null || slot.assignedPrompt == null || slot.isSubmitted || !slot.isFilled)
        {
            return;
        }

        string droppedWordTrimmed = slot.droppedWord.Trim();
        PromptQuality matchQuality = CheckAnswerQuality(slot.assignedPrompt, droppedWordTrimmed);
        bool hasScoringMatch = matchQuality == PromptQuality.Meh
            || matchQuality == PromptQuality.Ok
            || matchQuality == PromptQuality.Good;

        if (hasScoringMatch)
        {
            TotalScore += GetPointsForQuality(matchQuality);
        }

        PromptEntry currentPrompt = slot.assignedPrompt;
        slot.isSubmitted = true;
        AssignRandomPromptToSlot(slotIndex, currentPrompt);
    }

    private void AssignRandomPromptToSlot(int slotIndex, PromptEntry promptToAvoid)
    {
        if (!IsValidSlotIndex(slotIndex) || promptPool == null || promptPool.Length == 0)
        {
            return;
        }

        PromptSlot slot = promptSlots[slotIndex];
        if (slot == null)
        {
            return;
        }

        PromptEntry nextPrompt = GetRandomPrompt(promptToAvoid);
        slot.SetPrompt(nextPrompt);
    }

    private PromptEntry GetRandomPrompt(PromptEntry promptToAvoid)
    {
        if (promptPool == null || promptPool.Length == 0)
        {
            return null;
        }

        if (promptPool.Length == 1 || promptToAvoid == null)
        {
            return promptPool[Random.Range(0, promptPool.Length)];
        }

        for (int i = 0; i < 8; i++)
        {
            PromptEntry candidate = promptPool[Random.Range(0, promptPool.Length)];
            if (candidate != promptToAvoid)
            {
                return candidate;
            }
        }

        foreach (PromptEntry prompt in promptPool)
        {
            if (prompt != promptToAvoid)
            {
                return prompt;
            }
        }

        return promptPool[0];
    }

    private PromptQuality CheckAnswerQuality(PromptEntry prompt, string word)
    {
        if (prompt == null || string.IsNullOrWhiteSpace(word))
        {
            return (PromptQuality)(-1);
        }

        if (IsWordInArray(prompt.goodAnswers, word))
        {
            return PromptQuality.Good;
        }

        if (IsWordInArray(prompt.okAnswers, word))
        {
            return PromptQuality.Ok;
        }

        if (IsWordInArray(prompt.mehAnswers, word))
        {
            return PromptQuality.Meh;
        }

        return (PromptQuality)(-1);
    }

    private bool IsWordInArray(string[] array, string word)
    {
        if (array == null || array.Length == 0)
        {
            return false;
        }

        foreach (string item in array)
        {
            if (string.Equals(item.Trim(), word, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public void CancelPrompt(int slotIndex)
    {
        if (!IsValidSlotIndex(slotIndex))
        {
            return;
        }

        PromptSlot slot = promptSlots[slotIndex];
        if (slot == null || slot.assignedPrompt == null)
        {
            return;
        }

        if (slot.isSubmitted)
        {
            return;
        }

        slot.CancelFill();
    }

    public void ClearAllPromptSlots()
    {
        if (promptSlots == null)
        {
            return;
        }

        foreach (PromptSlot slot in promptSlots)
        {
            if (slot == null)
            {
                continue;
            }

            slot.Clear();
        }
    }

    public void AssignRandomPromptsToSlots()
    {
        if (promptPool == null || promptPool.Length == 0 || promptSlots == null || promptSlots.Length == 0)
        {
            return;
        }

        // For testing: allow repeats by selecting directly from promptPool.
        // Repeat-prevention logic is intentionally disabled below.
        // List<PromptEntry> availablePrompts = new List<PromptEntry>(promptPool);

        for (int i = 0; i < promptSlots.Length; i++)
        {
            PromptSlot slot = promptSlots[i];
            if (slot == null)
            {
                continue;
            }

            // if (availablePrompts.Count == 0)
            // {
            //     Debug.LogWarning("Not enough unique prompts for all slots.", this);
            //     break;
            // }

            // int randomIndex = Random.Range(0, availablePrompts.Count);
            // PromptEntry randomPrompt = availablePrompts[randomIndex];
            // availablePrompts.RemoveAt(randomIndex);

            PromptEntry randomPrompt = promptPool[Random.Range(0, promptPool.Length)];
            slot.SetPrompt(randomPrompt);
        }
    }

    private int GetPointsForQuality(PromptQuality quality)
    {
        switch (quality)
        {
            case PromptQuality.Meh:
                return mehPoints;
            case PromptQuality.Good:
                return goodPoints;
            default:
                return okPoints;
        }
    }

    private bool IsValidSlotIndex(int slotIndex)
    {
        return promptSlots != null && slotIndex >= 0 && slotIndex < promptSlots.Length;
    }

    private bool AssignToFirstEmptySlot(string word)
    {
        foreach (DraggableWord slot in wordSlots)
        {
            if (slot == null || slot.HasWord)
            {
                continue;
            }

            slot.SetWord(word);
            return true;
        }

        return false;
    }

    private void HandlePromptActionClick(Vector2 worldPoint)
    {
        if (promptSlots == null)
        {
            return;
        }

        for (int i = 0; i < promptSlots.Length; i++)
        {
            PromptSlot slot = promptSlots[i];
            if (slot == null)
            {
                continue;
            }

            if (slot.ContainsSubmitPoint(worldPoint))
            {
                SubmitPrompt(i);
                return;
            }

            if (slot.ContainsCancelPoint(worldPoint))
            {
                CancelPrompt(i);
                return;
            }
        }
    }

    private void ResolveInputActions()
    {
        if (clickAction == null)
        {
            clickAction = InputSystem.actions.FindAction(clickActionName);
        }

        if (pointerAction == null)
        {
            pointerAction = InputSystem.actions.FindAction(pointerActionName);
        }
    }

    private void RefreshWordCount()
    {
        if (wordSlots == null || wordSlots.Length == 0)
        {
            currentWordCount = 0;
            return;
        }

        int count = 0;
        foreach (DraggableWord slot in wordSlots)
        {
            if (slot != null && slot.HasWord)
            {
                count++;
            }
        }

        currentWordCount = count;
    }
}

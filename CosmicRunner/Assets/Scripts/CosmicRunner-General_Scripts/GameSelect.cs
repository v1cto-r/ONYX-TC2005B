using UnityEngine;
using UnityEngine.UI;

public class GameSelect : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button gioButton;
    [SerializeField] private Button victorButton;
    [SerializeField] private Button marinoButton;
    [SerializeField] private Button jorgeButton;
    [SerializeField] private Button nicteButton;

    [Header("Sprites")]
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite gioBeatenSprite;
    [SerializeField] private Sprite victorBeatenSprite;
    [SerializeField] private Sprite marinoBeatenSprite;
    [SerializeField] private Sprite jorgeBeatenSprite;
    [SerializeField] private Sprite nicteBeatenSprite;

    private void Start()
    {
        RefreshButtons();
    }

    private void RefreshButtons()
    {
        bool gioBeaten = MinigameProgress.IsBeaten(MinigameProgress.GiovanniId);
        bool victorBeaten = MinigameProgress.IsBeaten(MinigameProgress.VictorId);
        bool marinoBeaten = MinigameProgress.IsBeaten(MinigameProgress.MarinoId);
        bool jorgeBeaten = MinigameProgress.IsBeaten(MinigameProgress.JorgeId);
        bool nicteBeaten = MinigameProgress.IsBeaten(MinigameProgress.NicteId);

        SetButtonState(gioButton, true, gioBeaten, gioBeatenSprite);
        SetButtonState(victorButton, gioBeaten, victorBeaten, victorBeatenSprite);
        SetButtonState(marinoButton, victorBeaten, marinoBeaten, marinoBeatenSprite);
        SetButtonState(jorgeButton, marinoBeaten, jorgeBeaten, jorgeBeatenSprite);
        SetButtonState(nicteButton, jorgeBeaten, nicteBeaten, nicteBeatenSprite);
    }

    private void SetButtonState(Button button, bool isUnlocked, bool isBeaten, Sprite beatenSprite)
    {
        if (button != null)
        {
            button.interactable = isUnlocked;
        }

        Image image = GetSourceImage(button);
        if (image == null)
        {
            return;
        }

        if (isBeaten && beatenSprite != null)
        {
            image.sprite = beatenSprite;
        }
        else if (!isBeaten && lockedSprite != null)
        {
            image.sprite = lockedSprite;
        }
    }

    private Image GetSourceImage(Button button)
    {
        if (button == null)
        {
            return null;
        }

        return button.targetGraphic as Image;
    }

    private Sprite GetBeatenSprite(Image image)
    {
        if (image == null)
        {
            return null;
        }

        if (image == GetSourceImage(gioButton))
        {
            return gioBeatenSprite;
        }

        if (image == GetSourceImage(victorButton))
        {
            return victorBeatenSprite;
        }

        if (image == GetSourceImage(marinoButton))
        {
            return marinoBeatenSprite;
        }

        if (image == GetSourceImage(jorgeButton))
        {
            return jorgeBeatenSprite;
        }

        if (image == GetSourceImage(nicteButton))
        {
            return nicteBeatenSprite;
        }

        return null;
    }

    public void NicteMinigameButton()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("AtaqueEstelarGameStart");
    }

    public void JorgeMinigameButton()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene_Jorge");
    }

    public void MarinoMinigameButton()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene_MECS");
    }

    public void VictorMinigameButton()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene_AB");
    }

    public void GioMinigameButton()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("IEGameStart");
    }

    public void BackToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
    }
}

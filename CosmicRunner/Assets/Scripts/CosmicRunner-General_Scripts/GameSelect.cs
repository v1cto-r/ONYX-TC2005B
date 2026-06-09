using UnityEngine;
using UnityEngine.UI;

public class GameSelect : MonoBehaviour
{
    private Button gioButton;
    private Button victorButton;
    private Button marinoButton;
    private Button jorgeButton;
    private Button nicteButton;

    private Image gioImage;
    private Image victorImage;
    private Image marinoImage;
    private Image jorgeImage;
    private Image nicteImage;

    private Sprite lockedSprite;
    private Sprite unlockedSprite;

    private void Start()
    {
        CacheReferences();
        RefreshButtons();
    }

    private void CacheReferences()
    {
        gioButton = FindButton("Gio Button");
        victorButton = FindButton("Victor Button");
        marinoButton = FindButton("Marino Button");
        jorgeButton = FindButton("Jorge Button");
        nicteButton = FindButton("Nicte Button");

        gioImage = GetButtonImage(gioButton);
        victorImage = GetButtonImage(victorButton);
        marinoImage = GetButtonImage(marinoButton);
        jorgeImage = GetButtonImage(jorgeButton);
        nicteImage = GetButtonImage(nicteButton);

        lockedSprite = FindSprite("minigames_0");
        unlockedSprite = FindSprite("minigames_1");
    }

    private Button FindButton(string objectName)
    {
        GameObject buttonObject = GameObject.Find(objectName);
        if (buttonObject == null)
        {
            return null;
        }

        return buttonObject.GetComponent<Button>();
    }

    private Image GetButtonImage(Button button)
    {
        if (button == null)
        {
            return null;
        }

        return button.GetComponent<Image>();
    }

    private Sprite FindSprite(string spriteName)
    {
        Sprite[] sprites = Resources.FindObjectsOfTypeAll<Sprite>();
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] != null && sprites[i].name == spriteName)
            {
                return sprites[i];
            }
        }

        return null;
    }

    private void RefreshButtons()
    {
        bool gioBeaten = MinigameProgress.IsBeaten(MinigameProgress.GiovanniId);
        bool victorBeaten = MinigameProgress.IsBeaten(MinigameProgress.VictorId);
        bool marinoBeaten = MinigameProgress.IsBeaten(MinigameProgress.MarinoId);
        bool jorgeBeaten = MinigameProgress.IsBeaten(MinigameProgress.JorgeId);
        bool nicteBeaten = MinigameProgress.IsBeaten(MinigameProgress.NicteId);

        SetButtonState(gioButton, gioImage, true, gioBeaten);
        SetButtonState(victorButton, victorImage, gioBeaten, victorBeaten);
        SetButtonState(marinoButton, marinoImage, victorBeaten, marinoBeaten);
        SetButtonState(jorgeButton, jorgeImage, marinoBeaten, jorgeBeaten);
        SetButtonState(nicteButton, nicteImage, jorgeBeaten, nicteBeaten);
    }

    private void SetButtonState(Button button, Image image, bool isUnlocked, bool isBeaten)
    {
        if (button != null)
        {
            button.interactable = isUnlocked;
        }

        if (image == null)
        {
            return;
        }

        if (isBeaten && unlockedSprite != null)
        {
            image.sprite = unlockedSprite;
        }
        else if (!isBeaten && lockedSprite != null)
        {
            image.sprite = lockedSprite;
        }
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

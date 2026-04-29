using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ComboUI : MonoBehaviour
{
    public Image[] comboImages;
    public int currentCombo = 0;
    GeneralUI generalUI;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI comboLabel;
    float timer = 0f;


     void Awake()
    {
        resetCombo();
    }

    void Update()
    {

        timer+= Time.deltaTime;
        if (timer >= 5f)
        {
            resetCombo();
            timer = 0f;
            }
    }
    public void resetCombo()
    {
        for (int i = 0; i < comboImages.Length; i++)
            {
                comboImages[i].enabled = false;
            }
        currentCombo = 0;
        UpdateComboText(currentCombo);
        UpdateComboLabel(currentCombo);
    }

    public void setComboToMax()
    {
        currentCombo = comboImages.Length;
        updateComboBar(currentCombo);
        UpdateComboText(currentCombo);
        UpdateComboLabel(currentCombo);
    }

    public void UpdateCombo(int combo)
    {
        currentCombo = combo;
        Debug.Log("Combo updated: " + currentCombo);
        updateComboBar(currentCombo);
        UpdateComboText(currentCombo);
        UpdateComboLabel(currentCombo);
    }

    public void updateComboBar(int combo)
    {
        if(combo>=0 && combo <= comboImages.Length)
        {
            for (int i = 0; i < comboImages.Length; i++)
            {
                if (i < combo)
                {
                    comboImages[i].enabled = true;
                }
                else
                {
                    comboImages[i].enabled = false;
                }
            }
        }
    }

    public void UpdateComboText(int combo)
    {
        if (comboText != null)
        {
            comboText.text =combo.ToString();
        }
    }

    public void UpdateComboLabel(int combo)
    {
        if (comboLabel != null)
        {
            switch (combo)
            {
                case 2:
                    comboLabel.text = "¡Bien!";
                    break;
                case 5:
                    comboLabel.text = "¡Increible!";
                    generalUI.UpdateCredits(200);
                    break;
                case 8:
                    comboLabel.text = "¡Asombroso!";
                    generalUI.UpdateCredits(300);
                    break;
                default:
                    comboLabel.text = "";
                    break;
            }
        }
    }


}

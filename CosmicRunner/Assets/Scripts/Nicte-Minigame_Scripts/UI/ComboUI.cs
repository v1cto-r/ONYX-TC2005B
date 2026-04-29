using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ComboUI : MonoBehaviour
{
    public Image[] comboImages;
    public int currentCombo = 0;

    public TextMeshProUGUI comboText;
    public TextMeshProUGUI comboLabel;
    float timer = 0f;


     void Awake()
    {
        resetCombo();
    }

    void Update()
    {
        Debug.Log("timer: " + timer);
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
        if(currentCombo>=0 && currentCombo <= comboImages.Length)
        {
            for (int i = 0; i < comboImages.Length; i++)
            {
                if (i < currentCombo)
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
                    comboLabel.text = "Bien!";
                    break;
                case 5:
                    comboLabel.text = "Ostias que increible!";
                    break;
                case 8:
                    comboLabel.text = "Asombroso!";
                    break;
                default:
                    comboLabel.text = "";
                    break;
            }
        }
    }


}

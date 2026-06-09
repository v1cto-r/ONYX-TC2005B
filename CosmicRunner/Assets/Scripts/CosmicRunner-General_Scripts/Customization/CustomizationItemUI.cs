using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationItemUI : MonoBehaviour
{
    [Header("UI")]
    public Image itemImage;
    public Button selectButton;
    public TMP_Text buttonText;

    private StoreCustomizationManager.CustomizationAsset currentAsset;
    private Action<StoreCustomizationManager.CustomizationAsset> onSelect;

    public void Setup(StoreCustomizationManager.CustomizationAsset asset, Sprite previewSprite, Action<StoreCustomizationManager.CustomizationAsset> selectAction)
    {
        FindReferencesIfNeeded();

        currentAsset = asset;
        onSelect = selectAction;

        if (itemImage != null)
        {
            itemImage.sprite = previewSprite;
            itemImage.preserveAspect = true;
        }
        else
        {
            Debug.LogWarning("No se encontro Item_Icon en " + gameObject.name);
        }

        if (buttonText != null)
        {
            buttonText.text = asset.equipped ? "Seleccionado" : "Seleccionar";
        }

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.interactable = !asset.equipped;
            selectButton.onClick.AddListener(SelectItem);
        }
        else
        {
            Debug.LogWarning("No se encontro Purple_Large_Button en " + gameObject.name);
        }
    }

    public void SetEquipped(bool equipped)
    {
        if (currentAsset != null)
        {
            currentAsset.equipped = equipped;
        }

        if (buttonText != null)
        {
            buttonText.text = equipped ? "Seleccionado" : "Seleccionar";
        }

        if (selectButton != null)
        {
            selectButton.interactable = !equipped;
        }
    }

    private void SelectItem()
    {
        if (currentAsset == null || onSelect == null)
        {
            return;
        }

        onSelect.Invoke(currentAsset);
    }

    private void FindReferencesIfNeeded()
    {
        if (itemImage == null)
        {
            Transform iconTransform = FindChildRecursive(transform, "Item_Icon");

            if (iconTransform != null)
            {
                itemImage = iconTransform.GetComponent<Image>();
            }
        }

        if (selectButton == null)
        {
            Transform buttonTransform = FindChildRecursive(transform, "Purple_Large_Button");

            if (buttonTransform != null)
            {
                selectButton = buttonTransform.GetComponent<Button>();
            }
        }

        if (buttonText == null && selectButton != null)
        {
            buttonText = selectButton.GetComponentInChildren<TMP_Text>(true);
        }

        if (buttonText == null)
        {
            TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

            foreach (TMP_Text text in texts)
            {
                if (text.gameObject.name.Contains("Text"))
                {
                    buttonText = text;
                    break;
                }
            }
        }
    }

    private Transform FindChildRecursive(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }

            Transform result = FindChildRecursive(child, childName);

            if (result != null)
            {
                return result;
            }
        }

        return null;
    }
}
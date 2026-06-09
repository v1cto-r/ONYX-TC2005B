using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MenuProfileIconLoader : MonoBehaviour
{
    [Header("API")]
    public bool loadFromApi = true;
    public string apiBaseUrl = "https://jorge.onyx.14082006.xyz";
    public int defaultUserId = 1;

    [Header("UI")]
    public Image targetImage;

    [Header("Sprites de iconos")]
    public IconSprite[] iconSprites;

    private void Start()
    {
        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }

        ApplySavedIcon();

        if (loadFromApi)
        {
            StartCoroutine(LoadIconFromApi());
        }
    }

    private void ApplySavedIcon()
    {
        int equippedIconAssetId = PlayerPrefs.GetInt("EquippedIconAssetId", 0);
        ApplyIcon(equippedIconAssetId);
    }

    private IEnumerator LoadIconFromApi()
    {
        int userId = PlayerPrefs.GetInt("UserId", defaultUserId);
        string url = apiBaseUrl + "/api/tienda/customizacion/" + userId;

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.certificateHandler = new ForceAcceptAll();

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning("No se pudo cargar el icono del menu: " + request.error);
            yield break;
        }

        CustomizationResponse response = JsonUtility.FromJson<CustomizationResponse>(request.downloadHandler.text);

        if (response == null || !response.exito || response.icons == null)
        {
            Debug.LogWarning("La API no regreso datos validos para el icono del menu.");
            yield break;
        }

        int equippedIconAssetId = 0;

        foreach (CustomizationAsset icon in response.icons)
        {
            if (icon.equipped)
            {
                equippedIconAssetId = icon.asset_id;
                break;
            }
        }

        if (equippedIconAssetId > 0)
        {
            PlayerPrefs.SetInt("EquippedIconAssetId", equippedIconAssetId);
        }
        else
        {
            PlayerPrefs.DeleteKey("EquippedIconAssetId");
        }

        PlayerPrefs.Save();
        ApplyIcon(equippedIconAssetId);
    }

    private void ApplyIcon(int assetId)
    {
        if (targetImage == null)
        {
            return;
        }

        Sprite selectedSprite = FindSprite(assetId);

        if (selectedSprite == null)
        {
            return;
        }

        targetImage.sprite = selectedSprite;
        targetImage.preserveAspect = true;
    }

    private Sprite FindSprite(int assetId)
    {
        if (iconSprites == null)
        {
            return null;
        }

        foreach (IconSprite iconSprite in iconSprites)
        {
            if (iconSprite.assetId == assetId)
            {
                return iconSprite.sprite;
            }
        }

        return null;
    }

    [Serializable]
    public class IconSprite
    {
        public int assetId;
        public Sprite sprite;
    }

    [Serializable]
    private class CustomizationResponse
    {
        public bool exito;
        public CustomizationAsset[] icons;
    }

    [Serializable]
    private class CustomizationAsset
    {
        public int asset_id;
        public bool equipped;
    }
}
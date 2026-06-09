using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class StoreCustomizationManager : MonoBehaviour
{
    [Header("API")]
    public string apiBaseUrl = "https://jorge.onyx.14082006.xyz";
    public int defaultUserId = 1;

    [Header("UI")]
    public Transform gridContent;
    public GameObject itemTemplate;
    public TMP_Text statusText;

    [Header("Sprites de skins")]
    public AssetSprite[] skinSprites;

    [Header("Sprites de iconos")]
    public AssetSprite[] iconSprites;

    private CustomizationResponse currentData;
    private string currentSection = "skins";

    private void Start()
    {
        if (itemTemplate != null)
        {
            itemTemplate.SetActive(false);
        }

        StartCoroutine(LoadCustomization());
    }

    public void ShowSkins()
    {
        currentSection = "skins";

        if (currentData != null)
        {
            BuildGrid(currentData.skins, skinSprites);
        }
    }

    public void ShowIcons()
    {
        currentSection = "icons";

        if (currentData != null)
        {
            BuildGrid(currentData.icons, iconSprites);
        }
    }

    private IEnumerator LoadCustomization()
    {
        int userId = PlayerPrefs.GetInt("UserId", defaultUserId);

        SetStatus("Cargando...");

        string url = apiBaseUrl + "/api/tienda/customizacion/" + userId;

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.certificateHandler = new ForceAcceptAll();

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            SetStatus("No se pudo cargar la customización.");
            Debug.LogError("Error al cargar customización: " + request.error);
            Debug.LogError("Respuesta API: " + request.downloadHandler.text);
            yield break;
        }

        currentData = JsonUtility.FromJson<CustomizationResponse>(request.downloadHandler.text);

        if (currentData == null || !currentData.exito)
        {
            SetStatus("La API no regresó datos válidos.");
            Debug.LogError("Respuesta inválida: " + request.downloadHandler.text);
            yield break;
        }

        SaveEquippedItems();

        SetStatus("");
        ShowSkins();
    }

    private void BuildGrid(CustomizationAsset[] assets, AssetSprite[] sprites)
    {
        ClearGrid();

        if (assets == null || assets.Length == 0)
        {
            SetStatus("No tienes objetos comprados en esta sección.");
            return;
        }

        SetStatus("");

        foreach (CustomizationAsset asset in assets)
        {
            GameObject itemObject = Instantiate(itemTemplate, gridContent);
            itemObject.SetActive(true);

            CustomizationItemUI itemUI = itemObject.GetComponent<CustomizationItemUI>();

            if (itemUI == null)
            {
                itemUI = itemObject.AddComponent<CustomizationItemUI>();
            }

            Sprite previewSprite = FindSprite(asset.asset_id, sprites);
            itemUI.Setup(asset, previewSprite, EquipAsset);
        }
    }

    private void ClearGrid()
    {
        if (gridContent == null || itemTemplate == null)
        {
            return;
        }

        for (int i = gridContent.childCount - 1; i >= 0; i--)
        {
            Transform child = gridContent.GetChild(i);

            if (child.gameObject != itemTemplate)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void EquipAsset(CustomizationAsset asset)
    {
        StartCoroutine(PostEquipAsset(asset));
    }

    private IEnumerator PostEquipAsset(CustomizationAsset asset)
    {
        int userId = PlayerPrefs.GetInt("UserId", defaultUserId);

        EquipRequest data = new EquipRequest
        {
            usuario_id = userId,
            asset_id = asset.asset_id
        };

        string json = JsonUtility.ToJson(data);
        byte[] body = Encoding.UTF8.GetBytes(json);

        string url = apiBaseUrl + "/api/tienda/customizacion/equipar";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.certificateHandler = new ForceAcceptAll();

        SetStatus("Guardando...");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            SetStatus("No se pudo equipar el objeto.");
            Debug.LogError("Error al equipar asset: " + request.error);
            Debug.LogError("Respuesta API: " + request.downloadHandler.text);
            yield break;
        }

        EquipResponse response = JsonUtility.FromJson<EquipResponse>(request.downloadHandler.text);

        if (response == null || !response.exito)
        {
            SetStatus("La API no pudo equipar el objeto.");
            Debug.LogError("Respuesta inválida al equipar: " + request.downloadHandler.text);
            yield break;
        }

        UpdateEquippedLocal(asset);

        SetStatus("");
    }

    private void UpdateEquippedLocal(CustomizationAsset selectedAsset)
    {
        CustomizationAsset[] assetsToUpdate = selectedAsset.category == "icons"
            ? currentData.icons
            : currentData.skins;

        if (assetsToUpdate != null)
        {
            foreach (CustomizationAsset asset in assetsToUpdate)
            {
                asset.equipped = asset.asset_id == selectedAsset.asset_id;
            }
        }

        SaveEquippedItems();

        if (currentSection == "icons")
        {
            BuildGrid(currentData.icons, iconSprites);
        }
        else
        {
            BuildGrid(currentData.skins, skinSprites);
        }
    }

    private void SaveEquippedItems()
    {
        SaveEquippedSkin();
        SaveEquippedIcon();
    }

    private void SaveEquippedSkin()
    {
        if (currentData == null || currentData.skins == null)
        {
            return;
        }

        foreach (CustomizationAsset skin in currentData.skins)
        {
            if (skin.equipped)
            {
                PlayerPrefs.SetInt("EquippedSkinAssetId", skin.asset_id);
                PlayerPrefs.Save();
                return;
            }
        }

        PlayerPrefs.DeleteKey("EquippedSkinAssetId");
    }

    private void SaveEquippedIcon()
    {
        if (currentData == null || currentData.icons == null)
        {
            return;
        }

        foreach (CustomizationAsset icon in currentData.icons)
        {
            if (icon.equipped)
            {
                PlayerPrefs.SetInt("EquippedIconAssetId", icon.asset_id);
                PlayerPrefs.Save();
                return;
            }
        }

        PlayerPrefs.DeleteKey("EquippedIconAssetId");
    }

    private Sprite FindSprite(int assetId, AssetSprite[] sprites)
    {
        if (sprites == null)
        {
            return null;
        }

        foreach (AssetSprite assetSprite in sprites)
        {
            if (assetSprite.assetId == assetId)
            {
                return assetSprite.sprite;
            }
        }

        return null;
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }

        if (!string.IsNullOrWhiteSpace(message))
        {
            Debug.Log(message);
        }
    }

    [Serializable]
    public class AssetSprite
    {
        public int assetId;
        public Sprite sprite;
    }

    [Serializable]
    public class CustomizationResponse
    {
        public bool exito;
        public UserData usuario;
        public CustomizationAsset[] skins;
        public CustomizationAsset[] icons;
    }

    [Serializable]
    public class UserData
    {
        public int user_id;
        public string username;
        public int credits;
    }

    [Serializable]
    public class CustomizationAsset
    {
        public int asset_id;
        public string name;
        public string category;
        public int cost;
        public string description;
        public bool available;
        public bool equipped;
        public string image_url;
        public string image_url_2;
        public string image_url_3;
        public string[] image_urls;
    }

    [Serializable]
    public class EquipRequest
    {
        public int usuario_id;
        public int asset_id;
    }

    [Serializable]
    public class EquipResponse
    {
        public bool exito;
        public string mensaje;
        public CustomizationAsset asset;
    }
}
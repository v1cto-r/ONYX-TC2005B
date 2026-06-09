using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMenuAnimations : MonoBehaviour
{
    [Header("API")]
    public bool loadFromApi = true;
    public string apiBaseUrl = "https://jorge.onyx.14082006.xyz";
    public int defaultUserId = 1;

    [Header("Animator Controllers")]
    public RuntimeAnimatorController defaultBlueController;
    public RuntimeAnimatorController yellowController;
    public RuntimeAnimatorController redController;
    public RuntimeAnimatorController pinkController;
    public RuntimeAnimatorController greenController;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        ApplySavedSkin();

        if (loadFromApi)
        {
            StartCoroutine(LoadSkinFromApi());
        }
    }

    private void ApplySavedSkin()
    {
        int equippedSkinAssetId = PlayerPrefs.GetInt("EquippedSkinAssetId", 0);
        ApplySkin(equippedSkinAssetId);
    }

    private IEnumerator LoadSkinFromApi()
    {
        int userId = PlayerPrefs.GetInt("UserId", defaultUserId);
        string url = apiBaseUrl + "/api/tienda/customizacion/" + userId;

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.certificateHandler = new ForceAcceptAll();

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning("No se pudo cargar la skin del menu: " + request.error);
            yield break;
        }

        CustomizationResponse response = JsonUtility.FromJson<CustomizationResponse>(request.downloadHandler.text);

        if (response == null || !response.exito || response.skins == null)
        {
            Debug.LogWarning("La API no regreso datos validos para la skin del menu.");
            yield break;
        }

        int equippedSkinAssetId = 0;

        foreach (CustomizationAsset skin in response.skins)
        {
            if (skin.equipped)
            {
                equippedSkinAssetId = skin.asset_id;
                break;
            }
        }

        if (equippedSkinAssetId > 0)
        {
            PlayerPrefs.SetInt("EquippedSkinAssetId", equippedSkinAssetId);
        }
        else
        {
            PlayerPrefs.DeleteKey("EquippedSkinAssetId");
        }

        PlayerPrefs.Save();
        ApplySkin(equippedSkinAssetId);
    }

    private void ApplySkin(int assetId)
    {
        if (animator == null)
        {
            return;
        }

        RuntimeAnimatorController selectedController = GetControllerForAsset(assetId);

        if (selectedController == null)
        {
            return;
        }

        animator.runtimeAnimatorController = selectedController;
        animator.Rebind();
        animator.Update(0f);
    }

    private RuntimeAnimatorController GetControllerForAsset(int assetId)
    {
        switch (assetId)
        {
            case 1:
                return yellowController != null ? yellowController : defaultBlueController;

            case 2:
                return redController != null ? redController : defaultBlueController;

            case 3:
                return pinkController != null ? pinkController : defaultBlueController;

            case 4:
                return greenController != null ? greenController : defaultBlueController;

            default:
                return defaultBlueController;
        }
    }

    [Serializable]
    private class CustomizationResponse
    {
        public bool exito;
        public CustomizationAsset[] skins;
    }

    [Serializable]
    private class CustomizationAsset
    {
        public int asset_id;
        public bool equipped;
    }
}
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
namespace MECS
{
    public class MarinoPlayerSkinLoader : MonoBehaviour
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
        private Animator playerAnimator;
        private void Start()
        {
            playerAnimator = GetComponent<Animator>();
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
                Debug.LogWarning("No se pudo cargar la skin desde la API: " + request.error);
                yield break;
            }
            
            CustomizationResponse response = JsonUtility.FromJson<CustomizationResponse>(request.downloadHandler.text);
            
            if (response == null || !response.exito || response.skins == null)
            {
                Debug.LogWarning("La API no regreso datos validos de skins.");
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
            
            PlayerPrefs.SetInt("EquippedSkinAssetId", equippedSkinAssetId);
            PlayerPrefs.Save();
            
            ApplySkin(equippedSkinAssetId);
        }

        private void ApplySkin(int assetId)
        {
            if (playerAnimator == null)
            {
                return;
            }
            
            RuntimeAnimatorController controllerToUse = GetControllerForAsset(assetId);
            
            if (controllerToUse == null)
            {
                return;
            }
            
            playerAnimator.runtimeAnimatorController = controllerToUse;
            playerAnimator.Rebind();
            playerAnimator.Update(0f);
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
}
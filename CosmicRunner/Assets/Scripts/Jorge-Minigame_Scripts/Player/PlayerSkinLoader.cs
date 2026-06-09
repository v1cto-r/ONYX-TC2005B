using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace JorgeGame
{
    public class PlayerSkinLoader : MonoBehaviour
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

        private Animator animatorController;

        private void Start()
        {
            animatorController = GetComponent<Animator>();

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
            if (animatorController == null)
            {
                return;
            }

            RuntimeAnimatorController controllerToUse = GetControllerForAsset(assetId);

            if (controllerToUse == null)
            {
                return;
            }

            animatorController.runtimeAnimatorController = controllerToUse;
            animatorController.Rebind();
            animatorController.Update(0f);
        }

        private RuntimeAnimatorController GetControllerForAsset(int assetId)
        {
            RuntimeAnimatorController selectedController = null;

            switch (assetId)
            {
                case 1:
                    selectedController = yellowController;
                    break;
                case 2:
                    selectedController = redController;
                    break;
                case 3:
                    selectedController = pinkController;
                    break;
                case 4:
                    selectedController = greenController;
                    break;
                default:
                    selectedController = defaultBlueController;
                    break;
            }

            if (selectedController != null)
            {
                return selectedController;
            }

            if (defaultBlueController != null)
            {
                return defaultBlueController;
            }

            return animatorController.runtimeAnimatorController;
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
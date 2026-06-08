using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Gio.Minigame
{
    public class UIController : MonoBehaviour
    {
        public TextMeshProUGUI textoZonaSuperior;
        public TextMeshProUGUI textoZonaInferior;
        public TextMeshProUGUI textoAdvertenciaGeneral;
        private bool advertenciaMostrada = true;
        public GameObject prefabEnemigo;
        public Transform puntoAparicionSuperior;
        public Transform puntoAparicionInferior;
        
        public float tiempoDecision = 10f;
        public float intervaloEntreAtaques = 5f; 
        private bool ataqueEnCurso = false;

        public GameObject marcoSuperior;
        public GameObject marcoInferior;
        
        private void Start()
        {
            textoZonaSuperior.text = "";
            textoZonaInferior.text = "";
            if (textoAdvertenciaGeneral != null)
            {
                textoAdvertenciaGeneral.text = "Cuidado, la zona con el peor prompt será atacada";
                textoAdvertenciaGeneral.gameObject.SetActive(true);
            }

            if (marcoSuperior != null) marcoSuperior.SetActive(false);
            if (marcoInferior != null) marcoInferior.SetActive(false);

            StartCoroutine(CicloDeAtaques());
        }

        private IEnumerator CicloDeAtaques()
        {
            while (true)
            {
                yield return new WaitForSeconds(intervaloEntreAtaques);
                if (textoAdvertenciaGeneral != null) textoAdvertenciaGeneral.gameObject.SetActive(false);
                
                if (APIAttackManager.Instance != null && APIAttackManager.Instance.listaPrompts.Count > 0 && !ataqueEnCurso)
                {
                    StartCoroutine(EjecutarAtaque());
                }
            }
        }

        private IEnumerator EjecutarAtaque()
        {
            ataqueEnCurso = true;

            int indexAleatorio = Random.Range(0, APIAttackManager.Instance.listaPrompts.Count);
            Prompt promptSeleccionado = APIAttackManager.Instance.listaPrompts[indexAleatorio];

            int zonaCorrecta = Random.Range(0, 2);
            Transform puntoAtaque; 

            if (zonaCorrecta == 0)
            {
                textoZonaSuperior.text = promptSeleccionado.prompt_correcto;
                textoZonaInferior.text = promptSeleccionado.prompt_incorrecto;
                puntoAtaque = puntoAparicionInferior; 
            }
            else
            {
                textoZonaSuperior.text = promptSeleccionado.prompt_incorrecto;
                textoZonaInferior.text = promptSeleccionado.prompt_correcto;
                puntoAtaque = puntoAparicionSuperior; 
            }

            if (marcoSuperior != null) marcoSuperior.SetActive(true);
            if (marcoInferior != null) marcoInferior.SetActive(true);
            
            yield return new WaitForSeconds(tiempoDecision);

            if (marcoSuperior != null) marcoSuperior.SetActive(false);
            if (marcoInferior != null) marcoInferior.SetActive(false);

            textoZonaSuperior.text = "";
            textoZonaInferior.text = "";

            Instantiate(prefabEnemigo, puntoAtaque.position, puntoAtaque.rotation);
            if (SFXManager.Instance != null) SFXManager.Instance.PlayEnemySound();
            ataqueEnCurso = false;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Gio.Minigame
{
    public class UIController : MonoBehaviour
    {
        [Header("Textos de API")]
        public TextMeshProUGUI textoZonaSuperior;
        public TextMeshProUGUI textoZonaInferior;
        
        [Header("Enemigo y Puntos de Aparición")]
        public GameObject prefabEnemigo;
        public Transform puntoAparicionSuperior;
        public Transform puntoAparicionInferior;
        
        [Header("Tiempos")]
        public float tiempoDecision = 10f;
        public float intervaloEntreAtaques = 5f; 
        private bool ataqueEnCurso = false;

        [Header("Marcos de Advertencia (Estáticos)")]
        [Tooltip("El objeto de UI con el contorno de la zona superior")]
        public GameObject marcoSuperior;
        [Tooltip("El objeto de UI con el contorno de la zona inferior")]
        public GameObject marcoInferior;
        
        private void Start()
        {
            textoZonaSuperior.text = "";
            textoZonaInferior.text = "";

            // Asegurar que los marcos inicien ocultos
            if (marcoSuperior != null) marcoSuperior.SetActive(false);
            if (marcoInferior != null) marcoInferior.SetActive(false);

            StartCoroutine(CicloDeAtaques());
        }

        private IEnumerator CicloDeAtaques()
        {
            while (true)
            {
                yield return new WaitForSeconds(intervaloEntreAtaques);
                
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

            // ACTIVACIÓN SIMULTÁNEA: Ambos marcos se encienden de forma fija junto con los textos
            if (marcoSuperior != null) marcoSuperior.SetActive(true);
            if (marcoInferior != null) marcoInferior.SetActive(true);

            // Esperar el tiempo de decisión del jugador
            yield return new WaitForSeconds(tiempoDecision);

            // DESACTIVACIÓN SIMULTÁNEA: Se apagan ambos marcos y se limpian los textos
            if (marcoSuperior != null) marcoSuperior.SetActive(false);
            if (marcoInferior != null) marcoInferior.SetActive(false);

            textoZonaSuperior.text = "";
            textoZonaInferior.text = "";

            Instantiate(prefabEnemigo, puntoAtaque.position, puntoAtaque.rotation);
            ataqueEnCurso = false;
        }
    }
}
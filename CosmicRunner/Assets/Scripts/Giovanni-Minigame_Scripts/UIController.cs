using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace Gio.Minigame{
public class AttackManager : MonoBehaviour
{
    public TextMeshProUGUI textoZonaSuperior;
    public TextMeshProUGUI textoZonaInferior;
    
    public GameObject prefabEnemigo;
    public Transform puntoAparicionSuperior;
    public Transform puntoAparicionInferior;
    
    public float tiempoDecision = 10f;
    public float intervaloEntreAtaques = 5f; 
    private bool ataqueEnCurso = false;
    
    private void Start()
    {
        textoZonaSuperior.text = "";
        textoZonaInferior.text = "";

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

        yield return new WaitForSeconds(tiempoDecision);

        textoZonaSuperior.text = "";
        textoZonaInferior.text = "";

        Instantiate(prefabEnemigo, puntoAtaque.position, puntoAtaque.rotation);
        ataqueEnCurso = false;
    }
}
}
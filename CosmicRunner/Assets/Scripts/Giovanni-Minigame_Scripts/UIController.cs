using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AttackManager : MonoBehaviour
{
    [Header("Referencias UI")]
    public TextMeshProUGUI textoZonaSuperior;
    public TextMeshProUGUI textoZonaInferior;
    
    [Header("Referencias Spawner")]
    public GameObject prefabEnemigo;
    public Transform puntoAparicionSuperior;
    public Transform puntoAparicionInferior;
    
    [Header("Configuración")]
    public float tiempoAdvertencia = 10f;
    public float intervaloEntreAtaques = 15f; // Tiempo desde que termina un ataque hasta que empieza otro

    private bool ataqueEnCurso = false;

    private void Start()
    {
        // Limpiar UI al inicio
        textoZonaSuperior.text = "";
        textoZonaInferior.text = "";
        
        // Iniciar el ciclo de ataques (asegúrate de que APIAttackManager haya cargado los datos primero)
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

        // 1. Seleccionar un prompt aleatorio
        int indexAleatorio = Random.Range(0, APIAttackManager.Instance.listaPrompts.Count);
        Prompt promptSeleccionado = APIAttackManager.Instance.listaPrompts[indexAleatorio];

        // 2. Decidir aleatoriamente dónde va el correcto (0 = Superior, 1 = Inferior)
        int zonaCorrecta = Random.Range(0, 2);
        Transform puntoAtaque; // El punto donde aparecerá el enemigo

        if (zonaCorrecta == 0)
        {
            textoZonaSuperior.text = promptSeleccionado.prompt_correcto;
            textoZonaInferior.text = promptSeleccionado.prompt_incorrecto;
            puntoAtaque = puntoAparicionInferior; // Enemigo ataca la zona incorrecta
        }
        else
        {
            textoZonaSuperior.text = promptSeleccionado.prompt_incorrecto;
            textoZonaInferior.text = promptSeleccionado.prompt_correcto;
            puntoAtaque = puntoAparicionSuperior; // Enemigo ataca la zona incorrecta
        }

        // 3. Esperar el tiempo de gracia (10 segundos)
        yield return new WaitForSeconds(tiempoAdvertencia);

        // 4. Limpiar UI
        textoZonaSuperior.text = "";
        textoZonaInferior.text = "";

        // 5. Instanciar enemigo en la zona del prompt incorrecto
        if (prefabEnemigo != null && puntoAtaque != null)
        {
            Instantiate(prefabEnemigo, puntoAtaque.position, puntoAtaque.rotation);
        }

        ataqueEnCurso = false;
    }
}
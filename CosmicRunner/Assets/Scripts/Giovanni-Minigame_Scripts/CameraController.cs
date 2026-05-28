using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Gio.Minigame{
public class CamaraController : MonoBehaviour
{
    [Header("Objetivo y Suavizado")]
    public Transform objetivo;
    public float velocidadCamara = 0.04f;
    public Vector3 desplazamiento;

    [Header("Ventana de Movimiento (Deadzone)")]
    public Vector2 ventana = new Vector2(7f, 2f);
    
    // Este será nuestro "objetivo virtual". La cámara lo seguirá a él.
    private Vector3 centroVentanaActual;

    private void Start()
    {
        if (objetivo != null)
        {
            centroVentanaActual = objetivo.position;
        }
    }

    private void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 posObjetivo = objetivo.position;

        // 1. Calculamos la distancia entre la nave y nuestro centro virtual
        float distanciaX = posObjetivo.x - centroVentanaActual.x;
        float distanciaY = posObjetivo.y - centroVentanaActual.y;

        // 2. Si la nave empuja el límite X de la ventana, movemos el centro virtual
        if (Mathf.Abs(distanciaX) > ventana.x)
        {
            centroVentanaActual.x += distanciaX - (ventana.x * Mathf.Sign(distanciaX));
        }

        // 3. Si la nave empuja el límite Y de la ventana, movemos el centro virtual
        if (Mathf.Abs(distanciaY) > ventana.y)
        {
            centroVentanaActual.y += distanciaY - (ventana.y * Mathf.Sign(distanciaY));
        }

        // 4. Aplicamos el Lerp (suavizado) hacia el objetivo virtual, NO hacia la nave
        Vector3 posicionDeseada = centroVentanaActual + desplazamiento;
        transform.position = Vector3.Lerp(transform.position, posicionDeseada, velocidadCamara);
    }

    // Para visualizar la ventana en la escena de Unity mientras ajustas los valores
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(centroVentanaActual, new Vector3(ventana.x * 2, ventana.y * 2, 0));
        }
    }
}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Gio.Minigame{
public class CamaraController : MonoBehaviour
{
    public Transform objetivo;
    public float velocidadCamara = 0.04f;
    public Vector3 desplazamiento;
    public Vector2 ventana = new Vector2(7f, 2f);
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

        float distanciaX = posObjetivo.x - centroVentanaActual.x;
        float distanciaY = posObjetivo.y - centroVentanaActual.y;

        if (Mathf.Abs(distanciaX) > ventana.x)
        {
            centroVentanaActual.x += distanciaX - (ventana.x * Mathf.Sign(distanciaX));
        }

        if (Mathf.Abs(distanciaY) > ventana.y)
        {
            centroVentanaActual.y += distanciaY - (ventana.y * Mathf.Sign(distanciaY));
        }

        Vector3 posicionDeseada = centroVentanaActual + desplazamiento;
        transform.position = Vector3.Lerp(transform.position, posicionDeseada, velocidadCamara);
    }

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
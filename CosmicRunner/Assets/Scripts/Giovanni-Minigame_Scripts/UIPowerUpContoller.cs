using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace Gio.Minigame{
public class UIPowerUpContador : MonoBehaviour
{
    public static UIPowerUpContador Instance;
    public GameObject panelVisual; 
    public Image iconoItem;        
    public TextMeshProUGUI textoEtiqueta; 
    public GameObject[] barrasDeTiempo; 
    private float tiempoRestante;
    private float duracionTotal;
    private bool cuentaRegresivaActiva = false;

    private void Awake()
    {
        Instance = this;
        panelVisual.SetActive(false); // Ocultar
    }

    private void Update()
    {
        if (cuentaRegresivaActiva)
        {
            tiempoRestante -= Time.deltaTime;
            
            if (tiempoRestante > 0)
            {
                ActualizarBarras();
            }
            else
            {
                OcultarContador();
            }
        }
    }

    public void MostrarContador(Sprite icono, string etiqueta, float duracion)
    {
        iconoItem.sprite = icono;
        textoEtiqueta.text = etiqueta;
        
        duracionTotal = duracion;
        tiempoRestante = duracion;
        
        foreach (GameObject barra in barrasDeTiempo)
        {
            barra.SetActive(true);
        }

        cuentaRegresivaActiva = true;
        panelVisual.SetActive(true);
    }

    private void ActualizarBarras()
    {
        int barrasActivas = Mathf.CeilToInt((tiempoRestante / duracionTotal) * barrasDeTiempo.Length);

        for (int i = 0; i < barrasDeTiempo.Length; i++)
        {
            barrasDeTiempo[i].SetActive(i < barrasActivas);
        }
    }

    private void OcultarContador()
    {
        cuentaRegresivaActiva = false;
        panelVisual.SetActive(false);
    }
}
}
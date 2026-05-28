using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPowerUpContador : MonoBehaviour
{
    public static UIPowerUpContador Instance;

    [Header("Referencias UI Internas")]
    public GameObject panelVisual; // Asigna aquí el objeto padre "Potenciador"
    public Image iconoItem;        // Asigna aquí la "Image" donde va el escudo/icono
    public TextMeshProUGUI textoEtiqueta; // Asigna aquí "Text_PowerUp"
    
    [Header("Barras de Tiempo")]
    [Tooltip("Arrastra aquí Charge (0), Charge (1), Charge (2) y Charge (3) en orden")]
    public GameObject[] barrasDeTiempo; 

    private float tiempoRestante;
    private float duracionTotal;
    private bool cuentaRegresivaActiva = false;

    private void Awake()
    {
        Instance = this;
        panelVisual.SetActive(false); // Ocultar al inicio
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
        // 1. Actualizamos los visuales
        iconoItem.sprite = icono;
        textoEtiqueta.text = etiqueta;
        
        // 2. Configuramos los tiempos
        duracionTotal = duracion;
        tiempoRestante = duracion;
        
        // 3. Restauramos todas las barritas al 100%
        foreach (GameObject barra in barrasDeTiempo)
        {
            barra.SetActive(true);
        }

        // 4. Mostramos el panel
        cuentaRegresivaActiva = true;
        panelVisual.SetActive(true);
    }

    private void ActualizarBarras()
    {
        // Calculamos cuántas barras deberían estar encendidas basándonos en la proporción de tiempo
        // Usamos Mathf.CeilToInt para que la última barra no desaparezca hasta que el tiempo llegue a 0 exacto.
        int barrasActivas = Mathf.CeilToInt((tiempoRestante / duracionTotal) * barrasDeTiempo.Length);

        // Recorremos el arreglo de barras
        for (int i = 0; i < barrasDeTiempo.Length; i++)
        {
            // Si el índice (0, 1, 2, 3) es menor que las barras que deben estar activas, se enciende. Si no, se apaga.
            barrasDeTiempo[i].SetActive(i < barrasActivas);
        }
    }

    private void OcultarContador()
    {
        cuentaRegresivaActiva = false;
        panelVisual.SetActive(false);
    }
}
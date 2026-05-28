using UnityEngine;
using UnityEngine.SceneManagement;
namespace Gio.Minigame{
public class MenuManager : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    [Tooltip("El nombre exacto de la escena de tu nivel jugable")]
    public string escenaJuego = "Nivel1";
    
    [Tooltip("El nombre del menú principal del proyecto (para el botón salir)")]
    public string escenaHubPrincipal = "MenuPrincipal";

    [Header("Paneles UI (Solo para el Menú Principal)")]
    public GameObject panelAyuda;

    private void Start()
    {
        // CRUCIAL: Descongelar el tiempo por si venimos de la pantalla de Game Over
        Time.timeScale = 1f; 

        // Asegurarnos de que el panel de ayuda empiece oculto
        if (panelAyuda != null)
        {
            panelAyuda.SetActive(false);
        }
    }

    // --- FUNCIONES PARA LOS BOTONES ---

    public void BotonJugar()
    {
        SceneManager.LoadScene(escenaJuego);
    }

    public void BotonReiniciar()
    {
        // Reiniciar es funcionalmente lo mismo que darle a Jugar
        SceneManager.LoadScene(escenaJuego);
    }

    public void BotonMostrarAyuda()
    {
        if (panelAyuda != null)
        {
            panelAyuda.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Falta asignar el Panel de Ayuda en el Inspector.");
        }
    }

    public void BotonCerrarAyuda()
    {
        if (panelAyuda != null)
        {
            panelAyuda.SetActive(false);
        }
    }

    public void BotonSalir()
    {
        // Si el minijuego es parte de un proyecto más grande, regresa al menú del proyecto
        // Si quieres que cierre la aplicación completa, usa Application.Quit();
        
        Debug.Log("Saliendo al Hub Principal...");
        SceneManager.LoadScene(escenaHubPrincipal);
        
        // Application.Quit(); // Descomentar si se desea cerrar el .exe por completo
    }
}
}
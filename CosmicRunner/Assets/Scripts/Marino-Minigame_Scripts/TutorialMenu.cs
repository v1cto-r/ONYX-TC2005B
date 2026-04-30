using UnityEngine;
using UnityEngine.SceneManagement;

namespace MECS
{
    // Maneja navegacion simple entre paneles del tutorial
    public class TutorialMenu : MonoBehaviour
    {
        // Panel de la primera pagina del tutorial
        [Header("Panels")]
        [SerializeField] private GameObject tutorial1;
        // Panel de la segunda pagina del tutorial
        [SerializeField] private GameObject tutorial2;
        // Panel de la tercera pagina del tutorial
        [SerializeField] private GameObject tutorial3;

        // Indice del panel visible actual
        private int currentIndex = 0; // 0 = tutorial1, 1 = tutorial2, 2 = tutorial3

        // Arranca mostrando la primera pantalla del tutorial
        private void Start()
        {
            ShowPanel(0);
        }

        // Avanza a la siguiente pagina sin salirse del rango valido
        public void NextPanel()
        {
            ShowPanel(Mathf.Clamp(currentIndex + 1, 0, 2));
        }

        // Vuelve a la pagina anterior sin salirse del rango valido
        public void PrevPanel()
        {
            ShowPanel(Mathf.Clamp(currentIndex - 1, 0, 2));
        }

        // Boton para volver al menu principal
        public void BackButton()
        {
            SceneManager.LoadScene("MainMenuScene");
        }

        // Activa un solo panel y oculta los demas
        private void ShowPanel(int index)
        {
            // Guardamos el indice ya recortado al rango de paneles existentes
            currentIndex = Mathf.Clamp(index, 0, 2);

            // Solo dejamos visible el panel que corresponde al indice actual
            if (tutorial1 != null) tutorial1.SetActive(currentIndex == 0);
            if (tutorial2 != null) tutorial2.SetActive(currentIndex == 1);
            if (tutorial3 != null) tutorial3.SetActive(currentIndex == 2);
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

namespace AB
{
    // Cambia entre paneles a visualizar del tutorial
    public class TutorialMenu : MonoBehaviour
    {
        [Header("Tutorial Panels")]
        public GameObject tutorial1;
        public GameObject tutorial2;
        public GameObject tutorial3;
        public GameObject tutorial4;
        public GameObject tutorial5;

        [Header("Navigation Buttons")]
        public GameObject nextButton;
        public GameObject prevButton;



        // Indice del panel visible actual
        private int index = 0;

        // Arranca mostrando la primera pantalla del tutorial
        private void Start()
        {
            ShowPanel();
            ShowButtons();
        }

        // Avanza de pagina, sin salirse del rango
        public void NextPanel()
        {
            index = Mathf.Clamp(index + 1, 0, 4);
            ShowPanel();
            ShowButtons();
        }

        // Retrocede de pagina, sin salirse del rango
        public void PrevPanel()
        {
            index = Mathf.Clamp(index - 1, 0, 4);
            ShowPanel();
            ShowButtons();
        }

        // Boton para volver al menu principal
        public void BackButton()
        {
            SceneManager.LoadScene("MainMenuScene_AB");
        }

        private void ShowButtons()
        {
            if (nextButton) nextButton.SetActive(index < 4);
            if (prevButton) prevButton.SetActive(index > 0);
        }

        private void ShowPanel()
        {
            // Solo dejamos visible el panel que corresponde al indice actual
            if (tutorial1) tutorial1.SetActive(index == 0);
            if (tutorial2) tutorial2.SetActive(index == 1);
            if (tutorial3) tutorial3.SetActive(index == 2);
            if (tutorial4) tutorial4.SetActive(index == 3);
            if (tutorial5) tutorial5.SetActive(index == 4);
        }
    }
}

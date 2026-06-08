using UnityEngine;
using UnityEngine.SceneManagement;

namespace AB
{
    // Cambia entre paneles a visualizar del tutorial
    public class IETUtorialManager : MonoBehaviour
    {
        public GameObject tutorial1;
        public GameObject tutorial2;
        public GameObject tutorial3;
        public GameObject tutorial4;
        public GameObject tutorial5;
        public GameObject tutorial6;
        public GameObject nextButton;
        public GameObject prevButton;
        private int index = 0;
        private void Start()
        {
            ShowPanel();
            ShowButtons();
        }

        public void NextPanel()
        {
            index = Mathf.Clamp(index + 1, 0, 5);
            ShowPanel();
            ShowButtons();
        }

        public void PrevPanel()
        {
            index = Mathf.Clamp(index - 1, 0, 5);
            ShowPanel();
            ShowButtons();
        }
        public void GioMinigameButton()
        {
            SceneManager.LoadScene("IEGameStart");
        }

        private void ShowButtons()
        {
            if (nextButton) nextButton.SetActive(index < 5);
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
            if (tutorial6) tutorial6.SetActive(index == 5);
        }
    }
}

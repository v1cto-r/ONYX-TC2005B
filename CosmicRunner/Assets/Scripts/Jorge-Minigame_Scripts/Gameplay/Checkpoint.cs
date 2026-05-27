using UnityEngine;

namespace JorgeGame
{
    public class Checkpoint : MonoBehaviour
    {
        public UIQuestionManager uiManager;

        private bool activated = false;
        private SpriteRenderer sr;

        public Color normalColor = Color.white;
        public Color correctColor = new Color(0.4f, 1f, 0.4f);
        public Color wrongColor = new Color(1f, 0.4f, 0.4f);

        void Start()
        {
            sr = GetComponent<SpriteRenderer>();

            if (sr != null)
                sr.color = normalColor;

            if (uiManager == null)
                uiManager = FindAnyObjectByType<UIQuestionManager>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && !activated)
            {
                activated = true;

                if (uiManager != null)
                {
                    uiManager.ShowQuestion(transform.position, this);
                }
                else
                {
                    Debug.LogWarning("No se encontro UIQuestionManager en la escena.");
                }
            }
        }

        public void SetCorrect()
        {
            if (sr != null)
                sr.color = correctColor;
        }

        public void SetWrong()
        {
            if (sr != null)
                sr.color = wrongColor;
        }
    }
}
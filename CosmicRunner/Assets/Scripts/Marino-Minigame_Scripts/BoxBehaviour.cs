using UnityEngine;

public class BoxBehaviour : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.CompareTag("Collector"))
        {
            PromptsControl promptsControl = PromptsControl.Instance;
            if (promptsControl == null)
            {
                promptsControl = FindAnyObjectByType<PromptsControl>(FindObjectsInactive.Include);
            }

            if (promptsControl != null)
            {
                promptsControl.HandleBoxCollected();
            }

            Destroy(gameObject);
            Debug.Log("Box Collected!");
        }
    }
}

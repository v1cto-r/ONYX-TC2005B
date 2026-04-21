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
            Destroy(gameObject);
            Debug.Log("Box Collected!");
        }
    }
}

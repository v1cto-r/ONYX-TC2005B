using System.Collections;
using UnityEngine;

public class SpawnerEcosistema : MonoBehaviour
{
    public GameObject[] prefabsObstaculos; // Arrastra aquí tus asteroides de 1, 2 y 3 vidas
    public GameObject[] prefabsPowerUps;   // Arrastra aquí tus items
    public GameObject prefabMoneda;

    [Header("Configuración de Spawns")]
    public float tiempoEntreObstaculos = 2f;
    public float tiempoEntreMonedas = 5f;
    public float tiempoEntrePowerUps = 15f;
    
    [Tooltip("Distancia desde el centro de la cámara donde aparecerán los objetos")]
    public float radioDeAparicion = 15f; 

    private Transform camaraTransform;

    void Start()
    {
        camaraTransform = Camera.main.transform;

        // Iniciamos los ciclos de aparición independientes
        StartCoroutine(SpawnRutina(prefabsObstaculos, tiempoEntreObstaculos));
        StartCoroutine(SpawnRutina(new GameObject[] { prefabMoneda }, tiempoEntreMonedas));
        StartCoroutine(SpawnRutina(prefabsPowerUps, tiempoEntrePowerUps));
    }

    private IEnumerator SpawnRutina(GameObject[] arrayPrefabs, float intervalo)
    {
        while (true)
        {
            yield return new WaitForSeconds(intervalo);

            if (arrayPrefabs != null && arrayPrefabs.Length > 0)
            {
                GameObject prefabElegido = arrayPrefabs[Random.Range(0, arrayPrefabs.Length)];


                Vector2 posicionAleatoria = Random.insideUnitCircle.normalized * radioDeAparicion;

                // CORRECTO: Sumamos X e Y, pero forzamos Z a ser 0 para que esté frente a la cámara
                Vector3 posicionSpawn = new Vector3(
                    camaraTransform.position.x + posicionAleatoria.x, 
                    camaraTransform.position.y + posicionAleatoria.y, 
                    0f
);
                // Instancia el objeto
                Instantiate(prefabElegido, posicionSpawn, Quaternion.identity);
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        if (Camera.main != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Camera.main.transform.position, radioDeAparicion);
        }
    }
}
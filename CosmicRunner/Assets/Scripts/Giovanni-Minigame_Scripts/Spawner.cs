using System.Collections;
using UnityEngine;
namespace Gio.Minigame{
public class SpawnerEcosistema : MonoBehaviour
{
    public GameObject[] prefabsObstaculos; 
    public GameObject[] prefabsPowerUps;   
    public GameObject prefabMoneda;

    [Header("Configuración de Spawns")]
    public float tiempoEntreObstaculos = 2f;
    public float tiempoEntreMonedas = 5f;
    public float tiempoEntrePowerUps = 15f;
    public float radioDeAparicion = 15f; 
     private Transform camaraTransform;

    void Start()
    {
        camaraTransform = Camera.main.transform;
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
                Vector3 posicionSpawn = new Vector3(
                    camaraTransform.position.x + posicionAleatoria.x, 
                    camaraTransform.position.y + posicionAleatoria.y, 
                    0f
                    );
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
}
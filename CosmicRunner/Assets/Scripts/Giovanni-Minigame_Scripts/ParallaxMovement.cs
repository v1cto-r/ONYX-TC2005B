using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Gio.Minigame{
public class ParallaxMovement : MonoBehaviour
{
    Transform cam; //Main Camera
    Vector3 camStartPos;
    float distance; //jarak antara start camera posisi dan current posisi

    GameObject[] backgrounds;
    Material[] mat;
    float[] backSpeed;

    float farthestBack;

    [Range(0.05f, 1f)]
    public float parallaxSpeed;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.transform;
        camStartPos = cam.position;

        int backCount = transform.childCount;
        mat = new Material[backCount];
        backSpeed = new float[backCount];
        backgrounds = new GameObject[backCount];

        for (int i = 0; i < backCount; i++)
        {
            backgrounds[i] = transform.GetChild(i).gameObject;
            mat[i] = backgrounds[i].GetComponent<Renderer>().material;
        }

        BackSpeedCalculate(backCount);
    }

    void BackSpeedCalculate(int backCount)
    {
        for (int i = 0; i < backCount; i++) //find the farthest background
        {
            if ((backgrounds[i].transform.position.z - cam.position.z) > farthestBack)
            {
                farthestBack = backgrounds[i].transform.position.z - cam.position.z;
            }
        }

        for (int i = 0; i < backCount; i++) //set the speed of bacground
        {
            backSpeed[i] = 1 - (backgrounds[i].transform.position.z - cam.position.z) / farthestBack;
        }
    }

    private void LateUpdate()
    {
        // 1. Calculamos la distancia recorrida en AMBOS ejes como un Vector2
        Vector2 distanceRecorrida = new Vector2(
            cam.position.x - camStartPos.x, 
            cam.position.y - camStartPos.y
        );

        // 2. Mantenemos el objeto contenedor pegado a la cámara en X e Y
        // Respetamos su posición original en Z para no romper la profundidad
        transform.position = new Vector3(cam.position.x, cam.position.y, transform.position.z);

        // 3. Aplicamos el desfase a cada material
        for (int i = 0; i < backgrounds.Length; i++)
        {
            float speed = backSpeed[i] * parallaxSpeed;
            
            // Pasamos el Vector2 completo, multiplicando ambos ejes por la velocidad
            mat[i].SetTextureOffset("_MainTex", distanceRecorrida * speed);
        }
    }
}
}
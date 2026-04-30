using UnityEngine;

namespace JorgeGame
{
public class SpawnPoint : MonoBehaviour
{
    // instancia global para acceder desde otros scripts
    public static SpawnPoint instance;

    // posicion donde reaparece el jugador
    public Vector3 respawnPoint;

    // guarda esta instancia para acceso global
    void Awake()
    {
        instance = this;
    }
}
}
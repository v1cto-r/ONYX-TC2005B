using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    // instancia global para acceder desde otros scripts
    public static CheckpointManager instance;

    // posicion donde reaparece el jugador
    public Vector3 respawnPoint;

    void Awake()
    {
        instance = this;
    }
}
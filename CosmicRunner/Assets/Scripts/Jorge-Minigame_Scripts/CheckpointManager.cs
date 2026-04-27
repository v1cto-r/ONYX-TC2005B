using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    // instancia global
    public static CheckpointManager instance;

    // posicion donde reaparece el jugador
    public Vector3 respawnPoint;

    void Awake()
    {
        instance = this;
    }
}
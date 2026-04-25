using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance;

    public Vector3 respawnPoint;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        respawnPoint = GameObject.FindGameObjectWithTag("Player").transform.position;
    }
}
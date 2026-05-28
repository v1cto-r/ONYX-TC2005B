using UnityEngine;

public enum TipoColeccionable 
{ 
    Moneda, 
    Defensa, 
    Ataque, 
    Velocidad 
}

[RequireComponent(typeof(Collider2D))]
public class Coleccionable : MonoBehaviour
{
    public TipoColeccionable tipoItem;
    [Tooltip("Cantidad de monedas que da, o duración en segundos si es un power-up")]
    public float valorODuracion = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Más adelante conectaremos esto al GameManager para activar las corrutinas de los power-ups
            Debug.Log($"Coleccionable recogido: {tipoItem}. Valor/Duración: {valorODuracion}");
            
            // Destruimos el objeto de la escena
            Destroy(gameObject);
        }
    }
}
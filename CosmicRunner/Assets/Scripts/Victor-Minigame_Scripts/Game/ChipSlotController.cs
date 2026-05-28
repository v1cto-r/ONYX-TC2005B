using UnityEngine;
using UnityEngine.EventSystems;

namespace AB
{
    public class ChipSlotController : MonoBehaviour, IDropHandler
    {
        // Que parte del prompt va a aceptar
        public int slotOrder;

        // Cuando se le suelta algo encima corre OnDrop
        public void OnDrop(PointerEventData eventData)
        {
            // El gameObject que se arrastró
            GameObject draggedObject = eventData.pointerDrag;

            // Obtener el chip controller del objeto
            ChipController draggedChip = draggedObject.GetComponent<ChipController>();

            // Si están en el orden correcto, marcar como correcto, si no, marcar como incorrecto
            if (draggedChip.FragmentOrder == slotOrder)
            {
                draggedChip.MarkCorrect(transform);
            }
            else
            {
                draggedChip.MarkIncorrect();
            }
        }
    }
}
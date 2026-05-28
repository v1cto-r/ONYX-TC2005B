using UnityEngine;
using UnityEngine.EventSystems;

namespace AB
{
    public class ChipSlotController : MonoBehaviour, IDropHandler
    {
        public int slotOrder;

        public void OnDrop(PointerEventData eventData)
        {
            GameObject draggedObject = eventData.pointerDrag;

            ChipController draggedChip = draggedObject.GetComponent<ChipController>();

            if (draggedChip.FragmentOrder == slotOrder)
            {
                SFXGameController.Instance.PlayChipPlaceSound();
                draggedChip.MarkCorrect(transform);
            }
            else
            {
                draggedChip.MarkIncorrect();
            }
        }
    }
}
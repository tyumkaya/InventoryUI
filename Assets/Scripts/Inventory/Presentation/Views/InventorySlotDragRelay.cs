using UnityEngine;
using UnityEngine.EventSystems;

namespace InventoryUI.Inventory.Presentation.Views
{
    public sealed class InventorySlotDragRelay : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        private InventorySlotView owner;

        public void Initialize(InventorySlotView slotView)
        {
            owner = slotView;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            owner?.HandleBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            owner?.HandleDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            owner?.HandleEndDrag(eventData);
        }

        public void OnDrop(PointerEventData eventData)
        {
            owner?.HandleDrop(eventData);
        }
    }
}

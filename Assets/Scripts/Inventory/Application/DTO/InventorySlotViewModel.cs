using UnityEngine;

namespace InventoryUI.Inventory.Application.DTO
{
    public sealed class InventorySlotViewModel
    {
        public int SlotId { get; set; }
        public bool IsUnlocked { get; set; }
        public bool IsEmpty { get; set; }
        public string ItemName { get; set; }
        public string ItemInfoText { get; set; }
        public int Quantity { get; set; }
        public int UnlockPrice { get; set; }
        public Sprite Icon { get; set; }
    }
}

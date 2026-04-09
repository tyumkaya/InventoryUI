using System;

namespace InventoryUI.Inventory.Infrastructure.Persistence
{
    [Serializable]
    public sealed class SlotSaveData
    {
        public int SlotId;
        public bool IsUnlocked;
        public string ItemId;
        public int Quantity;
    }
}

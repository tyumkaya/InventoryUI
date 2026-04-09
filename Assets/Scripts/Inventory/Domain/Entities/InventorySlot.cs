using System;

namespace InventoryUI.Inventory.Domain.Entities
{
    [Serializable]
    public class InventorySlot
    {
        public int SlotId;
        public bool IsUnlocked;
        public ItemStack Stack;

        public bool IsEmpty
        {
            get
            {
                return Stack == null || Stack.Quantity <= 0;
            }
        }
    }
}

using System;
using InventoryUI.Inventory.Domain.Enums;

namespace InventoryUI.Inventory.Domain.Entities
{
    [Serializable]
    public class ItemStack
    {
        public ItemId ItemId;
        public int Quantity;

        public ItemStack()
        {
            ItemId = ItemId.None;
            Quantity = 0;
        }

        public ItemStack(ItemId itemId, int quantity)
        {
            ItemId = itemId;
            Quantity = quantity;
        }
    }
}

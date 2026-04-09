using System;
using System.Collections.Generic;

namespace InventoryUI.Inventory.Domain.Entities
{
    [Serializable]
    public class InventoryState
    {
        public int Coins;
        public float TotalWeight;
        public List<InventorySlot> Slots = new List<InventorySlot>();
    }
}

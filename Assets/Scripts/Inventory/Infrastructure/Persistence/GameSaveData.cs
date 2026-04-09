using System;
using System.Collections.Generic;

namespace InventoryUI.Inventory.Infrastructure.Persistence
{
    [Serializable]
    public sealed class GameSaveData
    {
        public int Coins;
        public List<SlotSaveData> Slots = new List<SlotSaveData>();
    }
}

using System.Collections.Generic;

namespace InventoryUI.Inventory.Application.DTO
{
    public sealed class InventoryScreenModel
    {
        public int Coins { get; set; }
        public float TotalWeight { get; set; }
        public IReadOnlyList<InventorySlotViewModel> Slots { get; set; }
    }
}

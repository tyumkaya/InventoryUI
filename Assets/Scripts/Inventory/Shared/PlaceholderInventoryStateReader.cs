using System.Collections.Generic;
using InventoryUI.Inventory.Application.DTO;
using InventoryUI.Inventory.Application.Interfaces;
using InventoryUI.Inventory.Application.Ports;
using InventoryUI.Inventory.Configs.Inventory;
using InventoryUI.Inventory.Configs.Items;

namespace InventoryUI.Inventory
{
    public sealed class PlaceholderInventoryStateReader : IInventoryStateReader
    {
        public PlaceholderInventoryStateReader(
            IGameStateRepository stateRepository,
            InventoryConfig inventoryConfig,
            ItemDatabase itemDatabase)
        {
        }

        public InventoryScreenModel GetScreenModel()
        {
            return new InventoryScreenModel
            {
                Coins = 0,
                TotalWeight = 0f,
                Slots = new List<InventorySlotViewModel>()
            };
        }
    }
}

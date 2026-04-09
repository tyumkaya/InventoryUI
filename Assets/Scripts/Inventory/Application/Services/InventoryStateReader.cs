using System.Collections.Generic;
using System.Text;
using System.Linq;
using InventoryUI.Inventory.Application.DTO;
using InventoryUI.Inventory.Application.Interfaces;
using InventoryUI.Inventory.Application.Ports;
using InventoryUI.Inventory.Configs.Inventory;
using InventoryUI.Inventory.Configs.Items;

namespace InventoryUI.Inventory.Application.Services
{
    public sealed class InventoryStateReader : IInventoryStateReader
    {
        private readonly IGameStateRepository stateRepository;
        private readonly InventoryConfig inventoryConfig;
        private readonly ItemDatabase itemDatabase;

        public InventoryStateReader(
            IGameStateRepository stateRepository,
            InventoryConfig inventoryConfig,
            ItemDatabase itemDatabase)
        {
            this.stateRepository = stateRepository;
            this.inventoryConfig = inventoryConfig;
            this.itemDatabase = itemDatabase;
        }

        public InventoryScreenModel GetScreenModel()
        {
            var state = stateRepository.Current;
            var slots = new List<InventorySlotViewModel>(state.Slots.Count);

            foreach (var slot in state.Slots.OrderBy(slot => slot.SlotId))
            {
                var itemConfig = slot.IsEmpty ? null : itemDatabase.Get(slot.Stack.ItemId);
                slots.Add(new InventorySlotViewModel
                {
                    SlotId = slot.SlotId,
                    IsUnlocked = slot.IsUnlocked,
                    IsEmpty = slot.IsEmpty,
                    ItemName = itemConfig != null ? itemConfig.DisplayName : string.Empty,
                    ItemInfoText = BuildItemInfoText(itemConfig, slot.IsEmpty ? 0 : slot.Stack.Quantity),
                    Quantity = slot.IsEmpty ? 0 : slot.Stack.Quantity,
                    UnlockPrice = GetUnlockPrice(slot.SlotId),
                    Icon = itemConfig != null ? itemConfig.Icon : null
                });
            }

            return new InventoryScreenModel
            {
                Coins = state.Coins,
                TotalWeight = state.TotalWeight,
                Slots = slots
            };
        }

        private int GetUnlockPrice(int slotId)
        {
            var cost = inventoryConfig.UnlockCosts.FirstOrDefault(entry => entry.SlotId == slotId);
            return cost.Cost;
        }

        private static string BuildItemInfoText(ItemConfig itemConfig, int quantity)
        {
            if (itemConfig == null)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();
            builder.AppendLine($"Name: {itemConfig.DisplayName}");
            builder.AppendLine($"Type: {itemConfig.ItemType}");
            builder.AppendLine($"Weight: {itemConfig.Weight:0.##}");
            builder.AppendLine($"Max Stack: {itemConfig.MaxStack}");
            builder.Append($"Quantity: {quantity}");

            if (itemConfig is WeaponItemConfig weaponConfig)
            {
                builder.AppendLine();
                builder.AppendLine($"Damage: {weaponConfig.Damage}");
                builder.Append($"Ammo ID: {weaponConfig.AmmoItemId}");
            }
            else if (itemConfig is ProtectionItemConfig protectionConfig)
            {
                builder.AppendLine();
                builder.Append($"Protection: {protectionConfig.Protection}");
            }

            return builder.ToString();
        }
    }
}

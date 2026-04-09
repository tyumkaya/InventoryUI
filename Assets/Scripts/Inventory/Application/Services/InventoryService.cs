using System;
using System.Collections.Generic;
using System.Linq;
using InventoryUI.Inventory.Configs.Inventory;
using InventoryUI.Inventory.Configs.Items;
using InventoryUI.Inventory.Domain.Entities;
using InventoryUI.Inventory.Domain.Enums;
using InventoryUI.Inventory.Infrastructure.Persistence;

namespace InventoryUI.Inventory.Application.Services
{
    public sealed class InventoryService
    {
        private readonly InventoryConfig inventoryConfig;
        private readonly ItemDatabase itemDatabase;
        private readonly Application.Ports.IRandomService randomService;

        public InventoryService(
            InventoryConfig inventoryConfig,
            ItemDatabase itemDatabase,
            Application.Ports.IRandomService randomService)
        {
            this.inventoryConfig = inventoryConfig;
            this.itemDatabase = itemDatabase;
            this.randomService = randomService;
        }

        public InventoryState CreateInitialState()
        {
            var state = new InventoryState();

            for (var i = 0; i < inventoryConfig.TotalSlots; i++)
            {
                state.Slots.Add(new InventorySlot
                {
                    SlotId = i,
                    IsUnlocked = i < inventoryConfig.InitiallyUnlockedSlots,
                    Stack = null
                });
            }

            RecalculateTotalWeight(state);
            return state;
        }

        public InventoryState CreateStateFromSave(GameSaveData saveData)
        {
            if (saveData == null)
            {
                return CreateInitialState();
            }

            var state = CreateInitialState();
            state.Coins = saveData.Coins;

            foreach (var slotData in saveData.Slots)
            {
                if (slotData == null || slotData.SlotId < 0 || slotData.SlotId >= state.Slots.Count)
                {
                    continue;
                }

                var slot = state.Slots[slotData.SlotId];
                slot.IsUnlocked = slotData.IsUnlocked;

                // If saved item id is invalid, keep the slot empty instead of failing the whole load.
                if (string.IsNullOrWhiteSpace(slotData.ItemId) ||
                    !Enum.TryParse(slotData.ItemId, out ItemId itemId) ||
                    itemId == ItemId.None ||
                    slotData.Quantity <= 0)
                {
                    slot.Stack = null;
                    continue;
                }

                slot.Stack = new ItemStack(itemId, slotData.Quantity);
            }

            NormalizeState(state);
            return state;
        }

        public GameSaveData CreateSaveData(InventoryState state)
        {
            var saveData = new GameSaveData
            {
                Coins = state.Coins
            };

            foreach (var slot in state.Slots)
            {
                saveData.Slots.Add(new SlotSaveData
                {
                    SlotId = slot.SlotId,
                    IsUnlocked = slot.IsUnlocked,
                    ItemId = slot.IsEmpty ? string.Empty : slot.Stack.ItemId.ToString(),
                    Quantity = slot.IsEmpty ? 0 : slot.Stack.Quantity
                });
            }

            return saveData;
        }

        public void NormalizeState(InventoryState state)
        {
            if (state == null)
            {
                return;
            }

            if (state.Slots == null)
            {
                state.Slots = new List<InventorySlot>();
            }

            while (state.Slots.Count < inventoryConfig.TotalSlots)
            {
                var slotId = state.Slots.Count;
                state.Slots.Add(new InventorySlot
                {
                    SlotId = slotId,
                    IsUnlocked = slotId < inventoryConfig.InitiallyUnlockedSlots,
                    Stack = null
                });
            }

            if (state.Slots.Count > inventoryConfig.TotalSlots)
            {
                state.Slots = state.Slots.Take(inventoryConfig.TotalSlots).ToList();
            }

            for (var i = 0; i < state.Slots.Count; i++)
            {
                var slot = state.Slots[i];
                slot.SlotId = i;

                if (!slot.IsUnlocked)
                {
                    slot.Stack = null;
                }

                if (slot.Stack != null && (slot.Stack.ItemId == ItemId.None || slot.Stack.Quantity <= 0))
                {
                    slot.Stack = null;
                }
            }

            RecalculateTotalWeight(state);
        }

        public void RecalculateTotalWeight(InventoryState state)
        {
            if (state == null)
            {
                return;
            }

            var totalWeight = 0f;
            foreach (var slot in state.Slots)
            {
                if (slot.IsEmpty)
                {
                    continue;
                }

                var config = itemDatabase.Get(slot.Stack.ItemId);
                if (config == null)
                {
                    continue;
                }

                totalWeight += config.Weight * slot.Stack.Quantity;
            }

            state.TotalWeight = totalWeight;
        }

        public bool TryUnlockNextSlot(InventoryState state, out int slotId, out int cost)
        {
            slotId = -1;
            cost = 0;

            var slot = state.Slots.FirstOrDefault(candidate => !candidate.IsUnlocked);
            if (slot == null)
            {
                return false;
            }

            slotId = slot.SlotId;
            cost = GetUnlockCost(slotId);
            slot.IsUnlocked = true;
            return true;
        }

        public int GetUnlockCost(int slotId)
        {
            var unlockCost = inventoryConfig.UnlockCosts.FirstOrDefault(cost => cost.SlotId == slotId);
            return unlockCost.Cost;
        }

        public bool TryAddRandomItem(InventoryState state, out int slotId, out string itemName)
        {
            slotId = -1;
            itemName = string.Empty;

            var candidates = itemDatabase.Items
                .Where(item => item != null && item.ItemType != ItemType.Ammo)
                .ToList();

            if (candidates.Count == 0)
            {
                return false;
            }

            var emptySlots = state.Slots.Where(slot => slot.IsUnlocked && slot.IsEmpty).ToList();
            if (emptySlots.Count == 0)
            {
                return false;
            }

            var item = candidates[randomService.RangeInclusive(0, candidates.Count - 1)];
            var targetSlot = emptySlots[randomService.RangeInclusive(0, emptySlots.Count - 1)];
            targetSlot.Stack = new ItemStack(item.ItemId, 1);
            slotId = targetSlot.SlotId;
            itemName = item.DisplayName;

            RecalculateTotalWeight(state);
            return true;
        }

        public AmmoAddResult TryAddRandomAmmo(InventoryState state)
        {
            var ammoConfigs = itemDatabase.Items
                .OfType<AmmoItemConfig>()
                .ToList();

            if (ammoConfigs.Count == 0)
            {
                return AmmoAddResult.Fail();
            }

            var ammoConfig = ammoConfigs[randomService.RangeInclusive(0, ammoConfigs.Count - 1)];
            var quantityToAdd = randomService.RangeInclusive(10, 30);
            var remaining = quantityToAdd;
            var additions = new List<AmmoAddition>();

            foreach (var slot in state.Slots.Where(slot => slot.IsUnlocked && !slot.IsEmpty && slot.Stack.ItemId == ammoConfig.ItemId))
            {
                var capacity = Math.Max(0, ammoConfig.MaxStack - slot.Stack.Quantity);
                if (capacity <= 0)
                {
                    continue;
                }

                var added = Math.Min(capacity, remaining);
                slot.Stack.Quantity += added;
                remaining -= added;
                additions.Add(new AmmoAddition(slot.SlotId, ammoConfig.ItemId, added));

                if (remaining <= 0)
                {
                    break;
                }
            }

            if (remaining > 0)
            {
                foreach (var slot in state.Slots.Where(slot => slot.IsUnlocked && slot.IsEmpty))
                {
                    var added = Math.Min(ammoConfig.MaxStack, remaining);
                    slot.Stack = new ItemStack(ammoConfig.ItemId, added);
                    remaining -= added;
                    additions.Add(new AmmoAddition(slot.SlotId, ammoConfig.ItemId, added));

                    if (remaining <= 0)
                    {
                        break;
                    }
                }
            }

            if (additions.Count == 0)
            {
                return AmmoAddResult.Fail();
            }

            RecalculateTotalWeight(state);
            return AmmoAddResult.Success(ammoConfig.ItemId, quantityToAdd - remaining, additions);
        }

        public bool TryRemoveRandomItem(InventoryState state, out int slotId, out int quantity, out string itemName)
        {
            slotId = -1;
            quantity = 0;
            itemName = string.Empty;

            var occupiedSlots = state.Slots.Where(slot => slot.IsUnlocked && !slot.IsEmpty).ToList();
            if (occupiedSlots.Count == 0)
            {
                return false;
            }

            var slot = occupiedSlots[randomService.RangeInclusive(0, occupiedSlots.Count - 1)];
            slotId = slot.SlotId;
            quantity = slot.Stack.Quantity;
            var itemConfig = itemDatabase.Get(slot.Stack.ItemId);
            itemName = itemConfig != null ? itemConfig.DisplayName : slot.Stack.ItemId.ToString();
            slot.Stack = null;

            RecalculateTotalWeight(state);
            return true;
        }

        public bool TryMoveItem(InventoryState state, int fromSlotId, int toSlotId)
        {
            if (state == null || fromSlotId == toSlotId)
            {
                return false;
            }

            var sourceSlot = GetSlot(state, fromSlotId);
            var targetSlot = GetSlot(state, toSlotId);
            if (sourceSlot == null || targetSlot == null || !sourceSlot.IsUnlocked || !targetSlot.IsUnlocked || sourceSlot.IsEmpty)
            {
                return false;
            }

            if (targetSlot.IsEmpty)
            {
                targetSlot.Stack = sourceSlot.Stack;
                sourceSlot.Stack = null;
                RecalculateTotalWeight(state);
                return true;
            }

            if (TryMergeStacks(sourceSlot, targetSlot))
            {
                RecalculateTotalWeight(state);
                return true;
            }

            var temp = targetSlot.Stack;
            targetSlot.Stack = sourceSlot.Stack;
            sourceSlot.Stack = temp;
            RecalculateTotalWeight(state);
            return true;
        }

        public ShootResult TryShootRandomWeapon(InventoryState state)
        {
            var weaponSlots = state.Slots
                .Where(slot => slot.IsUnlocked && !slot.IsEmpty)
                .Where(slot => itemDatabase.Get(slot.Stack.ItemId) is WeaponItemConfig)
                .ToList();

            if (weaponSlots.Count == 0)
            {
                return ShootResult.FailNoWeapon();
            }

            var weaponSlot = weaponSlots[randomService.RangeInclusive(0, weaponSlots.Count - 1)];
            var weaponConfig = itemDatabase.Get(weaponSlot.Stack.ItemId) as WeaponItemConfig;
            if (weaponConfig == null)
            {
                return ShootResult.FailNoWeapon();
            }

            var ammoSlot = state.Slots
                .Where(slot => slot.IsUnlocked && !slot.IsEmpty && slot.Stack.ItemId == weaponConfig.AmmoItemId)
                .FirstOrDefault();

            if (ammoSlot == null)
            {
                return ShootResult.FailNoAmmo(weaponConfig.DisplayName);
            }

            ammoSlot.Stack.Quantity -= 1;
            if (ammoSlot.Stack.Quantity <= 0)
            {
                ammoSlot.Stack = null;
            }

            RecalculateTotalWeight(state);

            var ammoConfig = itemDatabase.Get(weaponConfig.AmmoItemId);
            return ShootResult.Success(weaponConfig.DisplayName, ammoConfig != null ? ammoConfig.DisplayName : weaponConfig.AmmoItemId.ToString(), weaponConfig.Damage);
        }

        private InventorySlot GetSlot(InventoryState state, int slotId)
        {
            if (slotId < 0 || slotId >= state.Slots.Count)
            {
                return null;
            }

            return state.Slots[slotId];
        }

        private bool TryMergeStacks(InventorySlot sourceSlot, InventorySlot targetSlot)
        {
            if (sourceSlot.IsEmpty || targetSlot.IsEmpty || sourceSlot.Stack.ItemId != targetSlot.Stack.ItemId)
            {
                return false;
            }

            var itemConfig = itemDatabase.Get(sourceSlot.Stack.ItemId);
            if (itemConfig == null || itemConfig.MaxStack <= 1)
            {
                return false;
            }

            var freeSpace = itemConfig.MaxStack - targetSlot.Stack.Quantity;
            if (freeSpace <= 0)
            {
                return false;
            }

            var movedQuantity = Math.Min(freeSpace, sourceSlot.Stack.Quantity);
            targetSlot.Stack.Quantity += movedQuantity;
            sourceSlot.Stack.Quantity -= movedQuantity;

            if (sourceSlot.Stack.Quantity <= 0)
            {
                sourceSlot.Stack = null;
            }

            return movedQuantity > 0;
        }

        public sealed class AmmoAddition
        {
            public AmmoAddition(int slotId, ItemId itemId, int quantity)
            {
                SlotId = slotId;
                ItemId = itemId;
                Quantity = quantity;
            }

            public int SlotId { get; }
            public ItemId ItemId { get; }
            public int Quantity { get; }
        }

        public sealed class AmmoAddResult
        {
            private AmmoAddResult(bool isSuccess, ItemId ammoItemId, int totalAdded, IReadOnlyList<AmmoAddition> additions)
            {
                IsSuccess = isSuccess;
                AmmoItemId = ammoItemId;
                TotalAdded = totalAdded;
                Additions = additions;
            }

            public bool IsSuccess { get; }
            public ItemId AmmoItemId { get; }
            public int TotalAdded { get; }
            public IReadOnlyList<AmmoAddition> Additions { get; }

            public static AmmoAddResult Success(ItemId ammoItemId, int totalAdded, IReadOnlyList<AmmoAddition> additions)
            {
                return new AmmoAddResult(true, ammoItemId, totalAdded, additions);
            }

            public static AmmoAddResult Fail()
            {
                return new AmmoAddResult(false, ItemId.None, 0, Array.Empty<AmmoAddition>());
            }
        }

        public sealed class ShootResult
        {
            private ShootResult(bool isSuccess, string errorMessage, string weaponName, string ammoName, int damage)
            {
                IsSuccess = isSuccess;
                ErrorMessage = errorMessage;
                WeaponName = weaponName;
                AmmoName = ammoName;
                Damage = damage;
            }

            public bool IsSuccess { get; }
            public string ErrorMessage { get; }
            public string WeaponName { get; }
            public string AmmoName { get; }
            public int Damage { get; }

            public static ShootResult Success(string weaponName, string ammoName, int damage)
            {
                return new ShootResult(true, string.Empty, weaponName, ammoName, damage);
            }

            public static ShootResult FailNoWeapon()
            {
                return new ShootResult(false, "Нет оружия", string.Empty, string.Empty, 0);
            }

            public static ShootResult FailNoAmmo(string weaponName)
            {
                return new ShootResult(false, $"Нет патронов для {weaponName}", weaponName, string.Empty, 0);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace InventoryUI.Inventory.Configs.Inventory
{
    [CreateAssetMenu(menuName = "Inventory/Configs/Inventory Config", fileName = "InventoryConfig")]
    public sealed class InventoryConfig : ScriptableObject
    {
        [SerializeField] private int totalSlots = 50;
        [SerializeField] private int initiallyUnlockedSlots = 15;
        [SerializeField] private List<SlotUnlockCost> unlockCosts = new List<SlotUnlockCost>();

        public int TotalSlots => totalSlots;
        public int InitiallyUnlockedSlots => initiallyUnlockedSlots;
        public IReadOnlyList<SlotUnlockCost> UnlockCosts => unlockCosts;
    }

    [Serializable]
    public struct SlotUnlockCost
    {
        public int SlotId;
        public int Cost;
    }
}

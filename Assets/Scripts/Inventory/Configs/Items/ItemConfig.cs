using InventoryUI.Inventory.Domain.Enums;
using UnityEngine;

namespace InventoryUI.Inventory.Configs.Items
{
    public abstract class ItemConfig : ScriptableObject
    {
        [SerializeField] private ItemId itemId = ItemId.None;
        [SerializeField] private string displayName;
        [SerializeField] private ItemType itemType = ItemType.None;
        [SerializeField] private Sprite icon;
        [SerializeField] private int maxStack = 1;
        [SerializeField] private float weight = 1f;

        public ItemId ItemId => itemId;
        public string DisplayName => displayName;
        public ItemType ItemType => itemType;
        public abstract ItemCategory Category { get; }
        public Sprite Icon => icon;
        public int MaxStack => maxStack;
        public float Weight => weight;
    }
}

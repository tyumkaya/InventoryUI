using System.Collections.Generic;
using System.Linq;
using InventoryUI.Inventory.Domain.Enums;
using UnityEngine;

namespace InventoryUI.Inventory.Configs.Items
{
    [CreateAssetMenu(menuName = "Inventory/Items/Item Database", fileName = "ItemDatabase")]
    public sealed class ItemDatabase : ScriptableObject
    {
        [SerializeField] private List<ItemConfig> items = new List<ItemConfig>();

        public IReadOnlyList<ItemConfig> Items => items;

        public ItemConfig Get(ItemId itemId)
        {
            return items.FirstOrDefault(item => item != null && item.ItemId == itemId);
        }

        public IReadOnlyList<ItemConfig> GetByType(ItemType itemType)
        {
            return items.Where(item => item != null && item.ItemType == itemType).ToList();
        }
    }
}

using InventoryUI.Inventory.Domain.Enums;
using UnityEngine;

namespace InventoryUI.Inventory.Configs.Items
{
    [CreateAssetMenu(menuName = "Inventory/Items/Weapon Item Config", fileName = "WeaponItemConfig")]
    public sealed class WeaponItemConfig : ItemConfig
    {
        [SerializeField] private int damage;
        [SerializeField] private ItemId ammoItemId = ItemId.None;

        public override ItemCategory Category => ItemCategory.Weapon;
        public int Damage => damage;
        public ItemId AmmoItemId => ammoItemId;
    }
}

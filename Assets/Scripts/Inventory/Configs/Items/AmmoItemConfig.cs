using InventoryUI.Inventory.Domain.Enums;
using UnityEngine;

namespace InventoryUI.Inventory.Configs.Items
{
    [CreateAssetMenu(menuName = "Inventory/Items/Ammo Item Config", fileName = "AmmoItemConfig")]
    public sealed class AmmoItemConfig : ItemConfig
    {
        public override ItemCategory Category => ItemCategory.Ammo;
    }
}

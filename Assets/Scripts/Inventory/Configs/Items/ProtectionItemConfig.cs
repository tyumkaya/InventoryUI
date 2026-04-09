using InventoryUI.Inventory.Domain.Enums;
using UnityEngine;

namespace InventoryUI.Inventory.Configs.Items
{
    [CreateAssetMenu(menuName = "Inventory/Items/Protection Item Config", fileName = "ProtectionItemConfig")]
    public sealed class ProtectionItemConfig : ItemConfig
    {
        [SerializeField] private int protection;

        public override ItemCategory Category => ItemCategory.Protection;
        public int Protection => protection;
    }
}

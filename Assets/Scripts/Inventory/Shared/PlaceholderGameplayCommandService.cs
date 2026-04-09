using InventoryUI.Inventory.Application.DTO;
using InventoryUI.Inventory.Application.Interfaces;
using InventoryUI.Inventory.Application.Ports;
using InventoryUI.Inventory.Configs.Inventory;
using InventoryUI.Inventory.Configs.Items;

namespace InventoryUI.Inventory
{
    public sealed class PlaceholderGameplayCommandService : IGameplayCommandService
    {
        public PlaceholderGameplayCommandService(
            ISaveGateway saveGateway,
            IRandomService randomService,
            IGameLogger logger,
            IGameStateRepository stateRepository,
            InventoryConfig inventoryConfig,
            ItemDatabase itemDatabase)
        {
        }

        public ActionExecutionResult Load()
        {
            return ActionExecutionResult.Pending("Load pipeline is not implemented yet.");
        }

        public ActionExecutionResult AddCoins()
        {
            return ActionExecutionResult.Pending("Add coins is not implemented yet.");
        }

        public ActionExecutionResult AddRandomItem()
        {
            return ActionExecutionResult.Pending("Add item is not implemented yet.");
        }

        public ActionExecutionResult AddRandomAmmo()
        {
            return ActionExecutionResult.Pending("Add ammo is not implemented yet.");
        }

        public ActionExecutionResult ShootRandomWeapon()
        {
            return ActionExecutionResult.Pending("Shoot is not implemented yet.");
        }

        public ActionExecutionResult RemoveRandomItem()
        {
            return ActionExecutionResult.Pending("Remove item is not implemented yet.");
        }

        public ActionExecutionResult UnlockNextSlot()
        {
            return ActionExecutionResult.Pending("Unlock slot is not implemented yet.");
        }

        public ActionExecutionResult TryUnlockSlot(int slotId)
        {
            return ActionExecutionResult.Pending("Unlock slot is not implemented yet.");
        }

        public ActionExecutionResult TryMoveItem(int fromSlotId, int toSlotId)
        {
            return ActionExecutionResult.Pending("Move item is not implemented yet.");
        }

        public ActionExecutionResult Save()
        {
            return ActionExecutionResult.Pending("Save pipeline is not implemented yet.");
        }
    }
}

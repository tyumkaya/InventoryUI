using InventoryUI.Inventory.Application.DTO;

namespace InventoryUI.Inventory.Application.Interfaces
{
    public interface IGameplayCommandService
    {
        ActionExecutionResult Load();
        ActionExecutionResult AddCoins();
        ActionExecutionResult AddRandomItem();
        ActionExecutionResult AddRandomAmmo();
        ActionExecutionResult ShootRandomWeapon();
        ActionExecutionResult RemoveRandomItem();
        ActionExecutionResult UnlockNextSlot();
        ActionExecutionResult TryUnlockSlot(int slotId);
        ActionExecutionResult TryMoveItem(int fromSlotId, int toSlotId);
        ActionExecutionResult Save();
    }
}

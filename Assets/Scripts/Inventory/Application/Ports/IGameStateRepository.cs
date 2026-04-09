using InventoryUI.Inventory.Domain.Entities;

namespace InventoryUI.Inventory.Application.Ports
{
    public interface IGameStateRepository
    {
        InventoryState Current { get; }
        void Set(InventoryState state);
    }
}

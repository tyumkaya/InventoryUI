using InventoryUI.Inventory.Application.Ports;
using InventoryUI.Inventory.Domain.Entities;

namespace InventoryUI.Inventory.Infrastructure.Persistence
{
    public sealed class InMemoryGameStateRepository : IGameStateRepository
    {
        public InventoryState Current { get; private set; } = new InventoryState();

        public void Set(InventoryState state)
        {
            Current = state ?? new InventoryState();
        }
    }
}

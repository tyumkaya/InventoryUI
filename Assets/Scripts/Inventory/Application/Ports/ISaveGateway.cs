using InventoryUI.Inventory.Infrastructure.Persistence;

namespace InventoryUI.Inventory.Application.Ports
{
    public interface ISaveGateway
    {
        GameSaveData Load();
        void Save(GameSaveData data);
    }
}

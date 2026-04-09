using InventoryUI.Inventory.Application.DTO;

namespace InventoryUI.Inventory.Application.Interfaces
{
    public interface IInventoryStateReader
    {
        InventoryScreenModel GetScreenModel();
    }
}

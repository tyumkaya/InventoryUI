namespace InventoryUI.Inventory.Application.Ports
{
    public interface IGameLogger
    {
        void Log(string message);
        void LogError(string message);
    }
}

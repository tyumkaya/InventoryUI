namespace InventoryUI.Inventory.Application.Ports
{
    public interface IRandomService
    {
        int RangeInclusive(int minInclusive, int maxInclusive);
    }
}

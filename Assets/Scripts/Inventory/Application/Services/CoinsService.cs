using InventoryUI.Inventory.Domain.Entities;
using InventoryUI.Inventory.Application.Ports;

namespace InventoryUI.Inventory.Application.Services
{
    public sealed class CoinsService
    {
        private readonly IRandomService randomService;

        public CoinsService(IRandomService randomService)
        {
            this.randomService = randomService;
        }

        public void Add(InventoryState state, int amount)
        {
            if (state == null || amount <= 0)
            {
                return;
            }

            state.Coins += amount;
        }

        public bool TrySpend(InventoryState state, int amount)
        {
            if (state == null || amount <= 0 || state.Coins < amount)
            {
                return false;
            }

            state.Coins -= amount;
            return true;
        }

        public int RollAddCoinsAmount()
        {
            return randomService.RangeInclusive(9, 99);
        }
    }
}

using InventoryUI.Inventory.Application.Ports;
using UnityEngine;

namespace InventoryUI.Inventory.Infrastructure.Random
{
    public sealed class UnityRandomService : IRandomService
    {
        public int RangeInclusive(int minInclusive, int maxInclusive)
        {
            return UnityEngine.Random.Range(minInclusive, maxInclusive + 1);
        }
    }
}

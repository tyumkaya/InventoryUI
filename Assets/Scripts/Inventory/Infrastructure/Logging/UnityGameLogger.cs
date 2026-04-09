using InventoryUI.Inventory.Application.Ports;
using UnityEngine;

namespace InventoryUI.Inventory.Infrastructure.Logging
{
    public sealed class UnityGameLogger : IGameLogger
    {
        public void Log(string message)
        {
            Debug.Log(WrapInRedColor(message));
        }

        public void LogError(string message)
        {
            Debug.LogError(WrapInRedColor(message));
        }

        private static string WrapInRedColor(string message)
        {
            return $"<color=red>{message}</color>";
        }
    }
}

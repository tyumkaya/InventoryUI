using System.IO;
using InventoryUI.Inventory.Application.Ports;
using UnityEngine;

namespace InventoryUI.Inventory.Infrastructure.Persistence
{
    public sealed class JsonFileSaveGateway : ISaveGateway
    {
        private const string FileName = "inventory-save.json";

        private string FilePath
        {
            get
            {
                return Path.Combine(UnityEngine.Application.persistentDataPath, FileName);
            }
        }

        public GameSaveData Load()
        {
            if (!File.Exists(FilePath))
            {
                return null;
            }

            var json = File.ReadAllText(FilePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            return JsonUtility.FromJson<GameSaveData>(json);
        }

        public void Save(GameSaveData data)
        {
            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(FilePath, json);
        }
    }
}

using InventoryUI.Inventory.Application.DTO;
using TMPro;
using UnityEngine;

namespace InventoryUI.Inventory.Presentation.Views
{
    public sealed class HudView : MonoBehaviour
    {
        [SerializeField] private TMP_Text coinsText;
        [SerializeField] private TMP_Text weightText;

        public void Render(InventoryScreenModel model)
        {
            if (model == null)
            {
                return;
            }

            if (coinsText != null)
            {
                coinsText.text = "Монеты: " + model.Coins.ToString();
            }

            if (weightText != null)
            {
                weightText.text = "Вес: " + model.TotalWeight.ToString("0.##");
            }
        }
    }
}

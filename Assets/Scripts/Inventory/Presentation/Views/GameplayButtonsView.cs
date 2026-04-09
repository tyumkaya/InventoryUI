using System;
using UnityEngine;
using UnityEngine.UI;

namespace InventoryUI.Inventory.Presentation.Views
{
    public sealed class GameplayButtonsView : MonoBehaviour
    {
        [SerializeField] private Button addCoinsButton;
        [SerializeField] private Button addItemButton;
        [SerializeField] private Button addAmmoButton;
        [SerializeField] private Button shootButton;
        [SerializeField] private Button removeItemButton;

        public event Action AddCoinsClicked;
        public event Action AddItemClicked;
        public event Action AddAmmoClicked;
        public event Action ShootClicked;
        public event Action RemoveItemClicked;

        private void Awake()
        {
            if (addCoinsButton != null)
            {
                addCoinsButton.onClick.AddListener(() => AddCoinsClicked?.Invoke());
            }

            if (addItemButton != null)
            {
                addItemButton.onClick.AddListener(() => AddItemClicked?.Invoke());
            }

            if (addAmmoButton != null)
            {
                addAmmoButton.onClick.AddListener(() => AddAmmoClicked?.Invoke());
            }

            if (shootButton != null)
            {
                shootButton.onClick.AddListener(() => ShootClicked?.Invoke());
            }

            if (removeItemButton != null)
            {
                removeItemButton.onClick.AddListener(() => RemoveItemClicked?.Invoke());
            }
        }
    }
}

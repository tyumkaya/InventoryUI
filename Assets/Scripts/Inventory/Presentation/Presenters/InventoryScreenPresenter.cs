using InventoryUI.Inventory.Application.Interfaces;
using InventoryUI.Inventory.Presentation.Views;

namespace InventoryUI.Inventory.Presentation.Presenters
{
    public sealed class InventoryScreenPresenter
    {
        private readonly GameplayButtonsView gameplayButtonsView;
        private readonly HudView hudView;
        private readonly InventoryGridView inventoryGridView;
        private readonly IGameplayCommandService commandService;
        private readonly IInventoryStateReader stateReader;
        private int? draggedSlotId;

        public InventoryScreenPresenter(
            GameplayButtonsView gameplayButtonsView,
            HudView hudView,
            InventoryGridView inventoryGridView,
            IGameplayCommandService commandService,
            IInventoryStateReader stateReader)
        {
            this.gameplayButtonsView = gameplayButtonsView;
            this.hudView = hudView;
            this.inventoryGridView = inventoryGridView;
            this.commandService = commandService;
            this.stateReader = stateReader;
        }

        public void Initialize()
        {
            gameplayButtonsView.AddCoinsClicked += OnAddCoinsClicked;
            gameplayButtonsView.AddItemClicked += OnAddItemClicked;
            gameplayButtonsView.AddAmmoClicked += OnAddAmmoClicked;
            gameplayButtonsView.ShootClicked += OnShootClicked;
            gameplayButtonsView.RemoveItemClicked += OnRemoveItemClicked;
            inventoryGridView.LockedSlotClicked += OnLockedSlotClicked;
            inventoryGridView.UnlockedSlotClicked += OnUnlockedSlotClicked;
            inventoryGridView.SlotDragStarted += OnSlotDragStarted;
            inventoryGridView.SlotDragEnded += OnSlotDragEnded;
            inventoryGridView.SlotDroppedOn += OnSlotDroppedOn;

            commandService.Load();
            Refresh();
        }

        public void Dispose()
        {
            gameplayButtonsView.AddCoinsClicked -= OnAddCoinsClicked;
            gameplayButtonsView.AddItemClicked -= OnAddItemClicked;
            gameplayButtonsView.AddAmmoClicked -= OnAddAmmoClicked;
            gameplayButtonsView.ShootClicked -= OnShootClicked;
            gameplayButtonsView.RemoveItemClicked -= OnRemoveItemClicked;
            inventoryGridView.LockedSlotClicked -= OnLockedSlotClicked;
            inventoryGridView.UnlockedSlotClicked -= OnUnlockedSlotClicked;
            inventoryGridView.SlotDragStarted -= OnSlotDragStarted;
            inventoryGridView.SlotDragEnded -= OnSlotDragEnded;
            inventoryGridView.SlotDroppedOn -= OnSlotDroppedOn;
        }

        private void OnAddCoinsClicked()
        {
            commandService.AddCoins();
            Refresh();
        }

        private void OnAddItemClicked()
        {
            commandService.AddRandomItem();
            Refresh();
        }

        private void OnAddAmmoClicked()
        {
            commandService.AddRandomAmmo();
            Refresh();
        }

        private void OnShootClicked()
        {
            commandService.ShootRandomWeapon();
            Refresh();
        }

        private void OnRemoveItemClicked()
        {
            commandService.RemoveRandomItem();
            Refresh();
        }

        private void OnLockedSlotClicked(int slotId)
        {
            commandService.TryUnlockSlot(slotId);
            Refresh();
        }

        private void OnUnlockedSlotClicked(int slotId)
        {
            // Future hook for item info popup from UnlockedView button.
        }

        private void OnSlotDragStarted(int slotId)
        {
            draggedSlotId = slotId;
        }

        private void OnSlotDragEnded(int slotId)
        {
            draggedSlotId = null;
        }

        private void OnSlotDroppedOn(int slotId)
        {
            if (!draggedSlotId.HasValue)
            {
                return;
            }

            var sourceSlotId = draggedSlotId.Value;
            draggedSlotId = null;
            commandService.TryMoveItem(sourceSlotId, slotId);
            Refresh();
        }

        private void Refresh()
        {
            var model = stateReader.GetScreenModel();
            hudView.Render(model);
            inventoryGridView.Render(model.Slots);
        }
    }
}

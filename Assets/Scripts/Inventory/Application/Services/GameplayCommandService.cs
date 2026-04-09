using InventoryUI.Inventory.Application.DTO;
using InventoryUI.Inventory.Application.Interfaces;
using InventoryUI.Inventory.Application.Ports;

namespace InventoryUI.Inventory.Application.Services
{
    public sealed class GameplayCommandService : IGameplayCommandService
    {
        private readonly ISaveGateway saveGateway;
        private readonly IGameLogger logger;
        private readonly IGameStateRepository stateRepository;
        private readonly CoinsService coinsService;
        private readonly InventoryService inventoryService;

        public GameplayCommandService(
            ISaveGateway saveGateway,
            IGameLogger logger,
            IGameStateRepository stateRepository,
            CoinsService coinsService,
            InventoryService inventoryService)
        {
            this.saveGateway = saveGateway;
            this.logger = logger;
            this.stateRepository = stateRepository;
            this.coinsService = coinsService;
            this.inventoryService = inventoryService;
        }

        public ActionExecutionResult Load()
        {
            var saveData = saveGateway.Load();
            var state = inventoryService.CreateStateFromSave(saveData);
            stateRepository.Set(state);

            if (saveData == null)
            {
                Save();
            }

            return ActionExecutionResult.Success(string.Empty, false);
        }

        public ActionExecutionResult AddCoins()
        {
            var state = stateRepository.Current;
            var amount = coinsService.RollAddCoinsAmount();
            coinsService.Add(state, amount);
            inventoryService.RecalculateTotalWeight(state);
            Persist();

            var message = $"Добавлено {amount} монет";
            logger.Log(message);
            return ActionExecutionResult.Success(message);
        }

        public ActionExecutionResult AddRandomItem()
        {
            var state = stateRepository.Current;
            if (!inventoryService.TryAddRandomItem(state, out var slotId, out var itemName))
            {
                logger.LogError("Инвентарь полон");
                return ActionExecutionResult.Failure("Инвентарь полон");
            }

            Persist();
            var message = $"Добавлено {itemName} в слот: {slotId}";
            logger.Log(message);
            return ActionExecutionResult.Success(message);
        }

        public ActionExecutionResult AddRandomAmmo()
        {
            var state = stateRepository.Current;
            var result = inventoryService.TryAddRandomAmmo(state);
            if (!result.IsSuccess)
            {
                logger.LogError("Инвентарь полон");
                return ActionExecutionResult.Failure("Инвентарь полон");
            }

            Persist();

            // TЗ не фиксирует точный формат этих сообщений, поэтому логируем каждое фактическое изменение слота отдельно.
            foreach (var addition in result.Additions)
            {
                logger.Log($"Добавлено {addition.Quantity} {addition.ItemId} в слот: {addition.SlotId}");
            }

            return ActionExecutionResult.Success("Ammo added.");
        }

        public ActionExecutionResult ShootRandomWeapon()
        {
            var state = stateRepository.Current;
            var result = inventoryService.TryShootRandomWeapon(state);
            if (!result.IsSuccess)
            {
                logger.LogError(result.ErrorMessage);
                return ActionExecutionResult.Failure(result.ErrorMessage);
            }

            Persist();
            var message = $"Выстрел из {result.WeaponName}, патроны: {result.AmmoName}, урон: {result.Damage}";
            logger.Log(message);
            return ActionExecutionResult.Success(message);
        }

        public ActionExecutionResult RemoveRandomItem()
        {
            var state = stateRepository.Current;
            if (!inventoryService.TryRemoveRandomItem(state, out var slotId, out var quantity, out var itemName))
            {
                logger.LogError("Инвентарь пуст");
                return ActionExecutionResult.Failure("Инвентарь пуст");
            }

            Persist();
            var message = $"Удалено {quantity} {itemName} из слота: {slotId}";
            logger.Log(message);
            return ActionExecutionResult.Success(message);
        }

        public ActionExecutionResult UnlockNextSlot()
        {
            var state = stateRepository.Current;
            var nextLockedSlotId = state.Slots.FindIndex(slot => !slot.IsUnlocked);
            if (nextLockedSlotId < 0)
            {
                return ActionExecutionResult.Failure("Все слоты уже открыты");
            }

            var cost = inventoryService.GetUnlockCost(nextLockedSlotId);
            if (!coinsService.TrySpend(state, cost))
            {
                return ActionExecutionResult.Failure("Недостаточно монет");
            }

            if (!inventoryService.TryUnlockNextSlot(state, out _, out _))
            {
                return ActionExecutionResult.Failure("Все слоты уже открыты");
            }

            Persist();
            return ActionExecutionResult.Success(string.Empty);
        }

        public ActionExecutionResult TryUnlockSlot(int slotId)
        {
            var state = stateRepository.Current;
            var nextLockedSlotId = state.Slots.FindIndex(slot => !slot.IsUnlocked);
            if (nextLockedSlotId < 0)
            {
                return ActionExecutionResult.Failure("Все слоты уже открыты");
            }

            if (slotId != nextLockedSlotId)
            {
                return ActionExecutionResult.Failure("Слот нельзя открыть");
            }

            return UnlockNextSlot();
        }

        public ActionExecutionResult TryMoveItem(int fromSlotId, int toSlotId)
        {
            var state = stateRepository.Current;
            if (!inventoryService.TryMoveItem(state, fromSlotId, toSlotId))
            {
                return ActionExecutionResult.Failure("Невозможно переместить предмет");
            }

            Persist();
            return ActionExecutionResult.Success(string.Empty);
        }

        public ActionExecutionResult Save()
        {
            Persist();
            return ActionExecutionResult.Success(string.Empty, false);
        }

        private void Persist()
        {
            saveGateway.Save(inventoryService.CreateSaveData(stateRepository.Current));
        }
    }
}

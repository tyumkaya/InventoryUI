using InventoryUI.Inventory.Application.Interfaces;
using InventoryUI.Inventory.Application.Services;
using InventoryUI.Inventory.Configs.Inventory;
using InventoryUI.Inventory.Configs.Items;
using InventoryUI.Inventory.Infrastructure.Logging;
using InventoryUI.Inventory.Infrastructure.Persistence;
using InventoryUI.Inventory.Infrastructure.Random;
using InventoryUI.Inventory.Presentation.Presenters;
using InventoryUI.Inventory.Presentation.Views;
using UnityEngine;

namespace InventoryUI.Inventory.Presentation.Bootstrap
{
    public sealed class InventoryBootstrapper : MonoBehaviour
    {
        [Header("Configs")]
        [SerializeField] private InventoryConfig inventoryConfig;
        [SerializeField] private ItemDatabase itemDatabase;

        [Header("Views")]
        [SerializeField] private GameplayButtonsView gameplayButtonsView;
        [SerializeField] private HudView hudView;
        [SerializeField] private InventoryGridView inventoryGridView;

        private IGameplayCommandService commandService;
        private IInventoryStateReader stateReader;
        private InventoryScreenPresenter presenter;

        private void Awake()
        {
            var saveGateway = new JsonFileSaveGateway();
            var randomService = new UnityRandomService();
            var logger = new UnityGameLogger();
            var stateRepository = new InMemoryGameStateRepository();
            var coinsService = new CoinsService(randomService);
            var inventoryService = new InventoryService(inventoryConfig, itemDatabase, randomService);

            commandService = new GameplayCommandService(
                saveGateway,
                logger,
                stateRepository,
                coinsService,
                inventoryService);

            stateReader = new InventoryStateReader(
                stateRepository,
                inventoryConfig,
                itemDatabase);

            presenter = new InventoryScreenPresenter(
                gameplayButtonsView,
                hudView,
                inventoryGridView,
                commandService,
                stateReader);

            presenter.Initialize();
        }

        private void OnDestroy()
        {
            presenter?.Dispose();
        }
    }
}

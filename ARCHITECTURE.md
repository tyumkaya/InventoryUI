# Inventory Test Task Architecture

## Goals

- Keep gameplay logic independent from `MonoBehaviour` where practical.
- Separate config data, runtime state, persistence DTO, and UI rendering.
- Use `ScriptableObject` for editable item data and slot unlock pricing.
- Save/load through JSON file in `Application.persistentDataPath`.
- Do not add optional features outside the task.

## Fixed Assumptions

- Slot count is fixed at 50.
- Initially unlocked slots count is fixed by config and defaults to 15.
- Unlocking always opens the next locked slot in order.
- Unlock prices are configured per slot index from 15 to 49.
- Non-ammo items have stack size `1`.
- Ammo items may stack; max stack size is configured in item config.
- `Remove item` removes the whole content of one random unlocked non-empty slot.
- `Add item` uses only categories `Weapon`, `Head`, `Torso` as required by the task.
- `Shoot` chooses a random weapon from inventory, consumes exactly `1` compatible ammo, and logs weapon/ammo/damage from config.
- Save file stores only runtime state. Static data stays in `ScriptableObject`.

## Folder Structure

```text
Assets/
  Scripts/
    Inventory/
      Domain/
        Enums/
        Entities/
        ValueObjects/
      Application/
        DTO/
        Interfaces/
        Ports/
        UseCases/
      Infrastructure/
        Persistence/
        Random/
        Logging/
      Presentation/
        Bootstrap/
        Presenters/
        Views/
      Configs/
        Items/
        Inventory/
      Shared/
```

## Main Classes And Responsibilities

### Domain

- `ItemId` - stable item identifiers used in config, runtime state, and save data.
- `ItemType` - concrete gameplay item kind: `Head`, `Torso`, `Ammo`, `Weapon`.
- `ItemCategory` - distinguishes protection / ammo / weapon config types.
- `ItemStack` - runtime stack model: item id + quantity.
- `InventorySlot` - runtime slot model: slot id, unlock flag, optional stack.
- `InventoryState` - runtime aggregate: coins + slots + calculated total weight.

### Application

- `IGameplayCommandService` - UI-facing command port for gameplay buttons and slot unlock action.
- `IInventoryStateReader` - UI-facing query port that returns screen view models.
- `CoinsService` - adds/spends coins in runtime state.
- `InventoryService` - owns inventory rules, load/save mapping, slot unlocks, random item/ammo actions, shooting, removal, and total weight calculation.
- `GameplayCommandService` - orchestrates commands, logs required messages, and persists after each successful mutation.
- `InventoryStateReader` - builds a screen-friendly read model from runtime state and config.
- `ActionExecutionResult` - standard result model for commands.
- `InventoryScreenModel` - read model for the whole screen.
- `InventorySlotViewModel` - read model for one slot.
- `IGameStateRepository` - stores current runtime state in memory.
- `ISaveGateway` - loads and saves JSON persistence DTO.
- `IRandomService` - wraps random generation so use cases stay Unity-independent.
- `IGameLogger` - central log output abstraction.

### Infrastructure

- `GameSaveData` - JSON root DTO for persistence.
- `SlotSaveData` - JSON DTO for one slot.
- `InMemoryGameStateRepository` - runtime state holder.
- `JsonFileSaveGateway` - reads/writes JSON file in persistent data path.
- `UnityRandomService` - adapter over `UnityEngine.Random`.
- `UnityGameLogger` - adapter over `Debug.Log`.

### Presentation

- `InventoryBootstrapper` - explicit composition root in scene. Wires configs, repositories, services, use cases, presenter.
- `InventoryScreenPresenter` - connects UI events to command/query services and refreshes views.
- `GameplayButtonsView` - emits button click events only.
- `HudView` - displays coins and total weight.
- `InventoryGridView` - displays slot view models.
- `InventorySlotView` - displays one slot state.

### Configs

- `ItemConfig` - base editable item definition: id, item type, icon, weight, and stack size.
- `ProtectionItemConfig` - config for wearable items, includes `Protection`.
- `AmmoItemConfig` - config for ammo items without extra combat fields.
- `WeaponItemConfig` - config for weapon items, includes `Damage` and `AmmoItemId`.
- `ItemDatabase` - list-based lookup for all item configs used by application logic and UI.
- `InventoryConfig` - editable inventory constants: total slots, initial unlocked slots, ordered unlock prices.

## Save Data Model

```json
{
  "coins": 0,
  "slots": [
    {
      "slotId": 0,
      "isUnlocked": true,
      "itemId": "PistolAmmo",
      "quantity": 20
    }
  ]
}
```

Notes:

- Empty slot is stored with empty `itemId` and `quantity = 0`.
- Opened slots are persisted explicitly.
- Item position is preserved by `slotId`.
- Total weight is not persisted; it is derived from state + config.

## Data Flow

```text
UI Button
  -> InventoryScreenPresenter
  -> IGameplayCommandService / application service
  -> IGameStateRepository mutates InventoryState
  -> ISaveGateway saves GameSaveData
  -> IInventoryStateReader builds InventoryScreenModel
  -> HudView / InventoryGridView refresh
```

On startup:

```text
InventoryBootstrapper
  -> GameplayCommandService.Load
  -> ISaveGateway load
  -> if save missing: build default state from InventoryConfig
  -> store in IGameStateRepository
  -> presenter refreshes UI
```

## Implementation Order

1. Create configs and fill all items / prices in editor.
2. Implement save/load mapping between `GameSaveData` and `InventoryState`.
3. Implement query model builder for HUD and slot rendering.
4. Implement command use cases one by one in task priority:
   - load
   - add coins
   - add item
   - add ammo
   - shoot
   - remove item
   - unlock slot
5. Bind presenter to scene UI and prefab views.
6. Verify persistence, logs, weight calculation, and edge cases.

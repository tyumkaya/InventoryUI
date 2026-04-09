# InventoryUI
- Unity-проект тестового задания `Junior Unity Developer`.
- Выполнение тестового задания заняло у меня ~7 часов чистого времени.
## Стек
- Unity `2022.3.62`
- Платформа: `Android`
- Ориентация: `Portrait`
- Сохранения: `JSON` в `Application.persistentDataPath`
## Реализовано

- Монеты с сохранением между сессиями
- Инвентарь на `50` слотов
- Изначально открыто `15` слотов
- Открытие новых слотов по порядку за монеты
- Добавление случайных предметов
- Добавление случайных патронов со стаканием
- Удаление случайного предмета
- Выстрел из случайного оружия с расходом нужных патронов
- Подсчёт общего веса
- Drag-and-drop между слотами
- Объединение стаков при переносе
- Обмен предметов местами при переносе
- Popup с информацией о предмете

## Структура

- `Assets/Configs` — конфиги для детальной настройки предметов и ячеек в удобном для редактирования виде
- `Assets/Scripts/Inventory/Domain` — модели и enum
- `Assets/Scripts/Inventory/Application` — сервисы и use-case слой
- `Assets/Scripts/Inventory/Infrastructure` — сохранения, логирование, random
- `Assets/Scripts/Inventory/Presentation` — bootstrap, presenter, views
- `Assets/Scripts/Inventory/Configs` — `ScriptableObject` код конфигов


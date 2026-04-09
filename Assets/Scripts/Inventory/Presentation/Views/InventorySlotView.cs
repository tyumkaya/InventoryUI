using System;
using System.Collections;
using InventoryUI.Inventory.Application.DTO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InventoryUI.Inventory.Presentation.Views
{
    public sealed class InventorySlotView : MonoBehaviour
    {
        [SerializeField] private GameObject unlockedView;
        [SerializeField] private Button unlockedButton;
        [SerializeField] private GameObject lockedView;
        [SerializeField] private Button lockedButton;
        [SerializeField] private Image iconImage;
        [SerializeField] private GameObject stackCountRoot;
        [SerializeField] private TMP_Text quantityText;
        [SerializeField] private TMP_Text unlockPriceText;
        [SerializeField] private GameObject itemInfoPopupRoot;
        [SerializeField] private TMP_Text itemInfoText;
        [SerializeField] private float popupFadeDuration = 0.15f;

        private InventorySlotViewModel currentModel;
        private CanvasGroup itemInfoCanvasGroup;
        private Coroutine popupAnimationCoroutine;
        private bool isItemInfoVisible;

        public event Action<int> LockedClicked;
        public event Action<int> UnlockedClicked;
        public event Action<int, Sprite, Vector2> DragStarted;
        public event Action<Vector2> DragMoved;
        public event Action<int> DragEnded;
        public event Action<int> DroppedOn;

        private void Awake()
        {
            ResolveReferences();
            EnsurePopupCanvasGroup();
            SetPopupVisible(false, true);

            if (lockedButton != null)
            {
                lockedButton.onClick.AddListener(OnLockedButtonClicked);
            }

            if (unlockedButton != null)
            {
                unlockedButton.onClick.AddListener(OnUnlockedButtonClicked);
                ConfigureDragRelay(unlockedButton);
            }
        }

        private void OnValidate()
        {
            ResolveReferences();
        }

        public void Render(InventorySlotViewModel model)
        {
            if (model == null)
            {
                return;
            }

            currentModel = model;

            if (lockedView != null)
            {
                lockedView.SetActive(!model.IsUnlocked);
            }

            if (unlockedView != null)
            {
                unlockedView.SetActive(model.IsUnlocked);
            }

            if (!model.IsUnlocked || model.IsEmpty)
            {
                SetPopupVisible(false, true);
            }

            if (iconImage != null)
            {
                iconImage.enabled = model.IsUnlocked && !model.IsEmpty;
                iconImage.sprite = model.Icon;
            }

            if (quantityText != null)
            {
                quantityText.text = model.Quantity > 1 ? model.Quantity.ToString() : string.Empty;
            }

            if (stackCountRoot != null)
            {
                stackCountRoot.SetActive(model.IsUnlocked && model.Quantity > 1);
            }

            if (unlockPriceText != null)
            {
                unlockPriceText.text = model.IsUnlocked ? string.Empty : model.UnlockPrice.ToString();
            }

            if (itemInfoText != null)
            {
                itemInfoText.text = model.ItemInfoText;
            }
        }

        private void OnLockedButtonClicked()
        {
            if (currentModel == null)
            {
                return;
            }

            LockedClicked?.Invoke(currentModel.SlotId);
        }

        private void OnUnlockedButtonClicked()
        {
            if (currentModel == null)
            {
                return;
            }

            ToggleItemInfoPopup();
            UnlockedClicked?.Invoke(currentModel.SlotId);
        }

        public void HandleBeginDrag(PointerEventData eventData)
        {
            if (currentModel == null || !currentModel.IsUnlocked || currentModel.IsEmpty)
            {
                return;
            }

            SetPopupVisible(false);
            DragStarted?.Invoke(currentModel.SlotId, currentModel.Icon, eventData.position);
        }

        public void HandleDrag(PointerEventData eventData)
        {
            DragMoved?.Invoke(eventData.position);
        }

        public void HandleEndDrag(PointerEventData eventData)
        {
            if (currentModel == null)
            {
                return;
            }

            DragEnded?.Invoke(currentModel.SlotId);
        }

        public void HandleDrop(PointerEventData eventData)
        {
            if (currentModel == null)
            {
                return;
            }

            DroppedOn?.Invoke(currentModel.SlotId);
        }

        private void ResolveReferences()
        {
            if (lockedView == null)
            {
                var lockedTransform = transform.Find("LockedView");
                if (lockedTransform != null)
                {
                    lockedView = lockedTransform.gameObject;
                }
            }

            if (unlockedView == null)
            {
                var unlockedTransform = transform.Find("UnlockedView");
                if (unlockedTransform != null)
                {
                    unlockedView = unlockedTransform.gameObject;
                }
            }

            if (lockedButton == null && lockedView != null)
            {
                lockedButton = lockedView.GetComponentInChildren<Button>(true);
            }

            if (unlockedButton == null && unlockedView != null)
            {
                unlockedButton = unlockedView.GetComponentInChildren<Button>(true);
            }

            if (itemInfoPopupRoot == null)
            {
                var infoTransform = transform.Find("ItemInfoPopup");
                if (infoTransform != null)
                {
                    itemInfoPopupRoot = infoTransform.gameObject;
                }
            }

            if (itemInfoText == null && itemInfoPopupRoot != null)
            {
                itemInfoText = itemInfoPopupRoot.GetComponentInChildren<TMP_Text>(true);
            }
        }

        private void ConfigureDragRelay(Button button)
        {
            if (button == null)
            {
                return;
            }

            var relay = button.GetComponent<InventorySlotDragRelay>();
            if (relay == null)
            {
                relay = button.gameObject.AddComponent<InventorySlotDragRelay>();
            }

            relay.Initialize(this);
        }

        private void ToggleItemInfoPopup()
        {
            if (currentModel == null || currentModel.IsEmpty)
            {
                return;
            }

            SetPopupVisible(!isItemInfoVisible);
        }

        private void EnsurePopupCanvasGroup()
        {
            if (itemInfoPopupRoot == null)
            {
                return;
            }

            itemInfoCanvasGroup = itemInfoPopupRoot.GetComponent<CanvasGroup>();
            if (itemInfoCanvasGroup == null)
            {
                itemInfoCanvasGroup = itemInfoPopupRoot.AddComponent<CanvasGroup>();
            }
        }

        private void SetPopupVisible(bool visible, bool instant = false)
        {
            if (itemInfoPopupRoot == null)
            {
                return;
            }

            EnsurePopupCanvasGroup();
            isItemInfoVisible = visible;

            if (popupAnimationCoroutine != null)
            {
                StopCoroutine(popupAnimationCoroutine);
                popupAnimationCoroutine = null;
            }

            if (instant || itemInfoCanvasGroup == null)
            {
                ApplyPopupState(visible ? 1f : 0f, visible);
                return;
            }

            popupAnimationCoroutine = StartCoroutine(AnimatePopup(visible));
        }

        private IEnumerator AnimatePopup(bool visible)
        {
            itemInfoPopupRoot.SetActive(true);
            itemInfoCanvasGroup.interactable = false;
            itemInfoCanvasGroup.blocksRaycasts = false;

            var startAlpha = itemInfoCanvasGroup.alpha;
            var targetAlpha = visible ? 1f : 0f;
            var elapsed = 0f;

            while (elapsed < popupFadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                var progress = popupFadeDuration <= 0f ? 1f : Mathf.Clamp01(elapsed / popupFadeDuration);
                itemInfoCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
                yield return null;
            }

            ApplyPopupState(targetAlpha, visible);
            popupAnimationCoroutine = null;
        }

        private void ApplyPopupState(float alpha, bool visible)
        {
            if (itemInfoCanvasGroup != null)
            {
                itemInfoCanvasGroup.alpha = alpha;
                itemInfoCanvasGroup.interactable = false;
                itemInfoCanvasGroup.blocksRaycasts = false;
            }

            itemInfoPopupRoot.SetActive(visible);
        }
    }
}

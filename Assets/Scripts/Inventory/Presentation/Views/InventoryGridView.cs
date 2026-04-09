using System;
using System.Collections.Generic;
using InventoryUI.Inventory.Application.DTO;
using UnityEngine;
using UnityEngine.UI;

namespace InventoryUI.Inventory.Presentation.Views
{
    public sealed class InventoryGridView : MonoBehaviour
    {
        [SerializeField] private List<InventorySlotView> slotViews = new List<InventorySlotView>();
        [SerializeField] private RectTransform dragPreviewRoot;
        [SerializeField] private Vector2 dragPreviewSize = new Vector2(80f, 80f);

        public event Action<int> LockedSlotClicked;
        public event Action<int> UnlockedSlotClicked;
        public event Action<int> SlotDragStarted;
        public event Action<int> SlotDragEnded;
        public event Action<int> SlotDroppedOn;

        private Canvas parentCanvas;
        private RectTransform activeDragPreview;
        private Image activeDragPreviewImage;

        private void Awake()
        {
            ResolveDragPreviewRoot();
            EnsureDragPreview();

            foreach (var slotView in slotViews)
            {
                if (slotView == null)
                {
                    continue;
                }

                slotView.LockedClicked += OnLockedSlotClicked;
                slotView.UnlockedClicked += OnUnlockedSlotClicked;
                slotView.DragStarted += OnSlotDragStarted;
                slotView.DragMoved += OnSlotDragMoved;
                slotView.DragEnded += OnSlotDragEnded;
                slotView.DroppedOn += OnSlotDroppedOn;
            }
        }

        private void OnDestroy()
        {
            foreach (var slotView in slotViews)
            {
                if (slotView == null)
                {
                    continue;
                }

                slotView.LockedClicked -= OnLockedSlotClicked;
                slotView.UnlockedClicked -= OnUnlockedSlotClicked;
                slotView.DragStarted -= OnSlotDragStarted;
                slotView.DragMoved -= OnSlotDragMoved;
                slotView.DragEnded -= OnSlotDragEnded;
                slotView.DroppedOn -= OnSlotDroppedOn;
            }
        }

        public void Render(IReadOnlyList<InventorySlotViewModel> slots)
        {
            if (slots == null)
            {
                return;
            }

            var count = Mathf.Min(slotViews.Count, slots.Count);
            for (var i = 0; i < count; i++)
            {
                slotViews[i].Render(slots[i]);
            }
        }

        private void OnLockedSlotClicked(int slotId)
        {
            LockedSlotClicked?.Invoke(slotId);
        }

        private void OnUnlockedSlotClicked(int slotId)
        {
            UnlockedSlotClicked?.Invoke(slotId);
        }

        private void OnSlotDragStarted(int slotId, Sprite icon, Vector2 screenPosition)
        {
            ShowDragPreview(icon, screenPosition);
            SlotDragStarted?.Invoke(slotId);
        }

        private void OnSlotDragMoved(Vector2 screenPosition)
        {
            MoveDragPreview(screenPosition);
        }

        private void OnSlotDragEnded(int slotId)
        {
            HideDragPreview();
            SlotDragEnded?.Invoke(slotId);
        }

        private void OnSlotDroppedOn(int slotId)
        {
            SlotDroppedOn?.Invoke(slotId);
        }

        private void ResolveDragPreviewRoot()
        {
            if (dragPreviewRoot == null)
            {
                parentCanvas = GetComponentInParent<Canvas>();
                if (parentCanvas != null)
                {
                    dragPreviewRoot = parentCanvas.transform as RectTransform;
                }
            }
            else
            {
                parentCanvas = dragPreviewRoot.GetComponentInParent<Canvas>();
            }
        }

        private void EnsureDragPreview()
        {
            if (dragPreviewRoot == null || activeDragPreview != null)
            {
                return;
            }

            var previewObject = new GameObject("DragPreview", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
            activeDragPreview = previewObject.GetComponent<RectTransform>();
            activeDragPreview.SetParent(dragPreviewRoot, false);
            activeDragPreview.anchorMin = new Vector2(0.5f, 0.5f);
            activeDragPreview.anchorMax = new Vector2(0.5f, 0.5f);
            activeDragPreview.pivot = new Vector2(0.5f, 0.5f);
            activeDragPreview.sizeDelta = dragPreviewSize;

            var canvasGroup = previewObject.GetComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;

            activeDragPreviewImage = previewObject.GetComponent<Image>();
            activeDragPreviewImage.raycastTarget = false;
            activeDragPreviewImage.preserveAspect = true;
            previewObject.SetActive(false);
        }

        private void ShowDragPreview(Sprite icon, Vector2 screenPosition)
        {
            if (activeDragPreview == null || activeDragPreviewImage == null || icon == null)
            {
                return;
            }

            activeDragPreviewImage.sprite = icon;
            activeDragPreview.gameObject.SetActive(true);
            MoveDragPreview(screenPosition);
        }

        private void MoveDragPreview(Vector2 screenPosition)
        {
            if (activeDragPreview == null || dragPreviewRoot == null)
            {
                return;
            }

            var eventCamera = parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? parentCanvas.worldCamera
                : null;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(dragPreviewRoot, screenPosition, eventCamera, out var localPoint))
            {
                activeDragPreview.anchoredPosition = localPoint;
            }
        }

        private void HideDragPreview()
        {
            if (activeDragPreview == null)
            {
                return;
            }

            activeDragPreview.gameObject.SetActive(false);
            if (activeDragPreviewImage != null)
            {
                activeDragPreviewImage.sprite = null;
            }
        }
    }
}

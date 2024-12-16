using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector]
    public Image buttonImage;
    public ItemCell itemCell;
    public Image edgeImage;
    public Image itemImage;
    public bool isEquip;
    public bool isForge;
    public bool forgeSlot;
    [HideInInspector]
    public InventoryController inventoryController;
    public static ItemCell hoveringItem;
    [HideInInspector]
    public bool startedDragging;
    [HideInInspector]
    public Vector3 dragStartPos;
    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        inventoryController = FindObjectOfType<InventoryController>();
    }
    private void Update()
    {
        if (startedDragging && Vector3.Distance(Input.mousePosition, dragStartPos) > .3f)
        {
            inventoryController.BeginDrag(itemCell.itemData);
            startedDragging = false;
        }
    }
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        buttonImage.material = BaseUtils.highlightUI;
        itemImage.material = BaseUtils.highlightUI;
        startedDragging = false;
        inventoryController.EndDrag();
        if (forgeSlot)
        {
            SoundController.PlaySound("unequip");
            inventoryController.forgeController.RemoveForgeItem(itemCell);
        }
        else if (isEquip)
        {
            inventoryController.OnDequip(itemCell);
            inventoryController.backpackController.Setup(true);
        }
        else if (!itemCell.emptyItem)
        {
            inventoryController.SwitchItems(itemCell, null, isForge);
        }
    }
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        inventoryController.HideTooltip();
        buttonImage.material = BaseUtils.normalUI;
        itemImage.material = BaseUtils.normalUI;
        if (isEquip || itemCell.emptyItem || forgeSlot)
        {
            return;
        }
        startedDragging = true;
        dragStartPos = Input.mousePosition;
    }
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        hoveringItem = itemCell;
        if (inventoryController.showingPreview)
        {
            if (edgeImage != null)
            {
                edgeImage.material = BaseUtils.highLineUI;
            }
            buttonImage.material = BaseUtils.highLineUI;
            itemImage.material = BaseUtils.highLineUI;
        }
        else
        {
            buttonImage.material = BaseUtils.highlightUI;
            itemImage.material = BaseUtils.highlightUI;
            if (!itemCell.emptyItem)
            {
                inventoryController.ShowTooltip(isForge, isEquip, buttonImage.rectTransform, itemCell.itemData);
            }
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        hoveringItem = null;
        if (edgeImage != null)
        {
            edgeImage.material = BaseUtils.normalUI;
        }
        buttonImage.material = BaseUtils.normalUI;
        itemImage.material = BaseUtils.normalUI;
        inventoryController.HideTooltip();
    }
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (isEquip || itemCell.emptyItem || forgeSlot)
        {
            return;
        }
        if (hoveringItem != null)
        {
            inventoryController.SwitchItems(itemCell, hoveringItem, isForge);
        }
        hoveringItem = null;
        inventoryController.EndDrag();
    }
    private void OnDisable()
    {
        buttonImage.material = BaseUtils.normalUI;
        itemImage.material = BaseUtils.normalUI;
        if (edgeImage != null)
        {
            edgeImage.material = BaseUtils.normalUI;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChestItem : DragItem
{
    public PresaleController presaleController;
    public int index;
    public override void OnPointerClick(PointerEventData eventData)
    {
        buttonImage.material = BaseUtils.highlightUI;
        itemImage.material = BaseUtils.highlightUI;
        startedDragging = false;
        inventoryController.EndDrag();
        presaleController.OnLootItemClick(index);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        inventoryController.HideTooltip();
        buttonImage.material = BaseUtils.normalUI;
        itemImage.material = BaseUtils.normalUI;
    }
    public override void OnPointerUp(PointerEventData eventData)
    {

    }
    public override void OnPointerEnter(PointerEventData eventData)
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
                inventoryController.ShowTooltip(true, isEquip, buttonImage.rectTransform, itemCell.itemData);
            }
        }
    }
}

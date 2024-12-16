using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AuctionHallButton : CustomButton
{
    public AuctionHallCell auctionHallCell;
    public RectTransform cellRect;
    private MainMenuController mainMenuController;
    private void Start()
    {
        mainMenuController = FindObjectOfType<MainMenuController>();
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        ScriptableItem scriptableItem = BaseUtils.itemDict[auctionHallCell.data.itemData.itemType];
        if (Database.HasEquippedType(scriptableItem.classType, scriptableItem.equipType, out int databaseIndex))
        {
            mainMenuController.ShowTooltip(cellRect, auctionHallCell.data.itemData, Database.databaseStruct.ownedItems[databaseIndex]);
        }
        else
        {
            mainMenuController.ShowTooltip(cellRect, auctionHallCell.data.itemData);
        }
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        mainMenuController.HideTooltip();
    }
}

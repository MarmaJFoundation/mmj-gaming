using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LeaderboardButton : CustomButton
{
    public LeaderboardCell leaderboardCell;
    public RectTransform cellRect;
    public int itemIndex;
    private MainMenuController mainMenuController;
    private void Start()
    {
        mainMenuController = FindObjectOfType<MainMenuController>();
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (leaderboardCell.data.charLoadout.Count <= itemIndex)
        {
            return;
        }
        ScriptableItem scriptableItem = BaseUtils.itemDict[leaderboardCell.data.charLoadout[itemIndex].itemType];
        if (Database.HasEquippedType(scriptableItem.classType, scriptableItem.equipType, out int databaseIndex))
        {
            mainMenuController.ShowTooltip(cellRect, leaderboardCell.data.charLoadout[itemIndex], Database.databaseStruct.ownedItems[databaseIndex]);
        }
        else
        {
            mainMenuController.ShowTooltip(cellRect, leaderboardCell.data.charLoadout[itemIndex]);
        }
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        mainMenuController.HideTooltip();
    }
}

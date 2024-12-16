using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuctionHallCell : EnhancedScrollerCellView
{
    public int DataIndex { get; private set; }
    [HideInInspector]
    public AHItemData data;
    private AuctionHallController auctionHallController;
    public Image itemImage;
    public Image borderImage;
    public CustomText itemName;
    public CustomText priceText;
    public CustomText[] statTexts;
    public GameObject innerObj;
    private bool selfItem;
    public void SetData(AuctionHallController auctionHallController, int dataIndex, AHItemData data)
    {
        this.auctionHallController = auctionHallController;
        this.data = data;
        DataIndex = dataIndex;
        if (data == null)
        {
            innerObj.SetActive(false);
            return;
        }
        innerObj.SetActive(true);
        /*string nameString = data.itemData.itemName;
        float value = Screen.width / CustomScaler.canvasScale / auctionHallController.charLimit;
        int maxChars = Mathf.RoundToInt(value * value * value * value);
        if (nameString.Length > maxChars)
        {
            nameString = nameString.Substring(0, maxChars);
            nameString += "..";
        }*/
        selfItem = Database.HasItem(data.itemData.itemID);
        itemName.SetString(data.itemData.itemName, BaseUtils.rarityColors[data.rarity]);
        switch (data.auctionType)
        {
            case AuctionType.Normal:
                if (selfItem)
                {
                    priceText.SetString($"you are selling this item", Color.white);
                }
                else
                {
                    priceText.SetString($"click to buy for {data.itemData.price} @", BaseUtils.enabledColor);
                }
                break;
            case AuctionType.Sell:
                priceText.SetString($"click to sell this item", BaseUtils.enabledColor);
                break;
            case AuctionType.Auction:
                priceText.SetString($"currently selling for {data.itemData.price} @", BaseUtils.enabledColor);
                break;
            case AuctionType.Transaction:
                if (data.itemData.price < 0)
                {
                    priceText.SetString($"bought this item for {data.itemData.price * -1} @", BaseUtils.enabledColor);
                }
                else
                {
                    //priceText.SetString($"sold this item for {data.itemData.price} @", BaseUtils.enabledColor);
                    priceText.SetString($"you sold this item", BaseUtils.enabledColor);
                }
                break;
        }
        ScriptableItem scriptableItem = BaseUtils.itemDict[data.itemData.itemType];
        itemImage.sprite = scriptableItem.itemSprite;
        borderImage.sprite = BaseUtils.itemBorders[(int)BaseUtils.itemDict[data.itemData.itemType].rarityType];
        if (data.itemStatStruct.damage > 0)
        {
            statTexts[0].gameObject.SetActive(true);
            statTexts[0].SetString(data.itemStatStruct.damage.ToString());
        }
        else
        {
            statTexts[0].gameObject.SetActive(false);
        }
        if (data.itemStatStruct.maxHealth > 0)
        {
            statTexts[1].gameObject.SetActive(true);
            statTexts[1].SetString(data.itemStatStruct.maxHealth.ToString());
        }
        else
        {
            statTexts[1].gameObject.SetActive(false);
        }
        if (data.itemStatStruct.defense > 0)
        {
            statTexts[2].gameObject.SetActive(true);
            statTexts[2].SetString($"{Mathf.Round(data.itemStatStruct.defense * 100) / 100}%");
        }
        else
        {
            statTexts[2].gameObject.SetActive(false);
        }
        if (data.itemStatStruct.dodge > 0)
        {
            statTexts[3].gameObject.SetActive(true);
            statTexts[3].SetString($"{Mathf.Round(data.itemStatStruct.dodge * 100) / 100}%");
        }
        else
        {
            statTexts[3].gameObject.SetActive(false);
        }
        if (data.itemStatStruct.critChance > 0)
        {
            statTexts[4].gameObject.SetActive(true);
            statTexts[4].SetString($"{Mathf.Round(data.itemStatStruct.critChance * 100) / 100}%");
        }
        else
        {
            statTexts[4].gameObject.SetActive(false);
        }
        if (data.itemStatStruct.lifeSteal > 0)
        {
            statTexts[5].gameObject.SetActive(true);
            statTexts[5].SetString($"{Mathf.Round(data.itemStatStruct.lifeSteal * 100) / 100}%");
        }
        else
        {
            statTexts[5].gameObject.SetActive(false);
        }
        statTexts[6].SetString($"{data.itemStatStruct.ToStatRank()}");
    }
    public void OnClick()
    {
        if (selfItem && data.auctionType == AuctionType.Normal)
        {
            BaseUtils.ShowWarningMessage("Not possible.", new string[1] { "You cannot buy your own item!" });
            return;
        }
        auctionHallController.OnItemClick(data);
    }
}

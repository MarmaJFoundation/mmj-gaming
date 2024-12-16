using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForgeController : MonoBehaviour
{
    public CameraController cameraController;
    public MainMenuController mainMenuController;
    public BackpackController backpackController;
    public NearHelper nearHelper;
    public ItemTooltip itemTooltip;
    public GameObject resultObj;
    public RectTransform itemOneRect;
    public RectTransform itemTwoRect;
    public RectTransform itemResultRect;
    public ItemCell itemOneCell;
    public ItemCell itemTwoCell;
    public ItemCell itemResultCell;
    public ItemCell itemOneSlotCell;
    public ItemCell itemTwoSlotCell;
    public CustomText itemOneTitle;
    public CustomText itemOneRarity;
    public CustomText itemTwoTitle;
    public CustomText itemTwoRarity;
    public CustomText itemResultRarity;
    public CustomText[] itemOneStats;
    public CustomText[] itemTwoStats;
    public Image[] itemOneSuffixes;
    public Image[] itemTwoSuffixes;
    public Image[] itemResultSuffixes;
    public CustomText[] itemResultStats;
    public CustomText[] itemResultDesc;
    public CustomText itemResultCost;
    public Image itemOneBackground;
    public Image itemTwoBackground;
    public Image itemResultBackground;
    public Image itemResultTitle;
    public GameObject forgeButton;
    public Canvas forgeCanvas;
    public ItemCell[] itemCells;
    public ItemCell[] slotCells;
    public Color destroyedColor;
    public Sprite randomSprite;
    private StatStruct itemOneStatStruct;
    private StatStruct itemTwoStatStruct;
    private int currentPrice;
    private RarityType currentRarity;
    private ItemType currentType;

    private ItemToken forgedItem;
    public void Setup()
    {
        forgeCanvas.gameObject.SetActive(true);
        forgeCanvas.enabled = true;
        RemoveForgeItem(slotCells[0]);
        RemoveForgeItem(slotCells[1]);
        UpdateItems();
        CheckForForge();
    }
    public void UpdateItems()
    {
        for (int i = 0; i < Database.maxItems; i++)
        {
            int itemID = Database.GetItemFromVisualIndex(i);
            int databaseIndex = Database.GetItemFromID(itemID);
            itemCells[i].SetForgeItem(this, new ItemCellData() { databaseIndex = databaseIndex, displayIndex = i, itemID = itemID });
        }
    }
    public void SetForgeItem(ItemCell itemCellA, ItemCell itemCellB)
    {
        ScriptableItem scriptableItem = BaseUtils.itemDict[itemCellA.itemData.itemType];
        if (itemCellA.itemData.equipClass != ClassType.None || itemCellA.itemData.price != 0)
        {
            BaseUtils.ShowWarningMessage("Not possible!", new string[1] { "You cannot reforge selling or equipped items." });
            return;
        }
        if (itemCellB.forgeIndex == 1)
        {
            if (!slotCells[1].emptyItem && (BaseUtils.itemDict[slotCells[1].itemData.itemType].rarityType != scriptableItem.rarityType))
            {
                BaseUtils.ShowWarningMessage("Not possible!", new string[1] { "Items need to be of the same rarity." });
                return;
            }
            else
            {
                itemCellB.SetForgeItem(itemCellA.itemData);
                itemOneStatStruct = BaseUtils.GenerateItemStatStruct(itemCellA.itemData, scriptableItem.classType, scriptableItem.equipType);
                PlaySound(scriptableItem);
                BaseUtils.InstantiateEffect((EffectType)(55 + (int)scriptableItem.rarityType), itemOneRect.position, false);
                SetItemTooltip(itemOneStats, itemOneCell.itemFrameImage, itemOneCell.itemImage, itemOneTitle, itemOneRarity, itemCellA.itemData, itemOneSuffixes, itemOneStatStruct, scriptableItem);
                itemOneBackground.material = BaseUtils.rarityMaterials[(int)scriptableItem.rarityType];
            }
        }
        else
        {
            if (!slotCells[0].emptyItem && (BaseUtils.itemDict[slotCells[0].itemData.itemType].rarityType != scriptableItem.rarityType))
            {
                BaseUtils.ShowWarningMessage("Not possible!", new string[1] { "Items need to be of the same rarity." });
                return;
            }
            else
            {
                itemCellB.SetForgeItem(itemCellA.itemData);
                itemTwoStatStruct = BaseUtils.GenerateItemStatStruct(itemCellA.itemData, scriptableItem.classType, scriptableItem.equipType);
                PlaySound(scriptableItem);
                BaseUtils.InstantiateEffect((EffectType)(55 + (int)scriptableItem.rarityType), itemTwoRect.position, false);
                SetItemTooltip(itemTwoStats, itemTwoCell.itemFrameImage, itemTwoCell.itemImage, itemTwoTitle, itemTwoRarity, itemCellA.itemData, itemTwoSuffixes, itemTwoStatStruct, scriptableItem);
                itemTwoBackground.material = BaseUtils.rarityMaterials[(int)scriptableItem.rarityType];
            }
        }
        CheckForForge();
    }
    private void PlaySound(ScriptableItem scriptableItem)
    {
        SoundController.PlaySound("sparks", 1, true);
        switch (scriptableItem.equipType)
        {
            case EquipType.Armor:
                SoundController.PlaySound("Equip_armor", .5f, true);
                break;
            case EquipType.Helmet:
                SoundController.PlaySound("Equip_armor", .5f, true);
                break;
            case EquipType.Weapon:
                if (scriptableItem.classType == ClassType.Mage)
                {
                    SoundController.PlaySound("Equip_weapon", 1, true);
                }
                else
                {
                    SoundController.PlaySound("Equip_weapon_var", .5f, true);
                }
                break;
            case EquipType.Boots:
                SoundController.PlaySound("Equip_armor", .5f, true);
                break;
            case EquipType.Necklace:
                SoundController.PlaySound("Equip_Jewel", .5f, true);
                break;
            case EquipType.Ring:
                SoundController.PlaySound("Equip_Jewel", .5f, true);
                break;
            case EquipType.Empty:
                break;
        }
    }
    private void SetItemTooltip(CustomText[] itemStats, Image itemBorder, Image itemImage, CustomText titleText, CustomText itemRarity, ItemData itemData, Image[] suffixes, StatStruct statStruct, ScriptableItem scriptableItem)
    {
        itemBorder.sprite = BaseUtils.itemBorders[(int)scriptableItem.rarityType];
        itemImage.sprite = scriptableItem.itemSprite;
        titleText.SetString(itemData.itemName, BaseUtils.rarityColors[(int)scriptableItem.rarityType]);
        string classString = scriptableItem.classType == ClassType.None ? "" : $" {scriptableItem.classType}";
        itemRarity.gameObject.SetActive(true);
        itemRarity.SetString($"{scriptableItem.rarityType}{classString} {scriptableItem.equipType}", BaseUtils.rarityColors[(int)scriptableItem.rarityType]);
        for (int i = 0; i < itemStats.Length; i++)
        {
            itemStats[i].gameObject.SetActive(false);
        }
        if (statStruct.damage > 0)
        {
            SetItemStat(itemStats, suffixes, 0, statStruct.damage);
        }
        else
        {
            itemStats[0].gameObject.SetActive(false);
        }
        if (statStruct.maxHealth > 0)
        {
            SetItemStat(itemStats, suffixes, 1, statStruct.maxHealth);
        }
        else
        {
            itemStats[1].gameObject.SetActive(false);
        }
        if (statStruct.defense > 0)
        {
            SetItemStat(itemStats, suffixes, 2, Mathf.Round(statStruct.defense * 100) / 100, "%");
        }
        else
        {
            itemStats[2].gameObject.SetActive(false);
        }
        if (statStruct.dodge > 0)
        {
            SetItemStat(itemStats, suffixes, 3, Mathf.Round(statStruct.dodge * 100) / 100, "%");
        }
        else
        {
            itemStats[3].gameObject.SetActive(false);
        }
        if (statStruct.critChance > 0)
        {
            SetItemStat(itemStats, suffixes, 4, Mathf.Round(statStruct.critChance * 100) / 100, "%");
        }
        else
        {
            itemStats[4].gameObject.SetActive(false);
        }
        if (statStruct.lifeSteal > 0)
        {
            SetItemStat(itemStats, suffixes, 5, Mathf.Round(statStruct.lifeSteal * 100) / 100, "%");
        }
        else
        {
            itemStats[5].gameObject.SetActive(false);
        }
    }
    private void SetItemStat(CustomText[] itemStats, Image[] suffixes, int index, float value, string suffix = "")
    {
        itemStats[index].gameObject.SetActive(true);
        string addPrefix = value > 0 ? "+" : "";
        itemStats[index].SetString($"{addPrefix}{value}{suffix}", Color.white);
        StartCoroutine(WaitAndFixSuffix(suffixes[index].rectTransform));
    }
    private IEnumerator WaitAndFixSuffix(RectTransform rectTransform)
    {
        yield return new WaitForEndOfFrame();
        rectTransform.SetAsLastSibling();
    }
    public void RemoveForgeItem(ItemCell itemCell)
    {
        itemCell.SetForgeItem(new ItemData() { itemType = ItemType.None });
        if (itemCell.forgeIndex == 1)
        {
            itemOneRarity.gameObject.SetActive(false);
            itemOneBackground.material = BaseUtils.rarityMaterials[0];
            itemOneCell.SetForgeItem(new ItemData() { itemType = ItemType.None });
            itemOneSlotCell.SetForgeItem(new ItemData() { itemType = ItemType.None });
            itemOneTitle.SetString("??", Color.white);
            for (int i = 0; i < 6; i++)
            {
                itemOneStats[i].gameObject.SetActive(false);
            }
        }
        else
        {
            itemTwoRarity.gameObject.SetActive(false);
            itemTwoBackground.material = BaseUtils.rarityMaterials[0];
            itemTwoCell.SetForgeItem(new ItemData() { itemType = ItemType.None });
            itemTwoSlotCell.SetForgeItem(new ItemData() { itemType = ItemType.None });
            itemTwoTitle.SetString("??", Color.white);
            for (int i = 0; i < 6; i++)
            {
                itemTwoStats[i].gameObject.SetActive(false);
            }
        }
        UpdateItems();
        CheckForForge();
    }
    private void CheckForForge()
    {
        for (int i = 0; i < 6; i++)
        {
            itemResultStats[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < 3; i++)
        {
            itemResultDesc[i].gameObject.SetActive(false);
        }
        if (slotCells[0].emptyItem || slotCells[1].emptyItem)
        {
            itemResultRarity.gameObject.SetActive(false);
            itemResultCell.SetForgeItem(new ItemData() { itemType = ItemType.None });
            itemResultCost.gameObject.SetActive(false);
            forgeButton.SetActive(false);
            itemResultBackground.material = BaseUtils.rarityMaterials[0];
            itemResultTitle.material = BaseUtils.rarityMaterials[0];
        }
        else
        {
            ScriptableItem scriptableItem = BaseUtils.itemDict[slotCells[0].itemData.itemType];
            currentRarity = scriptableItem.rarityType;
            currentType = slotCells[0].itemData.itemType;
            //string classString = scriptableItem.classType == ClassType.None ? "" : $" {scriptableItem.classType}";
            itemResultRarity.gameObject.SetActive(true);
            itemResultRarity.SetString($"random {scriptableItem.rarityType} item", BaseUtils.rarityColors[(int)scriptableItem.rarityType]);
            //itemResultCell.SetForgeItem(slotCells[0].itemData);
            itemResultCell.itemImage.sprite = randomSprite;
            itemResultCell.itemFrameImage.sprite = backpackController.itemBorders[(int)BaseUtils.itemDict[slotCells[0].itemData.itemType].rarityType];
            itemResultBackground.material = BaseUtils.rarityMaterials[(int)scriptableItem.rarityType];
            itemResultTitle.material = BaseUtils.rarityMaterials[(int)scriptableItem.rarityType];
            SoundController.PlaySound("Loot_sound_var", 1, true);
            BaseUtils.InstantiateEffect((EffectType)(55 + (int)scriptableItem.rarityType), itemResultRect.position, false);
            /*if (itemOneStatStruct.damage > 0 || itemTwoStatStruct.damage > 0)
            {
                SetResultStats(0);
            }
            if (itemOneStatStruct.maxHealth > 0 || itemTwoStatStruct.maxHealth > 0)
            {
                SetResultStats(1);
            }
            if (itemOneStatStruct.defense > 0 || itemTwoStatStruct.defense > 0)
            {
                SetResultStats(2);
            }
            if (itemOneStatStruct.dodge > 0 || itemTwoStatStruct.dodge > 0)
            {
                SetResultStats(3);
            }
            if (itemOneStatStruct.critChance > 0 || itemTwoStatStruct.critChance > 0)
            {
                SetResultStats(4);
            }
            if (itemOneStatStruct.lifeSteal > 0 || itemTwoStatStruct.lifeSteal > 0)
            {
                SetResultStats(5);
            }*/
            for (int i = 0; i < 3; i++)
            {
                itemResultStats[i].gameObject.SetActive(true);
                //itemResultStats[i].SetString("+??");
                //StartCoroutine(WaitAndFixSuffix(itemResultSuffixes[i].rectTransform));
            }
            itemResultCost.gameObject.SetActive(true);
            forgeButton.SetActive(true);
            switch (scriptableItem.rarityType)
            {
                case RarityType.Common:
                    itemResultDesc[0].gameObject.SetActive(true);
                    itemResultDesc[0].SetString("20% chance of being destroyed!", destroyedColor);
                    itemResultDesc[1].gameObject.SetActive(true);
                    itemResultDesc[1].SetString("79% chance of being common", BaseUtils.rarityColors[0]);
                    itemResultDesc[2].gameObject.SetActive(true);
                    itemResultDesc[2].SetString("1% chance of being rare", BaseUtils.rarityColors[1]);
                    itemResultCost.SetString("cost: free");
                    currentPrice = 0;
                    break;
                case RarityType.Rare:
                    itemResultDesc[0].gameObject.SetActive(true);
                    itemResultDesc[0].SetString("10% chance of being common", BaseUtils.rarityColors[0]);
                    itemResultDesc[1].gameObject.SetActive(true);
                    itemResultDesc[1].SetString("89% chance of being rare", BaseUtils.rarityColors[1]);
                    itemResultDesc[2].gameObject.SetActive(true);
                    itemResultDesc[2].SetString("1% chance of being epic", BaseUtils.rarityColors[2]);
                    itemResultCost.SetString("cost: 5 @");
                    currentPrice = 5;
                    break;
                case RarityType.Epic:
                    itemResultDesc[0].gameObject.SetActive(true);
                    itemResultDesc[0].SetString("5% chance of being rare", BaseUtils.rarityColors[1]);
                    itemResultDesc[1].gameObject.SetActive(true);
                    itemResultDesc[1].SetString("94% chance of being epic", BaseUtils.rarityColors[2]);
                    itemResultDesc[2].gameObject.SetActive(true);
                    itemResultDesc[2].SetString("1% chance of being legendary", BaseUtils.rarityColors[3]);
                    itemResultCost.SetString("cost: 10 @");
                    currentPrice = 10;
                    break;
                case RarityType.Legendary:
                    itemResultDesc[0].gameObject.SetActive(true);
                    itemResultDesc[0].SetString("100% chance of being legendary", BaseUtils.rarityColors[3]);
                    itemResultCost.SetString("cost: 50 @");
                    currentPrice = 50;
                    break;
            }
        }
    }
    public void OnForgeClick()
    {
        if (Database.databaseStruct.pixelTokens < currentPrice)
        {
            BaseUtils.ShowWarningMessage("Insuffient tokens", new string[2] { "You lack the necessary pixel tokens to buy this item", "would you like to trade more @ ?" }, BaseUtils.OnAcceptTradeToken);
            return;
        }
        if (currentPrice == 0)
        {
            BaseUtils.ShowWarningMessage("Forging item", new string[2] { $"Are you sure you want to forge these items for free?", "Be aware that the new item is completely random!" }, OnAcceptForge);
        }
        else
        {
            BaseUtils.ShowWarningMessage("Forging item", new string[2] { $"Are you sure you want to forge these items for {currentPrice} @ ?", "Be aware that the new item is completely random!" }, OnAcceptForge);
        }
    }
    private void OnAcceptForge()
    {
        if (BaseUtils.offlineMode)
        {
            bool itemBroke = false;
            RarityType goRarity = RarityType.Common;
            int rarityChance = BaseUtils.RandomInt(0, 100);
            switch (currentRarity)
            {
                case RarityType.Common:
                    if (rarityChance <= 1)
                    {
                        goRarity = RarityType.Rare;
                    }
                    else if (rarityChance <= 20)
                    {
                        itemBroke = true;
                    }
                    else if (rarityChance <= 79)
                    {
                        goRarity = RarityType.Common;
                    }
                    break;
                case RarityType.Rare:
                    if (rarityChance <= 1)
                    {
                        goRarity = RarityType.Epic;
                    }
                    else if (rarityChance <= 10)
                    {
                        goRarity = RarityType.Common;
                    }
                    else if (rarityChance <= 89)
                    {
                        goRarity = RarityType.Rare;
                    }
                    break;
                case RarityType.Epic:
                    if (rarityChance <= 1)
                    {
                        goRarity = RarityType.Legendary;
                    }
                    else if (rarityChance <= 5)
                    {
                        goRarity = RarityType.Rare;
                    }
                    else if (rarityChance <= 94)
                    {
                        goRarity = RarityType.Epic;
                    }
                    break;
                case RarityType.Legendary:
                    goRarity = RarityType.Legendary;
                    break;
            }
            int typeIndex;
            ItemType goItemType;
            switch (currentRarity)
            {
                case RarityType.Rare:
                    typeIndex = BaseUtils.RandomInt(0, BaseUtils.rareItems.Length);
                    break;
                case RarityType.Epic:
                    typeIndex = BaseUtils.RandomInt(0, BaseUtils.epicItems.Length);
                    break;
                case RarityType.Legendary:
                    typeIndex = BaseUtils.RandomInt(0, BaseUtils.legendaryItems.Length);
                    break;
                default:
                    typeIndex = BaseUtils.RandomInt(0, BaseUtils.commonItems.Length);
                    break;
            }
            switch (goRarity)
            {
                case RarityType.Rare:
                    goItemType = BaseUtils.rareItems[typeIndex];
                    break;
                case RarityType.Epic:
                    goItemType = BaseUtils.epicItems[typeIndex];
                    break;
                case RarityType.Legendary:
                    goItemType = BaseUtils.legendaryItems[typeIndex];
                    break;
                default:
                    goItemType = BaseUtils.commonItems[typeIndex];
                    break;
            }
            ItemData forgeItem = BaseUtils.GenerateRandomItem(goRarity, goItemType);
            StartCoroutine(ShowForgeItem(itemBroke, null));
        }
        else
        {
            StartCoroutine(nearHelper.RequestForgeItem(slotCells[0].itemData.itemID, slotCells[1].itemData.itemID));
        }
    }
    public void OnForgeItemCallback(ItemToken itemToken)
    {
        forgedItem = itemToken;
        nearHelper.dataGetState = DataGetState.AfterForge;
        StartCoroutine(nearHelper.GetPlayerData());
    }
    public void OnReceiveNewPlayerData()
    {
        StartCoroutine(ShowForgeItem(forgedItem == null, forgedItem));
    }
    private IEnumerator ShowForgeItem(bool itemBroke, ItemToken forgeItem)
    {
        //cameraController.ShakeCamera(2);
        if (itemBroke)
        {
            BaseUtils.InstantiateEffect(EffectType.DestroyedItem, Vector3.zero, false);
            BaseUtils.ShowWarningMessage("Oh no!", new string[2] { "The item you attempted to forge broke", "better luck next time :(" });
        }
        else
        {
            mainMenuController.SetupBlackScreen();
            resultObj.SetActive(false);
            yield return new WaitForSeconds(1 / 4f);
            itemTooltip.Setup(forgeItem.ToItemData(), "Forged item!", true);
            /*if (BaseUtils.offlineMode)
            {
                Database.AddItem(forgeItem);
            }*/
        }
        if (BaseUtils.offlineMode)
        {
            Database.RemoveItem(slotCells[0].itemData.itemID);
            Database.RemoveItem(slotCells[1].itemData.itemID);
        }
        while (itemTooltip.gameObject.activeSelf)
        {
            yield return null;
        }
        resultObj.SetActive(true);
        RemoveForgeItem(slotCells[0]);
        RemoveForgeItem(slotCells[1]);
        UpdateItems();
        CheckForForge();
        mainMenuController.RemoveBlackScreen();
    }
    private void SetResultStats(int index)
    {
        itemResultStats[index].gameObject.SetActive(true);
        itemResultStats[index].SetString("+??");
        StartCoroutine(WaitAndFixSuffix(itemResultSuffixes[index].rectTransform));
    }

    public void Dispose()
    {
        forgeCanvas.gameObject.SetActive(false);
        forgeCanvas.enabled = false;
    }
}

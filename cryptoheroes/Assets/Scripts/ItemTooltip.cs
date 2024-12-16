using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    public CustomText titleOneText;
    public CustomText titleTwoText;
    public CustomText titleText;
    public CustomText stateText;
    public RectTransform rectTransform;
    public RectTransform itemOneWindow;
    public RectTransform itemTwoWindow;
    public HorizontalLayoutGroup mainLayout;
    public LayoutElement itemOneLayout;
    public LayoutElement itemTwoLayout;
    public GameObject lootButtonObj;
    public GameObject titleObj;
    public GameObject effectsObj;
    public GameObject stateObj;
    public Image itemOneBorder;
    public Image itemTwoBorder;
    public Image itemOneImage;
    public Image itemTwoImage;
    public Image rarityBackground;
    public GameObject[] rarityEffects;
    public CustomText itemOneRarity;
    public CustomText itemTwoRarity;
    public CustomText[] itemOneStats;
    public CustomText[] itemTwoStats;
    public CustomText[] itemChangeStats;
    public Image[] itemOneSuffixes;
    public Image[] itemTwoSuffixes;
    public Image[] itemChangeSuffixes;
    public Color positiveColor;
    public Color negativeColor;
    public GameObject compareStatsObj;
    private Vector3 screenPos;
    //private StatStruct statStruct;
    private bool showLootButton;
    public void Setup(ItemData itemData, string titleString = "", bool showLootButton = false)
    {
        this.showLootButton = showLootButton;
        BaseUtils.showingLoot = showLootButton;
        gameObject.SetActive(true);
        itemTwoWindow.gameObject.SetActive(false);
        compareStatsObj.SetActive(false);
        itemOneLayout.minWidth = itemData.itemName.Length * 6;
        if (showLootButton)
        {
            titleText.SetString(titleString);
        }
        SetItemData(titleOneText, itemOneBorder, itemOneImage, itemOneRarity, itemOneStats, itemOneSuffixes, itemData, BaseUtils.itemDict[itemData.itemType], true);
    }
    private void LateUpdate()
    {
        itemTwoWindow.pivot = new Vector2(screenPos.x > Screen.width / 2 ? 1 : 0, .5f);
        itemTwoWindow.anchoredPosition = new Vector2(screenPos.x < Screen.width / 2 ? rectTransform.sizeDelta.x / 2 + 10 : -rectTransform.sizeDelta.x / 2 - 10, 26);
    }
    public void Dispose()
    {
        BaseUtils.showingLoot = false;
        gameObject.SetActive(false);
    }
    public void Setup(ItemData itemData, ItemData equipData, Vector3 rectPos)
    {
        showLootButton = false;
        screenPos = BaseUtils.mainCam.WorldToScreenPoint(rectPos);
        itemOneLayout.minWidth = itemData.itemName.Length * 6;
        itemTwoLayout.minWidth = equipData.itemName.Length * 6;
        //mainLayout.padding = new RectOffset((int)itemOneWindow.sizeDelta.x, 0, 0, 0);
        gameObject.SetActive(true);
        itemTwoWindow.gameObject.SetActive(true);
        compareStatsObj.SetActive(true);
        SetItemData(titleOneText, itemOneBorder, itemOneImage, itemOneRarity, itemOneStats, itemOneSuffixes, itemData, BaseUtils.itemDict[itemData.itemType], true);
        SetItemData(titleTwoText, itemTwoBorder, itemTwoImage, itemTwoRarity, itemTwoStats, itemTwoSuffixes, equipData, BaseUtils.itemDict[equipData.itemType], false);
        ScriptableItem scriptableItem = BaseUtils.itemDict[itemData.itemType];
        ScriptableItem scriptableEquip = BaseUtils.itemDict[equipData.itemType];
        for (int i = 0; i < itemChangeStats.Length; i++)
        {
            itemChangeStats[i].gameObject.SetActive(false);
        }
        StatStruct statStruct = BaseUtils.GenerateItemStatStruct(itemData, scriptableItem.classType, scriptableItem.equipType);
        StatStruct equipStruct = BaseUtils.GenerateItemStatStruct(equipData, scriptableEquip.classType, scriptableEquip.equipType);
        int damageDiff = statStruct.damage - equipStruct.damage;
        if (damageDiff != 0)
        {
            SetItemStat(itemChangeStats, itemChangeSuffixes, 0, damageDiff, "", true);
        }
        else
        {
            itemChangeStats[0].gameObject.SetActive(false);
        }
        int healthDiff = statStruct.maxHealth - equipStruct.maxHealth;
        if (healthDiff != 0)
        {
            SetItemStat(itemChangeStats, itemChangeSuffixes, 1, healthDiff, "", true);
        }
        else
        {
            itemChangeStats[1].gameObject.SetActive(false);
        }
        float defenseDiff = statStruct.defense - equipStruct.defense;
        if (defenseDiff != 0)
        {
            SetItemStat(itemChangeStats, itemChangeSuffixes, 2, Mathf.Round(defenseDiff * 100) / 100, "%", true);
        }
        else
        {
            itemChangeStats[2].gameObject.SetActive(false);
        }
        float dodgeDiff = statStruct.dodge - equipStruct.dodge;
        if (dodgeDiff != 0)
        {
            SetItemStat(itemChangeStats, itemChangeSuffixes, 3, Mathf.Round(dodgeDiff * 100) / 100, "%", true);
        }
        else
        {
            itemChangeStats[3].gameObject.SetActive(false);
        }
        float critChanceDiff = statStruct.critChance - equipStruct.critChance;
        if (critChanceDiff != 0)
        {
            SetItemStat(itemChangeStats, itemChangeSuffixes, 4, Mathf.Round(critChanceDiff * 100) / 100, "%", true);
        }
        else
        {
            itemChangeStats[4].gameObject.SetActive(false);
        }
        float lifeStealDiff = statStruct.lifeSteal - equipStruct.lifeSteal;
        if (lifeStealDiff != 0)
        {
            SetItemStat(itemChangeStats, itemChangeSuffixes, 5, Mathf.Round(lifeStealDiff * 100) / 100, "%", true);
        }
        else
        {
            itemChangeStats[5].gameObject.SetActive(false);
        }
        int statRankDiff = statStruct.ToStatRank() - equipStruct.ToStatRank();
        if (statRankDiff != 0)
        {
            SetItemStat(itemChangeStats, itemChangeSuffixes, 6, statRankDiff, "", true);
        }
        else
        {
            itemChangeStats[6].gameObject.SetActive(false);
        }
    }
    private void SetItemData(CustomText titleText, Image itemBorder, Image itemImage, CustomText itemRarity, CustomText[] itemStats, Image[] suffixes, ItemData itemData, ScriptableItem scriptableItem, bool primaryItem)
    {
        if (showLootButton)
        {
            rectTransform.anchoredPosition = Vector2.zero;
            rarityBackground.material = BaseUtils.rarityMaterials[(int)scriptableItem.rarityType];
            for (int i = 0; i < 4; i++)
            {
                rarityEffects[i].SetActive(i == (int)scriptableItem.rarityType);
            }
            BaseUtils.InstantiateEffect((EffectType)(8 + (int)scriptableItem.rarityType), Vector3.up * BaseUtils.mainCam.transform.position.y, false);
            SoundController.PlaySound("Loot_sound");
            switch (scriptableItem.equipType)
            {
                case EquipType.Armor:
                    SoundController.PlaySound("Equip_armor");
                    break;
                case EquipType.Helmet:
                    SoundController.PlaySound("Equip_armor");
                    break;
                case EquipType.Weapon:
                    if (scriptableItem.classType == ClassType.Mage)
                    {
                        SoundController.PlaySound("Equip_weapon");
                    }
                    else
                    {
                        SoundController.PlaySound("Equip_weapon_var");
                    }
                    break;
                case EquipType.Boots:
                    SoundController.PlaySound("Equip_armor");
                    break;
                case EquipType.Necklace:
                    SoundController.PlaySound("Equip_Jewel");
                    break;
                case EquipType.Ring:
                    SoundController.PlaySound("Equip_Jewel");
                    break;
                case EquipType.Empty:
                    break;
            }
            SoundController.PlaySound("Loot_loop_sparkles2");
        }
        else
        {
            SoundController.PlaySound("Button_hover");
        }
        if (primaryItem)
        {
            if (!showLootButton)
            {
                if (itemData.equipClass != ClassType.None)
                {
                    stateObj.SetActive(true);
                    stateText.SetString($"Equipped by {itemData.equipClass}", BaseUtils.stateColors[1]);
                }
                else if (itemData.price != 0)
                {
                    stateObj.SetActive(true);
                    if (BaseUtils.itemViewState == 1)
                    {
                        if (itemData.price < 0)
                        {
                            stateText.SetString($"Bought for {itemData.price * -1} @", BaseUtils.stateColors[2]);
                        }
                        else
                        {
                            stateText.SetString($"Sold for {itemData.price} @", BaseUtils.stateColors[2]);
                        }
                    }
                    else
                    {
                        stateText.SetString($"Selling for {itemData.price} @", BaseUtils.stateColors[2]);
                    }
                }
                else
                {
                    stateObj.SetActive(false);
                }
            }
            else
            {
                stateObj.SetActive(false);
            }
        }
        effectsObj.SetActive(showLootButton);
        titleObj.SetActive(showLootButton);
        lootButtonObj.SetActive(showLootButton);
        itemBorder.sprite = BaseUtils.itemBorders[(int)scriptableItem.rarityType];
        itemImage.sprite = scriptableItem.itemSprite;
        titleText.SetString(itemData.itemName, BaseUtils.rarityColors[(int)scriptableItem.rarityType]);
        string classString = scriptableItem.classType == ClassType.None ? "" : $" {scriptableItem.classType}";
        itemRarity.SetString($"{scriptableItem.rarityType}{classString} {scriptableItem.equipType}", BaseUtils.rarityColors[(int)scriptableItem.rarityType]);
        for (int i = 0; i < itemStats.Length; i++)
        {
            itemStats[i].gameObject.SetActive(false);
        }
        StatStruct statStruct = BaseUtils.GenerateItemStatStruct(itemData, scriptableItem.classType, scriptableItem.equipType);
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
        SetItemStat(itemStats, suffixes, 6, statStruct.ToStatRank());
    }
    private void SetItemStat(CustomText[] itemStats, Image[] suffixes, int index, float value, string suffix = "", bool colored = false)
    {
        itemStats[index].gameObject.SetActive(true);
        string addPrefix = value > 0 ? "+" : "";
        if (colored)
        {
            itemStats[index].SetString($"{addPrefix}{value}{suffix}", value < 0 ? negativeColor : positiveColor);
            suffixes[index].color = value < 0 ? negativeColor : positiveColor;
        }
        else
        {
            itemStats[index].SetString($"{addPrefix}{value}{suffix}", Color.white);
        }
        StartCoroutine(WaitAndFixSuffix(suffixes[index].rectTransform));
    }
    private IEnumerator WaitAndFixSuffix(RectTransform rectTransform)
    {
        yield return new WaitForEndOfFrame();
        rectTransform.SetAsLastSibling();
    }
}

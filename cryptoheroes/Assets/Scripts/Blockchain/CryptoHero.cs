using System;
using System.Collections.Generic;
using UnityEngine;

public class CryptoHero
{
    public static void FillDatabaseFromPlayerResponse(PlayerDataResponse res)
    {
        Database.databaseStruct.pixelTokens = (int)((long.Parse(res.balance.pixeltoken) / 1000000));
        Database.databaseStruct.charStructs = ParseCharStructs(res.characters);
        Database.databaseStruct.genesisTime = TimestampHelper.GetDateTimeFromTimestamp(res.balance.titan_timer);
        Database.databaseStruct.genesisAmount = res.balance.titan;
        Database.databaseStruct.fightBalance = res.playerdata.fight_balance;
        Database.databaseStruct.presaleChests = new List<RarityType>();
        for (int i = 0; i < res.balance.common; i++)
        {
            Database.databaseStruct.presaleChests.Add(RarityType.Common);
        }
        for (int i = 0; i < res.balance.rare; i++)
        {
            Database.databaseStruct.presaleChests.Add(RarityType.Rare);
        }
        for (int i = 0; i < res.balance.epic; i++)
        {
            Database.databaseStruct.presaleChests.Add(RarityType.Epic);
        }
        for (int i = 0; i < res.balance.legendary; i++)
        {
            Database.databaseStruct.presaleChests.Add(RarityType.Legendary);
        }
        Database.databaseStruct.ownedItems.Clear();
        List<ItemData> items = ParseItemStructs(res.items);
        if (!Database.databaseStruct.firstLogin)
        {
            List<ItemEquipInfo> itemEquipInfos = GetClassEquips(res.characters);
            for (int i = 0; i < items.Count; i++)
            {
                ClassType classType = GetEquipClass(itemEquipInfos, items[i].itemID);
                Database.SetEquip(items[i].itemID, classType, true);
            }
        }
        else
        {
            for (int j = 0; j < Database.databaseStruct.itemEquipInfos.Count; j++)
            {
                Queue<int> itemsToRemove = new Queue<int>();
                if (Database.databaseStruct.itemEquipInfos[j].playerAccount == Database.databaseStruct.playerAccount)
                {
                    for (int i = 0; i < Database.databaseStruct.itemEquipInfos[j].itemEquipInfo.Count; i++)
                    {
                        bool hasItem = false;
                        for (int k = 0; k < items.Count; k++)
                        {
                            if (items[k].itemID == Database.databaseStruct.itemEquipInfos[j].itemEquipInfo[i].itemID)
                            {
                                items[k] = new ItemData(
                                    items[k].cooldown,
                                    items[k].strength,
                                    items[k].dexterity,
                                    items[k].endurance,
                                    items[k].intelligence,
                                    items[k].luck,
                                    items[k].itemName,
                                    items[k].itemID,
                                    items[k].price,
                                    items[k].itemType,
                                    Database.databaseStruct.itemEquipInfos[j].itemEquipInfo[i].classType);
                                hasItem = true;
                                break;
                            }
                        }
                        if (!hasItem)
                        {
                            itemsToRemove.Enqueue(i);
                        }
                    }
                    break;
                }
                while (itemsToRemove.Count > 0)
                {
                    Database.databaseStruct.itemEquipInfos[j].itemEquipInfo.RemoveAt(itemsToRemove.Dequeue());
                }
            }
        }
        for (int i = 0; i < Database.databaseStruct.visualIndexes.Count; i++)
        {
            bool hasItem = false;
            for (int k = 0; k < items.Count; k++)
            {
                if (items[k].itemID == Database.databaseStruct.visualIndexes[i].itemID)
                {
                    hasItem = true;
                    break;
                }
            }
            if (!hasItem)
            {
                Database.databaseStruct.visualIndexes.RemoveAt(i);
                i--;
            }
        }
        Database.GetItemsOnSale();
        for (int i = 0; i < items.Count; i++)
        {
            if (Database.ItemHasVisualIndex(items[i].itemID))
            {
                Database.databaseStruct.ownedItems.Add(items[i]);
            }
            else
            {
                Database.AddItem(items[i]);
            }
            if (Database.itemsOnSale.Contains(items[i].itemID))
            {
                Database.itemsOnSale.Remove(items[i].itemID);
            }
        }
        Database.SaveDatabase();
    }
    private static List<ItemEquipInfo> GetClassEquips(List<CharacterData> characterDatas)
    {
        List<ItemEquipInfo> equipInfos = new List<ItemEquipInfo>();
        for (int i = 0; i < characterDatas.Count; i++)
        {
            for (int k = 0; k < characterDatas[i].inventory.Count; k++)
            {
                equipInfos.Add(new ItemEquipInfo() { classType = (ClassType)characterDatas[i].class_type, itemID = characterDatas[i].inventory[k] });
            }
        }
        return equipInfos;
    }
    private static List<ItemData> ParseItemStructs(List<ItemToken> items)
    {
        List<ItemData> itemStructs = new List<ItemData>();
        for (int i = 0; i < items.Count; i++)
        {
            long.TryParse(items[i].price, out long rawPrice);
            int.TryParse(items[i].token_id, out int itemID);
            itemStructs.Add(
                new ItemData(
                    TimestampHelper.GetDateTimeFromTimestamp(items[i].cd / 1000 / 1000),
                    items[i].strength,
                    items[i].dexterity,
                    items[i].endurance,
                    items[i].intelligence,
                    items[i].luck,
                    BaseUtils.GenerateItemName(BaseUtils.itemDict[(ItemType)items[i].item_type].synonymString, (RarityType)items[i].rarity_type, itemID),
                    itemID,
                    (int)(rawPrice / 1000000),
                    (ItemType)items[i].item_type,
                    ClassType.None));
        }
        return itemStructs;
    }
    private static ClassType GetEquipClass(List<ItemEquipInfo> classEquips, int itemID)
    {
        for (int i = 0; i < classEquips.Count; i++)
        {
            if (classEquips[i].itemID == itemID)
            {
                return classEquips[i].classType;
            }
        }
        return ClassType.None;
    }
    private static List<CharData> ParseCharStructs(List<CharacterData> chars)
    {
        List<CharData> charStructs = new List<CharData>();
        for (int i = 0; i < chars.Count; i++)
        {
            charStructs.Add(new CharData(
                (ClassType)chars[i].class_type,
                chars[i].experience,
                chars[i].level,
                TimestampHelper.GetDateTimeFromTimestamp(chars[i].injured_timer),
                chars[i].strengthPotion,
                chars[i].staminaPotion,
                chars[i].luckPotion));
        }
        return charStructs;
    }
}

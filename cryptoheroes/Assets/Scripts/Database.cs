using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
//using this struct to transfer item data
public struct ItemData
{
    public DateTime cooldown;
    public int strength;
    public int dexterity;
    public int endurance;
    public int intelligence;
    public int luck;
    public int itemID;
    //price in which its selling on store, 0 if its not selling at all
    public int price;
    //name is unique and generated for each item
    public string itemName;
    public ItemType itemType;
    //which class has this item equipped
    public ClassType equipClass;

    public ItemData(DateTime cooldown, int strength, int dexterity, int endurance, int intelligence, int luck, string itemName, int itemID, int price, ItemType itemType, ClassType equipClass)
    {
        this.cooldown = cooldown;
        this.strength = strength;
        this.dexterity = dexterity;
        this.endurance = endurance;
        this.intelligence = intelligence;
        this.luck = luck;
        this.itemID = itemID;
        this.price = price;
        this.itemName = itemName;
        this.itemType = itemType;
        this.equipClass = equipClass;
    }
}
public struct CharData
{
    public ClassType classType;
    public int experience;
    public int level;
    public DateTime injuredTimer;
    public PotionData strengthPotion;
    public PotionData staminaPotion;
    public PotionData luckPotion;

    public CharData(ClassType classType, int experience, int level, DateTime injuredTimer, PotionData strengthPotion, PotionData staminaPotion, PotionData luckPotion)
    {
        this.classType = classType;
        this.experience = experience;
        this.level = level;
        this.injuredTimer = injuredTimer;
        this.strengthPotion = strengthPotion;
        this.staminaPotion = staminaPotion;
        this.luckPotion = luckPotion;
    }
}
public class AccEquipInfo
{
    public string playerAccount;
    public List<ItemEquipInfo> itemEquipInfo;
}
public class AccSaleInfo
{
    public string playerAccount;
    public List<ItemData> itemInfo;
}
[Serializable]
public struct PotionData
{
    public int amount;
    public int strength;

    public PotionData(int amount, int strength)
    {
        this.amount = amount;
        this.strength = strength;
    }
}
public class DatabaseStruct
{
    public string playerAccount;
    public string publicKey;
    public string privateKey;
    public bool validCredentials;
    public int genesisAmount;
    public bool firstLogin;
    public bool rainOff;
    public DateTime genesisTime;
    public int pixelTokens;
    public int lockSpeed;
    public int fightBalance;
    public int musicVolume;
    public int soundVolume;
    public float allowance;
    public List<RarityType> presaleChests = new List<RarityType>();
    public List<CharData> charStructs = new List<CharData>();
    public List<ItemData> ownedItems = new List<ItemData>();
    public List<ItemData> pastTransactions = new List<ItemData>();
    public List<AccSaleInfo> itemSaleInfos = new List<AccSaleInfo>();
    public List<AccEquipInfo> itemEquipInfos = new List<AccEquipInfo>();
    public List<VisualItemStruct> visualIndexes = new List<VisualItemStruct>();
}
public class Database : MonoBehaviour
{
    public static DatabaseStruct databaseStruct;
    private static string databaseName;
    private static readonly string currentVersion = "0.2";
    public static readonly int maxItems = 60;
    public static readonly List<int> itemsOnSale = new List<int>();
    private void Awake()
    {
        databaseName = "CryptoHeroDatabase";
        if (PlayerPrefs.HasKey("CHCVersion"))
        {
            string clientVersion = PlayerPrefs.GetString("CHCVersion");
            if (clientVersion != currentVersion && PlayerPrefs.HasKey(databaseName))
            {
                PlayerPrefs.DeleteKey(databaseName);
            }
        }
        else if (PlayerPrefs.HasKey(databaseName))
        {
            PlayerPrefs.DeleteKey(databaseName);
        }

        PlayerPrefs.SetString("CHCVersion", currentVersion);

        if (PlayerPrefs.HasKey(databaseName))
        {
            LoadDatabase();
        }
        else
        {
            databaseStruct = new DatabaseStruct
            {
                lockSpeed = -1
            };
            SaveDatabase();
        }
    }
    private void LoadDatabase()
    {
        databaseStruct = XMLGenerator.DeserializeObject(PlayerPrefs.GetString(databaseName), typeof(DatabaseStruct)) as DatabaseStruct;
    }
    public static void SaveDatabase()
    {
        PlayerPrefs.SetString(databaseName, XMLGenerator.SerializeObject(databaseStruct, typeof(DatabaseStruct)));
    }
    public static void AddItem(ItemData itemData)
    {
        if (databaseStruct.ownedItems.Count >= 60)
        {
            return;
        }
        //databaseStruct.itemIndexes.Add(databaseStruct.ownedItems.Count);
        List<int> emptyVisualIndexes = new List<int>();
        for (int i = 0; i < maxItems; i++)
        {
            emptyVisualIndexes.Add(i);
        }
        for (int i = 0; i < databaseStruct.visualIndexes.Count; i++)
        {
            emptyVisualIndexes.Remove(databaseStruct.visualIndexes[i].visualIndex);
        }
        emptyVisualIndexes.Sort((x, y) => x.CompareTo(y));
        databaseStruct.ownedItems.Add(itemData);
        databaseStruct.visualIndexes.Add(new VisualItemStruct(emptyVisualIndexes[0], itemData.itemID));
    }
    public static void RemoveItem(int itemID)
    {
        for (int i = 0; i < databaseStruct.ownedItems.Count; i++)
        {
            if (databaseStruct.ownedItems[i].itemID == itemID)
            {
                databaseStruct.ownedItems.RemoveAt(i);
                break;
            }
        }
        for (int i = 0; i < databaseStruct.visualIndexes.Count; i++)
        {
            if (databaseStruct.visualIndexes[i].itemID == itemID)
            {
                databaseStruct.visualIndexes.RemoveAt(i);
                break;
            }
        }
    }
    public static void SetVisualIndex(int itemID, int visualIndex)
    {
        for (int i = 0; i < databaseStruct.visualIndexes.Count; i++)
        {
            if (databaseStruct.visualIndexes[i].itemID == itemID)
            {
                databaseStruct.visualIndexes[i] = new VisualItemStruct(visualIndex, itemID);
                break;
            }
        }
    }
    public static bool ItemHasVisualIndex(int itemID)
    {
        for (int i = 0; i < databaseStruct.visualIndexes.Count; i++)
        {
            if (databaseStruct.visualIndexes[i].itemID == itemID)
            {
                return true;
            }
        }
        return false;
    }
    public static int GetItemFromVisualIndex(int visualIndex)
    {
        for (int i = 0; i < databaseStruct.visualIndexes.Count; i++)
        {
            if (databaseStruct.visualIndexes[i].visualIndex == visualIndex)
            {
                return databaseStruct.visualIndexes[i].itemID;
            }
        }
        return -1;
    }
    public static int GetItemFromID(int itemID)
    {
        for (int i = 0; i < databaseStruct.ownedItems.Count; i++)
        {
            if (databaseStruct.ownedItems[i].itemID == itemID)
            {
                return i;
            }
        }
        return -1;
    }
    public static ItemData GetItem(int itemID)
    {
        for (int i = 0; i < databaseStruct.ownedItems.Count; i++)
        {
            if (databaseStruct.ownedItems[i].itemID == itemID)
            {
                return databaseStruct.ownedItems[i];
            }
        }
        return new ItemData();
    }
    public static bool HasItem(int itemID)
    {
        for (int i = 0; i < databaseStruct.ownedItems.Count; i++)
        {
            if (databaseStruct.ownedItems[i].itemID == itemID)
            {
                return true;
            }
        }
        return false;
    }
    public static bool HasClass(ClassType classType)
    {
        for (int i = 0; i < databaseStruct.charStructs.Count; i++)
        {
            if (databaseStruct.charStructs[i].classType == classType)
            {
                return true;
            }
        }
        return false;
    }
    public static bool HasEquippedType(ClassType classType, EquipType equipType, out int databaseIndex)
    {
        databaseIndex = -1;
        for (int i = 0; i < databaseStruct.ownedItems.Count; i++)
        {
            if (databaseStruct.ownedItems[i].equipClass != ClassType.None)
            {
                ClassType itemClass = BaseUtils.itemDict[databaseStruct.ownedItems[i].itemType].classType;
                if (BaseUtils.itemDict[databaseStruct.ownedItems[i].itemType].equipType == equipType &&
                    itemClass == classType)
                {
                    databaseIndex = i;
                    return true;
                }
            }
        }
        return false;
    }
    public static List<ItemType> GetEquippedItemTypes(ClassType classType)
    {
        List<ItemType> equippedItems = new List<ItemType>();
        for (int i = 0; i < databaseStruct.ownedItems.Count; i++)
        {
            if (databaseStruct.ownedItems[i].equipClass == classType)
            {
                equippedItems.Add(databaseStruct.ownedItems[i].itemType);
            }
        }
        return equippedItems;
    }
    public static List<ItemData> GetEquippedItemDatas(ClassType classType)
    {
        List<ItemData> equippedItems = new List<ItemData>();
        for (int i = 0; i < databaseStruct.ownedItems.Count; i++)
        {
            if (databaseStruct.ownedItems[i].equipClass == classType)
            {
                equippedItems.Add(databaseStruct.ownedItems[i]);
            }
        }
        return equippedItems;
    }
    public static void AddTransaction(ItemData itemData, bool buying)
    {
        databaseStruct.pastTransactions.Add(new ItemData(
                itemData.cooldown,
                itemData.strength,
                itemData.dexterity,
                itemData.endurance,
                itemData.intelligence,
                itemData.luck,
                itemData.itemName,
                itemData.itemID,
                buying ? itemData.price * -1 : itemData.price,
                itemData.itemType,
                itemData.equipClass));
    }
    public static void SetPrice(int itemID, int price)
    {
        for (int i = 0; i < databaseStruct.ownedItems.Count; i++)
        {
            if (databaseStruct.ownedItems[i].itemID == itemID)
            {
                databaseStruct.ownedItems[i] = new ItemData(
                databaseStruct.ownedItems[i].cooldown,
                databaseStruct.ownedItems[i].strength,
                databaseStruct.ownedItems[i].dexterity,
                databaseStruct.ownedItems[i].endurance,
                databaseStruct.ownedItems[i].intelligence,
                databaseStruct.ownedItems[i].luck,
                databaseStruct.ownedItems[i].itemName,
                databaseStruct.ownedItems[i].itemID,
                price,
                databaseStruct.ownedItems[i].itemType,
                databaseStruct.ownedItems[i].equipClass);
                break;
            }
        }
    }
    public static void AddSale(int itemID)
    {
        int accountIndex = -1;
        for (int k = 0; k < databaseStruct.itemSaleInfos.Count; k++)
        {
            if (databaseStruct.itemSaleInfos[k].playerAccount == databaseStruct.playerAccount)
            {
                accountIndex = k;
                break;
            }
        }
        if (accountIndex == -1)
        {
            databaseStruct.itemSaleInfos.Add(new AccSaleInfo() { itemInfo = new List<ItemData>(), playerAccount = databaseStruct.playerAccount });
            accountIndex = databaseStruct.itemSaleInfos.Count - 1;
        }
        databaseStruct.itemSaleInfos[accountIndex].itemInfo.Add(GetItem(itemID));
    }
    public static void RemoveSale(int itemID)
    {
        for (int i = 0; i < databaseStruct.itemSaleInfos.Count; i++)
        {
            if (databaseStruct.itemSaleInfos[i].playerAccount == databaseStruct.playerAccount)
            {
                for (int k = 0; k < databaseStruct.itemSaleInfos[i].itemInfo.Count; k++)
                {
                    if (databaseStruct.itemSaleInfos[i].itemInfo[k].itemID == itemID)
                    {
                        databaseStruct.itemSaleInfos[i].itemInfo.RemoveAt(k);
                        break;
                    }
                }
                break;
            }
        }
    }
    public static ItemData GetSaleItem(int itemID)
    {
        for (int i = 0; i < databaseStruct.itemSaleInfos.Count; i++)
        {
            if (databaseStruct.itemSaleInfos[i].playerAccount == databaseStruct.playerAccount)
            {
                for (int k = 0; k < databaseStruct.itemSaleInfos[i].itemInfo.Count; k++)
                {
                    if (databaseStruct.itemSaleInfos[i].itemInfo[k].itemID == itemID)
                    {
                        return databaseStruct.itemSaleInfos[i].itemInfo[k];
                    }
                }
                break;
            }
        }
        return new ItemData();
    }
    public static void GetItemsOnSale()
    {
        itemsOnSale.Clear();
        for (int i = 0; i < databaseStruct.itemSaleInfos.Count; i++)
        {
            if (databaseStruct.itemSaleInfos[i].playerAccount == databaseStruct.playerAccount)
            {
                for (int k = 0; k < databaseStruct.itemSaleInfos[i].itemInfo.Count; k++)
                {
                    itemsOnSale.Add(databaseStruct.itemSaleInfos[i].itemInfo[k].itemID);
                }
            }
        }
    }
    public static void SetEquip(int itemID, ClassType classType, bool fromSync = false)
    {
        int accountIndex = -1;
        for (int k = 0; k < databaseStruct.itemEquipInfos.Count; k++)
        {
            if (databaseStruct.itemEquipInfos[k].playerAccount == databaseStruct.playerAccount)
            {
                accountIndex = k;
                break;
            }
        }
        if (accountIndex == -1)
        {
            databaseStruct.itemEquipInfos.Add(new AccEquipInfo() { itemEquipInfo = new List<ItemEquipInfo>(), playerAccount = databaseStruct.playerAccount });
            accountIndex = databaseStruct.itemEquipInfos.Count - 1;
        }
        bool hasEquipInfo = false;
        for (int i = 0; i < databaseStruct.itemEquipInfos[accountIndex].itemEquipInfo.Count; i++)
        {
            if (databaseStruct.itemEquipInfos[accountIndex].itemEquipInfo[i].itemID == itemID)
            {
                databaseStruct.itemEquipInfos[accountIndex].itemEquipInfo[i].classType = classType;
                hasEquipInfo = true;
                break;
            }
        }
        if (!hasEquipInfo)
        {
            databaseStruct.itemEquipInfos[accountIndex].itemEquipInfo.Add(new ItemEquipInfo() { itemID = itemID, classType = classType });
        }
        if (!fromSync)
        {
            for (int i = 0; i < databaseStruct.ownedItems.Count; i++)
            {
                if (databaseStruct.ownedItems[i].itemID == itemID)
                {
                    databaseStruct.ownedItems[i] = new ItemData(
                    databaseStruct.ownedItems[i].cooldown,
                    databaseStruct.ownedItems[i].strength,
                    databaseStruct.ownedItems[i].dexterity,
                    databaseStruct.ownedItems[i].endurance,
                    databaseStruct.ownedItems[i].intelligence,
                    databaseStruct.ownedItems[i].luck,
                    databaseStruct.ownedItems[i].itemName,
                    databaseStruct.ownedItems[i].itemID,
                    databaseStruct.ownedItems[i].price,
                    databaseStruct.ownedItems[i].itemType,
                    classType);
                    break;
                }
            }
            SaveDatabase();
        }
    }
    public static void AddClass(ClassType classType)
    {
        databaseStruct.charStructs.Add(new CharData(classType, 0, 1, DateTime.Now.AddHours(-1), new PotionData(0, 0), new PotionData(0, 0), new PotionData(0, 0)));
    }
    public static void AddExp(ClassType classType, int exp)
    {
        for (int i = 0; i < databaseStruct.charStructs.Count; i++)
        {
            if (databaseStruct.charStructs[i].classType == classType)
            {
                databaseStruct.charStructs[i] = new CharData(
                databaseStruct.charStructs[i].classType,
                databaseStruct.charStructs[i].experience + exp,
                databaseStruct.charStructs[i].level,
                databaseStruct.charStructs[i].injuredTimer,
                databaseStruct.charStructs[i].strengthPotion,
                databaseStruct.charStructs[i].staminaPotion,
                databaseStruct.charStructs[i].luckPotion);
                break;
            }
        }
    }
    public static void AddLevel(ClassType classType)
    {
        for (int i = 0; i < databaseStruct.charStructs.Count; i++)
        {
            if (databaseStruct.charStructs[i].classType == classType)
            {
                databaseStruct.charStructs[i] = new CharData(
                databaseStruct.charStructs[i].classType,
                databaseStruct.charStructs[i].experience,
                databaseStruct.charStructs[i].level + 1,
                databaseStruct.charStructs[i].injuredTimer,
                databaseStruct.charStructs[i].strengthPotion,
                databaseStruct.charStructs[i].staminaPotion,
                databaseStruct.charStructs[i].luckPotion);
                break;
            }
        }
    }
    public static void SetInjured(ClassType classType, DateTime injuredTimer)
    {
        for (int i = 0; i < databaseStruct.charStructs.Count; i++)
        {
            if (databaseStruct.charStructs[i].classType == classType)
            {
                databaseStruct.charStructs[i] = new CharData(
                databaseStruct.charStructs[i].classType,
                databaseStruct.charStructs[i].experience,
                databaseStruct.charStructs[i].level,
                injuredTimer,
                databaseStruct.charStructs[i].strengthPotion,
                databaseStruct.charStructs[i].staminaPotion,
                databaseStruct.charStructs[i].luckPotion);
                break;
            }
        }
    }
    public static void SetStrengthPotion(ClassType classType, PotionData potionData)
    {
        for (int i = 0; i < databaseStruct.charStructs.Count; i++)
        {
            if (databaseStruct.charStructs[i].classType == classType)
            {
                databaseStruct.charStructs[i] = new CharData(
                databaseStruct.charStructs[i].classType,
                databaseStruct.charStructs[i].experience,
                databaseStruct.charStructs[i].level,
                databaseStruct.charStructs[i].injuredTimer,
                potionData,
                databaseStruct.charStructs[i].staminaPotion,
                databaseStruct.charStructs[i].luckPotion);
                break;
            }
        }
    }
    public static void SetStaminaPotion(ClassType classType, PotionData potionData)
    {
        for (int i = 0; i < databaseStruct.charStructs.Count; i++)
        {
            if (databaseStruct.charStructs[i].classType == classType)
            {
                databaseStruct.charStructs[i] = new CharData(
                databaseStruct.charStructs[i].classType,
                databaseStruct.charStructs[i].experience,
                databaseStruct.charStructs[i].level,
                databaseStruct.charStructs[i].injuredTimer,
                databaseStruct.charStructs[i].strengthPotion,
                potionData,
                databaseStruct.charStructs[i].luckPotion);
                break;
            }
        }
    }
    public static void SetLuckPotion(ClassType classType, PotionData potionData)
    {
        for (int i = 0; i < databaseStruct.charStructs.Count; i++)
        {
            if (databaseStruct.charStructs[i].classType == classType)
            {
                databaseStruct.charStructs[i] = new CharData(
                databaseStruct.charStructs[i].classType,
                databaseStruct.charStructs[i].experience,
                databaseStruct.charStructs[i].level,
                databaseStruct.charStructs[i].injuredTimer,
                databaseStruct.charStructs[i].strengthPotion,
                databaseStruct.charStructs[i].staminaPotion,
                potionData);
                break;
            }
        }
    }
    public static PotionData GetStaminaPotion(ClassType classType)
    {
        for (int i = 0; i < databaseStruct.charStructs.Count; i++)
        {
            if (databaseStruct.charStructs[i].classType == classType)
            {
                return databaseStruct.charStructs[i].staminaPotion;
            }
        }
        return new PotionData();
    }
    public static PotionData GetLuckPotion(ClassType classType)
    {
        for (int i = 0; i < databaseStruct.charStructs.Count; i++)
        {
            if (databaseStruct.charStructs[i].classType == classType)
            {
                return databaseStruct.charStructs[i].luckPotion;
            }
        }
        return new PotionData();
    }
    public static PotionData GetStrengthPotion(ClassType classType)
    {
        for (int i = 0; i < databaseStruct.charStructs.Count; i++)
        {
            if (databaseStruct.charStructs[i].classType == classType)
            {
                return databaseStruct.charStructs[i].strengthPotion;
            }
        }
        return new PotionData();
    }
    public static bool HasStaminaPotion(ClassType classType)
    {
        for (int i = 0; i < databaseStruct.charStructs.Count; i++)
        {
            if (databaseStruct.charStructs[i].classType == classType)
            {
                return databaseStruct.charStructs[i].staminaPotion.amount > 0;
            }
        }
        return false;
    }
    public static bool HasLuckPotion(ClassType classType)
    {
        for (int i = 0; i < databaseStruct.charStructs.Count; i++)
        {
            if (databaseStruct.charStructs[i].classType == classType)
            {
                return databaseStruct.charStructs[i].luckPotion.amount > 0;
            }
        }
        return false;
    }
    public static bool HasStrengthPotion(ClassType classType)
    {
        for (int i = 0; i < databaseStruct.charStructs.Count; i++)
        {
            if (databaseStruct.charStructs[i].classType == classType)
            {
                return databaseStruct.charStructs[i].strengthPotion.amount > 0;
            }
        }
        return false;
    }
    public static CharData GetCharStruct(ClassType classType)
    {
        for (int i = 0; i < databaseStruct.charStructs.Count; i++)
        {
            if (databaseStruct.charStructs[i].classType == classType)
            {
                return databaseStruct.charStructs[i];
            }
        }
        return databaseStruct.charStructs[0];
    }
    private void OnApplicationQuit()
    {
        SaveDatabase();
    }
}
public struct VisualItemStruct
{
    public int visualIndex;
    public int itemID;

    public VisualItemStruct(int visualIndex, int itemID)
    {
        this.visualIndex = visualIndex;
        this.itemID = itemID;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(Database))]
public class OfflineDatabaseUI : Editor
{
    public override void OnInspectorGUI()
    {
        //Database generator = (Database)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Clear Database"))
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
#endif

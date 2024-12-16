using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public ForgeController forgeController;
    public BackpackController backpackController;
    public CameraController cameraController;
    public MainMenuController mainMenuController;
    public GameController gameController;
    public GameObject inventoryWindow;
    public GameObject titleWindow;
    public Transform[] classesTransform;
    public CharacterInfoController[] classesInfo;
    public ItemCell[] equipCells;
    public ItemCell[] forgeCells;
    public CustomText titleText;
    public CustomText healthText;
    public CustomText critChanceText;
    public CustomText lifeStealText;
    public CustomText damageText;
    public CustomText expText;
    public CustomText defenseText;
    public CustomText dodgeText;
    public CustomText statRankText;

    /*public CustomText strengthText;
    public CustomText dexterityText;
    public CustomText intelligenceText;
    public CustomText enduranceText;
    public CustomText luckText;*/

    public Image itemPreviewEdge;
    public Image itemPreviewItem;
    private int classIndex;
    [HideInInspector]
    public bool showingPreview;
    public ItemData draggingItem;
    [HideInInspector]
    public ClassType classType;
    public void Setup(ClassType classType)
    {
        if (inventoryWindow.activeSelf)
        {
            Dispose();
            return;
        }
        this.classType = classType;
        classIndex = (int)classType - 1;
        inventoryWindow.SetActive(true);
        mainMenuController.buttonsWindow.SetActive(false);
        backpackController.Setup(false);
        for (int i = 0; i < 3; i++)
        {
            classesInfo[i].gameObject.SetActive(i == classIndex);
            classesInfo[i].onInventory = (i == classIndex);
            if (i == classIndex)
            {
                classesInfo[i].MoveParts();
            }
        }
        //titleText.SetString($"{classType} inventory lvl: {Database.databaseStruct.charStructs[classIndex].level}");
        UpdateEquipItemCells(classType);
        cameraController.MoveCamera(classesTransform[classIndex].transform.position + cameraController.inventoryOffset + Vector3.back * 10);
        cameraController.ZoomCamera(4.6f);
    }
    private void UpdateStats(GameCharController gameCharController)
    {
        gameCharController.SetCharStruct();
        int charLevel = Database.GetCharStruct(classType).level;
        int baseExp = BaseUtils.GetExpForNextLevel(charLevel - 1);
        int currentExp = Database.GetCharStruct(classType).experience;
        Color textColor = Database.HasStrengthPotion(classType) ? BaseUtils.enabledColor : Color.white;
        expText.SetString($"exp: {Mathf.RoundToInt(currentExp - baseExp)} / {BaseUtils.GetExpForNextLevel(charLevel) - baseExp}", BaseUtils.enabledColor);
        damageText.SetString($"damage: {Mathf.RoundToInt(gameCharController.statStruct.damage * .5f)}-{gameCharController.statStruct.damage}", textColor);
        healthText.SetString($"max health: {gameCharController.statStruct.maxHealth}", textColor);
        defenseText.SetString($"defense: {Mathf.Round(gameCharController.statStruct.defense * 100) / 100}%", textColor);
        dodgeText.SetString($"dodge: {Mathf.Round(gameCharController.statStruct.dodge * 100) / 100}%", textColor);
        critChanceText.SetString($"crit chance: {Mathf.Round(gameCharController.statStruct.critChance * 100) / 100}%", textColor);
        lifeStealText.SetString($"life steal: {Mathf.Round(gameCharController.statStruct.lifeSteal * 100) / 100}%", textColor);
        statRankText.SetString($"stat rank: {gameCharController.statStruct.ToStatRank() + charLevel * 100}", textColor);
        /*strengthText.SetString($"strength: {gameCharController.strength}");
        dexterityText.SetString($"dexterity: {gameCharController.dexterity}");
        enduranceText.SetString($"endurance: {gameCharController.endurance}");
        intelligenceText.SetString($"intellect: {gameCharController.intelligence}");
        luckText.SetString($"luck: {gameCharController.luck}");*/
    }
    public void UpdateEquipItemCells(ClassType classType)
    {
        for (int i = 0; i < 6; i++)
        {
            bool hasItem = false;
            for (int k = 0; k < Database.databaseStruct.ownedItems.Count; k++)
            {
                if (Database.databaseStruct.ownedItems[k].equipClass == classType &&
                    BaseUtils.itemDict[Database.databaseStruct.ownedItems[k].itemType].equipType == (EquipType)i)
                {
                    equipCells[i].SetEquipItem(Database.databaseStruct.ownedItems[k], (EquipType)i);
                    hasItem = true;
                    break;
                }
            }
            if (!hasItem)
            {
                equipCells[i].SetEquipItem(new ItemData() { itemType = ItemType.None, itemID = -1 }, (EquipType)i);
            }
        }
        UpdateStats(gameController.gameChars[classIndex]);
    }

    private void LateUpdate()
    {
        if (showingPreview)
        {
            itemPreviewEdge.rectTransform.position = BaseUtils.mainCam.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 20) * cameraController.OrthoDiff;
        }
    }
    public void Dispose(bool intoWindow = false)
    {
        if (!inventoryWindow.activeSelf)
        {
            return;
        }
        EndDrag();
        if (!intoWindow)
        {
            mainMenuController.buttonsWindow.SetActive(true);
            cameraController.MoveCamera(Vector3.back * 10);
            cameraController.ZoomCamera(6f);
            for (int i = 0; i < 3; i++)
            {
                classesInfo[i].gameObject.SetActive(true);
                classesInfo[i].onInventory = false;
                if (i == classIndex && !intoWindow)
                {
                    classesInfo[i].MoveParts();
                }
            }
        }
        inventoryWindow.SetActive(false);
    }
    public void BeginDrag(ItemData itemData)
    {
        if (showingPreview)
        {
            return;
        }
        showingPreview = true;
        draggingItem = itemData;
        itemPreviewEdge.gameObject.SetActive(true);
        itemPreviewEdge.sprite = backpackController.itemBorders[(int)BaseUtils.itemDict[itemData.itemType].rarityType];
        itemPreviewItem.sprite = BaseUtils.itemDict[itemData.itemType].itemSprite;
    }
    public void EndDrag()
    {
        showingPreview = false;
        itemPreviewEdge.gameObject.SetActive(false);
    }
    public void OnDequip(ItemCell equipCell)
    {
        SoundController.PlaySound("unequip");
        Database.SetEquip(equipCell.itemData.itemID, ClassType.None);
        UpdateEquipItemCells(classType);
        gameController.UpdateCharacter(classType);
    }
    private void OnEquip(ItemCell itemCell, ItemCell equipCell)
    {
        equipCell.SetData(this, itemCell.itemCellData);
        Database.SetEquip(equipCell.itemData.itemID, classType);
        UpdateEquipItemCells(classType);
        gameController.UpdateCharacter(classType);
        switch (equipCell.equipType)
        {
            case EquipType.Armor:
                SoundController.PlaySound("Equip_armor", .5f, true);
                break;
            case EquipType.Helmet:
                SoundController.PlaySound("Equip_armor", .5f, true);
                break;
            case EquipType.Weapon:
                if (equipCell.classType == ClassType.Mage)
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
        SoundController.PlaySound("sparks", 1, true);
        BaseUtils.InstantiateEffect((EffectType)(12 + (int)equipCell.rarity), gameController.gameChars[(int)classType - 1].transform.position + Vector3.up * 40f.ToScale(), true);
    }
    public void SwitchItems(ItemCell itemCellA, ItemCell itemCellB, bool fromForge)
    {
        if (itemCellB == null)
        {
            if (fromForge)
            {
                itemCellB = forgeCells[0].emptyItem ? forgeCells[0] : forgeCells[1];
            }
            else
            {
                itemCellB = equipCells[(int)itemCellA.equipType];
            }
        }
        if (itemCellB.forgeIndex != 0)
        {
            forgeController.SetForgeItem(itemCellA, itemCellB);
            forgeController.UpdateItems();
            return;
        }
        if (itemCellB.equipSlot)
        {
            if (itemCellA.blockedItem || (itemCellA.classType != classType && itemCellA.classType != ClassType.None))
            {
                return;
            }
            if (itemCellA.equipType == itemCellB.equipType)
            {
                if (itemCellA.itemData.itemID == itemCellB.itemData.itemID)
                {
                    return;
                }
                if (Database.HasEquippedType(itemCellA.classType, itemCellA.equipType, out int _))
                {
                    OnDequip(itemCellA.equipSlot ? itemCellA : itemCellB);
                    OnEquip(itemCellA.equipSlot ? itemCellB : itemCellA, itemCellA.equipSlot ? itemCellA : itemCellB);
                    backpackController.Setup(true);
                    return;
                }
                if (itemCellA.equipSlot)
                {
                    OnEquip(itemCellB, itemCellA);
                }
                else if (itemCellB.equipSlot)
                {
                    OnEquip(itemCellA, itemCellB);
                }
                backpackController.Setup(true);
            }
            return;
        }
        if (fromForge && itemCellB.itemData.itemID != 0 && (itemCellB.itemData.itemID == forgeController.itemOneSlotCell.itemData.itemID || itemCellB.itemData.itemID == forgeController.itemTwoSlotCell.itemData.itemID))
        {
            forgeController.UpdateItems();
            return;
        }    
        int indexA = itemCellA.displayIndex;
        int indexB = itemCellB.displayIndex;
        int idA = itemCellA.emptyItem ? -1 : itemCellA.itemData.itemID;
        int idB = itemCellB.emptyItem ? -1 : itemCellB.itemData.itemID;
        Database.SetVisualIndex(idA, indexB);
        Database.SetVisualIndex(idB, indexA);
        if (fromForge)
        {
            forgeController.UpdateItems();
        }
        else
        {
            backpackController.Setup(true);
        }
    }
    public void ShowTooltip(bool fromForge, bool fromEquip, RectTransform target, ItemData itemData)
    {
        if (itemData.itemType == ItemType.None)
        {
            return;
        }
        ClassType itemClass = BaseUtils.itemDict[itemData.itemType].classType;
        if (!fromEquip && (fromForge || itemClass == classType || itemClass == ClassType.None) && Database.HasEquippedType(itemClass, BaseUtils.itemDict[itemData.itemType].equipType, out int databaseIndex))
        {
            if (Database.databaseStruct.ownedItems[databaseIndex].itemID == itemData.itemID)
            {
                mainMenuController.ShowTooltip(target, itemData);
            }
            else
            {
                mainMenuController.ShowTooltip(target, itemData, Database.databaseStruct.ownedItems[databaseIndex]);
            }
        }
        else
        {
            mainMenuController.ShowTooltip(target, itemData);
        }
    }
    public void HideTooltip()
    {
        mainMenuController.HideTooltip();
    }
}

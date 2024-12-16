using UnityEngine;
using UnityEngine.UI;
public class ItemCell : MonoBehaviour
{
    public BackpackController backpackController;
    public ItemCellData itemCellData;
    public ItemData itemData;
    public Image itemImage;
    public Image itemFrameImage;
    public bool equipSlot;
    public int forgeIndex;
    public int displayIndex;
    public RarityType rarity;
    public EquipType equipType;
    public ClassType classType;
    private DragItem dragItem;
    public bool emptyItem;
    public bool blockedItem;

    public void SetData(InventoryController inventoryController, ItemCellData itemCellData)
    {
        this.itemCellData = itemCellData;
        if (dragItem == null)
        {
            dragItem = GetComponent<DragItem>();
        }
        emptyItem = itemCellData.itemID == -1;
        displayIndex = itemCellData.displayIndex;
        if (emptyItem)
        {
            itemFrameImage.sprite = backpackController.itemBorders[4];
            itemImage.sprite = backpackController.emptyItem;
            equipType = EquipType.Empty;
            return;
        }
        itemData = Database.databaseStruct.ownedItems[itemCellData.databaseIndex];
        ScriptableItem scriptableItem = BaseUtils.itemDict[itemData.itemType];
        rarity = scriptableItem.rarityType;
        blockedItem = itemData.price != 0 || scriptableItem.classType != inventoryController.classType;
        if (itemData.equipClass != ClassType.None || blockedItem)
        {
            if (itemData.price != 0)
            {
                itemFrameImage.sprite = BaseUtils.emptyBorders[1];
            }
            else if (itemData.equipClass != ClassType.None)
            {
                itemFrameImage.sprite = BaseUtils.emptyBorders[2];
            }
            else
            {
                itemFrameImage.sprite = BaseUtils.emptyBorders[0];
            }
            itemImage.color = BaseUtils.transparentColor;
        }
        else
        {
            itemFrameImage.sprite = backpackController.itemBorders[(int)BaseUtils.itemDict[itemData.itemType].rarityType];
            itemImage.color = Color.white;
        }
        itemImage.sprite = BaseUtils.itemDict[itemData.itemType].itemSprite;
        equipType = scriptableItem.equipType;
        classType = scriptableItem.classType;
    }
    public void SetForgeItem(ItemData itemData)
    {
        this.itemData = itemData;
        if (itemData.itemType == ItemType.None)
        {
            itemFrameImage.sprite = backpackController.itemBorders[4];
            itemImage.sprite = backpackController.emptyItem;
            emptyItem = true;
            return;
        }
        emptyItem = false;
        itemFrameImage.sprite = backpackController.itemBorders[(int)BaseUtils.itemDict[itemData.itemType].rarityType];
        itemImage.sprite = BaseUtils.itemDict[itemData.itemType].itemSprite;
    }
    public void SetForgeItem(ForgeController forgeController, ItemCellData itemCellData)
    {
        this.itemCellData = itemCellData;
        if (dragItem == null)
        {
            dragItem = GetComponent<DragItem>();
        }
        emptyItem = itemCellData.itemID == -1 || forgeController.slotCells[0].itemData.itemID == itemCellData.itemID || forgeController.slotCells[1].itemData.itemID == itemCellData.itemID;
        displayIndex = itemCellData.displayIndex;
        if (emptyItem)
        {
            itemFrameImage.sprite = backpackController.itemBorders[4];
            itemImage.sprite = backpackController.emptyItem;
            equipType = EquipType.Empty;
            return;
        }
        itemData = Database.databaseStruct.ownedItems[itemCellData.databaseIndex];
        ScriptableItem scriptableItem = BaseUtils.itemDict[itemData.itemType];
        rarity = scriptableItem.rarityType;
        if (itemData.equipClass != ClassType.None || itemData.price != 0)
        {
            if (itemData.price != 0)
            {
                itemFrameImage.sprite = BaseUtils.emptyBorders[1];
            }
            else if (itemData.equipClass != ClassType.None)
            {
                itemFrameImage.sprite = BaseUtils.emptyBorders[2];
            }
            else
            {
                itemFrameImage.sprite = BaseUtils.emptyBorders[0];
            }
            itemImage.color = BaseUtils.transparentColor;
        }
        else
        {
            itemFrameImage.sprite = backpackController.itemBorders[(int)BaseUtils.itemDict[itemData.itemType].rarityType];
            itemImage.color = Color.white;
        }
        itemImage.sprite = BaseUtils.itemDict[itemData.itemType].itemSprite;
        equipType = scriptableItem.equipType;
        classType = scriptableItem.classType;
    }
    public void SetEquipItem(ItemData itemData, EquipType equipType)
    {
        this.itemData = itemData;
        if (itemData.itemType == ItemType.None)
        {
            itemFrameImage.sprite = backpackController.itemBorders[4];
            itemImage.sprite = backpackController.baseEquipSprites[(int)equipType];
            return;
        }
        itemFrameImage.sprite = backpackController.itemBorders[(int)BaseUtils.itemDict[itemData.itemType].rarityType];
        itemImage.sprite = BaseUtils.itemDict[itemData.itemType].itemSprite;
    }
}

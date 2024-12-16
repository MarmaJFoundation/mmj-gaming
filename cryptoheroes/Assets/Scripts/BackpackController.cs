using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BackpackController : MonoBehaviour
{
    //private readonly List<ItemCellData> baseData = new List<ItemCellData>();
    //private readonly List<ItemCellData> elementData = new List<ItemCellData>();
    public InventoryController inventoryController;
    public MainMenuController mainMenuController;
    public NearHelper nearHelper;
    public Sprite[] itemBorders;
    public Sprite[] baseEquipSprites;
    public Sprite emptyItem;
    public Sprite equippedItem;
    public CustomText pageCounter;
    public PageButton[] pageButtons;
    public Image[] pageButtonImages;
    public Image[] pageButtonArrows;
    public RectTransform[] cellPages;
    public ItemCell[] itemCells;
    public CustomInput sellInput;
    public GameObject sellingWindow;
    public EventSystem eventSystem;
    public Image sellWindowItemBorder;
    public Image sellWindowItemImage;
    private int sellingPrice;
    private ItemData trashingItem;
    private bool isTrashing;
    private ItemData sellingItem;
    private bool isSelling;
    private int currentPage;
    private int hoveringButton;
    private float buttonTimer;
    public void Setup(bool keepPage)
    {
        SetInnerPage(keepPage ? currentPage : 0);
        if (Database.itemsOnSale.Count > 0)
        {
            StartCoroutine(ShowSoldItems());
        }
    }
    private IEnumerator ShowSoldItems()
    {
        yield return new WaitForSeconds(.15f);
        for (int i = 0; i < Database.itemsOnSale.Count; i++)
        {
            ItemData itemData = Database.GetSaleItem(Database.itemsOnSale[i]);
            Database.AddTransaction(itemData, false);
            Database.RemoveSale(itemData.itemID);
            BaseUtils.ShowWarningMessage("Item Sold!", new string[2] { $"You have sucessfully sold {itemData.itemName} for {itemData.price} @", "You can find more info on the market." }, itemData);
            while (BaseUtils.showingWarn)
            {
                yield return null;
            }
        }
        Database.itemsOnSale.Clear();
        Database.SaveDatabase();
    }
    public void SetPage(bool goingLeft)
    {
        SetInnerPage(goingLeft ? 0 : 1);
    }
    private void LateUpdate()
    {
        if (hoveringButton != 0)
        {
            buttonTimer += Time.deltaTime;
            if (buttonTimer > .5f)
            {
                SetPage(hoveringButton == 1);
                buttonTimer = 0;
                hoveringButton = 0;
            }
        }
        else
        {
            buttonTimer = 0;
        }
        if (isTrashing && !Input.GetMouseButton(0))
        {
            isTrashing = false;
            if (trashingItem.price != 0)
            {
                BaseUtils.ShowWarningMessage("Error", new string[1] { "You cannot trash items that are currently being sold" });
            }
            else if (trashingItem.equipClass != ClassType.None)
            {
                BaseUtils.ShowWarningMessage("Error", new string[1] { "You cannot trash items that are currently equipped"});
            }
            else
            {
                BaseUtils.ShowWarningMessage("Wait!", new string[3] { "Are you sure you want to trash", $"{trashingItem.itemName}?", "You cannot revert this action!" }, trashingItem, OnAcceptTrashItem);
            }
        }
        if (isSelling && !Input.GetMouseButton(0))
        {
            isSelling = false;
            if (sellingItem.price != 0)
            {
                BaseUtils.ShowWarningMessage("Stop Selling", new string[2] { $"Do you wish to stop selling {sellingItem.itemName}?", "It will be removed from the market." }, sellingItem, OnAcceptStopSell);
            }
            else if (sellingItem.equipClass != ClassType.None)
            {
                BaseUtils.ShowWarningMessage("Error", new string[1] { "You cannot sell items that are currently equipped" });
            }
            else
            {
                sellingWindow.SetActive(true);
                sellInput.ResetInput();
                sellInput.OnPointerClick(null);
                eventSystem.SetSelectedGameObject(sellInput.gameObject);
                ScriptableItem scriptableItem = BaseUtils.itemDict[sellingItem.itemType];
                sellWindowItemBorder.sprite = BaseUtils.itemBorders[(int)scriptableItem.rarityType];
                sellWindowItemImage.sprite = scriptableItem.itemSprite;
                //BaseUtils.ShowWarningMessage("Wait!", new string[3] { "Are you sure you want to trash", $"{trashingItem.itemName}?", "You cannot revert this action!" }, trashingItem, OnAcceptTrashItem);
            }
        }
    }
    private void OnAcceptStopSell()
    {
        StartCoroutine(nearHelper.RequestCancelItemSell2(sellingItem.itemID));
    }
    public void OnCancelItemSell()
    {
        BaseUtils.ShowWarningMessage("Removed from Market", new string[2] { $"You have removed {sellingItem.itemName}", "that was listed in the market." }, sellingItem);
        Database.SetPrice(sellingItem.itemID, 0);
        Database.RemoveSale(sellingItem.itemID);
        SetInnerPage(currentPage);
    }
    public void OnTypeSellPrice(string input)
    {
        int.TryParse(input, out sellingPrice);
    }
    public void OnSellExitClick()
    {
        sellingWindow.SetActive(false);
    }
    public void OnSellConfirmClick()
    {
        if (sellingPrice == 0 || sellingPrice > 100000)
        {
            BaseUtils.ShowWarningMessage("Error!", new string[1] { "Cannot sell your item for this price." });
            return;
        }
        ScriptableItem scriptableItem = BaseUtils.itemDict[sellingItem.itemType];
        if (scriptableItem.rarityType == RarityType.Epic && sellingPrice < 50)
        {
            BaseUtils.ShowWarningMessage("Error!", new string[1] { "Epic items cannot be sold for less than 50 @" });
            return;
        }
        if (scriptableItem.rarityType == RarityType.Legendary && sellingPrice < 300)
        {
            BaseUtils.ShowWarningMessage("Error!", new string[1] { "Legendary items cannot be sold for less than 300 @" });
            return;
        }
        if (sellingItem.cooldown > System.DateTime.Now)
        {
            BaseUtils.ShowWarningMessage("Error!", new string[2] { "You cannot sell this item yet.", $"Please wait another {sellingItem.cooldown.ToHoursTime()} hours" });
            return;
        }
        if (BaseUtils.offlineMode)
        {
            OnAcceptItemSell();
        }
        else
        {
            StartCoroutine(nearHelper.RequestSellItem2(sellingItem.itemID, sellingPrice));
        }
    }
    public void OnAcceptItemSell()
    {
        sellingWindow.SetActive(false);
        BaseUtils.ShowWarningMessage("Selling Item!", new string[2] { $"You have put {sellingItem.itemName} on the market", $"for a total amount of {sellingPrice} @" }, sellingItem);
        Database.SetPrice(sellingItem.itemID, sellingPrice);
        Database.AddSale(sellingItem.itemID);
        sellingPrice = 0;
        SetInnerPage(currentPage);
        SoundController.PlaySound("Sell_item_var", 1, true);
    }
    public void OnTrashClick()
    {
        BaseUtils.ShowWarningMessage("Trashing Items", new string[2] { "In order to trash an item", "drag it into this button." });
    }
    public void OnSellClick()
    {
        BaseUtils.ShowWarningMessage("Sellings Items", new string[2] { "In order to sell an item", "drag it into this button." });
    }
    public void OnTrashButtonEnter()
    {
        if (inventoryController.showingPreview)
        {
            trashingItem = inventoryController.draggingItem;
            isTrashing = true;
        }
    }
    public void OnTrashButtonExit()
    {
        isTrashing = false;
    }
    public void OnSellButtonEnter()
    {
        if (inventoryController.showingPreview)
        {
            sellingItem = inventoryController.draggingItem;
            isSelling = true;
        }
    }
    public void OnSellButtonExit()
    {
        isSelling = false;
    }
    private void OnAcceptTrashItem()
    {
        if (BaseUtils.offlineMode)
        {
            OnTrashItemCallback();
        }
        else
        {
            StartCoroutine(nearHelper.RequestTrashItem(trashingItem.itemID));
        }
    }
    public void OnTrashItemCallback()
    {
        SoundController.PlaySound("Trash_item_var", 1, true);
        nearHelper.dataGetState = DataGetState.AfterTrash;
        StartCoroutine(nearHelper.GetPlayerData());
        //Database.RemoveItem(inventoryController.draggingItem.itemID);
        //inventoryController.UpdateEquipItemCells(inventoryController.classType);
        //Setup(true);
    }
    public void OnReceiveNewPlayerData()
    {
        Setup(true);
    }
    private void SetInnerPage(int pageNumber)
    {
        pageCounter.SetString(pageNumber == 0 ? "1/2" : "2/2");
        currentPage = pageNumber;
        pageButtonImages[currentPage].sprite = BaseUtils.buttonSprites[1];
        pageButtonArrows[currentPage].color = BaseUtils.disabledColor;
        pageButtons[currentPage].enabled = false;
        cellPages[currentPage].anchoredPosition = Vector2.zero;
        int counterPage = currentPage == 0 ? 1 : 0;
        pageButtonImages[counterPage].sprite = BaseUtils.buttonSprites[0];
        pageButtonArrows[counterPage].color = BaseUtils.enabledColor;
        pageButtons[counterPage].enabled = true;
        cellPages[counterPage].anchoredPosition = Vector2.right * 10000f;

        //elementData.Clear();
        //baseData.Clear();
        for (int i = 0; i < Database.maxItems; i++)
        {
            int itemID = Database.GetItemFromVisualIndex(i);
            int databaseIndex = Database.GetItemFromID(itemID);
            itemCells[i].SetData(inventoryController, new ItemCellData() { databaseIndex = databaseIndex, displayIndex = i, itemID = itemID });
        }
    }
    public void OnPageButtonEnter(bool left)
    {
        if (inventoryController.showingPreview)
        {
            hoveringButton = left ? 1 : 2;
        }
    }
    public void OnPageButtonExit()
    {
        hoveringButton = 0;
    }
}
public class ItemCellData
{
    public int databaseIndex;
    public int displayIndex;
    public int itemID;
}
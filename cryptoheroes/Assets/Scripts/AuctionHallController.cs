using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AuctionHallController : MonoBehaviour, IEnhancedScrollerDelegate
{
    private readonly SmallList<AHItemData> elementData = new SmallList<AHItemData>();
    //private readonly SmallList<AHItemData> baseData = new SmallList<AHItemData>();
    public MainMenuController mainMenuController;
    public NearHelper nearHelper;
    public ItemTooltip itemTooltip;
    public EnhancedScroller scroller;
    public EnhancedScrollerCellView cellViewPrefab;
    public Canvas auctionHallCanvas;
    public Image sortToggleButton;
    public GameObject leftWindow;
    public CustomInput sellInput;
    public GameObject sellingWindow;
    public EventSystem eventSystem;
    public Image sellWindowItemBorder;
    public Image sellWindowItemImage;
    private int sellingPrice;
    public int charLimit;
    private bool sortingUpwards;
    private string rankInput;
    private string equipInput;
    private string classInput;
    private string sortByInput;
    private string statInput;
    public CustomInput[] allInputs;
    public CustomText[] tabTexts;
    public Image[] tabButtons;
    public static bool resolutionChanged;
    private AHItemData itemToBuy;
    private readonly SmallList<AHItemData> startData = new SmallList<AHItemData>();
    private void Start()
    {
        for (int i = 0; i < 500; i++)
        {
            ItemData itemData = BaseUtils.GenerateRandomItem((RarityType)BaseUtils.RandomInt(0, 4), ItemType.None, BaseUtils.RandomInt(100, 5000));
            ScriptableItem scriptableItem = BaseUtils.itemDict[itemData.itemType];
            startData.Add(new AHItemData()
            {
                itemData = itemData,
                rarity = (int)scriptableItem.rarityType,
                auctionType = AuctionType.Normal,
                itemStatStruct = BaseUtils.GenerateItemStatStruct(itemData, scriptableItem.classType, scriptableItem.equipType)
            });
        }
    }
    public void Setup()
    {
        scroller.Delegate = this;
        for (int i = 0; i < allInputs.Length; i++)
        {
            allInputs[i].ResetInput(false, true);
        }
        rankInput = "";
        equipInput = "";
        classInput = "";
        sortByInput = "";
        statInput = "";
        sortingUpwards = false;
        resolutionChanged = false;
        auctionHallCanvas.enabled = true;
        auctionHallCanvas.gameObject.SetActive(true);
        sortToggleButton.sprite = BaseUtils.toggleSprites[sortingUpwards ? 1 : 0];
        OnTabClick(0);
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
            BaseUtils.ShowWarningMessage("Item Sold!", new string[2] { $"You have sucessfully sold {itemData.itemName} for {itemData.price} @", "You can find more info on the completed window." }, itemData);
            while (BaseUtils.showingWarn)
            {
                yield return null;
            }
        }
        Database.itemsOnSale.Clear();
        Database.SaveDatabase();
    }
    public void OnSearchClick()
    {
        if (BaseUtils.offlineMode)
        {
            elementData.Clear();
            for (int i = 0; i < startData.Count; i++)
            {
                elementData.Add(startData[i]);
            }
            SortData();
        }
        else
        {
            if (!System.Enum.TryParse(rankInput, out RarityType rarityType))
            {
                rarityType = RarityType.None;
            }
            if (!System.Enum.TryParse(classInput, out ClassType classType))
            {
                classType = ClassType.None;
            }
            if (!System.Enum.TryParse(equipInput, out EquipType equipType))
            {
                equipType = EquipType.Empty;
            }
            if (!int.TryParse(statInput, out int minStat))
            {
                minStat = 0;
            }
            StartCoroutine(nearHelper.RequestMarketData(classType, equipType, rarityType, minStat));
        }
    }
    public void OnReceiveAuctionHallItems(List<MarketWrapper> marketWrapper)
    {
        elementData.Clear();
        //Debug.Log(marketWrapper.Count);
        for (int i = 0; i < marketWrapper.Count; i++)
        {
            ItemData itemData = marketWrapper[i].item_data.ToItemData();
            ScriptableItem scriptableItem = BaseUtils.itemDict[itemData.itemType];
            elementData.Add(new AHItemData()
            {
                itemData = itemData,
                rarity = (int)scriptableItem.rarityType,
                auctionType = AuctionType.Normal,
                itemStatStruct = BaseUtils.GenerateItemStatStruct(itemData, scriptableItem.classType, scriptableItem.equipType)
            });
        }
        SortData();
    }
    private void SortData()
    {
        List<AHItemData> listToSort = new List<AHItemData>();
        /*for (int i = 0; i < baseData.Count; i++)
        {
            bool rankCheck = rankInput.Length <= 0;
            bool classCheck = classInput.Length <= 0;
            bool equipCheck = equipInput.Length <= 0;
            ScriptableItem scriptableItem = BaseUtils.itemDict[baseData[i].itemData.itemType];
            if (rankInput.Length > 0)
            {
                string rankString = scriptableItem.rarityType.ToString().ToLower();
                rankCheck = rankString.Contains(rankInput) || rankInput.Contains(rankString);
            }
            if (classInput.Length > 0)
            {
                string classString = scriptableItem.classType.ToString().ToLower();
                classCheck = classString.Contains(classInput) || classInput.Contains(classString) || classString == "none";
            }
            if (equipInput.Length > 0)
            {
                string equipString = scriptableItem.equipType.ToString().ToLower();
                equipCheck = equipString.Contains(equipInput) || equipInput.Contains(equipString);
            }
            if (rankCheck && classCheck && equipCheck)
            {
                listToSort.Add(baseData[i]);
            }
        }*/
        for (int i = 0; i < elementData.Count; i++)
        {
            listToSort.Add(elementData[i]);
        }
        if (sortByInput.Length > 0)
        {
            if ("stat".Contains(sortByInput) || sortByInput.Contains("stat"))
            {
                if (sortingUpwards)
                {
                    listToSort.Sort((x, y) => x.StatRank.CompareTo(y.StatRank));
                }
                else
                {
                    listToSort.Sort((x, y) => y.StatRank.CompareTo(x.StatRank));
                }
            }
            else if ("price".Contains(sortByInput) || sortByInput.Contains("price"))
            {
                if (sortingUpwards)
                {
                    listToSort.Sort((x, y) => x.itemData.price.CompareTo(y.itemData.price));
                }
                else
                {
                    listToSort.Sort((x, y) => y.itemData.price.CompareTo(x.itemData.price));
                }
            }
            else if ("rarity".Contains(sortByInput) || sortByInput.Contains("rarity"))
            {
                if (sortingUpwards)
                {
                    listToSort.Sort((x, y) => x.rarity.CompareTo(y.rarity));
                }
                else
                {
                    listToSort.Sort((x, y) => y.rarity.CompareTo(x.rarity));
                }
            }
        }
        elementData.Clear();
        for (int i = 0; i < listToSort.Count; i++)
        {
            elementData.Add(listToSort[i]);
        }
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
        System.GC.Collect();
        scroller.ReloadData();
    }
    public void Dispose()
    {
        BaseUtils.itemViewState = 0;
        elementData.Clear();
        scroller.ReloadData();
        if (!auctionHallCanvas.enabled)
        {
            return;
        }
        auctionHallCanvas.enabled = false;
        auctionHallCanvas.gameObject.SetActive(false);
    }
    public void OnTypeRankInput(string inputText)
    {
        if (rankInput == inputText)
        {
            return;
        }
        rankInput = inputText;
        //SortData();
    }
    public void OnTypeEquipInput(string inputText)
    {
        if (equipInput == inputText)
        {
            return;
        }
        equipInput = inputText;
        //SortData();
    }
    public void OnTypeClassInput(string inputText)
    {
        if (classInput == inputText)
        {
            return;
        }
        classInput = inputText;
        //SortData();
    }
    public void OnTypeStatInput(string inputText)
    {
        if (statInput == inputText)
        {
            return;
        }
        statInput = inputText;
        //SortData();
    }
    public void OnTypeSortByInput(string inputText)
    {
        if (sortByInput == inputText)
        {
            return;
        }
        sortByInput = inputText.ToLower();
        SortData();
    }
    public void OnItemClick(AHItemData ahData)
    {
        itemToBuy = ahData;
        switch (ahData.auctionType)
        {
            case AuctionType.Normal:
                if (Database.databaseStruct.ownedItems.Count >= 60)
                {
                    BaseUtils.ShowWarningMessage("Too many items!", new string[2] { "You have too many items in your inventory.", "Reforge, sell or destroy some before buying more." });
                }
                else if (Database.databaseStruct.pixelTokens < ahData.itemData.price)
                {
                    BaseUtils.ShowWarningMessage("Insuffient tokens", new string[2] { "You lack the necessary pixel tokens to buy this item", "would you like to trade more @ ?" }, BaseUtils.OnAcceptTradeToken);
                }
                else
                {
                    BaseUtils.ShowWarningMessage("Buying Item", new string[2] { $"Do you wish to buy {itemToBuy.itemData.itemName}", $"for a total amount of {ahData.itemData.price} @ ?" }, itemToBuy.itemData, OnAcceptItemBuy);
                }
                break;
            case AuctionType.Sell:
                sellingWindow.SetActive(true);
                sellInput.ResetInput();
                sellInput.OnPointerClick(null);
                eventSystem.SetSelectedGameObject(sellInput.gameObject);
                ScriptableItem scriptableItem = BaseUtils.itemDict[itemToBuy.itemData.itemType];
                sellWindowItemBorder.sprite = BaseUtils.itemBorders[(int)scriptableItem.rarityType];
                sellWindowItemImage.sprite = scriptableItem.itemSprite;
                break;
            case AuctionType.Auction:
                BaseUtils.ShowWarningMessage("Stop Selling", new string[2] { $"Do you wish to stop selling {itemToBuy.itemData.itemName}?", "It will be removed from the market." }, itemToBuy.itemData, OnAcceptStopSell);
                break;
            case AuctionType.Transaction:
                if (ahData.itemData.price < 0)
                {
                    BaseUtils.ShowWarningMessage("Buy Info", new string[2] { $"You bought {itemToBuy.itemData.itemName} on the market", $"for a total of {ahData.itemData.price * -1} @" }, itemToBuy.itemData);
                }
                else
                {
                    BaseUtils.ShowWarningMessage("Sell Info", new string[2] { $"You sold {itemToBuy.itemData.itemName} on the market", $"for a total of {ahData.itemData.price} @" }, itemToBuy.itemData);
                }
                break;
        }
    }
    private void OnAcceptStopSell()
    {
        StartCoroutine(nearHelper.RequestCancelItemSell(itemToBuy.itemData.itemID));
    }
    public void OnCancelItemSell()
    {
        BaseUtils.ShowWarningMessage("Removed from Market", new string[2] { $"You have removed {itemToBuy.itemData.itemName}", "that was listed in the market." }, itemToBuy.itemData);
        Database.SetPrice(itemToBuy.itemData.itemID, 0);
        Database.RemoveSale(itemToBuy.itemData.itemID);
        SetCurrentOffers();
    }
    private void OnAcceptItemBuy()
    {
        if (BaseUtils.offlineMode)
        {
            OnBuyNewItem();
        }
        else
        {
            StartCoroutine(nearHelper.RequestBuyItem(itemToBuy.itemData.itemID));
        }
    }
    public void ShowBuyAuthorize()
    {
        mainMenuController.authWindow.SetActive(true);
        nearHelper.dataGetState = DataGetState.MarketPurchase;
    }
    public void OnAuthorizedClick()
    {
        mainMenuController.authWindow.SetActive(false);
        StartCoroutine(nearHelper.GetPlayerData());
    }
    public void OnReceiveNewPlayerData()
    {
        BaseUtils.HideLoading();
        int itemIndex = -1;
        for (int i = 0; i < Database.databaseStruct.ownedItems.Count; i++)
        {
            if (Database.databaseStruct.ownedItems[i].itemID == itemToBuy.itemData.itemID)
            {
                itemIndex = i;
                break;
            }
        }
        if (itemIndex != -1)
        {
            OnBuyNewItem();
        }
        else
        {
            BaseUtils.ShowWarningMessage($"Error on purchase", new string[2] { $"The purchase for the item {itemToBuy.itemData.itemName} failed.", $"Please try again!" });
        }
        Database.SaveDatabase();
    }
    private void OnBuyNewItem()
    {
        //Database.databaseStruct.pixelTokens -= itemToBuy.itemData.price;
        //BaseUtils.ShowWarningMessage("Bought Item!", new string[2] { $"You have bought {itemToBuy.itemData.itemName}", $"for a total amount of {itemToBuy.itemData.price} @" }, itemToBuy.itemData);
        StartCoroutine(ShowItemBought());
        Database.AddTransaction(itemToBuy.itemData, true);
        //Database.AddItem(itemToBuy.itemData);
        //Database.SetPrice(itemToBuy.itemData.itemID, 0);
    }
    private IEnumerator ShowItemBought()
    {
        mainMenuController.SetupBlackScreen();
        yield return new WaitForSeconds(1 / 4f);
        itemTooltip.Setup(itemToBuy.itemData, "Bought item!", true);
        while (itemTooltip.gameObject.activeSelf)
        {
            yield return null;
        }
        mainMenuController.RemoveBlackScreen();
        OnSearchClick();
    }
    public void OnSellWindowCloseClick()
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
        ScriptableItem scriptableItem = BaseUtils.itemDict[itemToBuy.itemData.itemType];
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
        if (itemToBuy.itemData.cooldown > System.DateTime.Now)
        {
            BaseUtils.ShowWarningMessage("Error!", new string[2] { "You cannot sell this item yet.", $"Please wait another {itemToBuy.itemData.cooldown.ToHoursTime()} hours" });
            return;
        }
        if (BaseUtils.offlineMode)
        {
            OnAcceptItemSell();
        }
        else
        {
            StartCoroutine(nearHelper.RequestSellItem(itemToBuy.itemData.itemID, sellingPrice));
        }
    }
    public void OnAcceptItemSell()
    {
        sellingWindow.SetActive(false);
        BaseUtils.ShowWarningMessage("Selling Item!", new string[2] { $"You have put {itemToBuy.itemData.itemName} on the market", $"for a total amount of {sellingPrice} @" }, itemToBuy.itemData);
        Database.SetPrice(itemToBuy.itemData.itemID, sellingPrice);
        Database.AddSale(itemToBuy.itemData.itemID);
        sellingPrice = 0;
        ShowItemsForSell();
        SoundController.PlaySound("Sell_item_var", 1, true);
    }
    public void OnTypeSellPrice(string input)
    {
        int.TryParse(input, out sellingPrice);
    }
    public void OnTabClick(int windowIndex)
    {
        for (int i = 0; i < 4; i++)
        {
            if (i == windowIndex)
            {
                //tabWindows[i].SetActive(true);
                tabButtons[i].color = Color.white;
                tabTexts[i].SetString(BaseUtils.enabledColor);
                tabButtons[i].rectTransform.SetAsLastSibling();
            }
            else
            {
                //tabWindows[i].SetActive(false);
                tabButtons[i].color = BaseUtils.offColor;
                tabTexts[i].SetString(BaseUtils.textOffColor);
                tabButtons[i].rectTransform.SetAsFirstSibling();
            }
        }
        leftWindow.SetActive(windowIndex == 0);
        BaseUtils.itemViewState = 0;
        switch (windowIndex)
        {
            case 0:
                BaseUtils.itemViewState = 2;
                elementData.Clear();
                scroller.ReloadData();
                break;
            case 1:
                ShowItemsForSell();
                break;
            case 2:
                SetCurrentOffers();
                break;
            case 3:
                BaseUtils.itemViewState = 1;
                SetTransactions();
                break;
        }
    }

    private void ShowItemsForSell()
    {
        elementData.Clear();
        for (int i = 0; i < Database.databaseStruct.ownedItems.Count; i++)
        {
            if (Database.databaseStruct.ownedItems[i].equipClass != ClassType.None || Database.databaseStruct.ownedItems[i].price != 0)
            {
                continue;
            }
            ScriptableItem scriptableItem = BaseUtils.itemDict[Database.databaseStruct.ownedItems[i].itemType];
            elementData.Add(new AHItemData()
            {
                itemData = Database.databaseStruct.ownedItems[i],
                rarity = (int)scriptableItem.rarityType,
                auctionType = AuctionType.Sell,
                itemStatStruct = BaseUtils.GenerateItemStatStruct(Database.databaseStruct.ownedItems[i], scriptableItem.classType, scriptableItem.equipType)
            });
        }
        scroller.ReloadData();
    }

    private void SetCurrentOffers()
    {
        elementData.Clear();
        for (int i = 0; i < Database.databaseStruct.ownedItems.Count; i++)
        {
            if (Database.databaseStruct.ownedItems[i].equipClass != ClassType.None || Database.databaseStruct.ownedItems[i].price == 0)
            {
                continue;
            }
            ScriptableItem scriptableItem = BaseUtils.itemDict[Database.databaseStruct.ownedItems[i].itemType];
            elementData.Add(new AHItemData()
            {
                itemData = Database.databaseStruct.ownedItems[i],
                rarity = (int)scriptableItem.rarityType,
                auctionType = AuctionType.Auction,
                itemStatStruct = BaseUtils.GenerateItemStatStruct(Database.databaseStruct.ownedItems[i], scriptableItem.classType, scriptableItem.equipType)
            });
        }
        scroller.ReloadData();
    }

    private void SetTransactions()
    {
        elementData.Clear();
        for (int i = 0; i < Database.databaseStruct.pastTransactions.Count; i++)
        {
            ScriptableItem scriptableItem = BaseUtils.itemDict[Database.databaseStruct.pastTransactions[i].itemType];
            elementData.Add(new AHItemData()
            {
                itemData = Database.databaseStruct.pastTransactions[i],
                rarity = (int)scriptableItem.rarityType,
                auctionType = AuctionType.Transaction,
                itemStatStruct = BaseUtils.GenerateItemStatStruct(Database.databaseStruct.pastTransactions[i], scriptableItem.classType, scriptableItem.equipType)
            });
        }
        scroller.ReloadData();
    }

    public void OnToggleSortClick()
    {
        sortingUpwards = !sortingUpwards;
        sortToggleButton.sprite = BaseUtils.toggleSprites[sortingUpwards ? 1 : 0];
        SortData();
    }
    /*private void Update()
    {
        if (resolutionChanged)
        {
            scroller.ReloadData();
            resolutionChanged = false;
        }
    }*/
    #region EnhancedScroller Callbacks
    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return elementData.Count;
    }
    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 32f;
    }
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        AuctionHallCell cellView = scroller.GetCellView(cellViewPrefab) as AuctionHallCell;
        //cellView.name = "rankCell";
        cellView.SetData(this, dataIndex, elementData[dataIndex]);
        return cellView;
    }
    #endregion
}
public enum AuctionType
{
    Normal = 0,
    Sell = 1,
    Auction = 2,
    Transaction = 3
}
public class AHItemData
{
    public ItemData itemData;
    public StatStruct itemStatStruct;
    public AuctionType auctionType;
    public int rarity;

    public float StatRank
    {
        get
        {
            return itemStatStruct.damage * 2 + itemStatStruct.defense * 200 + itemStatStruct.dodge * 200 + itemStatStruct.lifeSteal * 200 + itemStatStruct.critChance * 200 + itemStatStruct.maxHealth / 2;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PresaleController : MonoBehaviour
{
    public MainMenuController mainMenuController;
    public NearHelper nearHelper;
    public ItemTooltip itemTooltip;
    public GameObject chestTabWindow;
    public GameObject mainWindow;
    public GameObject overlayMainChest;
    public GameObject titleObj;
    public GameObject title2Obj;
    public GameObject exitObj;
    public CustomText topText;
    public Image mainChestImage;
    public Image tabChestImage;
    public Image tabBackgroundImage;
    public Image mainBackgroundImage;
    public GameObject[] droppedObjs;
    public ItemCell[] droppedItems;
    public Image[] droppedBackgrounds;
    public Sprite[] mainChestSprites;
    public Sprite[] tabChestSprites;

    private float shakeDelay;
    private float shakeTimer;
    private bool animating;
    private bool firstSelect;
    private int selectedItem1;
    private int selectedItem2;
    private RarityType chestRarity;
    //private readonly List<ItemData> chestItems = new List<ItemData>();
    private readonly List<ItemData> lootedItems = new List<ItemData>();
    public void Setup()
    {
        animating = false;
        if (Database.databaseStruct.presaleChests.Count > 0)
        {
            chestTabWindow.SetActive(true);
            tabChestImage.sprite = tabChestSprites[(int)Database.databaseStruct.presaleChests[0]];
            tabChestImage.material = BaseUtils.outlineUI;
            tabChestImage.SetNativeSize();
            tabBackgroundImage.material = BaseUtils.rarityMaterials[(int)Database.databaseStruct.presaleChests[0]];
        }
        else
        {
            chestTabWindow.SetActive(false);
        }
    }
    private void Update()
    {
        if (animating)
        {
            return;
        }
        if (shakeDelay < 3)
        {
            shakeDelay += Time.deltaTime;
            return;
        }
        Vector3 goPos = BaseUtils.RandomBool() ? Vector3.right * 10 : Vector3.left * 10;
        if (shakeTimer <= 1)
        {
            if (!mainWindow.activeSelf)
            {
                tabChestImage.rectTransform.anchoredPosition = Vector3.Lerp(Vector3.up * 2, Vector3.up * 2 + goPos, shakeTimer.Evaluate(CurveType.ShakeCurve));
            }
            else
            {
                mainChestImage.rectTransform.anchoredPosition = Vector3.Lerp(Vector3.up * 18, Vector3.up * 18 + goPos, shakeTimer.Evaluate(CurveType.ShakeCurve));
            }
            shakeTimer += Time.deltaTime * 4;
        }
        else
        {
            if (!mainWindow.activeSelf)
            {
                tabChestImage.rectTransform.anchoredPosition = Vector3.up * 2;
            }
            else
            {
                mainChestImage.rectTransform.anchoredPosition = Vector3.up * 18;
            }
            shakeTimer = 0;
            shakeDelay = 0;
        }
    }
    public void OnTabChestClick()
    {
        mainMenuController.OnBackToMainScreenClick();
        mainWindow.SetActive(true);
        overlayMainChest.SetActive(true);
        mainChestImage.sprite = mainChestSprites[(int)Database.databaseStruct.presaleChests[0]];
        mainChestImage.material = BaseUtils.outlineUI;
        mainChestImage.SetNativeSize();
        mainBackgroundImage.material = BaseUtils.rarityMaterials[(int)Database.databaseStruct.presaleChests[0]];
        tabChestImage.rectTransform.anchoredPosition = Vector3.up * 2;
        mainChestImage.rectTransform.anchoredPosition = Vector3.up * 18;
        BaseUtils.InstantiateEffect((EffectType)(55 + (int)Database.databaseStruct.presaleChests[0]), mainChestImage.rectTransform.position, true);
        for (int i = 0; i < 10; i++)
        {
            droppedObjs[i].SetActive(false);
        }
    }
    public void OnOpenClick()
    {
        if (Database.databaseStruct.ownedItems.Count > 58)
        {
            BaseUtils.ShowWarningMessage("Inventory full!", new string[2] { "Your inventory is full.", "Try selling, forging or trashing some items." });
            return;
        }
        if (BaseUtils.offlineMode)
        {
            /*List<ItemData> droppedItems = new List<ItemData>();
            //ItemData startItem = BaseUtils.GenerateRandomItem(Database.databaseStruct.presaleChests[0]);
            //droppedItems.Add(startItem);
            for (int i = 0; i < 10; i++)
            {
                droppedItems.Add(BaseUtils.GenerateRandomItem(Database.databaseStruct.presaleChests[0]));
            }
            OnReceivePresaleChestItems(Database.databaseStruct.presaleChests[0], droppedItems);*/
        }
        else
        {
            chestRarity = Database.databaseStruct.presaleChests[0];
            StartCoroutine(nearHelper.RequestLootbox(Database.databaseStruct.presaleChests[0]));
            //RequestPresaleChestItems
        }
    }
    public void OnReceivePresaleChestItems(List<ItemToken> boxItems)
    {
        lootedItems.Clear();
        for (int i = 0; i < boxItems.Count; i++)
        {
            lootedItems.Add(boxItems[i].ToItemData());
        }
        firstSelect = false;
        animating = true;
        topText.SetString("Select one of the items");
        StartCoroutine(OpenChestCoroutine());
    }
    private IEnumerator OpenChestCoroutine()
    {
        //lootedItems.Clear();
        //lootedItems.AddRange(chestItems);
        BaseUtils.InstantiateEffect((EffectType)(55 + (int)chestRarity), mainChestImage.rectTransform.position, true);
        float timer = 0;
        Vector3 fromPos = mainChestImage.rectTransform.localPosition;
        while (timer <= 1)
        {
            mainChestImage.rectTransform.localPosition = Vector3.Lerp(fromPos, fromPos + Vector3.right * 10, timer.Evaluate(CurveType.ChestOpenCurve));
            mainChestImage.rectTransform.localScale = Vector3.Lerp(Vector3.one, new Vector3(1.2f, 0.8f, 1), timer.Evaluate(CurveType.ChestEaseInCurve));
            timer += Time.deltaTime * 1.5f;
            yield return null;
        }
        BaseUtils.InstantiateEffect((EffectType)(55 + (int)chestRarity), mainChestImage.rectTransform.position, true);
        fromPos = mainChestImage.rectTransform.localPosition;
        timer = 0;
        while (timer <= 1)
        {
            float yOffset = Mathf.Lerp(0, 10, timer.Evaluate(CurveType.JumpCurve));
            mainChestImage.rectTransform.localPosition = Vector3.Lerp(fromPos, fromPos + Vector3.up * yOffset, timer);
            mainChestImage.rectTransform.localScale = Vector3.Lerp(new Vector3(1.2f, 0.8f, 1), new Vector3(0.7f, 1.5f, 1), timer.Evaluate(CurveType.EaseOut));
            timer += Time.deltaTime * 7f;
            yield return null;
        }
        SoundController.PlaySound("Chest_open");
        BaseUtils.InstantiateEffect((EffectType)(8 + (int)chestRarity), mainChestImage.rectTransform.position, true);
        timer = 0;
        while (timer <= 1)
        {
            mainChestImage.rectTransform.localScale = Vector3.Lerp(new Vector3(0.7f, 1.5f, 1), Vector3.one, timer.Evaluate(CurveType.EaseOut));
            timer += Time.deltaTime * 7f;
            yield return null;
        }
        mainChestImage.rectTransform.localPosition = fromPos;
        mainChestImage.rectTransform.localScale = Vector3.one;
        for (int i = 0; i < Database.databaseStruct.presaleChests.Count; i++)
        {
            if (Database.databaseStruct.presaleChests[i] == chestRarity)
            {
                Database.databaseStruct.presaleChests.RemoveAt(i);
                break;
            }
        }
        titleObj.SetActive(true);
        title2Obj.SetActive(true);
        exitObj.SetActive(false);
        overlayMainChest.SetActive(false);
        for (int i = 0; i < 10; i++)
        {
            droppedBackgrounds[i].material = BaseUtils.rarityMaterials[(int)chestRarity];
            droppedObjs[i].SetActive(true);
            droppedItems[i].SetForgeItem(lootedItems[i]);
        }
    }
    public void OnExitClick()
    {
        mainWindow.SetActive(false);
        Setup();
    }
    public void OnLootItemClick(int index)
    {
        if (firstSelect)
        {
            selectedItem2 = index;
        }
        else
        {
            selectedItem1 = index;
        }
        BaseUtils.ShowWarningMessage(lootedItems[index].itemName, new string[2] { "This is the item you have selected", "are you sure of this decision?" }, lootedItems[index], OnAcceptChestItem);
    }
    private void OnAcceptChestItem()
    {
        StartCoroutine(ShowItemAndWait());
    }
    public void OnReceiveNewPlayerData()
    {
        if (Database.databaseStruct.presaleChests.Count > 0)
        {
            Setup();
            OnTabChestClick();
        }
        else
        {
            OnExitClick();
        }
    }
    private IEnumerator ShowItemAndWait()
    {
        mainMenuController.SetupBlackScreen();
        if (firstSelect)
        {
            itemTooltip.Setup(lootedItems[selectedItem2], "Looted item!", true);
            //Database.AddItem(lootedItems[selectedItem2]);
        }
        else
        {
            itemTooltip.Setup(lootedItems[selectedItem1], "Looted item!", true);
            //Database.AddItem(lootedItems[selectedItem1]);
        }
        for (int i = 0; i < 10; i++)
        {
            droppedObjs[i].SetActive(false);
        }
        titleObj.SetActive(false);
        title2Obj.SetActive(false);
        yield return new WaitForSeconds(1 / 3f);
        while (itemTooltip.gameObject.activeSelf)
        {
            yield return null;
        }
        mainMenuController.RemoveBlackScreen();
        exitObj.SetActive(true);
        if (!firstSelect)
        {
            for (int i = 0; i < 10; i++)
            {
                if (i == selectedItem1)
                {
                    droppedObjs[i].SetActive(false);
                    continue;
                }
                droppedBackgrounds[i].material = BaseUtils.rarityMaterials[(int)chestRarity];
                droppedObjs[i].SetActive(true);
                droppedItems[i].SetForgeItem(lootedItems[i]);
            }
            titleObj.SetActive(true);
            title2Obj.SetActive(true);
            topText.SetString("Select one more item!");
            firstSelect = true;
        }
        else
        {
            StartCoroutine(nearHelper.RequestItemLootbox(chestRarity, selectedItem1, selectedItem2));
        }
    }
}

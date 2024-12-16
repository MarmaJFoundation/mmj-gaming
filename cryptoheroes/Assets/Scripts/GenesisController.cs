using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenesisController : MonoBehaviour
{
    public MainMenuController mainMenuController;
    public NearHelper nearHelper;
    public ItemTooltip itemTooltip;
    public GameObject genesisTab;
    public GameObject genesisMain;
    public GameObject generateButton;
    public RectTransform timerBar;
    public CustomText timerText;
    public CustomText mainTimerText;

    private List<ItemToken> itemsGenerated;
    private float interval;
    private float timeScale;
    public void Setup()
    {
        if (Database.databaseStruct.genesisAmount > 0)
        {
            genesisTab.SetActive(true);
            interval = 0;
        }
    }
    private void Update()
    {
        if (interval > 0)
        {
            interval -= Time.deltaTime;
            return;
        }
        interval = 1;
        if (genesisTab.activeSelf || genesisMain.activeSelf)
        {
            timeScale = Database.databaseStruct.genesisTime.ToFloatTime() / DateTime.Now.AddDays(30).ToFloatTime();
        }
        if (genesisTab.activeSelf)
        {
            if (timeScale <= 0)
            {
                timerBar.transform.localScale = Vector3.one;
                timerText.SetString($"titan ready");
            }
            else
            {
                timerBar.transform.localScale = new Vector3(1 - timeScale, 1, 1);
                timerText.SetString($"titan: {Database.databaseStruct.genesisTime.ToDaysTime()}");
            }
        }
        if (genesisMain.activeSelf)
        {
            if (timeScale <= 0)
            {
                mainTimerText.SetString($"generation is ready!");
                generateButton.SetActive(true);
            }
            else
            {
                mainTimerText.SetString($"time left: {Database.databaseStruct.genesisTime.ToDetailedDaysTime()}");
                generateButton.SetActive(false);
            }
        }
    }
    public void OnGenerateClick()
    {
        if (Database.databaseStruct.ownedItems.Count > 60 - Database.databaseStruct.genesisAmount)
        {
            BaseUtils.ShowWarningMessage("Inventory full!", new string[2] { "Your inventory is full.", "Try selling, forging or trashing some items." });
            return;
        }
        if (BaseUtils.offlineMode)
        {
            /*RarityType rarityType = RarityType.Rare;
            if (BaseUtils.RandomInt(0, 100) > 98)
            {
                rarityType = RarityType.Legendary;
            }
            else if (BaseUtils.RandomInt(0, 100) > 30)
            {
                rarityType = RarityType.Epic;
            }
            OnReceiveGenesisItem(BaseUtils.GenerateRandomItem(rarityType));*/
        }
        else
        {
            StartCoroutine(nearHelper.RequestGenesis());
        }
    }
    /*public void OnReceiveGenesisItem(ItemData itemData)
    {
        StartCoroutine(ShowItemAndWait(itemData));
    }*/
    private IEnumerator ShowItemAndWait()
    {
        OnExitClick();
        mainMenuController.SetupBlackScreen(.75f);
        for (int i = 0; i < itemsGenerated.Count; i++)
        {
            ItemData itemData = itemsGenerated[i].ToItemData();
            itemTooltip.Setup(itemData, "Looted item!", true);
            if (BaseUtils.offlineMode)
            {
                Database.databaseStruct.genesisTime = DateTime.Now.AddDays(30);
                Database.AddItem(itemData);
            }
            yield return new WaitForSeconds(1 / 3f);
            while (itemTooltip.gameObject.activeSelf)
            {
                yield return null;
            }
            yield return new WaitForSeconds(.15f);
        }
        mainMenuController.RemoveBlackScreen();
    }
    public void OnGenesisCallback(GenesisWrapper genesisWrapper)
    {
        itemsGenerated = genesisWrapper.item_tokens;
        nearHelper.dataGetState = DataGetState.AfterGenesis;
        StartCoroutine(nearHelper.GetPlayerData());
    }
    public void OnReceiveNewPlayerData()
    {
        StartCoroutine(ShowItemAndWait());
    }
    public void OnTabClick()
    {
        mainMenuController.OnBackToMainScreenClick();
        genesisMain.SetActive(true);
        interval = 0;
    }
    public void OnExitClick()
    {
        genesisMain.SetActive(false);
    }
}

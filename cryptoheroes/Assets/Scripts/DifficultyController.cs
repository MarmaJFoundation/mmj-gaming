using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyController : MonoBehaviour
{
    public InventoryController inventoryController;
    public CameraController cameraController;
    public DungeonController dungeonController;
    public GameController gameController;
    public MainMenuController mainMenuController;
    public NearHelper nearHelper;
    public GameObject difficultyWindow;
    public GameObject refillButton;
    public GameObject confirmButton;
    public CustomText difficultyTitle;
    public CustomText[] commonTexts;
    public CustomText[] rareTexts;
    public CustomText[] epicTexts;
    public CustomText[] legendaryTexts;
    public CustomText[] restTexts;
    public CustomText[] expTexts;
    public CustomText[] recTexts;
    public GameObject[] difficultyBorders;
    public CustomButton[] difficultyButtons;
    public CharacterInfoController[] classesInfo;
    public Transform[] classesTransform;
    public Color[] faintRarityColors;
    private int selectedDifficulty;
    private int classIndex;
    private int previousFightPoints;
    private ClassType classType;
    public void Setup(ClassType classType)
    {
        if (difficultyWindow.activeSelf)
        {
            Dispose();
            return;
        }
        this.classType = classType;
        classIndex = (int)classType - 1;
        selectedDifficulty = 0;
        difficultyWindow.SetActive(true);
        mainMenuController.buttonsWindow.SetActive(false);
        for (int i = 0; i < 4; i++)
        {
            difficultyButtons[i].SetDeactivated(true);
        }
        for (int i = 0; i < 3; i++)
        {
            classesInfo[i].gameObject.SetActive(i == classIndex);
            classesInfo[i].onInventory = (i == classIndex);
            if (i == classIndex)
            {
                classesInfo[i].MoveParts();
            }
        }
        UpdateFightBalance();
        cameraController.MoveCamera(classesTransform[classIndex].transform.position + cameraController.inventoryOffset + Vector3.back * 10);
        cameraController.ZoomCamera(4.6f);
        OnDifficultyClick(0);
    }

    private void UpdateFightBalance()
    {
        if (Database.databaseStruct.fightBalance <= 0)
        {
            refillButton.SetActive(true);
            confirmButton.SetActive(false);
        }
        else
        {
            refillButton.SetActive(false);
            confirmButton.SetActive(true);
        }
    }

    public void Dispose(bool intoWindow = false)
    {
        difficultyBorders[selectedDifficulty].SetActive(false);
        difficultyButtons[selectedDifficulty].SetDeactivated(true);
        difficultyWindow.SetActive(false);
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
    }
    public void OnDifficultyClick(int difficulty)
    {
        difficultyBorders[selectedDifficulty].SetActive(false);
        difficultyButtons[selectedDifficulty].SetDeactivated(true);
        selectedDifficulty = difficulty;
        difficultyBorders[difficulty].SetActive(true);
        difficultyButtons[difficulty].SetDeactivated(false);
        int restRemoval = Database.HasStaminaPotion(classType) ? Database.GetStaminaPotion(classType).strength : 0;
        int potionLuck = Database.HasLuckPotion(classType) ? Database.GetLuckPotion(classType).strength : 0;
        float rareChance;
        float epicChance;
        float legChance;
        switch (difficulty)
        {
            case 0:
                difficultyTitle.SetString("Dungeon level: Easy");
                commonTexts[0].SetString("Chance of common", BaseUtils.rarityColors[0]);
                commonTexts[1].SetString("item drops: 100%", BaseUtils.rarityColors[0]);
                rareTexts[0].SetString("Chance of rare", faintRarityColors[1]);
                rareTexts[1].SetString("item drops: 0%", faintRarityColors[1]);
                epicTexts[0].SetString("Chance of epic", faintRarityColors[2]);
                epicTexts[1].SetString("item drops: 0%", faintRarityColors[2]);
                legendaryTexts[0].SetString("Chance of legendary", faintRarityColors[3]);
                legendaryTexts[1].SetString("item drops: 0%", faintRarityColors[3]);
                restTexts[1].SetString($"{30 - restRemoval} minutes");
                expTexts[0].SetString("10 exp on win");
                recTexts[0].SetString("minimum level of 1");
                recTexts[1].SetString("good common gear");
                break;
            case 1:
                rareChance = Mathf.Round(10f.ToDropChance(potionLuck) * 100) / 100;
                difficultyTitle.SetString("Dungeon level: Medium");
                commonTexts[0].SetString("Chance of common", BaseUtils.rarityColors[0]);
                commonTexts[1].SetString($"item drops: {100 - rareChance}%", BaseUtils.rarityColors[0]);
                rareTexts[0].SetString("Chance of rare", BaseUtils.rarityColors[1]);
                rareTexts[1].SetString($"item drops: {rareChance}%", BaseUtils.rarityColors[1]);
                epicTexts[0].SetString("Chance of epic", faintRarityColors[2]);
                epicTexts[1].SetString("item drops: 0%", faintRarityColors[2]);
                legendaryTexts[0].SetString("Chance of legendary", faintRarityColors[3]);
                legendaryTexts[1].SetString("item drops: 0%", faintRarityColors[3]);
                if (restRemoval > 0)
                {
                    restTexts[1].SetString($"{60 - restRemoval} minutes");
                }
                else
                {
                    restTexts[1].SetString("1 hour");
                }
                expTexts[0].SetString("20 exp on win");
                recTexts[0].SetString("minimum level of 20");
                recTexts[1].SetString("good rare gear");
                break;
            case 2:
                rareChance = Mathf.Round(15f.ToDropChance(potionLuck) * 100) / 100;
                epicChance = Mathf.Round(1f.ToDropChance(potionLuck) * 100) / 100;
                difficultyTitle.SetString("Dungeon level: Hard");
                commonTexts[0].SetString("Chance of common", BaseUtils.rarityColors[0]);
                commonTexts[1].SetString($"item drops: {100 - rareChance - epicChance}%", BaseUtils.rarityColors[0]);
                rareTexts[0].SetString("Chance of rare", BaseUtils.rarityColors[1]);
                rareTexts[1].SetString($"item drops: {rareChance}%", BaseUtils.rarityColors[1]);
                epicTexts[0].SetString("Chance of epic", BaseUtils.rarityColors[2]);
                epicTexts[1].SetString($"item drops: {epicChance}%", BaseUtils.rarityColors[2]);
                legendaryTexts[0].SetString("Chance of legendary", faintRarityColors[3]);
                legendaryTexts[1].SetString("item drops: 0%", faintRarityColors[3]);
                if (restRemoval > 0)
                {
                    restTexts[1].SetString($"3 hours and {60 - restRemoval} minutes");
                }
                else
                {
                    restTexts[1].SetString("4 hours");
                }
                expTexts[0].SetString("80 exp on win");
                recTexts[0].SetString("minimum level of 40");
                recTexts[1].SetString("good epic gear");
                break;
            case 3:
                rareChance = Mathf.Round(25f.ToDropChance(potionLuck) * 100) / 100;
                epicChance = Mathf.Round(5f.ToDropChance(potionLuck) * 100) / 100;
                legChance = Mathf.Round(1f.ToDropChance(potionLuck) * 100) / 100;
                difficultyTitle.SetString("Dungeon level: Hell");
                commonTexts[0].SetString("Chance of common", BaseUtils.rarityColors[0]);
                commonTexts[1].SetString($"item drops: {100 - rareChance - epicChance - legChance}%", BaseUtils.rarityColors[0]);
                rareTexts[0].SetString("Chance of rare", BaseUtils.rarityColors[1]);
                rareTexts[1].SetString($"item drops: {rareChance}%", BaseUtils.rarityColors[1]);
                epicTexts[0].SetString("Chance of epic", BaseUtils.rarityColors[2]);
                epicTexts[1].SetString($"item drops: {epicChance}%", BaseUtils.rarityColors[2]);
                legendaryTexts[0].SetString("Chance of legendary", BaseUtils.rarityColors[3]);
                legendaryTexts[1].SetString($"item drops: {legChance}%", BaseUtils.rarityColors[3]);
                if (restRemoval > 0)
                {
                    restTexts[1].SetString($"11 hours and {60 - restRemoval} minutes");
                }
                else
                {
                    restTexts[1].SetString("12 hours");
                }
                expTexts[0].SetString("240 exp on win");
                recTexts[0].SetString("minimum level of 70");
                recTexts[1].SetString("good legendary gear");
                break;
        }
    }
    public void OnRefillClick()
    {
        mainMenuController.HideTextTooltip();
        previousFightPoints = Database.databaseStruct.fightBalance;
        StartCoroutine(nearHelper.RequestFightRefill());
    }
    public void ShowRefillAuthorize()
    {
        mainMenuController.authWindow.SetActive(true);
        nearHelper.dataGetState = DataGetState.RefillPurchase;
    }
    public void OnAuthorizedClick()
    {
        mainMenuController.authWindow.SetActive(false);
        StartCoroutine(nearHelper.GetPlayerData());
    }
    public void OnReceiveNewPlayerData()
    {
        BaseUtils.HideLoading();
        if (previousFightPoints < Database.databaseStruct.fightBalance)
        {
            BaseUtils.ShowWarningMessage("Dungeon keys added!", new string[2] { "Your transaction was successful", "you have received 100 dungeon keys!" });
            UpdateFightBalance();
        }
        else
        {
            BaseUtils.ShowWarningMessage("Error on purchase", new string[2] { "The purchase for dungeon keys has failed", "please try again!" });
        }
    }
    public void OnConfirmClick()
    {
        if (Database.databaseStruct.ownedItems.Count >= 60)
        {
            BaseUtils.ShowWarningMessage("Too many items!", new string[2] { "You have too many items in your inventory.", "Reforge, sell or destroy some before going dungeon crawling." });
            return;
        }
        BaseUtils.onFight = true;
        GameCharController gameCharController = gameController.gameChars[classIndex];
        gameCharController.SetCharStruct();
        mainMenuController.HideTextTooltip();
        Dispose(true);
        cameraController.MoveCamera(Vector3.back * 10);
        cameraController.ZoomCamera(6f);
        for (int i = 0; i < 3; i++)
        {
            classesInfo[i].onInventory = false;
            if (i == classIndex)
            {
                classesInfo[i].MoveParts();
            }
        }
        switch (selectedDifficulty)
        {
            case 0:
                SoundController.PlaySound("Door_open", 1, true);
                break;
            case 1:
                SoundController.PlaySound("Door_open", 1, true);
                break;
            case 2:
                SoundController.PlaySound("Door_open_var", 1, true);
                break;
            case 3:
                SoundController.PlaySound("Door_open_hell", 1, true);
                break;
        }
        if (BaseUtils.offlineMode)
        {
            StartCoroutine(SimulateWaitDungeon(gameCharController));

        }
        else
        {
            StartCoroutine(nearHelper.RequestDungeonFight(selectedDifficulty, classType, Database.GetEquippedItemDatas(classType)));
        }
    }
    private IEnumerator SimulateWaitDungeon(GameCharController gameCharController)
    {
        BaseUtils.ShowLoading();
        yield return new WaitForSeconds(1);
        BaseUtils.HideLoading();
        OnReceiveDungeonStruct(dungeonController.GenerateFightStruct(selectedDifficulty, gameCharController.statStruct));
    }
    public void OnReceiveDungeonStruct(DungeonFightStruct dungeonFightStruct)
    {
        dungeonController.Setup(selectedDifficulty, classType, dungeonFightStruct);
    }
}

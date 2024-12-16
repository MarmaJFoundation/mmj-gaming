using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public struct RoomData
{
    public int killCount;
    public List<string> playerNames;
    public List<ClassType> playerClasses;
    public List<int> playerLevels;
    public List<int> playerRanks;
    public List<List<ItemType>> playerEquippedItems;
    public List<StatStruct> playerStatStructs;
    public List<DateTime> playerTimes;

    public RoomData(int killCount, List<string> playerNames, List<ClassType> playerClasses, List<int> playerLevels, List<int> playerRanks, List<StatStruct> playerStatStructs, List<List<ItemType>> playerEquippedItems, List<DateTime> playerTimes)
    {
        this.killCount = killCount;
        this.playerNames = playerNames;
        this.playerClasses = playerClasses;
        this.playerLevels = playerLevels;
        this.playerRanks = playerRanks;
        this.playerStatStructs = playerStatStructs;
        this.playerEquippedItems = playerEquippedItems;
        this.playerTimes = playerTimes;
    }
}
public class RaidController : MonoBehaviour, IEnhancedScrollerDelegate
{
    private readonly SmallList<RaidData> elementData = new SmallList<RaidData>();
    public Canvas raidCanvas;
    public BossFightController bossFightController;
    public MainMenuController mainMenuController;
    public NearHelper nearHelper;
    public EnhancedScroller scroller;
    public EnhancedScrollerCellView cellViewPrefab;
    public CustomText roomText;
    public CustomText rewardText;
    public Image backgroundImage;

    public GameObject[] bossBorders;
    public CustomButton[] bossButtons;
    public GameObject[] roomObjs;
    public Image[] roomClassBorders;
    public CustomText[] roomNameText;
    public CustomText[] roomLevelTexts;
    public CustomText[] roomLevelStatlvl;

    public GameObject raidClassWindow;
    public GameObject windowObj;
    public CustomText windowTitle;
    public CustomText buttonText;
    public CustomText[] windowTexts;
    public GameObject inputObj;
    public CustomInput roomInput;
    public CustomButton[] classButtons;
    public GameObject[] classOutlines;
    public Image[] classButtonImages;
    public CustomButton[] classButtons2;
    public GameObject[] classOutlines2;
    public Image[] classButtonImages2;
    public GameCharController[] gameCharControllers;
    public GameObject newRoomButton;
    public GameObject joinRoomButton;
    public GameObject raidButton;
    public GameObject refillButton;
    public GameObject kickButton;
    public GameObject kickWindow;
    public CustomInput kickInput;
    public EventSystem eventSystem;
    [HideInInspector]
    public ClassType selectedClass;
    public readonly RoomData[] roomDatas = new RoomData[4];
    public int selectedDifficulty;
    private int previousFightPoints;
    private bool creatingRoom;
    private bool hasRoom;
    private readonly bool[] hasClasses = new bool[3];
    //[HideInInspector]
    public int totalKillAmount;
    public void Setup()
    {
        hasRoom = false;
        scroller.Delegate = this;
        raidCanvas.enabled = true;
        raidCanvas.gameObject.SetActive(true);
        for (int i = 0; i < 4; i++)
        {
            bossButtons[i].SetDeactivated(true);
            roomDatas[i] = new RoomData();
        }
        for (int i = 0; i < 3; i++)
        {
            hasClasses[i] = Database.HasClass((ClassType)i + 1);
        }
        if (!BaseUtils.offlineMode)
        {
            StartCoroutine(nearHelper.RequestRoomData());
        }
        else
        {
            OnReceiveRoomData(null);
        }
        rewardText.SetString("");
    }
    public void OnKickClick()
    {
        kickWindow.SetActive(true);
        kickInput.ResetInput();
        kickInput.OnPointerClick(null);
        eventSystem.SetSelectedGameObject(kickInput.gameObject);
    }
    public void OnKickExitClick()
    {
        kickWindow.SetActive(false);
    }
    public void OnKickMemberExecute()
    {
        string kickWalletAdress = kickInput.typeString.ToLower();
        if (kickWalletAdress.Contains(".near"))
        {
            kickWalletAdress = kickWalletAdress.Remove(kickWalletAdress.IndexOf('.'), 5);
        }
        if (Database.databaseStruct.playerAccount.Contains(kickWalletAdress))
        {
            BaseUtils.ShowWarningMessage("Error kicking player", new string[1] { "You cannot kick yourself."});
            return;
        }
        for (int i = 0; i < roomDatas[selectedDifficulty].playerNames.Count; i++)
        {
            if (roomDatas[selectedDifficulty].playerNames[i].Contains(kickWalletAdress))
            {
                if (roomDatas[selectedDifficulty].playerTimes[i].ToFloatTime() / 3600f % 24f > 20)
                {
                    BaseUtils.ShowWarningMessage("Error kicking player", new string[2] { "You cannot kick this player!", "He has been in the party for longer than 20 hours." });
                    return;
                }
                break;
            }
        }
        StartCoroutine(nearHelper.RequestKickMember(kickWalletAdress + nearHelper.WalletSuffix));
    }
    public void KickMemberCallback(RoomWrapper roomWrapper)
    {
        kickWindow.SetActive(false);
        JoinRoom(roomWrapper);
    }
    public void OnReceiveRoomData(RoomWrapper roomWrapper)
    {
        if (roomWrapper == null)
        {
            hasRoom = false;
            UpdateButtons();
            SetRaidWindow(0);
            UpdateRoomInfo();
        }
        else
        {
            JoinRoom(roomWrapper);
        }
    }
    public void OnRefillClick()
    {
        previousFightPoints = Database.databaseStruct.fightBalance;
        StartCoroutine(nearHelper.RequestRaidRefill());
    }
    public void ShowRefillAuthorize()
    {
        mainMenuController.authWindow.SetActive(true);
        nearHelper.dataGetState = DataGetState.RaidPurchase;
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
        }
        else
        {
            BaseUtils.ShowWarningMessage("Error on purchase", new string[2] { "The purchase for dungeon keys has failed", "please try again!" });
        }
        UpdateButtons();
    }
    private void UpdateButtons()
    {
        if (!hasRoom)
        {
            newRoomButton.SetActive(Database.databaseStruct.fightBalance > 0);
            joinRoomButton.SetActive(Database.databaseStruct.fightBalance > 0);
            raidButton.SetActive(false);
            refillButton.SetActive(Database.databaseStruct.fightBalance <= 0);
            kickButton.SetActive(false);
        }
        else
        {
            newRoomButton.SetActive(false);
            joinRoomButton.SetActive(false);
            raidButton.SetActive(Database.databaseStruct.fightBalance > 0);
            refillButton.SetActive(Database.databaseStruct.fightBalance <= 0);
            kickButton.SetActive(roomDatas[selectedDifficulty].playerNames[0] == Database.databaseStruct.playerAccount);
        }
    }
    public void JoinRoom(RoomWrapper roomWrapper)
    {
        hasRoom = true;
        for (int i = 0; i < roomWrapper.playerNames.Count; i++)
        {
            if (roomWrapper.playerNames[i] == Database.databaseStruct.playerAccount)
            {
                selectedClass = (ClassType)roomWrapper.playerClasses[i];
                break;
            }
        }
        SetRaidWindow(roomWrapper.difficulty);
        AddNewRoomInfo(roomWrapper.ToRoomData(), roomWrapper.difficulty);
        UpdateButtons();
    }
    public void AddNewRoomInfo(RoomData roomData, int difficulty)
    {
        roomDatas[difficulty] = roomData;
        UpdateRoomInfo();
    }
    private void UpdateRoomInfo()
    {
        for (int i = 0; i < roomObjs.Length; i++)
        {
            roomObjs[i].SetActive(false);
        }
        if (roomDatas[selectedDifficulty].playerNames == null)
        {
            roomText.SetString("Not in a room.");
            return;
        }
        for (int i = 0; i < roomDatas[selectedDifficulty].playerLevels.Count; i++)
        {
            roomObjs[i].SetActive(true);
            switch (roomDatas[selectedDifficulty].playerClasses[i])
            {
                case ClassType.Mage:
                    roomClassBorders[i].sprite = BaseUtils.mageSprites[roomDatas[selectedDifficulty].playerRanks[i]];
                    break;
                case ClassType.Knight:
                    roomClassBorders[i].sprite = BaseUtils.knightSprites[roomDatas[selectedDifficulty].playerRanks[i]];
                    break;
                case ClassType.Ranger:
                    roomClassBorders[i].sprite = BaseUtils.rangerSprites[roomDatas[selectedDifficulty].playerRanks[i]];
                    break;
            }
            roomNameText[i].SetString(roomDatas[selectedDifficulty].playerNames[i]);
            roomLevelTexts[i].SetString($"level: {roomDatas[selectedDifficulty].playerLevels[i]}");
            roomLevelStatlvl[i].SetString($"stat lvl: {roomDatas[selectedDifficulty].playerStatStructs[i].ToStatRank() + roomDatas[selectedDifficulty].playerLevels[i] * 100}");
        }
        if (roomDatas[selectedDifficulty].playerLevels.Count == 8)
        {
            roomText.SetString($"8/8 ready!");
        }
        else
        {
            roomText.SetString($"{roomDatas[selectedDifficulty].playerLevels.Count}/8 not ready.");
        }
    }
    public void OnClassButtonClick(int classIndex)
    {
        if (!hasClasses[classIndex - 1])
        {
            BaseUtils.ShowWarningMessage("Not unlocked!", new string[2] { "You cannot select this character", "unlock it first." });
            return;
        }
        if (Database.GetCharStruct((ClassType)classIndex).injuredTimer > System.DateTime.Now)
        {
            BaseUtils.ShowWarningMessage("Character resting!", new string[2] { "You cannot select this character", "as its still resting from the last fight." });
            return;
        }
        if (selectedClass != ClassType.None)
        {
            classOutlines[(int)selectedClass - 1].SetActive(false);
        }
        selectedClass = (ClassType)classIndex;
        classOutlines[classIndex - 1].SetActive(true);
    }
    public void OnClassButtonClick2(int classIndex)
    {
        if (!hasClasses[classIndex - 1])
        {
            BaseUtils.ShowWarningMessage("Not unlocked!", new string[2] { "You cannot select this character", "unlock it first." });
            return;
        }
        if (Database.GetCharStruct((ClassType)classIndex).injuredTimer > System.DateTime.Now)
        {
            BaseUtils.ShowWarningMessage("Character resting!", new string[2] { "You cannot select this character", "as its still resting from the last fight." });
            return;
        }
        if (selectedClass != ClassType.None)
        {
            classOutlines2[(int)selectedClass - 1].SetActive(false);
        }
        selectedClass = (ClassType)classIndex;
        classOutlines2[classIndex - 1].SetActive(true);
    }
    public void OnWindowExecuteClick()
    {
        if (selectedClass == ClassType.None)
        {
            BaseUtils.ShowWarningMessage("Select a character!", new string[2] { "You cannot join or create a room", "Without selecting a character to use" });
            return;
        }
        if (creatingRoom)
        {
            if (BaseUtils.offlineMode)
            {
                AddNewRoomInfo(BaseUtils.GenerateRandomRoomInfo(selectedDifficulty, selectedClass), selectedDifficulty);
            }
            else
            {
                StartCoroutine(nearHelper.RequestCreateRoom(selectedDifficulty, selectedClass, Database.GetEquippedItemDatas(selectedClass)));
            }
        }
        else
        {
            string roomWalletAdress = roomInput.typeString.ToLower();
            if (roomWalletAdress.Contains(".near"))
            {
                roomWalletAdress = roomWalletAdress.Remove(roomWalletAdress.IndexOf('.'), 5);
            }
            StartCoroutine(nearHelper.RequestJoinRoom(selectedDifficulty, selectedClass, Database.GetEquippedItemDatas(selectedClass), roomWalletAdress + nearHelper.WalletSuffix));
        }
        classOutlines[(int)selectedClass - 1].SetActive(false);
        windowObj.SetActive(false);
    }
    public void OnMakeNewGroupClick()
    {
        windowObj.SetActive(true);
        windowTitle.SetString("Create New Group");
        windowTexts[0].SetString("To create a new group, select a character");
        windowTexts[1].SetString("And tell your friends to search for your wallet adress!");
        buttonText.SetString("Create Group");
        inputObj.SetActive(false);
        creatingRoom = true;
        SetClassSelect(classButtons, classOutlines, classButtonImages);
    }

    private void SetClassSelect(CustomButton[] classButtons, GameObject[] classOutlines, Image[] classButtonImages)
    {
        selectedClass = ClassType.None;
        for (int i = 0; i < 3; i++)
        {
            bool injured = Database.GetCharStruct((ClassType)i + 1).injuredTimer > System.DateTime.Now;
            classOutlines[i].SetActive(false);
            classButtons[i].SetDeactivated(!hasClasses[i] || injured);
        }
        if (hasClasses[0])
        {
            int mageLevel = Database.GetCharStruct(ClassType.Mage).level;
            classButtonImages[0].sprite = BaseUtils.mageSprites[BaseUtils.CalculateRank(mageLevel, BaseUtils.GenerateCharStruct(ClassType.Mage, mageLevel, Database.databaseStruct.ownedItems))];
        }
        else
        {
            classButtonImages[0].sprite = BaseUtils.mageSprites[0];
        }
        if (hasClasses[1])
        {
            int knightLevel = Database.GetCharStruct(ClassType.Knight).level;
            classButtonImages[1].sprite = BaseUtils.knightSprites[BaseUtils.CalculateRank(knightLevel, BaseUtils.GenerateCharStruct(ClassType.Knight, knightLevel, Database.databaseStruct.ownedItems))];
        }
        else
        {
            classButtonImages[1].sprite = BaseUtils.knightSprites[0];
        }
        if (hasClasses[2])
        {
            int rangerLevel = Database.GetCharStruct(ClassType.Ranger).level;
            classButtonImages[2].sprite = BaseUtils.rangerSprites[BaseUtils.CalculateRank(rangerLevel, BaseUtils.GenerateCharStruct(ClassType.Ranger, rangerLevel, Database.databaseStruct.ownedItems))];
        }
        else
        {
            classButtonImages[2].sprite = BaseUtils.rangerSprites[0];
        }
    }

    public void OnJoinExistingGroupClick()
    {
        windowObj.SetActive(true);
        windowTitle.SetString("Join Existing Group");
        windowTexts[0].SetString("To join an existing group, select a character");
        windowTexts[1].SetString("and input the group wallet adress below!");
        buttonText.SetString("Join Group");
        inputObj.SetActive(true);
        roomInput.ResetInput();
        roomInput.OnPointerClick(null);
        eventSystem.SetSelectedGameObject(roomInput.gameObject);
        creatingRoom = false;
        SetClassSelect(classButtons, classOutlines, classButtonImages);
    }
    public void OnExitWindowClick()
    {
        windowObj.SetActive(false);
    }
    public void OnExitWindowClick2()
    {
        raidClassWindow.SetActive(false);
    }
    public void OnRaidStartClick()
    {
        if (roomDatas[selectedDifficulty].playerNames == null)
        {
            BaseUtils.ShowWarningMessage("Not in a room!", new string[2] { $"You need to either join an existing room or create a new one", "remember that wallet adresses are used to invite and join rooms!" });
            return;
        }
        if (roomDatas[selectedDifficulty].playerNames.Count != 8 && !nearHelper.Testnet)
        {
            BaseUtils.ShowWarningMessage("Missing players!", new string[3] { $"you are missing { 8 - roomDatas[selectedDifficulty].playerNames.Count} players!", "try inviting more players into your group!", "remember your wallet adress is your group ID!" });
            return;
        }
        raidClassWindow.SetActive(true);
        SetClassSelect(classButtons2, classOutlines2, classButtonImages2);
    }
    public void OnRaidClassConfirm()
    {
        if (selectedClass == ClassType.None)
        {
            BaseUtils.ShowWarningMessage("Select a hero!", new string[1] { $"You must select a hero in order to raid." });
            return;
        }
        raidClassWindow.SetActive(false);
        if (BaseUtils.offlineMode)
        {
            OnRaidStartCallback(bossFightController.GenerateFightStruct(selectedDifficulty, roomDatas[selectedDifficulty]));
        }
        else
        {
            string roomWalletAdress = roomDatas[selectedDifficulty].playerNames[0];
            for (int i = 0; i < roomDatas[selectedDifficulty].playerNames.Count; i++)
            {
                if (roomDatas[selectedDifficulty].playerNames[i] == Database.databaseStruct.playerAccount)
                {
                    CharData charData = Database.GetCharStruct(selectedClass);
                    StatStruct statStruct = BaseUtils.GenerateCharStruct(selectedClass, charData.level, Database.GetEquippedItemDatas(selectedClass));
                    roomDatas[selectedDifficulty].playerClasses[i] = selectedClass;
                    roomDatas[selectedDifficulty].playerEquippedItems[i] = Database.GetEquippedItemTypes(selectedClass);
                    roomDatas[selectedDifficulty].playerLevels[i] = charData.level;
                    roomDatas[selectedDifficulty].playerStatStructs[i] = statStruct;
                    roomDatas[selectedDifficulty].playerRanks[i] = BaseUtils.CalculateRank(charData.level, statStruct);
                    break;
                }
            }
            nearHelper.StartCoroutine(nearHelper.RequestSimulateRaidFight(selectedClass, Database.GetEquippedItemDatas(selectedClass), roomWalletAdress));
        }
    }
    public void OnRaidStartCallback(BossFightStruct bossFightStruct)
    {
        bossFightController.Setup(selectedClass, selectedDifficulty, roomDatas[selectedDifficulty], bossFightStruct);
    }
    public void OnRaidBossClick(int difficulty)
    {
        SetRaidWindow(difficulty);
        if (BaseUtils.offlineMode)
        {
            for (int i = 0; i < 30; i++)
            {
                elementData.Clear();
                elementData.Add(new RaidData()
                {
                    bossKills = 300 - (i + 1) * 10,
                    position = i,
                    teamClasses = new ClassType[8] {(ClassType)BaseUtils.RandomInt(1,4), (ClassType)BaseUtils.RandomInt(1, 4) ,
                    (ClassType)BaseUtils.RandomInt(1, 4) , (ClassType)BaseUtils.RandomInt(1, 4), (ClassType)BaseUtils.RandomInt(1,4),
                    (ClassType)BaseUtils.RandomInt(1,4),(ClassType)BaseUtils.RandomInt(1,4),(ClassType)BaseUtils.RandomInt(1,4),},
                    teamNames = new string[8] {"Player"+BaseUtils.RandomInt(0, 100)+".near", "Player" + BaseUtils.RandomInt(0, 100) + ".near",
                    "Player"+BaseUtils.RandomInt(0, 100)+".near","Player"+BaseUtils.RandomInt(0, 100)+".near","Player"+BaseUtils.RandomInt(0, 100)+".near",
                    "Player"+BaseUtils.RandomInt(0, 100)+".near","Player"+BaseUtils.RandomInt(0, 100)+".near","Player"+BaseUtils.RandomInt(0, 100)+".near"},
                    difficulty = difficulty,
                    teamRanks = new int[8] {3 - Mathf.Clamp(Mathf.RoundToInt((i+1)/7f), 0, 3), 3 - Mathf.Clamp(Mathf.RoundToInt((i+1)/7f), 0, 3) , 3 - Mathf.Clamp(Mathf.RoundToInt((i+1)/7f), 0, 3),
                        3 - Mathf.Clamp(Mathf.RoundToInt((i+1)/7f), 0, 3), 3 - Mathf.Clamp(Mathf.RoundToInt((i+1)/7f), 0, 3), 3 - Mathf.Clamp(Mathf.RoundToInt((i+1)/7f), 0, 3),
                        3 - Mathf.Clamp(Mathf.RoundToInt((i+1)/7f), 0, 3), 3 - Mathf.Clamp(Mathf.RoundToInt((i+1)/7f), 0, 3)}
                });
            }
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
            scroller.ReloadData();
        }
        UpdateRoomInfo();
    }

    private void SetRaidWindow(int difficulty)
    {
        backgroundImage.material = BaseUtils.rarityMaterials[difficulty];
        bossBorders[selectedDifficulty].SetActive(false);
        bossButtons[selectedDifficulty].SetDeactivated(true);
        selectedDifficulty = difficulty;
        bossBorders[difficulty].SetActive(true);
        bossButtons[difficulty].SetDeactivated(false);
        if (!BaseUtils.offlineMode)
        {
            StartCoroutine(nearHelper.RequestRaidLeaderboardData(difficulty));
        }
        rewardText.SetString("");
        /*switch (difficulty)
        {
            case 0:
                rewardText.SetString("by end of the week, the top 30 teams will receive @ based on their groups kill count", Color.white);
                break;
            case 1:
                rewardText.SetString("by end of the week, the top 20 teams will receive @ based on their groups kill count", Color.white);
                break;
            case 2:
                rewardText.SetString("by end of the week, the top 10 teams will receive @ based on their groups kill count", Color.white);
                break;
            case 3:
                rewardText.SetString("by end of the week, the top 5 teams will receive @ based on their groups kill count", Color.white);
                break;
        }*/
    }
    public void OnReceiveLeaderboardData(List<RaidLeaderboardWrapper> raidLeaderboards)
    {
        elementData.Clear();
        int yourKillAmount = 0;
        totalKillAmount = 0;
        int threshHold = 20;
        switch (selectedDifficulty)
        {
            case 0:
                threshHold = 12;
                break;
            case 1:
                threshHold = 8;
                break;
            case 2:
                threshHold = 4;
                break;
            case 3:
                threshHold = 4;
                break;
        }
        bool hasSelfRank = false;
        if (raidLeaderboards.Count > 0)
        {
            for (int i = 0; i < raidLeaderboards[0].playerNames.Count; i++)
            {
                if (raidLeaderboards[0].playerNames[i] == Database.databaseStruct.playerAccount)
                {
                    yourKillAmount = raidLeaderboards[0].boss_kills;
                    hasSelfRank = true;
                    break;
                }
            }
            for (int i = 0; i < raidLeaderboards.Count; i++)
            {
                int listIndex;
                if (!hasSelfRank)
                {
                    listIndex = i + 1;
                }
                else
                {
                    listIndex = i == 0 ? raidLeaderboards[i].position : i;
                }
                elementData.Add(new RaidData()
                {
                    bossKills = raidLeaderboards[i].boss_kills,
                    position = listIndex,
                    teamClasses = ConvertToClassArray(raidLeaderboards[i].playerClasses),
                    teamNames = raidLeaderboards[i].playerNames.ToArray(),
                    difficulty = selectedDifficulty,
                    teamRanks = ConvertToCorrectRank(raidLeaderboards[i].playerRanks)
                });
                if ((i != 0 || !hasSelfRank) && i <= threshHold)
                {
                    totalKillAmount += raidLeaderboards[i].boss_kills;
                }
            }
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
        }
        scroller.ReloadData();
        if (!hasRoom)
        {
            rewardText.SetString($"You are not in a room to receive rewards. Join or create one.", Color.white);
        }
        else if (totalKillAmount > 0 && yourKillAmount > 0 && hasSelfRank && raidLeaderboards[0].position <= threshHold)
        {
            switch (selectedDifficulty)
            {
                case 0:
                    rewardText.SetString($"by end of the week, each member of your team will receive approximately {Mathf.RoundToInt(4000f / totalKillAmount * yourKillAmount / 8)} @", Color.white);
                    break;
                case 1:
                    rewardText.SetString($"by end of the week, each member of your team will receive approximately {Mathf.RoundToInt(4000f / totalKillAmount * yourKillAmount / 8)} @", Color.white);
                    break;
                case 2:
                    rewardText.SetString($"by end of the week, each member of your team will receive approximately {Mathf.RoundToInt(4000f / totalKillAmount * yourKillAmount / 8)} @", Color.white);
                    break;
                case 3:
                    rewardText.SetString($"by end of the week, each member of your team will receive approximately {Mathf.RoundToInt(3000f / totalKillAmount * yourKillAmount / 8)} @", Color.white);
                    break;
            }
            //rewardText.SetString($"please check discord for more info on your team rewards", Color.white);
        }
        else
        {
            rewardText.SetString($"your team is not eligible for rewards on this difficulty", BaseUtils.damageColor);
        }
    }
    private int[] ConvertToCorrectRank(List<int> ranks)
    {
        List<int> correctRanks = new List<int>();
        for (int i = 0; i < ranks.Count; i++)
        {
            correctRanks.Add(Mathf.Clamp(ranks[i] / 5000, 0, 3));
        }
        return correctRanks.ToArray();
    }
    private ClassType[] ConvertToClassArray(List<int> classes)
    {
        List<ClassType> classTypes = new List<ClassType>();
        for (int i = 0; i < classes.Count; i++)
        {
            classTypes.Add((ClassType)classes[i]);
        }
        return classTypes.ToArray();
    }
    public void Dispose()
    {
        raidCanvas.enabled = false;
        raidCanvas.gameObject.SetActive(false);
        bossBorders[selectedDifficulty].SetActive(false);
        bossButtons[selectedDifficulty].SetDeactivated(true);
    }
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
        RaidGroupCell cellView = scroller.GetCellView(cellViewPrefab) as RaidGroupCell;
        cellView.SetData(this, dataIndex, elementData[dataIndex]);
        return cellView;
    }
    #endregion
}
public class RaidData
{
    public string[] teamNames;
    public int[] teamRanks;
    public ClassType[] teamClasses;
    public int bossKills;
    public int position;
    public int difficulty;
}

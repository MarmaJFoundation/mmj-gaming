using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardController : MonoBehaviour, IEnhancedScrollerDelegate
{
    private readonly SmallList<LeaderboardData> elementData = new SmallList<LeaderboardData>();
    public EnhancedScroller scroller;
    public EnhancedScrollerCellView cellViewPrefab;
    public MainMenuController mainMenuController;
    public GameController gameController;
    public NearHelper nearHelper;
    public Image[] tabButtons;
    public CustomText[] tabTexts;
    public Canvas leaderboardCanvas;
    private int windowIndex;
    public void Setup()
    {
        scroller.Delegate = this;
        leaderboardCanvas.gameObject.SetActive(true);
        leaderboardCanvas.enabled = true;
        gameController.Dispose();
        OnTabClick(1);
    }
    public void OnTabClick(int windowIndex)
    {
        this.windowIndex = windowIndex;
        for (int i = 0; i < 4; i++)
        {
            if (i == windowIndex)
            {
                tabButtons[i].color = Color.white;
                tabTexts[i].SetString(BaseUtils.enabledColor);
                tabButtons[i].rectTransform.SetAsLastSibling();
            }
            else
            {
                tabButtons[i].color = BaseUtils.offColor;
                tabTexts[i].SetString(BaseUtils.textOffColor);
                tabButtons[i].rectTransform.SetAsFirstSibling();
            }
        }
        if (BaseUtils.offlineMode)
        {
            OnReceiveRankData(GenerateFakeWrapper(windowIndex));
        }
        else
        {
            StartCoroutine(nearHelper.RequestLeaderboardData());
        }
    }
    private LeaderboardWrapper GenerateFakeWrapper(int classType)
    {
        LeaderboardWrapper leaderboardWrapper = new LeaderboardWrapper();
        List<LeaderboardCharacter> data = new List<LeaderboardCharacter>();
        for (int i = 0; i < 50; i++)
        {
            data.Add(new LeaderboardCharacter()
            {
                account_id = "Offplayer " + i + ".near",
                char_level = 100 - ((i + 1) * 2),
                class_Type = classType == 0 ? BaseUtils.RandomInt(1, 4) : classType,
                position = i,
                inventory = new List<ItemWrapper> {
                            GenerateItemWrapper(BaseUtils.GenerateRandomItem((RarityType)3 - Mathf.Clamp(Mathf.RoundToInt((i+1)/10f), 0, 3))),
                             GenerateItemWrapper(BaseUtils.GenerateRandomItem((RarityType)3 - Mathf.Clamp(Mathf.RoundToInt((i+1)/10f), 0, 3))),
                             GenerateItemWrapper(BaseUtils.GenerateRandomItem((RarityType)3 - Mathf.Clamp(Mathf.RoundToInt((i+1)/10f), 0, 3))),
                             GenerateItemWrapper(BaseUtils.GenerateRandomItem((RarityType)3 - Mathf.Clamp(Mathf.RoundToInt((i+1)/10f), 0, 3))),
                             GenerateItemWrapper(BaseUtils.GenerateRandomItem((RarityType)3 - Mathf.Clamp(Mathf.RoundToInt((i+1)/10f), 0, 3))),
                             GenerateItemWrapper(BaseUtils.GenerateRandomItem((RarityType)3 - Mathf.Clamp(Mathf.RoundToInt((i+1)/10f), 0, 3)))}
            });
        }
        return leaderboardWrapper;
    }
    private ItemWrapper GenerateItemWrapper(ItemData itemData)
    {
        return new ItemWrapper()
        {
            dexterity = itemData.dexterity,
            endurance = itemData.endurance,
            luck = itemData.luck,
            strength = itemData.strength,
            intelligence = itemData.intelligence,
            item_type = (int)itemData.itemType,
            rarity_type = (int)BaseUtils.itemDict[itemData.itemType].rarityType,
            token_id = itemData.itemID.ToString()
        };
    }
    public void OnReceiveRankData(LeaderboardWrapper wrapperData)
    {
        elementData.Clear();
        if (windowIndex == 1)
        {
            ProcessLeaderboardDataList(wrapperData.mage, ClassType.Mage);
        }
        else if (windowIndex == 2)
        {
            ProcessLeaderboardDataList(wrapperData.knight, ClassType.Knight);
        }
        else
        {
            ProcessLeaderboardDataList(wrapperData.ranger, ClassType.Ranger);
        }
        scroller.ReloadData();
    }
    private void ProcessLeaderboardDataList(List<LeaderboardCharacter> leaderboardCharacters, ClassType classType)
    {
        if (leaderboardCharacters.Count == 0)
        {
            return;
        }
        bool hasSelfRank = leaderboardCharacters[0].account_id == Database.databaseStruct.playerAccount;
        for (int i = 0; i < leaderboardCharacters.Count; i++)
        {
            int listIndex;
            if (!hasSelfRank)
            {
                listIndex = i + 1;
            }
            else
            {
                listIndex = i == 0 ? leaderboardCharacters[i].position : i;
            }
            List<ItemData> loadout = GenerateItemDataLoadout(leaderboardCharacters[i].inventory);
            StatStruct charStruct = BaseUtils.GenerateCharStruct(classType, leaderboardCharacters[i].char_level, loadout, true);
            elementData.Add(new LeaderboardData()
            {
                rankPos = listIndex,
                selfRank = i == 0 && hasSelfRank,
                playerAccount = leaderboardCharacters[i].account_id,
                charLevel = leaderboardCharacters[i].char_level,
                charClass = classType,
                charLoadout = loadout,
                characterStruct = charStruct,
                //loadoutStructs = SetLoadoutStruct(loadout, classType)
            });
        }
    }
    private List<ItemData> GenerateItemDataLoadout(List<ItemWrapper> itemWrapper)
    {
        List<ItemData> loadout = new List<ItemData>();
        for (int i = 0; i < itemWrapper.Count; i++)
        {
            int.TryParse(itemWrapper[i].token_id, out int itemID);
            loadout.Add(new ItemData(
                System.DateTime.Now,
                itemWrapper[i].strength,
                itemWrapper[i].dexterity,
                itemWrapper[i].endurance,
                itemWrapper[i].intelligence,
                itemWrapper[i].luck,
                BaseUtils.GenerateItemName(BaseUtils.itemDict[(ItemType)itemWrapper[i].item_type].synonymString, (RarityType)itemWrapper[i].rarity_type, itemID),
                itemID,
                0,
                (ItemType)itemWrapper[i].item_type, ClassType.None));
        }
        return loadout;
    }
    /*private List<StatStruct> SetLoadoutStruct(List<ItemData> itemData, ClassType classType)
    {
        List<StatStruct> statStructs = new List<StatStruct>();
        for (int i = 0; i < itemData.Count; i++)
        {
            statStructs.Add(BaseUtils.GenerateItemStatStruct(itemData[i], classType, BaseUtils.itemDict[itemData[i].itemType].equipType));
        }
        return statStructs;
    }*/
    public void Dispose()
    {
        leaderboardCanvas.enabled = false;
        leaderboardCanvas.gameObject.SetActive(false);
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
        LeaderboardCell cellView = scroller.GetCellView(cellViewPrefab) as LeaderboardCell;
        //cellView.name = "rankCell";
        cellView.SetData(this, dataIndex, elementData[dataIndex]);
        return cellView;
    }
    #endregion
}
public class LeaderboardData
{
    public List<ItemData> charLoadout;
    //public List<StatStruct> loadoutStructs;
    public StatStruct characterStruct;
    public ClassType charClass;
    public string playerAccount;
    public int charLevel;
    public int rankPos;
    public bool selfRank;
    public int CharRank
    {
        get
        {
            return Mathf.Clamp((characterStruct.ToStatRank() + charLevel * 100) / 5000, 0, 3);
        }
    }
}

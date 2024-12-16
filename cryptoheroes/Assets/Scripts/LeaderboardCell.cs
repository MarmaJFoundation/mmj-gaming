using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
public class LeaderboardCell : EnhancedScrollerCellView
{
    public int DataIndex { get; private set; }
    [HideInInspector]
    public LeaderboardData data;
    public Image classFrame;
    public Image[] equipFrames;
    public Image[] equipImages;
    public CustomText titleText;
    public CustomText playerText;
    public CustomText levelText;
    public GameObject outlineObj;
    private LeaderboardController leaderboardController;
    public void SetData(LeaderboardController leaderboardController, int dataIndex, LeaderboardData data)
    {
        this.leaderboardController = leaderboardController;
        this.data = data;
        DataIndex = dataIndex;
        switch (data.charClass)
        {
            case ClassType.Mage:
                classFrame.sprite = BaseUtils.mageSprites[data.CharRank];
                break;
            case ClassType.Knight:
                classFrame.sprite = BaseUtils.knightSprites[data.CharRank];
                break;
            case ClassType.Ranger:
                classFrame.sprite = BaseUtils.rangerSprites[data.CharRank];
                break;
        }
        for (int i = 0; i < 6; i++)
        {
            if (i < data.charLoadout.Count)
            {
                ScriptableItem scriptableItem = BaseUtils.itemDict[data.charLoadout[i].itemType];
                equipFrames[i].sprite = BaseUtils.itemBorders[(int)scriptableItem.rarityType];
                equipImages[i].enabled = true;
                equipImages[i].sprite = scriptableItem.itemSprite;
            }
            else
            {
                equipFrames[i].sprite = BaseUtils.emptyBorders[0];
                equipImages[i].enabled = false;
            }
        }
        titleText.SetString($"{data.rankPos.ToNumberBigChar()}. {data.playerAccount}", BaseUtils.rankColors[Mathf.Clamp((data.charLevel * 100 + data.characterStruct.ToStatRank()) / 4000, 0, 5)]);
        playerText.SetString(data.playerAccount);
        levelText.SetString($"level: {data.charLevel} - stat level: {data.characterStruct.ToStatRank() + data.charLevel * 100}");
        outlineObj.SetActive(data.selfRank);
    }
}

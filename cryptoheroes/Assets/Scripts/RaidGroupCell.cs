using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaidGroupCell : EnhancedScrollerCellView
{
    public int DataIndex { get; private set; }
    [HideInInspector]
    public RaidData data;
    public Image[] teamBorders;
    public CustomTooltip[] teamTooltips;
    public CustomText titleText;
    public CustomText killsText;
    public GameObject outlineObj;
    private RaidController raidController;
    private int lastAmount;
    private int killRank;
    public void SetData(RaidController raidController, int dataIndex, RaidData data)
    {
        this.data = data;
        this.raidController = raidController;
        DataIndex = dataIndex;
        killRank = Mathf.Clamp(data.bossKills / 100, 0, 5);
        titleText.SetString($"{data.position.ToNumberBigChar()}. {data.teamNames[0]} team", BaseUtils.rankColors[killRank]);
        outlineObj.SetActive(false);
        if (dataIndex == 0)
        {
            for (int i = 0; i < data.teamNames.Length; i++)
            {
                if (data.teamNames[i] == Database.databaseStruct.playerAccount)
                {
                    outlineObj.SetActive(true);
                    break;
                }
            }
        }
        UpdateKillMessage();
        for (int i = 0; i < data.teamClasses.Length; i++)
        {
            teamImages[i].sprite = BaseUtils.knightSprites[(int)data.teamClasses[i]-1];
            teamImages[i].SetNativeSize();
            switch (data.teamClasses[i])
            {
                case ClassType.Mage:
                    teamBorders[i].sprite = BaseUtils.mageSprites[data.teamRanks[i]];
                    break;
                case ClassType.Knight:
                    teamBorders[i].sprite = BaseUtils.knightSprites[data.teamRanks[i]];
                    break;
                case ClassType.Ranger:
                    teamBorders[i].sprite = BaseUtils.rangerSprites[data.teamRanks[i]];
                    break;
            }
        }
        for (int i = 0; i < data.teamNames.Length; i++)
        {
            teamTooltips[i].tooltipText = new string[1] { data.teamNames[i] };
        }
    }

    private void UpdateKillMessage()
    {
        int threshHold = 20;
        switch (data.difficulty)
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
        if (data.bossKills == 0 || data.position > threshHold)
        {
            killsText.SetString($"not eligible for rewards", BaseUtils.damageColor);
        }
        else
        {
            switch (data.difficulty)
            {
                case 0:
                    killsText.SetString($"group kills: {data.bossKills}, member reward: {Mathf.RoundToInt(4000f / raidController.totalKillAmount * data.bossKills / 8)} @", BaseUtils.rankColors[killRank]);
                    break;
                case 1:
                    killsText.SetString($"group kills: {data.bossKills}, member reward: {Mathf.RoundToInt(4000f / raidController.totalKillAmount * data.bossKills / 8)} @", BaseUtils.rankColors[killRank]);
                    break;
                case 2:
                    killsText.SetString($"group kills: {data.bossKills}, member reward: {Mathf.RoundToInt(4000f / raidController.totalKillAmount * data.bossKills / 8)} @", BaseUtils.rankColors[killRank]);
                    break;
                case 3:
                    killsText.SetString($"group kills: {data.bossKills}, member reward: {Mathf.RoundToInt(3000f / raidController.totalKillAmount * data.bossKills / 8)} @", BaseUtils.rankColors[killRank]);
                    break;
            }
        }
        lastAmount = raidController.totalKillAmount;
    }

    private void Update()
    {
        if (lastAmount != raidController.totalKillAmount)
        {
            UpdateKillMessage();
        }
    }
}

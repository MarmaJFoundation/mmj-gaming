using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//stats of each monster
[Serializable]
public struct StatStruct
{
    public int maxHealth;
    public int damage;
    public float defense;
    public float dodge;
    public float lifeSteal;
    public float critChance;

    public StatStruct(int maxHealth, int damage, float defense, float dodge, float lifeSteal, float critChance)
    {
        this.maxHealth = maxHealth;
        this.damage = damage;
        this.defense = defense;
        this.dodge = dodge;
        this.lifeSteal = lifeSteal;
        this.critChance = critChance;
    }
}
//info of each attack
[Serializable]
public struct DungeonAttackStruct
{
    public bool attackingMonster;
    public int monsterIndex;
    public int damage;
    public int lifeSteal;
    public bool critted;
    public bool dodged;
    public bool playerDied;
    public bool monsterDied;

    public DungeonAttackStruct(bool attackingMonster, int monsterIndex, int damage, int lifeSteal, bool critted, bool dodged, bool playerDied, bool monsterDied)
    {
        this.attackingMonster = attackingMonster;
        this.monsterIndex = monsterIndex;
        this.damage = damage;
        this.lifeSteal = lifeSteal;
        this.critted = critted;
        this.dodged = dodged;
        this.playerDied = playerDied;
        this.monsterDied = monsterDied;
    }
}
//info of each round of monsters
[Serializable]
public struct DungeonRoundStruct
{
    public List<MonsterType> roundMonsterTypes;
    public List<StatStruct> roundMonsterStats;
    public List<DungeonAttackStruct> roundAttacks;

    public DungeonRoundStruct(List<MonsterType> roundMonsterTypes, List<StatStruct> roundMonsterStats, List<DungeonAttackStruct> roundAttacks)
    {
        this.roundMonsterTypes = roundMonsterTypes;
        this.roundMonsterStats = roundMonsterStats;
        this.roundAttacks = roundAttacks;
    }
}
//info of each dungeon level
[Serializable]
public struct DungeonLevelStruct
{
    public List<DungeonRoundStruct> dungeonRoundStructs;

    public DungeonLevelStruct(List<DungeonRoundStruct> dungeonRoundStructs)
    {
        this.dungeonRoundStructs = dungeonRoundStructs;
    }
}
//info of all rounds + result
[Serializable]
public struct DungeonFightStruct
{
    public bool victory;
    public DungeonItem itemDrop;
    public List<DungeonLevelStruct> levelStructs;

    public DungeonFightStruct(bool victory, List<DungeonLevelStruct> levelStructs, DungeonItem itemDrop)
    {
        this.victory = victory;
        this.levelStructs = levelStructs;
        this.itemDrop = itemDrop;
    }
}
[Serializable]
public struct DungeonItem
{
    public string token_id;
    public int item_type;
    public int rarity_type;
    public int equip_type;
    public int class_type;
    public int strength;
    public int dexterity;
    public int endurance;
    public int intelligence;
    public int luck;
}
public class DungeonController : BattleController
{
    public ItemTooltip itemTooltip;
    public Canvas dungeonCanvas;
    public GameObject dungeonGameObj;
    public GameObject mainGameObj;
    public GameObject[] chestPrefabs;
    public CustomText titleText;
    public Sprite[] shadowSprites;
    public Sprite[] chestOpenSprites;
    public GameObject[] easyDungeonVariations;
    public GameObject[] mediumDungeonVariations;
    public GameObject[] hardDungeonVariations;
    public GameObject[] hellDungeonVariations;
    public CustomButton soundButton;
    public CustomButton musicButton;
    public Color shadowColor;

    public readonly Queue<UnitController> unitPool = new Queue<UnitController>();
    public readonly List<MonsterController> activeMonsters = new List<MonsterController>();
    private readonly Vector3[] classPositions = new Vector3[3] { new Vector2(2.284f, -2.168f), new Vector2(-1.934f, -1.454f), new Vector2(6.534f, -1.977f) };

    private readonly List<Vector3[]> monsterPositions = new List<Vector3[]> {
    new Vector3[3] { new Vector2(5.5f, 0), new Vector2(4.5f, -1f), new Vector2(5.5f, -2f) },
    new Vector3[3] { new Vector2(5.5f, 0), new Vector2(4.5f, -1f), new Vector2(5.5f, -2f) },
    new Vector3[3] { new Vector2(7f, 0), new Vector2(5f, -1f), new Vector2(7f, -2f) },
    new Vector3[3] { new Vector2(8.5f, 0), new Vector2(6f, -1f), new Vector2(8.5f, -2f) }};

    private readonly Vector3[] monsterAttackPositions = new Vector3[3] { new Vector2(1, 0), new Vector2(1, -1f), new Vector2(1, -2f) };

    private readonly Vector3 heroPosition = new Vector3(-4, -1);
    private readonly List<MonsterType> easyDungeonMonsters = new List<MonsterType>() { MonsterType.Goblin1, MonsterType.Goblin2, MonsterType.Goblin1, MonsterType.Goblin2, MonsterType.Goblin3 };
    private readonly List<MonsterType> mediumDungeonMonsters = new List<MonsterType>() { MonsterType.Rat1, MonsterType.Rat2 };
    private readonly List<MonsterType> hardDungeonMonsters = new List<MonsterType>() { MonsterType.Demon1, MonsterType.Demon2 };
    private readonly List<MonsterType> hellDungeonMonsters = new List<MonsterType>() { MonsterType.Reaper1, MonsterType.Reaper2 };
    public void Setup(int difficulty, ClassType classType, DungeonFightStruct fightStruct)
    {
        this.classType = classType;
        this.difficulty = difficulty;
        timeLocked = false;
        speedWindow.SetActive(false);
        titleText.SetString("dungeon current level: 1/5");
        switch (difficulty)
        {
            case 0:
                SoundController.PlayMusic("CHDungeon1");
                break;
            case 1:
                SoundController.PlayMusic("CHDungeon2");
                break;
            case 2:
                SoundController.PlayMusic("CHDungeon3");
                break;
            case 3:
                SoundController.PlayMusic("CHDungeon4");
                break;
        }
        StartCoroutine(BattleCoroutine(fightStruct));
    }
    public void OnSoundClick()
    {
        if (Database.databaseStruct.soundVolume == 0)
        {
            Database.databaseStruct.soundVolume = 100;
        }
        else
        {
            Database.databaseStruct.soundVolume = 0;
        }
        soundButton.SetDeactivated(Database.databaseStruct.soundVolume == 100);
        Database.SaveDatabase();
    }
    public void OnMusicClick()
    {
        if (Database.databaseStruct.musicVolume == 0)
        {
            Database.databaseStruct.musicVolume = 100;
        }
        else
        {
            Database.databaseStruct.musicVolume = 0;
        }
        musicButton.SetDeactivated(Database.databaseStruct.musicVolume == 100);
        SoundController.OnChangeMusicVolume();
        Database.SaveDatabase();
    }
    public DungeonFightStruct GenerateFightStruct(int difficulty, StatStruct charStruct)
    {
        List<DungeonLevelStruct> levelStructs = new List<DungeonLevelStruct>();
        RarityType rarityType = RarityType.Common;
        float rarityChance = BaseUtils.RandomFloat(0, 100);
        int potionLuck = Database.HasLuckPotion(classType) ? Database.GetLuckPotion(classType).strength : 0;
        switch (difficulty)
        {
            case 0:
                rarityType = RarityType.Common;
                break;
            case 1:
                if (rarityChance < 10f.ToDropChance(potionLuck))
                {
                    rarityType = RarityType.Rare;
                }
                else
                {
                    rarityType = RarityType.Common;
                }
                break;
            case 2:
                if (rarityChance < 1f.ToDropChance(potionLuck))
                {
                    rarityType = RarityType.Epic;
                }
                else if (rarityChance < 15f.ToDropChance(potionLuck))
                {
                    rarityType = RarityType.Rare;
                }
                else
                {
                    rarityType = RarityType.Common;
                }
                break;
            case 3:
                if (rarityChance < 1f.ToDropChance(potionLuck))
                {
                    rarityType = RarityType.Legendary;
                }
                else if (rarityChance < 5f.ToDropChance(potionLuck))
                {
                    rarityType = RarityType.Epic;
                }
                else if (rarityChance < 25f.ToDropChance(potionLuck))
                {
                    rarityType = RarityType.Rare;
                }
                else
                {
                    rarityType = RarityType.Common;
                }
                break;
        }
        DungeonItem dropItem = ParseItemDataDungeon(BaseUtils.GenerateRandomItem(rarityType));
        int charHP = charStruct.maxHealth;
        for (int j = 0; j < 4; j++)
        {
            List<DungeonRoundStruct> roundStructs = new List<DungeonRoundStruct>();
            int monstersWaves = j == 3 ? 1 : 2;
            for (int k = 0; k < monstersWaves; k++)
            {
                List<StatStruct> roundMonstersStats = new List<StatStruct>();
                List<MonsterType> roundMonstersTypes = new List<MonsterType>();
                List<DungeonAttackStruct> roundAttacks = new List<DungeonAttackStruct>();
                List<int> monstersHP = new List<int>();
                if (j == 3)
                {
                    switch (difficulty)
                    {
                        case 0:
                            roundMonstersTypes.Add(MonsterType.GoblinBoss);
                            break;
                        case 1:
                            roundMonstersTypes.Add(MonsterType.RatBoss);
                            break;
                        case 2:
                            roundMonstersTypes.Add(MonsterType.DemonBoss);
                            break;
                        case 3:
                            roundMonstersTypes.Add(MonsterType.ReaperBoss);
                            break;
                    }
                }
                else
                {
                    switch (difficulty)
                    {
                        case 0:
                            for (int i = 0; i < 3; i++)
                            {
                                roundMonstersTypes.Add(easyDungeonMonsters[BaseUtils.RandomInt(0, easyDungeonMonsters.Count)]);
                            }
                            break;
                        case 1:
                            for (int i = 0; i < 3; i++)
                            {
                                roundMonstersTypes.Add(mediumDungeonMonsters[BaseUtils.RandomInt(0, mediumDungeonMonsters.Count)]);
                            }
                            break;
                        case 2:
                            for (int i = 0; i < 3; i++)
                            {
                                roundMonstersTypes.Add(hardDungeonMonsters[BaseUtils.RandomInt(0, hardDungeonMonsters.Count)]);
                            }
                            break;
                        case 3:
                            for (int i = 0; i < 3; i++)
                            {
                                roundMonstersTypes.Add(hellDungeonMonsters[BaseUtils.RandomInt(0, hellDungeonMonsters.Count)]);
                            }
                            break;
                    }
                }
                for (int i = 0; i < roundMonstersTypes.Count; i++)
                {
                    roundMonstersStats.Add(GenerateMonsterStats(difficulty, charStruct, roundMonstersTypes[i]));
                }
                for (int i = 0; i < roundMonstersStats.Count; i++)
                {
                    monstersHP.Add(roundMonstersStats[i].maxHealth);
                }
                int monstersAlive = roundMonstersStats.Count;
                while (monstersAlive > 0)
                {
                    int damage;
                    bool critted;
                    bool dodged;
                    int lifeSteal;
                    for (int i = 0; i < roundMonstersStats.Count; i++)
                    {
                        if (monstersHP[i] <= 0)
                        {
                            continue;
                        }
                        //monster attacks players
                        critted = false;
                        damage = 0;
                        lifeSteal = 0;
                        dodged = BaseUtils.RandomFloat(0, 100) < charStruct.dodge;
                        if (!dodged)
                        {
                            damage = Mathf.RoundToInt(BaseUtils.RandomFloat(roundMonstersStats[i].damage * .5f, roundMonstersStats[i].damage));
                            critted = BaseUtils.RandomFloat(0, 100) < roundMonstersStats[i].critChance;
                            if (critted)
                            {
                                damage *= 2;
                            }
                            damage -= Mathf.RoundToInt(damage * BaseUtils.RandomFloat(charStruct.defense * .5f, charStruct.defense * .9f) / 100);
                            lifeSteal = Mathf.RoundToInt(damage / 100f * roundMonstersStats[i].lifeSteal);
                            if ((lifeSteal + monstersHP[i]) >= roundMonstersStats[i].maxHealth)
                            {
                                lifeSteal -= (lifeSteal + monstersHP[i]) - roundMonstersStats[i].maxHealth;
                            }
                            monstersHP[i] += lifeSteal;
                            charHP -= damage;
                        }
                        roundAttacks.Add(new DungeonAttackStruct(false, i, damage, lifeSteal, critted, dodged, charHP <= 0, false));
                        //if player dies, stop everything here
                        if (charHP <= 0)
                        {
                            roundStructs.Add(new DungeonRoundStruct(roundMonstersTypes, roundMonstersStats, roundAttacks));
                            levelStructs.Add(new DungeonLevelStruct(roundStructs));
                            return new DungeonFightStruct(false, levelStructs, dropItem);
                        }
                        //player attacks monster
                        dodged = BaseUtils.RandomFloat(0, 100) < roundMonstersStats[i].dodge;
                        critted = false;
                        damage = 0;
                        lifeSteal = 0;
                        if (!dodged)
                        {
                            damage = Mathf.RoundToInt(BaseUtils.RandomFloat(charStruct.damage * .5f, charStruct.damage));
                            critted = BaseUtils.RandomFloat(0, 100) < charStruct.critChance;
                            if (critted)
                            {
                                damage *= 2;
                            }
                            damage -= Mathf.RoundToInt(damage * BaseUtils.RandomFloat(roundMonstersStats[i].defense * .5f, roundMonstersStats[i].defense * .9f) / 100);
                            lifeSteal = Mathf.RoundToInt(damage / 100f * charStruct.lifeSteal);
                            if ((lifeSteal + charHP) >= charStruct.maxHealth)
                            {
                                lifeSteal -= (lifeSteal + charHP) - charStruct.maxHealth;
                            }
                            charHP += lifeSteal;
                            monstersHP[i] -= damage;
                        }
                        roundAttacks.Add(new DungeonAttackStruct(true, i, damage, lifeSteal, critted, dodged, false, monstersHP[i] <= 0));
                        if (monstersHP[i] <= 0)
                        {
                            monstersAlive--;
                        }
                    }
                }
                roundStructs.Add(new DungeonRoundStruct(roundMonstersTypes, roundMonstersStats, roundAttacks));
            }
            levelStructs.Add(new DungeonLevelStruct(roundStructs));
        }
        return new DungeonFightStruct(true, levelStructs, dropItem);
    }
    private StatStruct GenerateMonsterStats(int difficulty, StatStruct charStruct, MonsterType monsterType)
    {
        ScriptableMonster scriptableMonster = BaseUtils.monsterDict[monsterType];
        int statSum = Mathf.RoundToInt(charStruct.critChance * 100) + Mathf.RoundToInt(charStruct.lifeSteal * 100) + Mathf.RoundToInt(charStruct.dodge * 100) + Mathf.RoundToInt(charStruct.defense * 100) + charStruct.damage + charStruct.maxHealth / 2;
        float multiplier = BaseUtils.RandomFloat(.01f, .02f) / (1 + statSum * .001f);
        int statAdd = Mathf.RoundToInt(Mathf.Clamp(statSum - 250, 1, Mathf.Infinity) * multiplier);
        float statMultiplier = 1 + (statSum / 6f * multiplier) + (difficulty * .1f);
        int dexterity = Mathf.RoundToInt((BaseUtils.RandomInt(scriptableMonster.dexRange) + statAdd) * statMultiplier);
        int strength = Mathf.RoundToInt((BaseUtils.RandomInt(scriptableMonster.strRange) + statAdd) * statMultiplier);
        int intelligence = Mathf.RoundToInt((BaseUtils.RandomInt(scriptableMonster.intRange) + statAdd) * statMultiplier);
        int endurance = Mathf.RoundToInt((BaseUtils.RandomInt(scriptableMonster.endRange) + statAdd) * statMultiplier);
        int luck = Mathf.RoundToInt((BaseUtils.RandomInt(scriptableMonster.lckRange) + statAdd) * statMultiplier);
        return BaseUtils.GenerateMonsterStatStruct(scriptableMonster.classType, dexterity, strength, intelligence, endurance, luck);
    }
    private IEnumerator BattleCoroutine(DungeonFightStruct fightStruct)
    {
        for (int j = 0; j < fightStruct.levelStructs.Count; j++)
        {
            mainMenuController.FadeInBlack();
            yield return new WaitForSeconds(.25f);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            yield return new WaitForEndOfFrame();
            mainMenuController.mainCanvas.enabled = false;
            mainMenuController.mainCanvas.gameObject.SetActive(false);
            dungeonCanvas.enabled = true;
            dungeonCanvas.gameObject.SetActive(true);
            float goZoom = 6.6f;
            Vector3 goCamPos = Vector3.back * 10;
            switch (difficulty)
            {
                case 0:
                    for (int i = 0; i < easyDungeonVariations.Length; i++)
                    {
                        easyDungeonVariations[i].SetActive(false);
                    }
                    easyDungeonVariations[BaseUtils.RandomInt(0, easyDungeonVariations.Length)].SetActive(true);
                    break;
                case 1:
                    for (int i = 0; i < mediumDungeonVariations.Length; i++)
                    {
                        mediumDungeonVariations[i].SetActive(false);
                    }
                    mediumDungeonVariations[BaseUtils.RandomInt(0, mediumDungeonVariations.Length)].SetActive(true);
                    break;
                case 2:
                    for (int i = 0; i < hardDungeonVariations.Length; i++)
                    {
                        hardDungeonVariations[i].SetActive(false);
                    }
                    hardDungeonVariations[BaseUtils.RandomInt(0, hardDungeonVariations.Length)].SetActive(true);
                    goCamPos = Vector3.back * 10f + Vector3.up;
                    goZoom = 7f;
                    break;
                case 3:
                    for (int i = 0; i < hellDungeonVariations.Length; i++)
                    {
                        hellDungeonVariations[i].SetActive(false);
                    }
                    hellDungeonVariations[BaseUtils.RandomInt(0, hellDungeonVariations.Length)].SetActive(true);
                    goCamPos = Vector3.back * 10f + Vector3.up * 2;
                    goZoom = 7f;
                    break;
            }
            if (j == 0)
            {
                cameraController.ZoomCamera(goZoom);
                cameraController.MoveCamera(goCamPos);
                mainGameObj.SetActive(false);
                dungeonGameObj.SetActive(true);
                CharController.transform.SetParent(dungeonGameObj.transform);
                CharController.shadowSprite.sprite = shadowSprites[0];
                CharController.shadowSprite.transform.localPosition = new Vector3(0, 0);
                CharController.Setup(mainMenuController, classType.ToString(), 4);
                soundButton.SetDeactivated(Database.databaseStruct.soundVolume == 100);
                musicButton.SetDeactivated(Database.databaseStruct.musicVolume == 100);
            }
            else
            {
                CharController.StopAllCoroutines();
            }
            CharController.transform.position = heroPosition + Vector3.left * 4;
            CharController.pivotTransform.localScale = new Vector3(-1, 1, 1);
            gameController.FixHpBars();
            mainMenuController.FadeOutBlack();
            yield return new WaitForSeconds(.25f);
            StartCoroutine(WalkCoroutine(CharController, heroPosition));
            if (j == 0)
            {
                int goTimeScale = Database.databaseStruct.lockSpeed == -1 ? 1 : Database.databaseStruct.lockSpeed;
                float timer = 0;
                while (timer <= 1)
                {
                    Time.timeScale = Mathf.Lerp(0, 1, timer.Evaluate(CurveType.EaseIn));
                    timer += Time.unscaledDeltaTime * 2;
                    yield return null;
                }
                Time.timeScale = goTimeScale;
                speedWindow.SetActive(true);
                SetTimerProperties();
                lockButtonImage.sprite = Database.databaseStruct.lockSpeed == -1 ? lockSprites[0] : lockSprites[1];
            }
            yield return new WaitForSeconds(.5f);
            CharController.StartCoroutine(IdleCoroutine(CharController.pivotTransform));
            yield return new WaitForSeconds(.5f);
            bool charApproached = false;
            List<DungeonRoundStruct> dungeonRoundStructs = fightStruct.levelStructs[j].dungeonRoundStructs;
            for (int k = 0; k < dungeonRoundStructs.Count; k++)
            {
                ClearMonsters();
                for (int i = 0; i < dungeonRoundStructs[k].roundMonsterStats.Count; i++)
                {
                    AddMonster(dungeonRoundStructs[k].roundMonsterTypes[i], dungeonRoundStructs[k].roundMonsterStats[i], difficulty, dungeonRoundStructs[k].roundMonsterStats.Count == 1 ? 1 : i);
                }
                for (int i = 0; i < activeMonsters.Count; i++)
                {
                    StartCoroutine(WalkCoroutine(activeMonsters[i], monsterPositions[difficulty][activeMonsters[i].index]));
                    StartCoroutine(FadeInCoroutine(activeMonsters[i].monsterSprite, Color.white));
                    StartCoroutine(FadeInCoroutine(activeMonsters[i].shadowSprite, shadowColor));
                    yield return new WaitForSeconds(.25f);
                    activeMonsters[i].StartCoroutine(IdleCoroutine(activeMonsters[i].pivotTransform));
                }
                yield return new WaitForSeconds(.5f);
                for (int i = 0; i < dungeonRoundStructs[k].roundAttacks.Count; i++)
                {
                    DungeonAttackStruct dungeonAttackStruct = dungeonRoundStructs[k].roundAttacks[i];
                    int monsterIndex = dungeonAttackStruct.monsterIndex;
                    if (!dungeonAttackStruct.attackingMonster)
                    {
                        //activeMonsters[monsterIndex].StopAllCoroutines();
                        StartCoroutine(WalkCoroutine(activeMonsters[monsterIndex], monsterAttackPositions[activeMonsters[monsterIndex].index], 10f));
                        if (!charApproached)
                        {
                            yield return new WaitForSeconds(.5f);
                            charApproached = true;
                            //charController.StopAllCoroutines();
                            StartCoroutine(WalkCoroutine(CharController, heroPosition + Vector3.right * 3));
                            yield return new WaitForSeconds(.5f);
                        }
                        yield return new WaitForSeconds(.5f);
                        switch (activeMonsters[monsterIndex].scriptableMonster.classType)
                        {
                            case ClassType.Mage:
                                StartCoroutine(MagicAttackCoroutine(dungeonAttackStruct.critted, dungeonAttackStruct.dodged, dungeonAttackStruct.damage, dungeonAttackStruct.lifeSteal, activeMonsters[monsterIndex], CharController));
                                yield return new WaitForSeconds(.25f);
                                break;
                            case ClassType.Knight:
                                StartCoroutine(MeleeAttackCoroutine(dungeonAttackStruct.critted, dungeonAttackStruct.dodged, dungeonAttackStruct.damage, dungeonAttackStruct.lifeSteal, activeMonsters[monsterIndex], CharController));
                                break;
                            case ClassType.Ranger:
                                StartCoroutine(DexAttackCoroutine(dungeonAttackStruct.critted, dungeonAttackStruct.dodged, dungeonAttackStruct.damage, dungeonAttackStruct.lifeSteal, activeMonsters[monsterIndex], CharController));
                                break;
                        }
                        yield return new WaitForSeconds(.25f);
                        if (dungeonAttackStruct.playerDied)
                        {
                            StartCoroutine(DefeatCoroutine());
                            yield break;
                        }
                    }
                    else
                    {
                        switch (CharController.classType)
                        {
                            case ClassType.Mage:
                                StartCoroutine(MagicAttackCoroutine(dungeonAttackStruct.critted, dungeonAttackStruct.dodged, dungeonAttackStruct.damage, dungeonAttackStruct.lifeSteal, CharController, activeMonsters[monsterIndex]));
                                yield return new WaitForSeconds(.25f);
                                break;
                            case ClassType.Knight:
                                StartCoroutine(MeleeAttackCoroutine(dungeonAttackStruct.critted, dungeonAttackStruct.dodged, dungeonAttackStruct.damage, dungeonAttackStruct.lifeSteal, CharController, activeMonsters[monsterIndex]));
                                break;
                            case ClassType.Ranger:
                                StartCoroutine(DexAttackCoroutine(dungeonAttackStruct.critted, dungeonAttackStruct.dodged, dungeonAttackStruct.damage, dungeonAttackStruct.lifeSteal, CharController, activeMonsters[monsterIndex]));
                                break;
                        }
                        yield return new WaitForSeconds(.25f);
                        if (!dungeonAttackStruct.monsterDied)
                        {
                            yield return new WaitForSeconds(.5f);
                            StartCoroutine(WalkCoroutine(activeMonsters[monsterIndex], monsterPositions[difficulty][activeMonsters[monsterIndex].index], 10f));
                        }
                    }
                    yield return new WaitForSeconds(.5f);
                }
            }
            yield return new WaitForSeconds(.5f);
            CharController.StartCoroutine(WalkCoroutine(CharController, heroPosition + Vector3.right * 12f, 13f, 4));
            yield return new WaitForSeconds(1);
            titleText.SetString($"dungeon current level: {j + 2}/5");
        }
        if (fightStruct.victory)
        {
            StartCoroutine(RewardCoroutine(fightStruct.itemDrop));
        }
    }
    private IEnumerator RewardCoroutine(DungeonItem itemDrop)
    {
        SoundController.StopMusic();
        while (timeLocked)
        {
            yield return null;
        }
        Time.timeScale = 1;
        speedWindow.SetActive(false);
        mainMenuController.FadeInBlack();
        yield return new WaitForSeconds(.25f);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        yield return new WaitForEndOfFrame();
        switch (difficulty)
        {
            case 0:
                for (int i = 0; i < easyDungeonVariations.Length; i++)
                {
                    easyDungeonVariations[i].SetActive(false);
                }
                easyDungeonVariations[BaseUtils.RandomInt(0, easyDungeonVariations.Length)].SetActive(true);
                break;
            case 1:
                for (int i = 0; i < mediumDungeonVariations.Length; i++)
                {
                    mediumDungeonVariations[i].SetActive(false);
                }
                mediumDungeonVariations[BaseUtils.RandomInt(0, mediumDungeonVariations.Length)].SetActive(true);
                break;
            case 2:
                for (int i = 0; i < hardDungeonVariations.Length; i++)
                {
                    hardDungeonVariations[i].SetActive(false);
                }
                hardDungeonVariations[BaseUtils.RandomInt(0, hardDungeonVariations.Length)].SetActive(true);
                break;
            case 3:
                for (int i = 0; i < hellDungeonVariations.Length; i++)
                {
                    hellDungeonVariations[i].SetActive(false);
                }
                hellDungeonVariations[BaseUtils.RandomInt(0, hellDungeonVariations.Length)].SetActive(true);
                break;
        }
        CharController.StopAllCoroutines();
        CharController.transform.position = heroPosition + Vector3.left * 4;
        CharController.pivotTransform.localScale = new Vector3(-1, 1, 1);
        CharController.healthbarController.gameObject.SetActive(false);
        GameObject chestObj = Instantiate(chestPrefabs[difficulty], dungeonGameObj.transform);
        chestObj.transform.position = heroPosition + Vector3.right * 10f;
        mainMenuController.FadeOutBlack();
        yield return new WaitForSeconds(.25f);
        SoundController.PlaySound("Victory_sound");
        victoryWindow.SetActive(true);
        BaseUtils.InstantiateEffect(EffectType.VictoryExplosion, Vector3.up * cameraController.transform.position.y, false);
        mainMenuController.SetupBlackScreen();
        CharController.StartCoroutine(WalkCoroutine(CharController, heroPosition + Vector3.right * 10f, 25f, 8));
        float timer = 0;
        Image[] windowImages = victoryWindow.GetComponentsInChildren<Image>();
        while (timer <= 1)
        {
            foreach (Image image in windowImages)
            {
                image.color = Color.Lerp(Color.clear, Color.white, timer.Evaluate(CurveType.EaseOut));
            }
            timer += Time.deltaTime * 3f;
            yield return null;
        }
        foreach (Image image in windowImages)
        {
            image.color = Color.white;
        }
        yield return new WaitForSeconds(2);
        CharController.StopAllCoroutines();
        CharController.StartCoroutine(WalkCoroutine(CharController, heroPosition + Vector3.right * 10f, 6f, 2));
        yield return new WaitForSeconds(1);
        mainMenuController.RemoveBlackScreen();
        timer = 0;
        while (timer <= 1)
        {
            foreach (Image image in windowImages)
            {
                image.color = Color.Lerp(Color.white, Color.clear, timer.Evaluate(CurveType.EaseIn));
            }
            timer += Time.deltaTime * 3f;
            yield return null;
        }
        victoryWindow.SetActive(false);
        yield return new WaitForSeconds(.5f);
        cameraController.ChestZoom();
        int sideMult;
        timer = 0;
        Vector3 fromPos = chestObj.transform.position;
        Vector3 fromCharPos = CharController.transform.position;
        while (timer <= 1)
        {
            CharController.transform.position = Vector3.Lerp(fromCharPos, fromCharPos + Vector3.right * .5f, timer.Evaluate(CurveType.ChestOpenCurve));
            CharController.pivotTransform.localScale = Vector3.Lerp(new Vector3(-1, 1, 1), new Vector3(-1.1f, 0.9f, 1), timer.Evaluate(CurveType.EaseOut));
            CharController.pivotTransform.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.forward * -25, timer.Evaluate(CurveType.EaseOut));
            chestObj.transform.position = Vector3.Lerp(fromPos, fromPos + Vector3.right * .5f, timer.Evaluate(CurveType.ChestOpenCurve));
            chestObj.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(1.2f, 0.8f, 1), timer.Evaluate(CurveType.ChestEaseInCurve));
            timer += Time.deltaTime * 1.5f;
            yield return null;
        }
        fromPos = chestObj.transform.position;
        timer = 0;
        sideMult = BaseUtils.RandomSign() * 15;
        SoundController.PlaySound("Chest_open");
        while (timer <= 1)
        {
            float yOffset = Mathf.Lerp(0, 1, timer.Evaluate(CurveType.JumpCurve));
            chestObj.transform.position = Vector3.Lerp(fromPos, fromPos + Vector3.up * yOffset, timer);
            CharController.pivotTransform.localScale = Vector3.Lerp(new Vector3(-1.1f, 0.9f, 1), new Vector3(-0.9f, 1.2f, 1), timer.Evaluate(CurveType.EaseOut));
            CharController.pivotTransform.localEulerAngles = Vector3.Lerp(Vector3.forward * -25, Vector3.forward * 15, timer.Evaluate(CurveType.EaseOut));
            chestObj.transform.localScale = Vector3.Lerp(new Vector3(1.2f, 0.8f, 1), new Vector3(0.7f, 1.5f, 1), timer.Evaluate(CurveType.EaseOut));
            chestObj.transform.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.forward * sideMult, timer.Evaluate(CurveType.EaseOut));
            timer += Time.deltaTime * 5f;
            yield return null;
        }
        chestObj.GetComponentInChildren<SpriteRenderer>().sprite = chestOpenSprites[difficulty];
        BaseUtils.InstantiateEffect(EffectType.DeathExplosion, chestObj.transform.position + Vector3.up * 40f.ToScale(), true);
        timer = 0;
        while (timer <= 1)
        {
            CharController.pivotTransform.localScale = Vector3.Lerp(new Vector3(-0.9f, 1.2f, 1), new Vector3(-1, 1, 1), timer.Evaluate(CurveType.EaseOut));
            CharController.pivotTransform.localEulerAngles = Vector3.Lerp(Vector3.forward * 15, Vector3.zero, timer.Evaluate(CurveType.EaseOut));
            chestObj.transform.localScale = Vector3.Lerp(new Vector3(0.7f, 1.5f, 1), Vector3.one, timer.Evaluate(CurveType.EaseOut));
            chestObj.transform.localEulerAngles = Vector3.Lerp(Vector3.forward * sideMult, Vector3.zero, timer.Evaluate(CurveType.EaseOut));
            timer += Time.deltaTime * 5f;
            yield return null;
        }
        CharController.StartCoroutine(IdleCoroutine(CharController.pivotTransform));
        mainMenuController.SetupBlackScreen();
        itemTooltip.Setup(ParseDungeonItem(itemDrop), "Looted item!", true);
        //Database.AddItem(itemDrop);
        yield return new WaitForSeconds(1 / 3f);
        while (itemTooltip.gameObject.activeSelf)
        {
            yield return null;
        }
        mainMenuController.RemoveBlackScreen();
        yield return new WaitForSeconds(1 / 3f);
        mainMenuController.FadeInBlack();
        yield return new WaitForSeconds(.25f);
        Destroy(chestObj);
        ExitBattle(true);
    }
    private DungeonItem ParseItemDataDungeon(ItemData itemData)
    {
        return new DungeonItem()
        {
            dexterity = itemData.dexterity,
            endurance = itemData.endurance,
            strength = itemData.strength,
            intelligence = itemData.intelligence,
            luck = itemData.luck,
            class_type = (int)ClassType.None,
            rarity_type = (int)BaseUtils.itemDict[itemData.itemType].rarityType,
            equip_type = (int)BaseUtils.itemDict[itemData.itemType].equipType,
            item_type = (int)itemData.itemType,
            token_id = itemData.itemID.ToString()
        };
    }
    private ItemData ParseDungeonItem(DungeonItem dungeonItem)
    {
        int.TryParse(dungeonItem.token_id, out int itemID);
        return new ItemData(DateTime.Now, dungeonItem.strength, dungeonItem.dexterity, dungeonItem.endurance, dungeonItem.intelligence, dungeonItem.luck,
            BaseUtils.GenerateItemName(BaseUtils.itemDict[(ItemType)dungeonItem.item_type].synonymString, (RarityType)dungeonItem.rarity_type, itemID), itemID, 0, (ItemType)dungeonItem.item_type, ClassType.None);
    }
    private void AddMonster(MonsterType monsterType, StatStruct statStruct, int difficulty, int i)
    {
        ScriptableMonster scriptableMonster = BaseUtils.monsterDict[monsterType];
        GameObject monsterObj = Instantiate(scriptableMonster.monsterPrefab, dungeonGameObj.transform);
        MonsterController monsterController = monsterObj.GetComponent<MonsterController>();
        monsterController.Setup(mainMenuController, this, statStruct, monsterType, scriptableMonster, i, monsterPositions[difficulty][i] + Vector3.right * 4);
        activeMonsters.Add(monsterController);
    }
    private IEnumerator WalkCoroutine(UnitController unitController, Vector3 gotoPos, float multiplier = 6f, int stepAmount = 2)
    {
        for (int i = 0; i < stepAmount; i++)
        {
            float timer = 0;
            Vector3 fromPos = unitController.transform.position;
            float rotation = BaseUtils.RandomBool() ? unitController.moveRotation : -unitController.moveRotation;
            while (timer <= 1)
            {
                unitController.transform.position = Vector3.LerpUnclamped(fromPos, gotoPos, timer / stepAmount);
                if (!unitController.slides)
                {
                    unitController.pivotTransform.localEulerAngles = Vector3.LerpUnclamped(Vector3.zero, Vector3.forward * rotation, timer.Evaluate(CurveType.PeakParabol));
                    if (!unitController.staticKnight)
                    {
                        unitController.pivotTransform.localScale = Vector3.LerpUnclamped(new Vector3(unitController.pivotTransform.localScale.x, 1, 1), new Vector3(unitController.pivotTransform.localScale.x, 1.05f, 1), timer.Evaluate(CurveType.SmoothParabol));
                    }
                }
                timer += Time.deltaTime / stepAmount * multiplier * 1.25f;
                yield return null;
            }
            SoundController.PlaySound("FootStep_loop", .25f, true);
            unitController.transform.position = Vector3.LerpUnclamped(fromPos, gotoPos, 1f / stepAmount);
            if (!unitController.slides)
            {
                yield return new WaitForSeconds(.12f);
            }
        }
    }
    private IEnumerator FadeInCoroutine(SpriteRenderer spriteRenderer, Color goColor)
    {
        float timer = 0;
        while (timer <= 1)
        {
            spriteRenderer.color = Color.Lerp(Color.clear, goColor, timer);
            timer += Time.deltaTime * 3;
            yield return null;
        }
        spriteRenderer.color = goColor;
    }
    private void ClearMonsters()
    {
        for (int i = 0; i < activeMonsters.Count; i++)
        {
            if (activeMonsters[i].healthbarController != null && activeMonsters[i].healthbarController.gameObject != null)
            {
                Destroy(activeMonsters[i].healthbarController.gameObject);
            }
            if (activeMonsters[i] != null && activeMonsters[i].gameObject != null)
            {
                Destroy(activeMonsters[i].gameObject);
            }
        }
        activeMonsters.Clear();
    }
    public override void ExitBattle(bool victory)
    {
        for (int i = 0; i < easyDungeonVariations.Length; i++)
        {
            easyDungeonVariations[i].SetActive(false);
        }
        for (int i = 0; i < mediumDungeonVariations.Length; i++)
        {
            mediumDungeonVariations[i].SetActive(false);
        }
        for (int i = 0; i < hardDungeonVariations.Length; i++)
        {
            hardDungeonVariations[i].SetActive(false);
        }
        for (int i = 0; i < hellDungeonVariations.Length; i++)
        {
            hellDungeonVariations[i].SetActive(false);
        }
        ClearMonsters();
        mainMenuController.StopAllTexts();
        if (CharController.healthbarController != null)
        {
            Destroy(CharController.healthbarController.gameObject);
        }
        CharController.SetColor(Color.white);
        CharController.gameObject.SetActive(true);
        CharController.transform.SetParent(gameController.mainGameObj.transform);
        CharController.transform.position = classPositions[ClassIndex];
        CharController.transform.localScale = new Vector3(1, 1, 1);
        CharController.pivotTransform.localScale = new Vector3(1, 1, 1);
        CharController.pivotTransform.localRotation = Quaternion.identity;
        CharController.shadowSprite.sprite = shadowSprites[1];
        CharController.shadowSprite.transform.localPosition = new Vector3(.724f, -.027f);
        dungeonCanvas.enabled = false;
        dungeonCanvas.gameObject.SetActive(false);
        cameraController.ZoomCamera(6f);
        cameraController.thisCam.transform.position = Vector3.back * 10;
        cameraController.lastScreenWidth = 0;
        mainGameObj.SetActive(true);
        dungeonGameObj.SetActive(false);
        gameController.Setup();
        gameController.FixHpBars();
        mainMenuController.FadeOutBlack();
        mainMenuController.OnBackToMainScreenClick();
        CharController.characterInfo.dungeonObj.SetActive(false);
        BaseUtils.onFight = false;
        int charLevel = Database.GetCharStruct(classType).level;
        switch (difficulty)
        {
            case 0:
                Database.AddExp(classType, 10);
                break;
            case 1:
                Database.AddExp(classType, 20);
                break;
            case 2:
                Database.AddExp(classType, 60);
                break;
            case 3:
                Database.AddExp(classType, 240);
                break;
        }
        if (Database.GetCharStruct(classType).experience >= BaseUtils.GetExpForNextLevel(charLevel))
        {
            Database.AddLevel(classType);
        }
        StartCoroutine(RewardXP(victory));
        SoundController.PlayMusic("CHMenu");
    }
}

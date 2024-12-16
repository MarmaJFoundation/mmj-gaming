using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//info of each attack
[Serializable]
public struct BossAttackStruct
{
    public bool attackingBoss;
    public int playerIndex;
    public int damage;
    public int lifeSteal;
    public bool critted;
    public bool dodged;
    public bool playerDied;
    public bool bossDied;

    public BossAttackStruct(bool attackingBoss, int playerIndex, int damage, int lifeSteal, bool critted, bool dodged, bool playerDied, bool bossDied)
    {
        this.attackingBoss = attackingBoss;
        this.playerIndex = playerIndex;
        this.damage = damage;
        this.lifeSteal = lifeSteal;
        this.critted = critted;
        this.dodged = dodged;
        this.playerDied = playerDied;
        this.bossDied = bossDied;
    }
}
public struct BossFightStruct
{
    public List<BossAttackStruct> bossAttackStructs;
    public StatStruct bossStatStruct;
    public bool victory;

    public BossFightStruct(List<BossAttackStruct> bossAttackStructs, StatStruct bossStatStruct, bool victory)
    {
        this.bossAttackStructs = bossAttackStructs;
        this.bossStatStruct = bossStatStruct;
        this.victory = victory;
    }
}
public class BossFightController : BattleController
{
    public RaidController raidController;
    public Canvas bossCanvas;
    public GameObject fadeObj;
    public GameObject mainGameObj;
    public GameObject dungeonGameObj;
    public MonsterController[] bossControllers;
    public GameObject[] bossStages;
    public GameObject[] charPrefabs;
    public SpriteRenderer[] bossSprites;
    public HealthbarController bossHealthBarController;
    public HealthbarController[] healthBarControllers;
    public Vector3[] playerPositions;
    public CustomButton soundButton;
    public CustomButton musicButton;
    private readonly List<GameCharController> gameCharControllers = new List<GameCharController>();
    private RoomData roomData;
    public void Setup(ClassType classType, int difficulty, RoomData roomData, BossFightStruct fightStruct)
    {
        this.classType = classType;
        this.difficulty = difficulty;
        this.roomData = roomData;
        BaseUtils.onFight = true;
        timeLocked = false;
        speedWindow.SetActive(false);
        bossHealthBarController.gameObject.SetActive(true);
        for (int i = 0; i < 4; i++)
        {
            bossStages[i].SetActive(i == difficulty);
            bossControllers[i].transform.position = Vector3.up * 3;
            bossControllers[i].gameObject.SetActive(true);
            bossControllers[i].GetComponent<SpriteMover>().enabled = true;
            bossSprites[i].color = Color.white;
        }
        switch (difficulty)
        {
            case 0:
                SoundController.PlayMusic("CHBoss1");
                break;
            case 1:
                SoundController.PlayMusic("CHBoss2");
                break;
            case 2:
                SoundController.PlayMusic("CHBoss3");
                break;
            case 3:
                SoundController.PlayMusic("CHBoss3");
                break;
        }
        StartCoroutine(BossFightCoroutine(fightStruct));
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
    private IEnumerator BossFightCoroutine(BossFightStruct fightStruct)
    {
        mainMenuController.FadeInBlack();
        yield return new WaitForSeconds(.25f);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        yield return new WaitForEndOfFrame();
        SoundController.PlaySound("MonsterBossgrowl", 1, true);
        raidController.Dispose();
        fadeObj.SetActive(false);
        mainMenuController.currentWindow = WindowType.MainScreen;
        mainMenuController.mainCanvas.enabled = false;
        mainMenuController.mainCanvas.gameObject.SetActive(false);
        bossCanvas.enabled = true;
        bossCanvas.gameObject.SetActive(true);
        mainGameObj.SetActive(false);
        dungeonGameObj.SetActive(true);
        soundButton.SetDeactivated(Database.databaseStruct.soundVolume == 100);
        musicButton.SetDeactivated(Database.databaseStruct.musicVolume == 100);
        for (int i = 0; i < roomData.playerClasses.Count; i++)
        {
            GameObject playerObj = Instantiate(charPrefabs[(int)roomData.playerClasses[i] - 1]);
            GameCharController gameCharController = playerObj.GetComponent<GameCharController>();
            gameCharControllers.Add(gameCharController);
            gameCharController.healthbarController = healthBarControllers[i];
            gameCharController.pivotTransform.localScale = i < 4 ? new Vector3(-1, 1, 1) : Vector3.one;
            gameCharController.transform.position = playerPositions[i];
            gameCharController.Setup(mainMenuController, roomData.playerNames[i], 4, roomData.playerStatStructs[i], roomData.playerEquippedItems[i]);
            healthBarControllers[i].gameObject.SetActive(true);
        }
        Enum.TryParse("Boss" + (difficulty + 1), out MonsterType bossType);
        ScriptableMonster scriptableMonster = BaseUtils.monsterDict[bossType];
        //StatStruct bossStruct = GenerateBossStats(difficulty + 1, roomData.playerStatStructs, bossType, roomData.killCount);
        bossControllers[difficulty].healthbarController = bossHealthBarController;
        bossControllers[difficulty].statStruct = fightStruct.bossStatStruct;
        bossControllers[difficulty].gustEffect = EffectType.DemonBossGust;
        bossControllers[difficulty].attackEffect = EffectType.DemonBossExplo;
        bossControllers[difficulty].Setup(mainMenuController, scriptableMonster.monsterName, 0, false);
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
        yield return new WaitForSeconds(.25f);
        cameraController.ZoomCamera(8.5f);
        cameraController.MoveCamera(Vector3.back * 10 + Vector3.up);
        mainMenuController.FadeOutBlack();
        yield return new WaitForSeconds(.25f);
        for (int i = 0; i < fightStruct.bossAttackStructs.Count; i++)
        {
            if (fightStruct.bossAttackStructs[i].attackingBoss)
            {
                int playerIndex = fightStruct.bossAttackStructs[i].playerIndex;
                switch (roomData.playerClasses[playerIndex])
                {
                    case ClassType.Mage:
                        StartCoroutine(MagicAttackCoroutine(
                            fightStruct.bossAttackStructs[i].critted,
                            fightStruct.bossAttackStructs[i].dodged,
                            fightStruct.bossAttackStructs[i].damage,
                            fightStruct.bossAttackStructs[i].lifeSteal,
                            gameCharControllers[playerIndex],
                            bossControllers[difficulty], -5));
                        yield return new WaitForSeconds(.25f);
                        break;
                    case ClassType.Knight:
                        StartCoroutine(MeleeAttackCoroutine(
                            fightStruct.bossAttackStructs[i].critted,
                            fightStruct.bossAttackStructs[i].dodged,
                            fightStruct.bossAttackStructs[i].damage,
                            fightStruct.bossAttackStructs[i].lifeSteal,
                            gameCharControllers[playerIndex],
                            bossControllers[difficulty], -5));
                        break;
                    case ClassType.Ranger:
                        StartCoroutine(DexAttackCoroutine(
                            fightStruct.bossAttackStructs[i].critted,
                            fightStruct.bossAttackStructs[i].dodged,
                            fightStruct.bossAttackStructs[i].damage,
                            fightStruct.bossAttackStructs[i].lifeSteal,
                            gameCharControllers[playerIndex],
                            bossControllers[difficulty], -5));
                        break;
                }
            }
            else
            {
                bossControllers[difficulty].GetComponent<SpriteMover>().enabled = false;
                BaseUtils.InstantiateEffect(EffectType.BossGust, bossControllers[difficulty].transform.position, true, 1 + (1 + difficulty) * .1f);
                cameraController.BossZoom();
                yield return new WaitForSeconds(1);
                bossControllers[difficulty].GetComponent<SpriteMover>().enabled = true;
                BaseUtils.InstantiateEffect(EffectType.BossAttack, bossControllers[difficulty].transform.position, true, 1 + (1 + difficulty) * .1f);
                bool advanced = false;
                while (fightStruct.bossAttackStructs.Count > i && !fightStruct.bossAttackStructs[i].attackingBoss)
                {
                    BaseUtils.InstantiateEffect(EffectType.BossExplo,
                    gameCharControllers[fightStruct.bossAttackStructs[i].playerIndex].transform.position, true, 1 + (1 + difficulty) * .1f);
                    gameCharControllers[fightStruct.bossAttackStructs[i].playerIndex].OnDamage(
                    fightStruct.bossAttackStructs[i].critted,
                    fightStruct.bossAttackStructs[i].dodged,
                    fightStruct.bossAttackStructs[i].damage,
                    fightStruct.bossAttackStructs[i].lifeSteal, bossControllers[difficulty]);
                    i++;
                    advanced = true;
                }
                if (advanced)
                {
                    i--;
                }
            }
            yield return new WaitForSeconds(.5f);
        }
        while (timeLocked)
        {
            yield return null;
        }
        if (fightStruct.victory)
        {
            StartCoroutine(VictoryCoroutine());
        }
        else
        {
            StartCoroutine(DefeatCoroutine());
        }
    }
    private IEnumerator DeathEffectBoss()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int k = 0; k < 3; k++)
            {
                BaseUtils.InstantiateEffect(EffectType.BossDeath, bossControllers[difficulty].transform.position + new Vector3(BaseUtils.RandomInt(-50, 50), BaseUtils.RandomInt(-100, 100), 0).ToScale(), true, 1 + (1 + difficulty) * .1f);
                BaseUtils.InstantiateEffect(EffectType.RatBossDeath, bossControllers[difficulty].transform.position + new Vector3(BaseUtils.RandomInt(-50, 50), BaseUtils.RandomInt(-100, 100), 0).ToScale(), true, 1 + (1 + difficulty) * .1f);
                yield return new WaitForSeconds(.05f);
            }
            BaseUtils.InstantiateEffect(EffectType.BossAttack, bossControllers[difficulty].transform.position, true, 1 + (1 + difficulty) * .1f);
            cameraController.ShakeCamera(2);
            yield return new WaitForSeconds(.2f);
        }
    }
    private IEnumerator VictoryCoroutine()
    {
        SoundController.StopMusic();
        while (timeLocked)
        {
            yield return null;
        }
        bossHealthBarController.gameObject.SetActive(false);
        Time.timeScale = 1;
        speedWindow.SetActive(false);
        UnwarpTime();
        bossControllers[difficulty].GetComponent<SpriteMover>().enabled = false;
        BaseUtils.InstantiateEffect(EffectType.BossAttack, bossControllers[difficulty].transform.position, true, 1 + (1 + difficulty) * .1f);
        cameraController.ShakeCamera(2);
        yield return new WaitForSeconds(.5f);
        StartCoroutine(DeathEffectBoss());
        float timer = 0;
        Vector3 fromPos = bossControllers[difficulty].transform.position;
        while (timer <= 1)
        {
            bossControllers[difficulty].transform.position = Vector3.Lerp(fromPos, Vector3.up * 4, timer.Evaluate(CurveType.EaseOut));
            timer += Time.deltaTime * 5f;
            yield return null;
        }
        timer = 0;
        while (timer <= 1)
        {
            bossSprites[difficulty].color = Color.Lerp(Color.white, Color.clear, timer);
            bossControllers[difficulty].transform.position = Vector3.Lerp(Vector3.up * 4, Vector3.up * 1, timer);
            timer += Time.deltaTime * .3f;
            yield return null;
        }
        bossSprites[difficulty].color = Color.clear;
        bossControllers[difficulty].gameObject.SetActive(false);
        victoryWindow.SetActive(true);
        SoundController.PlaySound("Victory_sound");
        BaseUtils.InstantiateEffect(EffectType.VictoryExplosion, Vector3.up * cameraController.transform.position.y, false);
        mainMenuController.SetupBlackScreen();
        timer = 0;
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
        yield return new WaitForSeconds(1 / 3f);
        mainMenuController.FadeInBlack();
        yield return new WaitForSeconds(.25f);
        ExitBattle(true);
    }
    public BossFightStruct GenerateFightStruct(int difficulty, RoomData roomData)
    {
        List<int> charHPs = new List<int>();
        for (int i = 0; i < roomData.playerStatStructs.Count; i++)
        {
            charHPs.Add(roomData.playerStatStructs[i].maxHealth);
        }
        Enum.TryParse("Boss" + (difficulty + 1), out MonsterType bossType);
        StatStruct bossStruct = GenerateBossStats(difficulty + 1, roomData.playerStatStructs, bossType, roomData.killCount);
        int bossHP = bossStruct.maxHealth;
        int damage;
        bool critted;
        bool dodged;
        int lifeSteal;
        List<BossAttackStruct> bossAttackStructs = new List<BossAttackStruct>();
        while (HasPlayerAlive(charHPs))
        {
            //players attack
            for (int i = 0; i < roomData.playerStatStructs.Count; i++)
            {
                if (charHPs[i] <= 0)
                {
                    continue;
                }
                StatStruct playerStat = roomData.playerStatStructs[i];
                dodged = false;
                damage = Mathf.RoundToInt(BaseUtils.RandomFloat(playerStat.damage * .5f, playerStat.damage));
                critted = BaseUtils.RandomFloat(0, 100) < playerStat.critChance;
                if (critted)
                {
                    damage *= 2;
                }
                damage -= Mathf.RoundToInt(damage * BaseUtils.RandomFloat(bossStruct.defense * .5f, bossStruct.defense * .9f) / 100);
                lifeSteal = Mathf.RoundToInt(damage / 100f * playerStat.lifeSteal);
                if ((lifeSteal + charHPs[i]) >= playerStat.maxHealth)
                {
                    lifeSteal -= (lifeSteal + charHPs[i]) - playerStat.maxHealth;
                }
                charHPs[i] += lifeSteal;
                bossHP -= damage;
                //if boss dies, stop everything here, declare victory
                if (bossHP <= 0)
                {
                    bossAttackStructs.Add(new BossAttackStruct(true, i, damage, lifeSteal, critted, dodged, false, true));
                    return new BossFightStruct(bossAttackStructs, bossStruct, true);
                }
                else
                {
                    bossAttackStructs.Add(new BossAttackStruct(true, i, damage, lifeSteal, critted, dodged, false, false));
                }
            }
            //boss attacks
            List<int> attackedPlayers = new List<int>();
            for (int i = 0; i < Mathf.Clamp(BaseUtils.RandomInt(1, (difficulty + 1) * 3), 1, 8); i++)
            {
                int playerIndex = BaseUtils.RandomInt(0, 8);
                if (charHPs[playerIndex] > 0 && !attackedPlayers.Contains(playerIndex))
                {
                    attackedPlayers.Add(playerIndex);
                }
            }
            if (attackedPlayers.Count == 0)
            {
                for (int i = 0; i < charHPs.Count; i++)
                {
                    if (charHPs[i] > 0)
                    {
                        attackedPlayers.Add(i);
                        break;
                    }
                }
            }
            if (attackedPlayers.Count == 0)
            {
                return new BossFightStruct(bossAttackStructs, bossStruct, false);
            }
            for (int i = 0; i < attackedPlayers.Count; i++)
            {
                StatStruct playerStat = roomData.playerStatStructs[attackedPlayers[i]];
                critted = false;
                damage = 0;
                lifeSteal = 0;
                dodged = BaseUtils.RandomFloat(0, 100) < playerStat.dodge;
                if (!dodged)
                {
                    damage = Mathf.RoundToInt(BaseUtils.RandomFloat(bossStruct.damage * .5f, bossStruct.damage));
                    critted = BaseUtils.RandomFloat(0, 100) < bossStruct.critChance;
                    if (critted)
                    {
                        damage *= 2;
                    }
                    damage -= Mathf.RoundToInt(damage * BaseUtils.RandomFloat(playerStat.defense * .5f, playerStat.defense * .9f) / 100);
                    lifeSteal = Mathf.RoundToInt(damage / 100f * bossStruct.lifeSteal);
                    if ((lifeSteal + bossHP) >= bossStruct.maxHealth)
                    {
                        lifeSteal -= (lifeSteal + bossHP) - bossStruct.maxHealth;
                    }
                    bossHP += lifeSteal;
                    charHPs[attackedPlayers[i]] -= damage;
                }
                bossAttackStructs.Add(new BossAttackStruct(false, attackedPlayers[i], damage, lifeSteal, critted, dodged, charHPs[attackedPlayers[i]] <= 0, false));
            }
        }
        return new BossFightStruct(bossAttackStructs, bossStruct, false);
    }
    private bool HasPlayerAlive(List<int> charHPs)
    {
        for (int i = 0; i < charHPs.Count; i++)
        {
            if (charHPs[i] > 0)
            {
                return true;
            }
        }
        return false;
    }
    private StatStruct GenerateBossStats(int difficulty, List<StatStruct> charStructs, MonsterType monsterType, int bossKills)
    {
        ScriptableMonster scriptableMonster = BaseUtils.monsterDict[monsterType];
        int statSum = 0;
        for (int i = 0; i < charStructs.Count; i++)
        {
            statSum += Mathf.RoundToInt(charStructs[i].critChance * 100) + Mathf.RoundToInt(charStructs[i].lifeSteal * 100) + Mathf.RoundToInt(charStructs[i].dodge * 100) + Mathf.RoundToInt(charStructs[i].defense * 100) + charStructs[i].damage + charStructs[i].maxHealth / 2;
        }
        statSum /= charStructs.Count;
        float multiplier = BaseUtils.RandomFloat(.01f, .02f) / (1 + statSum * .001f);
        int statAdd = Mathf.RoundToInt(Mathf.Clamp(statSum - 250, 1, Mathf.Infinity) * multiplier);
        float statMultiplier = 1 + (statSum / 6f * multiplier) + (difficulty * .1f) + (bossKills * .01f);
        int dexterity = Mathf.RoundToInt((BaseUtils.RandomInt(scriptableMonster.dexRange) + statAdd) * statMultiplier);
        int strength = Mathf.RoundToInt((BaseUtils.RandomInt(scriptableMonster.strRange) + statAdd) * statMultiplier);
        int intelligence = Mathf.RoundToInt((BaseUtils.RandomInt(scriptableMonster.intRange) + statAdd) * statMultiplier);
        int endurance = Mathf.RoundToInt((BaseUtils.RandomInt(scriptableMonster.endRange) + statAdd) * statMultiplier);
        int luck = Mathf.RoundToInt((BaseUtils.RandomInt(scriptableMonster.lckRange) + statAdd) * statMultiplier);
        StatStruct bossStruct = BaseUtils.GenerateMonsterStatStruct(scriptableMonster.classType, dexterity, strength, intelligence, endurance, luck);
        bossStruct = new StatStruct(bossStruct.maxHealth, bossStruct.damage, 10, 0, bossStruct.lifeSteal, bossStruct.critChance);
        return bossStruct;
    }
    public override void ExitBattle(bool victory)
    {
        mainMenuController.StopAllTexts();
        for (int i = 0; i < gameCharControllers.Count; i++)
        {
            Destroy(gameCharControllers[i].gameObject);
        }
        bossStages[difficulty].SetActive(false);
        gameCharControllers.Clear();
        fadeObj.SetActive(true);
        bossCanvas.enabled = false;
        bossCanvas.gameObject.SetActive(false);
        cameraController.ZoomCamera(6f);
        cameraController.thisCam.transform.position = Vector3.back * 10;
        cameraController.lastScreenWidth = 0;
        mainGameObj.SetActive(true);
        dungeonGameObj.SetActive(false);
        gameController.Setup();
        gameController.FixHpBars();
        mainMenuController.FadeOutBlack();
        mainMenuController.OnBackToMainScreenClick();
        BaseUtils.onFight = false;
        CharController.characterInfo.dungeonObj.SetActive(false);
        StartCoroutine(RewardXP(victory));
        SoundController.PlayMusic("CHMenu");
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoController : MonoBehaviour
{
    public MainMenuController mainMenuController;
    public CameraController cameraController;
    public NearHelper nearHelper;
    public CustomText titleText;
    public CustomText hpText;
    public CustomText descriptionText;
    public GameObject hpBarObj;
    public GameObject unlockObj;
    public GameObject inventoryObj;
    public GameObject dungeonObj;
    public GameObject barObj;
    public RectTransform topPartRect;
    public RectTransform botPartRect;
    public RectTransform xpBarRect;
    public Vector3 offset;
    public Image hpBar;
    public ClassType classType;
    public Transform linkedTransform;
    public GameObject[] potionObjs;
    public CustomTooltip[] potionTooltips;
    [HideInInspector]
    public bool onInventory;
    private GameCharController gameCharController;
    private int currentExp;
    private int baseExp;
    private int charLevel;
    private int classIndex;
    private bool injuredChar;
    private Coroutine movePartCoroutine;
    public void Setup(GameCharController gameCharController, ClassType classType, bool unlockedChar, bool injuredChar)
    {
        this.gameCharController = gameCharController;
        this.injuredChar = injuredChar;
        classIndex = (int)classType - 1;
        if (BaseUtils.onFight)
        {
            descriptionText.gameObject.SetActive(false);
            inventoryObj.SetActive(false);
            dungeonObj.SetActive(false);
            unlockObj.SetActive(false);
            barObj.SetActive(unlockedChar);
            if (unlockedChar)
            {
                charLevel = Database.GetCharStruct(classType).level;
                titleText.SetString($"{classType} lvl: {charLevel}");
                baseExp = BaseUtils.GetExpForNextLevel(charLevel - 1);
                currentExp = Database.GetCharStruct(classType).experience;
                float goScale = Mathf.Clamp((float)(currentExp - baseExp) / (BaseUtils.GetExpForNextLevel(charLevel) - baseExp), .05f, 1);
                xpBarRect.localScale = new Vector3(goScale, 1, 1);
            }
            else
            {
                titleText.SetString(classType.ToString());
            }
        }
        else if (injuredChar)
        {
            dungeonObj.SetActive(false);
            descriptionText.gameObject.SetActive(true);
            SetBasicButtons();
        }
        else if (unlockedChar)
        {
            descriptionText.gameObject.SetActive(true);
            descriptionText.SetString("ready for battle");
            dungeonObj.SetActive(true);
            SetBasicButtons();
        }
        else
        {
            barObj.SetActive(false);
            descriptionText.gameObject.SetActive(true);
            descriptionText.SetString("hero locked");
            titleText.SetString(classType.ToString());
            inventoryObj.SetActive(false);
            dungeonObj.SetActive(false);
            unlockObj.SetActive(true);
        }
        if (Database.HasStrengthPotion(classType))
        {
            potionObjs[0].SetActive(true);
            potionTooltips[0].tooltipText[0] = $"{Database.GetStrengthPotion(classType).strength}% better stats";
        }
        else
        {
            potionObjs[0].SetActive(false);
        }
        if (Database.HasStaminaPotion(classType))
        {
            potionObjs[1].SetActive(true);
            potionTooltips[1].tooltipText[0] = $"{Database.GetStaminaPotion(classType).strength} min. rest save";
        }
        else
        {
            potionObjs[1].SetActive(false);
        }
        if (Database.HasLuckPotion(classType))
        {
            potionObjs[2].SetActive(true);
            potionTooltips[2].tooltipText[0] = $"{Database.GetLuckPotion(classType).strength}% better loot";
        }
        else
        {
            potionObjs[2].SetActive(false);
        }
    }
    public void FixHpBars()
    {
        topPartRect.anchoredPosition = new Vector2(0, 125f);
        botPartRect.anchoredPosition = new Vector2(0, -25f);
    }
    private void SetBasicButtons()
    {
        charLevel = Database.GetCharStruct(classType).level;
        titleText.SetString($"{classType} lvl: {charLevel}");
        inventoryObj.SetActive(true);
        unlockObj.SetActive(false);
        barObj.SetActive(true);
        baseExp = BaseUtils.GetExpForNextLevel(charLevel - 1);
        currentExp = Database.GetCharStruct(classType).experience;
        float goScale = Mathf.Clamp((float)(currentExp - baseExp) / (BaseUtils.GetExpForNextLevel(charLevel) - baseExp), .05f, 1);
        xpBarRect.localScale = new Vector3(goScale, 1, 1);
    }

    public void UpdateXpBar(int xpAmount, Color expColor)
    {
        currentExp += xpAmount;
        mainMenuController.InstantiateText(linkedTransform, $"+{xpAmount}xp", 0, expColor, false);
        if (currentExp >= BaseUtils.GetExpForNextLevel(charLevel))
        {
            charLevel++;
            titleText.SetString($"{classType} lvl:{charLevel}");
            baseExp = BaseUtils.GetExpForNextLevel(charLevel - 1);
            mainMenuController.InstantiateText(linkedTransform, "lvl up!", 500, Color.white, false);
            BaseUtils.InstantiateEffect(EffectType.LevelUp, linkedTransform.position + Vector3.up * 20f.ToScale(), true);
            SoundController.PlaySound("Hero_unlocked");
        }
        float goScale = Mathf.Clamp((float)(currentExp - baseExp) / (BaseUtils.GetExpForNextLevel(charLevel) - baseExp), .05f, 1);
        xpBarRect.localScale = new Vector3(goScale, 1, 1);
    }
    public void OnUnlockClick()
    {
        /*if (Database.databaseStruct.pixelTokens < 100)
        {
            BaseUtils.ShowWarningMessage("Insuffient tokens", new string[2] { "You lack the necessary pixel tokens to buy this item", "would you like to trade more @ ?" }, BaseUtils.OnAcceptTradeToken);
            return;
        }*/
        BaseUtils.ShowWarningMessage($"Unlocking {classType}", new string[1] { $"Do you wish to unlock {classType}?" }, OnUnlockAccept);
    }
    private void OnUnlockAccept()
    {
        if (BaseUtils.offlineMode)
        {
            OnSuccessfulUnlock();
        }
        else
        {
            StartCoroutine(nearHelper.RequestUnlockCharacter(classType));
        }
    }
    public void OnSuccessfulUnlock()
    {
        //Database.databaseStruct.pixelTokens -= 10;
        Database.AddClass(classType);
        gameCharController.Setup();
        cameraController.ShakeCamera(1);
        SoundController.PlaySound("Hero_unlocked");
        BaseUtils.InstantiateEffect(EffectType.UnlockCharacter, gameCharController.transform.position + Vector3.up * 20f.ToScale(), true);
    }
    public void MoveParts()
    {
        if (movePartCoroutine != null)
        {
            StopCoroutine(movePartCoroutine);
        }
        if (onInventory)
        {
            movePartCoroutine = StartCoroutine(MovePartsCoroutine(new Vector2(0, 145f), new Vector2(0, -35f)));
        }
        else
        {
            movePartCoroutine = StartCoroutine(MovePartsCoroutine(new Vector2(0, 125f), new Vector2(0, -25f)));
        }
    }
    private IEnumerator MovePartsCoroutine(Vector2 goTopPos, Vector2 goBotPos)
    {
        float timer = 0;
        Vector2 fromTopPos = topPartRect.anchoredPosition;
        Vector2 fromBotPos = botPartRect.anchoredPosition;
        while (timer <= 1)
        {
            topPartRect.anchoredPosition = Vector2.Lerp(fromTopPos, goTopPos, timer.Evaluate(CurveType.CameraEaseOut));
            botPartRect.anchoredPosition = Vector2.Lerp(fromBotPos, goBotPos, timer.Evaluate(CurveType.CameraEaseOut));
            timer += Time.deltaTime * cameraController.camSpeed;
            yield return null;
        }
    }
    private void LateUpdate()
    {
        transform.position = (linkedTransform.position + offset) * cameraController.OrthoDiff;
        if (injuredChar)
        {
            descriptionText.SetString($"resting for {Database.GetCharStruct(classType).injuredTimer.ToHoursTime()}");
            if (Database.GetCharStruct(classType).injuredTimer < DateTime.Now)
            {
                gameCharController.Setup();
                cameraController.ShakeCamera(1);
                BaseUtils.InstantiateEffect(EffectType.UnlockCharacter, gameCharController.transform.position + Vector3.up * 20f.ToScale(), true);
            }
        }
    }
}

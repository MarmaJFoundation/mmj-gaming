using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionController : MonoBehaviour
{
    public MainMenuController mainMenuController;
    public NearHelper nearHelper;
    public CameraController cameraController;
    public GameObject potionWindow;
    public Transform[] classesTransform;
    public CharacterInfoController[] classesInfo;
    [HideInInspector]
    public ClassType classType;
    public CustomText[] descriptionTexts;
    public CustomText[] activeTexts1;
    public CustomText[] activeTexts2;
    public CustomText[] activeTexts3;
    public CustomButton[] buyButtons;
    public CustomText[] buttonTexts;
    private int classIndex;
    private int potionToBuy;
    public void Setup(ClassType classType)
    {
        if (potionWindow.activeSelf)
        {
            Dispose();
            return;
        }
        this.classType = classType;
        classIndex = (int)classType - 1;
        potionWindow.SetActive(true);
        mainMenuController.buttonsWindow.SetActive(false);
        for (int i = 0; i < 3; i++)
        {
            classesInfo[i].gameObject.SetActive(i == classIndex);
            classesInfo[i].onInventory = (i == classIndex);
            if (i == classIndex)
            {
                classesInfo[i].MoveParts();
            }
        }
        //asd
        cameraController.MoveCamera(classesTransform[classIndex].transform.position + cameraController.inventoryOffset + Vector3.back * 10);
        cameraController.ZoomCamera(4.6f);
        PotionData strengthPotionData = Database.GetStrengthPotion(classType);
        PotionData staminaPotionData = Database.GetStaminaPotion(classType);
        PotionData luckPotionData = Database.GetLuckPotion(classType);
        if (Database.HasStrengthPotion(classType))
        {
            activeTexts1[0].SetString($"status: {strengthPotionData.amount} uses left", BaseUtils.enabledColor);
            activeTexts1[1].SetString($"effects: on next dungeon or raid", BaseUtils.enabledColor);
            activeTexts1[2].gameObject.SetActive(true);
            activeTexts1[2].SetString($"hero has {strengthPotionData.strength}% increased stats", BaseUtils.enabledColor);
            buyButtons[0].SetDeactivated(true);
            buttonTexts[0].SetString(BaseUtils.disabledColor);
        }
        else
        {
            activeTexts1[0].SetString($"status: not active", BaseUtils.disabledColor);
            activeTexts1[1].SetString($"effects: none", BaseUtils.disabledColor);
            activeTexts1[2].gameObject.SetActive(false);
            buyButtons[0].SetDeactivated(false);
            buttonTexts[0].SetString(BaseUtils.enabledColor);
        }
        if (Database.HasStaminaPotion(classType))
        {
            activeTexts2[0].SetString($"status: {staminaPotionData.amount} uses left", BaseUtils.enabledColor);
            activeTexts2[1].SetString($"effects: on next dungeon or raid", BaseUtils.enabledColor);
            activeTexts2[2].gameObject.SetActive(true);
            activeTexts2[2].SetString($"rest timer will be reduced by { staminaPotionData.strength} minutes", BaseUtils.enabledColor);
            buyButtons[1].SetDeactivated(true);
            buttonTexts[1].SetString(BaseUtils.disabledColor);
        }
        else
        {
            activeTexts2[0].SetString($"status: not active", BaseUtils.disabledColor);
            activeTexts2[1].SetString($"effects: none", BaseUtils.disabledColor);
            activeTexts2[2].gameObject.SetActive(false);
            buyButtons[1].SetDeactivated(false);
            buttonTexts[1].SetString(BaseUtils.enabledColor);
        }
        if (Database.HasLuckPotion(classType))
        {
            activeTexts3[0].SetString($"status: {luckPotionData.amount} uses left", BaseUtils.enabledColor);
            activeTexts3[1].SetString($"effects: on next successful dungeon", BaseUtils.enabledColor);
            activeTexts3[2].gameObject.SetActive(true);
            activeTexts3[2].SetString($"hero has {luckPotionData.strength}% better loot", BaseUtils.enabledColor);
            buyButtons[2].SetDeactivated(true);
            buttonTexts[2].SetString(BaseUtils.disabledColor);
        }
        else
        {
            activeTexts3[0].SetString($"status: not active", BaseUtils.disabledColor);
            activeTexts3[1].SetString($"effects: none", BaseUtils.disabledColor);
            activeTexts3[2].gameObject.SetActive(false);
            buyButtons[2].SetDeactivated(false);
            buttonTexts[2].SetString(BaseUtils.enabledColor);
        }
    }
    public void Dispose(bool intoWindow = false)
    {
        if (!potionWindow.activeSelf)
        {
            return;
        }
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
        potionWindow.SetActive(false);
    }
    public void OnBuyClick(int potionIndex)
    {
        switch (potionIndex)
        {
            case 0:
                BaseUtils.ShowWarningMessage("strength potion", new string[3] { "would you like to purchase a this potion", "for a total amount of 40 @?", "please be aware effectiveness is random!" }, new ItemData() { itemType = ItemType.Potion1 }, OnAcceptPotionBuy);
                break;
            case 1:
                BaseUtils.ShowWarningMessage("stamina potion", new string[3] { "would you like to purchase a this potion", "for a total amount of 40 @?", "please be aware effectiveness is random!" }, new ItemData() { itemType = ItemType.Potion2 }, OnAcceptPotionBuy);
                break;
            case 2:
                BaseUtils.ShowWarningMessage("luck potion", new string[3] { "would you like to purchase a this potion", "for a total amount of 40 @?", "please be aware effectiveness is random!" }, new ItemData() { itemType = ItemType.Potion3 }, OnAcceptPotionBuy);
                break;
        }
        potionToBuy = potionIndex;
    }
    private void OnAcceptPotionBuy()
    {
        if (Database.databaseStruct.pixelTokens < 40)
        {
            BaseUtils.ShowWarningMessage("Insuffient tokens", new string[2] { "You lack the necessary pixel tokens to buy this item", "would you like to trade more @ ?" }, BaseUtils.OnAcceptTradeToken);
            return;
        }
        if (BaseUtils.offlineMode)
        {
            OnPotionBuyCallback();
        }
        else
        {
            StartCoroutine(nearHelper.RequestPotionBuy((int)classType, potionToBuy));
        }
    }
    public void OnPotionBuyCallback(PotionData potionData = default)
    {
        switch (potionToBuy)
        {
            case 0:
                Database.SetStrengthPotion(classType, BaseUtils.offlineMode ? new PotionData(3, BaseUtils.RandomInt(7, 15)) : potionData);
                BaseUtils.InstantiateEffect(EffectType.StrengthPotionExplo, classesTransform[classIndex].position, true);
                break;
            case 1:
                Database.SetStaminaPotion(classType, BaseUtils.offlineMode ? new PotionData(3, BaseUtils.RandomInt(15, 30)) : potionData);
                BaseUtils.InstantiateEffect(EffectType.StaminaPotionExplo, classesTransform[classIndex].position, true);
                break;
            case 2:
                Database.SetLuckPotion(classType, BaseUtils.offlineMode ? new PotionData(3, BaseUtils.RandomInt(7, 15)) : potionData);
                BaseUtils.InstantiateEffect(EffectType.LuckPotionExplo, classesTransform[classIndex].position, true);
                break;
        }
        SoundController.PlaySound("Potion_sound", 1, true);
        Database.databaseStruct.pixelTokens -= 40;
        mainMenuController.OnBackToMainScreenClick(true);
    }
}

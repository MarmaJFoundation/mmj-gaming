using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageWindowController : MonoBehaviour
{
    public CustomText messageTitle;
    public CustomText[] messageLines;
    public Image itemBorder;
    public Image itemImage;
    public GameObject buttonsObj;
    public GameObject bottomBarObj;
    public GameObject exitObj;
    private Action OnAcceptCallback;
    public void Setup(string title, string[] message, ItemData itemData)
    {
        SoundController.PlaySound("Menu_No", 1, true);
        BaseUtils.showingWarn = true;
        SetItemWarning(title, message, itemData);
        buttonsObj.SetActive(false);
        bottomBarObj.SetActive(true);
        exitObj.SetActive(true);
    }
    public void Setup(string title, string[] message, ItemData itemData, Action OnAcceptCallback)
    {
        SoundController.PlaySound("Menu_No", 1, true);
        this.OnAcceptCallback = OnAcceptCallback;
        SetItemWarning(title, message, itemData);
        buttonsObj.SetActive(true);
        bottomBarObj.SetActive(false);
        exitObj.SetActive(true);
    }
    private void SetItemWarning(string title, string[] message, ItemData itemData)
    {
        ScriptableItem scriptableItem = BaseUtils.itemDict[itemData.itemType];
        messageTitle.SetString(title, BaseUtils.rarityColors[(int)scriptableItem.rarityType]);
        string itemPrefix = "          ";
        itemBorder.gameObject.SetActive(true);
        itemBorder.sprite = BaseUtils.itemBorders[(int)scriptableItem.rarityType];
        itemImage.sprite = scriptableItem.itemSprite;
        for (int i = 0; i < messageLines.Length; i++)
        {
            if (message.Length > i)
            {
                messageLines[i].gameObject.SetActive(true);
                if (i < 3)
                {
                    messageLines[i].SetString($"{itemPrefix}{message[i]}");
                }
                else
                {
                    messageLines[i].SetString(message[i]);
                }
            }
            else
            {
                messageLines[i].gameObject.SetActive(false);
            }
        }
    }
    public void Setup(string title, string[] message, Action OnAcceptCallback)
    {
        SoundController.PlaySound("Menu_No", 1, true);
        this.OnAcceptCallback = OnAcceptCallback;
        buttonsObj.SetActive(true);
        bottomBarObj.SetActive(false);
        SetMessage(title, message);
        exitObj.SetActive(true);
    }
    public void Setup(string title, string[] message)
    {
        SoundController.PlaySound("Menu_No", 1, true);
        buttonsObj.SetActive(false);
        bottomBarObj.SetActive(true);
        SetMessage(title, message);
        exitObj.SetActive(true);
    }
    public void Setup(string title, string[] message, bool lockWindow)
    {
        SoundController.PlaySound("Menu_No", 1, true);
        buttonsObj.SetActive(false);
        bottomBarObj.SetActive(true);
        SetMessage(title, message);
        exitObj.SetActive(!lockWindow);
    }

    private void SetMessage(string title, string[] message)
    {
        messageTitle.SetString(title);
        itemBorder.gameObject.SetActive(false);
        for (int i = 0; i < messageLines.Length; i++)
        {
            if (message.Length > i)
            {
                messageLines[i].gameObject.SetActive(true);
                messageLines[i].SetString(message[i]);
            }
            else
            {
                messageLines[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnAcceptClick()
    {
        OnAcceptCallback.Invoke();
        Destroy(gameObject);
        BaseUtils.showingWarn = false;
    }
    public void OnRefuseClick()
    {
        Destroy(gameObject);
        BaseUtils.showingWarn = false;
    }
}

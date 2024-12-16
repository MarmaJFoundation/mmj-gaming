﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [HideInInspector]
    public Image buttonImage;
    public UnityEvent unityEvent;
    public UnityEvent rightEvent;
    public bool deactivated;
    public bool ignoreDeactivatedClick;
    private void Awake()
    {
        buttonImage = GetComponent<Image>();
    }
    public void SetDeactivated(bool deactivated)
    {
        this.deactivated = deactivated;
        if (deactivated)
        {
            buttonImage.material = BaseUtils.grayscaleUI;
        }
        else
        {
            buttonImage.material = OutlineMaterial() ? BaseUtils.outlineUI : BaseUtils.normalUI;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (deactivated)
        {
            buttonImage.material = BaseUtils.graylightUI;
            if (ignoreDeactivatedClick)
            {
                StartCoroutine(ClickDelay(eventData.button == PointerEventData.InputButton.Right));
            }
        }
        else
        {
            StartCoroutine(ClickDelay(eventData.button == PointerEventData.InputButton.Right));
            buttonImage.material = OutlineMaterial() ? BaseUtils.highLineUI : BaseUtils.highlightUI;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (deactivated)
        {
            buttonImage.material = BaseUtils.grayscaleUI;
        }
        else
        {
            buttonImage.material = OutlineMaterial() ? BaseUtils.outlineUI : BaseUtils.normalUI;
        }
    }
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (deactivated)
        {
            buttonImage.material = BaseUtils.graylightUI;
        }
        else
        {
            buttonImage.material = OutlineMaterial() ? BaseUtils.highLineUI : BaseUtils.highlightUI;
        }
        //SoundController.PlaySound("Button_hover");
    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (deactivated)
        {
            buttonImage.material = BaseUtils.grayscaleUI;
        }
        else
        {
            buttonImage.material = OutlineMaterial() ? BaseUtils.outlineUI : BaseUtils.normalUI;
        }
    }
    private bool OutlineMaterial()
    {
        return buttonImage.material.GetFloat("_UseOutline") == 1;
    }
    private void OnDisable()
    {
        buttonImage.material = OutlineMaterial() ? BaseUtils.outlineUI : BaseUtils.normalUI;
    }
    private IEnumerator ClickDelay(bool rightClickEvent = false)
    {
        yield return new WaitForEndOfFrame();
        SoundController.PlaySound("Select_1", 1, true);
        if (rightClickEvent)
        {
            if (rightEvent != null)
            {
                rightEvent.Invoke();
            }
        }
        else
        {
            if (unityEvent != null)
            {
                unityEvent.Invoke();
            }
        }
    }
}

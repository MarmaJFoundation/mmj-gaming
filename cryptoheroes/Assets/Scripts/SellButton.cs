using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SellButton : CustomButton
{
    public BackpackController backpackController;
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        backpackController.OnSellButtonEnter();
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        backpackController.OnSellButtonExit();
    }
}

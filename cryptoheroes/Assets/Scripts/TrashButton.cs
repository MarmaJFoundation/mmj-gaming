using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrashButton : CustomButton
{
    public BackpackController backpackController;
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        backpackController.OnTrashButtonEnter();
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        backpackController.OnTrashButtonExit();
    }
}

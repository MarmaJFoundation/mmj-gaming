using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PageButton : CustomButton
{
    public int pageButton;
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        FindObjectOfType<BackpackController>().OnPageButtonEnter(pageButton == 1);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        FindObjectOfType<BackpackController>().OnPageButtonExit();
    }
}

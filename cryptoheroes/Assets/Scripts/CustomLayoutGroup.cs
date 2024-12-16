using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomLayoutGroup : MonoBehaviour
{
    public RectTransform[] elements;
    public RectTransform parent;
    public float posOffset;
    public float sizeOffset;
    private float lastScreenHeight;
    private void OnEnable()
    {
        OrganizeElements();
    }
    private void Update()
    {
        if (lastScreenHeight != Screen.height)
        {
            lastScreenHeight = Screen.height;
            OrganizeElements();
        }
    }
    private void OrganizeElements()
    {
        float size = parent.sizeDelta.y - sizeOffset;
        for (int i = 0; i < elements.Length; i++)
        {
            elements[i].anchoredPosition = new Vector2(0, posOffset + (size / elements.Length * (i+1)) + (elements[i].sizeDelta.y / 2) - (size / 2));
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CustomText : MonoBehaviour
{
    public string stringInput = "";
    public bool smallLetters;
    public bool outline;
    public bool lightOutline;
    public Color textColor = Color.white;
    [HideInInspector]
    public Color lastColor = Color.white;
    [HideInInspector]
    public string lastString = "";

    private BaseUtils baseUtils;

    private void Start()
    {
        if (!Application.isPlaying)
        {
            baseUtils = FindObjectOfType<BaseUtils>();
            baseUtils.SetLetterDicts();
        }
        UpdateText();
    }
    public void SetString(Color goColor)
    {
        if (textColor == goColor)
        {
            return;
        }
        textColor = goColor;
        UpdateText();
    }
    public void SetString(string gotoString)
    {
        if (stringInput == gotoString)
        {
            return;
        }
        stringInput = gotoString;
        UpdateText();
    }
    public void SetString(string gotoString, bool outline)
    {
        if (stringInput == gotoString)
        {
            return;
        }
        this.outline = outline;
        stringInput = gotoString;
        UpdateText();
    }
    public void SetString(string gotoString, Color goColor)
    {
        if (stringInput == gotoString && textColor == goColor)
        {
            return;
        }
        stringInput = gotoString;
        textColor = goColor;
        UpdateText();
    }
    private void Update()
    {
        if (!Application.isPlaying && stringInput != "" && (stringInput != lastString || textColor != lastColor))
        {
            UpdateText();
        }
    }
    public void UpdateText()
    {
        if (baseUtils == null)
        {
            baseUtils = FindObjectOfType<BaseUtils>();
        }
        if (BaseUtils.mediumLetterDict.Count == 0)
        {
            baseUtils.SetLetterDicts();
        }
        lastString = stringInput;
        lastColor = textColor;
        Image[] allChars = GetComponentsInChildren<Image>();
        for (int i = allChars.Length - 1; i >= 0; i--)
        {
            if (allChars[i].name.Contains("char"))
            {
                if (Application.isPlaying)
                {
                    /*if (smallLetters)
                    {
                        BaseUtils.smallLetterPool[allChars[i].name[0]].Enqueue(allChars[i]);
                    }
                    else
                    {
                        BaseUtils.mediumLetterPool[allChars[i].name[0]].Enqueue(allChars[i]);
                    }
                    allChars[i].transform.SetParent(BaseUtils.restingCanvas);*/
                    Destroy(allChars[i].gameObject);
                }
                else
                {
                    DestroyImmediate(allChars[i].gameObject);
                }
            }
        }
        if (smallLetters)
        {
            AddCharImage(BaseUtils.smallLetterDict, BaseUtils.smallLetterPool, stringInput.ToLower());
        }
        else
        {
            AddCharImage(BaseUtils.mediumLetterDict, BaseUtils.mediumLetterPool, stringInput.ToLower());
        }
    }

    private void AddCharImage(Dictionary<char, Sprite> letterDict, Dictionary<char, Queue<Image>> letterPool, string finalString)
    {
        for (int i = 0; i < finalString.Length; i++)
        {
            char gotoChar = letterDict.ContainsKey(finalString[i]) ? finalString[i] : '?';
            /*if (!letterDict.ContainsKey(finalString[i]))
            {
                continue;
            }*/
            Image letterImage;
            /*if (Application.isPlaying && letterPool[gotoChar].Count > 0)
            {
                letterImage = letterPool[gotoChar].Dequeue();
                letterImage.transform.SetParent(transform);
                letterImage.transform.localPosition = Vector3.zero;
                letterImage.transform.localScale = Vector3.one;
                letterImage.transform.localRotation = Quaternion.identity;
                letterImage.color = textColor;
                letterImage.material = outline ? lightOutline ? baseUtils._lightoutUI : baseUtils._outlineUI : baseUtils._normalUI;
                letterImage.SetNativeSize();
                continue;
            }*/
            GameObject letterObj = new GameObject(gotoChar + "char");
            letterObj.transform.SetParent(transform);
            letterObj.transform.localScale = Vector3.one;
            letterObj.transform.localPosition = Vector3.zero;
            //letterObj.transform.localRotation = Quaternion.identity;
            letterImage = letterObj.AddComponent<Image>();
            letterImage.sprite = letterDict[gotoChar];
            letterImage.raycastTarget = false;
            letterImage.material = outline ? lightOutline ? baseUtils._lightoutUI : baseUtils._darklineUI : baseUtils._normalUI;
            /*if (outline)
            {
                letterImage.material.SetColor("_OutlineColor", outlineColor);
            }*/
            letterImage.color = textColor;
            letterImage.SetNativeSize();
        }
    }
}

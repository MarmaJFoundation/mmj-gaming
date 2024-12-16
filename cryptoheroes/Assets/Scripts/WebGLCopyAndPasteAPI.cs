using UnityEngine;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;

public class WebGLCopyAndPasteAPI
{
    [DllImport("__Internal")]
    private static extern void initWebGLCopyAndPaste(StringCallback cutCopyCallback, StringCallback pasteCallback);
    [DllImport("__Internal")]
    private static extern void passCopyToBrowser(string str);

    delegate void StringCallback(string content);


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void Init()
    {
        if (!Application.isEditor)
        {
            initWebGLCopyAndPaste(GetClipboard, ReceivePaste);
        }
    }
    private static void SendKey(string baseKey)
    {
        string appleKey = "%" + baseKey;
        string naturalKey = "^" + baseKey;
        var currentObj = EventSystem.current.currentSelectedGameObject;
        if (currentObj == null)
        {
            return;
        }
        var input = currentObj.GetComponent<CustomInput>();
        if (input != null)
        {
            input.OnPressKey(Event.KeyboardEvent(naturalKey).keyCode);
            input.OnPressKey(Event.KeyboardEvent(appleKey).keyCode);
            return;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(StringCallback))]
    private static void GetClipboard(string key)
    {
        //SendKey(key);
        passCopyToBrowser(GUIUtility.systemCopyBuffer);
    }

    [AOT.MonoPInvokeCallback(typeof(StringCallback))]
    private static void ReceivePaste(string str)
    {
        GUIUtility.systemCopyBuffer = str;
    }
}
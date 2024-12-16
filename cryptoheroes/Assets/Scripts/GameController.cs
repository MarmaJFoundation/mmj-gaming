using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Canvas mainGameCanvas;
    public GameObject mainGameObj;
    public GameCharController[] gameChars;
    private readonly Dictionary<ClassType, GameCharController> charDict = new Dictionary<ClassType, GameCharController>();
    private void Awake()
    {
        for (int i = 0; i < gameChars.Length; i++)
        {
            charDict.Add(gameChars[i].classType, gameChars[i]);
        }
    }
    public void Setup(bool fromGetData = false)
    {
        mainGameCanvas.enabled = true;
        mainGameCanvas.gameObject.SetActive(true);
        mainGameObj.SetActive(true);
        for (int i = 0; i < gameChars.Length; i++)
        {
            gameChars[i].Setup(fromGetData);
        }
    }
    public void FixHpBars()
    {
        for (int i = 0; i < gameChars.Length; i++)
        {
            gameChars[i].FixHpBars();
        }
    }
    public void Dispose()
    {
        mainGameCanvas.enabled = false;
        mainGameCanvas.gameObject.SetActive(false);
        mainGameObj.SetActive(false);
    }
    public void UpdateCharacter(ClassType classType)
    {
        charDict[classType].UpdateItems();
    }
}

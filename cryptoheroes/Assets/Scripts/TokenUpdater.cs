using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenUpdater : MonoBehaviour
{
    public CustomText customText;
    public string prefix;
    private int lastTokenAmount = -1;
    private Coroutine lerpCoroutine;
    private void Update()
    {
        if (lastTokenAmount != Database.databaseStruct.pixelTokens)
        {
            if (lerpCoroutine != null)
            {
                StopCoroutine(lerpCoroutine);
            }
            lerpCoroutine = StartCoroutine(UpdateText(lastTokenAmount, Database.databaseStruct.pixelTokens));
            lastTokenAmount = Database.databaseStruct.pixelTokens;
        }
    }
    private IEnumerator UpdateText(float lastToken, float gotoToken)
    {
        float timer = 0;
        while (timer <= 1)
        {
            customText.SetString($"{prefix} balance: {Mathf.RoundToInt(Mathf.Lerp(lastToken, gotoToken, timer))} @");
            timer += Time.deltaTime * 5;
            yield return null;
        }
        customText.SetString($"{prefix} balance: {gotoToken} @");
    }
}

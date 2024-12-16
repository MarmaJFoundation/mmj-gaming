using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightBalanceUpdater : MonoBehaviour
{
    public CustomText customText;
    private int lastAmount = -1;
    private Coroutine lerpCoroutine;
    private void Update()
    {
        if (lastAmount != Database.databaseStruct.fightBalance)
        {
            if (lerpCoroutine != null)
            {
                StopCoroutine(lerpCoroutine);
            }
            lerpCoroutine = StartCoroutine(UpdateText(lastAmount, Database.databaseStruct.fightBalance));
            lastAmount = Database.databaseStruct.fightBalance;
        }
    }
    private IEnumerator UpdateText(float lastToken, float gotoToken)
    {
        float timer = 0;
        while (timer <= 1)
        {
            customText.SetString($"{Mathf.RoundToInt(Mathf.Lerp(lastToken, gotoToken, timer))} left");
            timer += Time.deltaTime * 5;
            yield return null;
        }
        customText.SetString($"{gotoToken} left");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarController : MonoBehaviour
{
    public RectTransform rectTransform;
    public CustomText hpText;
    public CustomText nameText;
    public Image hpBar;
    private Transform followTransform;
    private int maxHealth;
    private int currentHealth;
    private int height;
    public bool staticBar;
    public void Setup(Transform followTransform, int height, int maxHealth, string name)
    {
        this.followTransform = followTransform;
        this.maxHealth = maxHealth;
        this.height = height;
        gameObject.SetActive(true);
        currentHealth = maxHealth;
        nameText.SetString(name);
        hpText.SetString($"HP:{Mathf.Clamp(currentHealth, 0, Mathf.Infinity)}/{maxHealth}");
        hpBar.transform.localScale = new Vector3(Mathf.Clamp01(currentHealth / maxHealth), 1, 1);
    }
    private void LateUpdate()
    {
        //rectTransform.position = BaseUtils.mainCam.WorldToScreenPoint(followTransform.position + Vector3.up * 4 + Vector3.forward * 10);
        if (!staticBar)
        {
            rectTransform.position = followTransform.position + Vector3.up * height + Vector3.forward * 10;
        }
    }
    public void UpdateHealth(int currentHealth)
    {
        int fromHealth = this.currentHealth;
        this.currentHealth = currentHealth;
        hpText.SetString($"HP:{fromHealth}/{maxHealth}");
        hpBar.transform.localScale = new Vector3(1, Mathf.Clamp01(fromHealth / (float)maxHealth), 1);
        StartCoroutine(AnimateHPBar(fromHealth));
    }
    private IEnumerator AnimateHPBar(float fromHealth)
    {
        float timer = 0;
        while (timer <= 1)
        {
            int lerpHP = Mathf.RoundToInt(Mathf.Lerp(fromHealth, currentHealth, timer.Evaluate(CurveType.EaseOut)));
            hpText.SetString($"HP:{Mathf.Clamp(lerpHP, 0, Mathf.Infinity)}/{maxHealth}");
            hpBar.transform.localScale = new Vector3(Mathf.Clamp01(lerpHP / (float)maxHealth), 1, 1);
            timer += Time.deltaTime * 3;
            yield return null;
        }
        hpText.SetString($"HP:{Mathf.Clamp(currentHealth, 0, Mathf.Infinity)}/{maxHealth}");
        hpBar.transform.localScale = new Vector3(Mathf.Clamp01(currentHealth / (float)maxHealth), 1, 1);
    }
}

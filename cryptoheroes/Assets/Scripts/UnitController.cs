using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public Transform pivotTransform;
    [HideInInspector]
    public HealthbarController healthbarController;
    private MainMenuController mainMenuController;
    [HideInInspector]
    public EffectType gustEffect;
    [HideInInspector]
    public EffectType attackEffect;
    [HideInInspector]
    public EffectType projectileEffect;
    [HideInInspector]
    public EffectType deathEffect;
    [HideInInspector]
    public EffectType dyingEffect;
    public StatStruct statStruct;
    [HideInInspector]
    public int currentHealth;
    public bool slides;
    public bool staticKnight;
    public bool isBoss;
    [HideInInspector]
    public bool died;
    public int[] attackRotations = new int[3] { 50, 60, 30 };
    public int moveRotation = 7;
    private SpriteRenderer[] spriteRenderers;
    public virtual void Setup(MainMenuController mainMenuController, string name, int height, bool hasHealthbar = true)
    {
        this.mainMenuController = mainMenuController;
        currentHealth = statStruct.maxHealth;
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        if (hasHealthbar)
        {
            GameObject healthBarObj = Instantiate(BaseUtils.healthbarPrefab, BaseUtils.mainCanvas);
            healthbarController = healthBarObj.GetComponent<HealthbarController>();
        }
        if (healthbarController != null)
        {
            healthbarController.Setup(transform, height, statStruct.maxHealth, name);
        }
        died = false;
    }
    public void OnDamage(bool critted, bool dodged, int damage, int lifeSteal, UnitController attackerController)
    {
        if (dodged)
        {
            SoundController.PlaySound("Trash_item_var", 1, true);
            StartCoroutine(OnDodgeCoroutine());
            return;
        }
        currentHealth -= damage;
        healthbarController.UpdateHealth(currentHealth);
        BaseUtils.InstantiateEffect(attackerController.attackEffect, transform.position + Vector3.up * 40f.ToScale(), true, damage.ToEffectScale());
        mainMenuController.InstantiateText(transform, damage.ToString(), damage, BaseUtils.damageColor, false);
        if (lifeSteal > 0)
        {
            SoundController.PlaySound("Potion_sound_var", .25f, true);
            attackerController.currentHealth += lifeSteal;
            attackerController.StartCoroutine(HealAfterDelay(attackerController, lifeSteal));
        }
        if (critted)
        {
            SoundController.PlaySound("Critical_hit", 1, true);
            StartCoroutine(ShowCritAfterDelay());
        }
        switch (attackerController.attackEffect)
        {
            case EffectType.MageExplo:
                SoundController.PlaySound("magexplo", .5f, true);
                break;
            case EffectType.KnightExplo:
                SoundController.PlaySound("Attack_Punch", .5f, true);
                SoundController.PlaySound("Trash_item", .5f, true);
                break;
            case EffectType.RangerExplo:
                SoundController.PlaySound("Attack_Slash", .5f, true);
                SoundController.PlaySound("Trash_item_var", .5f, true);
                break;
            case EffectType.ShamanExplo:
                SoundController.PlaySound("Boss_magic_projectile_var", .5f, true);
                break;
            case EffectType.AssassinExplo:
                SoundController.PlaySound("Attack_Slash", .5f, true);
                SoundController.PlaySound("Trash_item_var", .5f, true);
                break;
            case EffectType.WarriorExplo:
                SoundController.PlaySound("Attack_Punch", .5f, true);
                SoundController.PlaySound("Trash_item", .5f, true);
                break;
            case EffectType.BruteExplo:
                SoundController.PlaySound("Attack_Punch_var", .5f, true);
                break;
            case EffectType.RatBossExplo:
                SoundController.PlaySound("Boss_magic_projectile_var", .5f, true);
                break;
            case EffectType.RatSoldierExplo:
                SoundController.PlaySound("Attack_Whack", .5f, true);
                break;
            case EffectType.DemonExplo:
                SoundController.PlaySound("Fire_slash", .5f, true);
                break;
            case EffectType.DemonBossExplo:
                SoundController.PlaySound("Attack_Punch", .5f, true);
                SoundController.PlaySound("Fire_slash", .5f, true);
                break;
            case EffectType.ReaperExplo:
                SoundController.PlaySound("Attack_Punch", .5f, true);
                SoundController.PlaySound("Boss_magic_projectile", .5f, true);
                break;
            case EffectType.ReaperBossExplo:
                SoundController.PlaySound("Attack_Punch", .5f, true);
                SoundController.PlaySound("Boss_magic_projectile_v", .5f, true);
                SoundController.PlaySound("Mage_projectile_3", .5f, true);
                break;
            case EffectType.BossExplo:
                SoundController.PlaySound("Attack_Punch", .5f, true);
                SoundController.PlaySound("Attack_Punch_var", .5f, true);
                break;
        }
        if (currentHealth > 0)
        {
            if (name.Contains("Demon1"))
            {
                SoundController.PlaySound("bosshit", .5f, true);
            }
            else if (name.Contains("Demon2"))
            {
                SoundController.PlaySound("bosshithigh", .5f, true);
            }
            else if (name.Contains("GoblinBoss"))
            {
                SoundController.PlaySound("bosshit", .5f, true);
            }
            else if (name.Contains("Goblin"))
            {
                SoundController.PlaySound("Monster_hit", .5f, true);
            }
            else if (name.Contains("Knight") || name.Contains("Mage") || name.Contains("Ranger"))
            {
                SoundController.PlaySound($"HumanHit_{BaseUtils.RandomInt(1, 4)}", .5f, true);
            }
            else if (name.Contains("RatBoss"))
            {
                SoundController.PlaySound("bosshithigh", .5f, true);
            }
            else if (name.Contains("Rat"))
            {
                SoundController.PlaySound("Monster_hithigh", .5f, true);
            }
            else if (name.Contains("Reaper3"))
            {
                SoundController.PlaySound("finalbosshit", .5f, true);
            }
            else if (name.Contains("Reaper"))
            {
                SoundController.PlaySound("finalhit", .5f, true);
            }
            StartCoroutine(DamageCoroutine(critted));
        }
        else
        {
            OnDeath();
        }
    }
    private IEnumerator ShowCritAfterDelay()
    {
        yield return new WaitForSeconds(.25f);
        mainMenuController.InstantiateText(transform, "crit!", 100, Color.white, false);
    }
    private IEnumerator HealAfterDelay(UnitController attackerController, int lifeStealDamage)
    {
        yield return new WaitForSeconds(.25f);
        attackerController.healthbarController.UpdateHealth(attackerController.currentHealth);
        mainMenuController.InstantiateText(attackerController.transform, $"+{lifeStealDamage}", lifeStealDamage, BaseUtils.healColor, true);
    }
    public virtual void OnDeath()
    {
        died = true;
        if (name.Contains("Demon1"))
        {
            SoundController.PlaySound("Monster_death", .5f, true);
        }
        else if (name.Contains("Demon2"))
        {
            SoundController.PlaySound("MonsterHit_2", .5f, true);
        }
        else if (name.Contains("DemonBoss"))
        {
            SoundController.PlaySound("MonsterHit_3", 1, true);
        }
        else if (name.Contains("GoblinBoss"))
        {
            SoundController.PlaySound("Growl_3", 1, true);
        }
        else if (name.Contains("Knight") || name.Contains("Mage") || name.Contains("Ranger"))
        {
            SoundController.PlaySound($"HumanDeath_{BaseUtils.RandomInt(1, 4)}", 1, true);
        }
        else if (name.Contains("RatBoss"))
        {
            SoundController.PlaySound("ratdeath", 1, true);
        }
        else if (name.Contains("Rat") || name.Contains("Goblin"))
        {
            SoundController.PlaySound("Monster_death_high", .5f, true);
        }
        else if (name.Contains("Reaper3"))
        {
            SoundController.PlaySound("MonsterDeath_2", 1, true);
        }
        else if (name.Contains("Reaper"))
        {
            SoundController.PlaySound("MonsterDeath_3", .5f, true);
        }
        StartCoroutine(DeathCoroutine());
    }
    public void SetColor(Color goColor)
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i].gameObject.layer == 3)
            {
                continue;
            }
            spriteRenderers[i].color = goColor;
        }
    }
    private IEnumerator OnDodgeCoroutine()
    {
        mainMenuController.InstantiateText(transform, "dodge!", 0, Color.white, true);
        float timer = 0;
        Vector3 fromPos = transform.position;
        float sideMult = transform.position.x < 0 ? -2.5f : 2.5f;
        while (timer <= 1)
        {
            transform.position = Vector3.Lerp(fromPos, fromPos + Vector3.right * sideMult, timer.Evaluate(CurveType.PeakParabol));
            pivotTransform.localEulerAngles = Vector3.LerpUnclamped(Vector3.zero, Vector3.forward * sideMult * -5, timer.Evaluate(CurveType.PeakParabol));
            timer += Time.deltaTime * 3f;
            yield return null;
        }
        transform.position = fromPos;
    }
    private IEnumerator DeathCoroutine()
    {
        BaseUtils.InstantiateEffect(dyingEffect, transform.position + Vector3.up * 10f.ToScale(), spriteRenderers[0]);
        float timer = 0;
        Vector3 fromPos = transform.position;
        Vector3 gotoPos = fromPos + .5f * BaseUtils.RandomSign() * Vector3.right;
        int sideMult = transform.position.x < 0 ? 75 : -75;
        while (timer <= 1)
        {
            float upOffset = Mathf.Lerp(0, .35f, timer.Evaluate(CurveType.JumpCurve));
            transform.position = fromPos + Vector3.up * upOffset;
            SetColor(Color.Lerp(Color.white, BaseUtils.damageColor, timer.Evaluate(CurveType.JumpCurve)));
            timer += Time.deltaTime * 8f;
            yield return null;
        }
        timer = 0;
        while (timer <= 1)
        {
            if (!isBoss)
            {
                pivotTransform.localEulerAngles = Vector3.LerpUnclamped(Vector3.zero, Vector3.forward * sideMult, timer.Evaluate(CurveType.EaseIn));
            }
            SetColor(Color.Lerp(Color.white, Color.red, timer.Evaluate(CurveType.DeathCurve)));
            transform.position = Vector3.LerpUnclamped(fromPos, gotoPos, timer.Evaluate(CurveType.DeathCurve));
            timer += Time.deltaTime * 1.5f;
            yield return null;
        }
        BaseUtils.InstantiateEffect(deathEffect, transform.position + Vector3.up * 10f.ToScale(), spriteRenderers[0]);
        SetColor(Color.white);
        OnDeathFinish();
    }

    public virtual void OnDeathFinish()
    {
        StopAllCoroutines();
        Destroy(healthbarController.gameObject);
        healthbarController = null;
        Destroy(gameObject);
    }

    private IEnumerator DamageCoroutine(bool critted)
    {
        float timer = 0;
        Vector3 fromPos = transform.position;
        float sideMult = transform.position.x < 0 ? 15 : -15;
        Vector3 shakePos = Vector3.right * sideMult * .2f;
        Vector3 fromScale = new Vector3(pivotTransform.transform.localScale.x, 1, 1);
        Vector3 gotoScale = fromScale * 1.1f;
        if (critted)
        {
            gotoScale *= 1.5f;
            shakePos *= 1.5f;
            sideMult *= 1.5f;
            //dungeonController.cameraController.ShakeCamera(5);
        }
        while (timer <= 1)
        {
            float upOffset = Mathf.Lerp(0, 1, timer.Evaluate(CurveType.JumpCurve));
            transform.position = Vector3.LerpUnclamped(fromPos, fromPos + shakePos + Vector3.up * upOffset, timer.Evaluate(CurveType.ShakeCurve));
            if (!isBoss)
            {
                pivotTransform.localEulerAngles = Vector3.LerpUnclamped(Vector3.zero, Vector3.forward * sideMult, timer.Evaluate(CurveType.DamagedCurve));
                pivotTransform.localScale = Vector3.LerpUnclamped(fromScale, gotoScale, timer.Evaluate(CurveType.DamagedCurve));
            }
            SetColor(Color.Lerp(Color.white, BaseUtils.damageColor, timer.Evaluate(CurveType.DamagedCurve)));
            timer += Time.deltaTime * 4;
            yield return null;
        }
        SetColor(Color.white);
        transform.position = fromPos;
        pivotTransform.localEulerAngles = Vector3.zero;
        pivotTransform.localScale = fromScale;
    }
}

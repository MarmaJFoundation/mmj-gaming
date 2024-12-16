using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    public NearHelper nearHelper;
    public CameraController cameraController;
    public MainMenuController mainMenuController;
    public GameController gameController;
    [HideInInspector]
    public bool timeLocked;
    public GameObject victoryWindow;
    public GameObject defeatWindow;
    public GameObject speedWindow;
    public Image[] buttonImages;
    public Image[] buttonArrows;
    public Image lockButtonImage;
    public Sprite[] lockSprites;
    public CustomText timerText;
    public ClassType classType;
    public int difficulty;
    public int ClassIndex
    {
        get
        {
            return (int)classType - 1;
        }
    }
    public GameCharController CharController
    {
        get
        {
            return gameController.gameChars[ClassIndex];
        }
    }
    public IEnumerator DefeatCoroutine()
    {
        Time.timeScale = 1;
        UnwarpTime();
        speedWindow.SetActive(false);
        defeatWindow.SetActive(true);
        SoundController.StopMusic();
        SoundController.PlaySound("Defeat_sound");
        mainMenuController.SetupBlackScreen();
        float timer = 0;
        Image[] windowImages = defeatWindow.GetComponentsInChildren<Image>();
        BaseUtils.InstantiateEffect(EffectType.DefeatExplosion, Vector3.up * cameraController.transform.position.y, false);
        while (timer <= 1)
        {
            foreach (Image image in windowImages)
            {
                image.color = Color.Lerp(Color.clear, Color.white, timer.Evaluate(CurveType.EaseOut));
            }
            timer += Time.deltaTime * 3f;
            yield return null;
        }
        foreach (Image image in windowImages)
        {
            image.color = Color.white;
        }
        yield return new WaitForSeconds(2f);
        timer = 0;
        while (timer <= 1)
        {
            foreach (Image image in windowImages)
            {
                image.color = Color.Lerp(Color.white, Color.clear, timer.Evaluate(CurveType.EaseIn));
            }
            timer += Time.deltaTime * 3f;
            yield return null;
        }
        defeatWindow.SetActive(false);
        mainMenuController.RemoveBlackScreen();
        mainMenuController.FadeInBlack();
        yield return new WaitForSeconds(.25f);
        ExitBattle(false);
    }
    public virtual void ExitBattle(bool victory)
    {

    }
    public IEnumerator IdleCoroutine(Transform pivotTransform)
    {
        yield return new WaitForSeconds(BaseUtils.RandomFloat(0, 1));
        while (true)
        {
            float timer = 0;
            while (timer <= 1)
            {
                float goScale = Mathf.Lerp(1, 1.06f, timer.Evaluate(CurveType.SmoothParabol));
                pivotTransform.localScale = new Vector3(pivotTransform.localScale.x, Mathf.Round(goScale * 150) / 150, 1);
                timer += Time.deltaTime * .4f;
                yield return null;
            }
        }
    }
    public IEnumerator DexAttackCoroutine(bool critted, bool dodged, int damage, int lifeSteal, UnitController attackerController, UnitController targetController, float heightOffset = 0)
    {
        BaseUtils.InstantiateEffect(attackerController.gustEffect, attackerController.transform.position + Vector3.up * 30f.ToScale(), true);
        if (critted && !dodged)
        {
            cameraController.CritZoom(targetController.transform, .5f);
        }
        float timer = 0;
        Vector3 fromPos = attackerController.transform.position;
        int sideMult = attackerController.transform.position.x > 0 ? attackerController.attackRotations[0] : -attackerController.attackRotations[0];
        bool hitted = false;
        float fromScale = attackerController.pivotTransform.transform.localScale.x;
        while (timer <= 1)
        {
            attackerController.transform.position = Vector3.LerpUnclamped(fromPos, targetController.transform.position + Vector3.up * heightOffset, timer.Evaluate(CurveType.AttackCurve));
            attackerController.pivotTransform.localEulerAngles = Vector3.LerpUnclamped(Vector3.zero, Vector3.forward * sideMult, timer.Evaluate(CurveType.DexAttackRot));
            attackerController.pivotTransform.localScale = Vector3.LerpUnclamped(new Vector3(fromScale, 1, 1), new Vector3(fromScale, 1, 1) * 1.1f, timer.Evaluate(CurveType.AttackCurve));
            if (timer >= .5f && !hitted)
            {
                hitted = true;
                targetController.OnDamage(critted, dodged, damage, lifeSteal, attackerController);
            }
            timer += Time.deltaTime * 3.5f;
            yield return null;
        }
        if (!hitted)
        {
            targetController.OnDamage(critted, dodged, damage, lifeSteal, attackerController);
        }
        attackerController.transform.position = fromPos;
        attackerController.pivotTransform.localEulerAngles = Vector3.zero;
        attackerController.pivotTransform.localScale = new Vector3(fromScale, 1, 1);
    }
    public IEnumerator MagicAttackCoroutine(bool critted, bool dodged, int damage, int lifeSteal, UnitController attackerController, UnitController targetController, float heightOffset = 0)
    {
        BaseUtils.InstantiateEffect(attackerController.gustEffect, attackerController.transform.position + Vector3.up * 30f.ToScale(), true);
        if (critted && !dodged)
        {
            cameraController.CritZoom(targetController.transform, .6f);
        }
        float timer = 0;
        int sideMult = attackerController.transform.position.x > 0 ? attackerController.attackRotations[1] : -attackerController.attackRotations[1];
        float fromScale = attackerController.pivotTransform.localScale.x;
        bool shoot = false;
        while (timer <= 1)
        {
            attackerController.pivotTransform.localEulerAngles = Vector3.LerpUnclamped(Vector3.zero, Vector3.forward * sideMult, timer.Evaluate(CurveType.MagicAttackRot));
            attackerController.pivotTransform.localScale = Vector3.LerpUnclamped(new Vector3(fromScale, 1, 1), new Vector3(fromScale, 1.2f, 1), timer.Evaluate(CurveType.MagicAttackScale));
            if (timer >= .5f && !shoot)
            {
                shoot = true;
                PlayProjectileSound(attackerController);
                BaseUtils.InstantiateEffect(
                    attackerController.projectileEffect,
                    attackerController.transform.position + Vector3.up * 40f.ToScale(),
                    targetController.transform.position + Vector3.up * 60f.ToScale() + Vector3.up * heightOffset,
                    true,
                    damage.ToEffectScale());
            }
            timer += Time.deltaTime * 3.5f;
            yield return null;
        }
        if (!shoot)
        {
            PlayProjectileSound(attackerController);
            BaseUtils.InstantiateEffect(
                attackerController.projectileEffect,
                attackerController.transform.position + Vector3.up * 40f.ToScale(),
                targetController.transform.position + Vector3.up * 60f.ToScale() + Vector3.up * heightOffset,
                true,
                damage.ToEffectScale());
        }
        attackerController.pivotTransform.localEulerAngles = Vector3.zero;
        attackerController.pivotTransform.localScale = new Vector3(fromScale, 1, 1);
        targetController.OnDamage(critted, dodged, damage, lifeSteal, attackerController);
    }

    private void PlayProjectileSound(UnitController attackerController)
    {
        switch (attackerController.projectileEffect)
        {
            case EffectType.MageProjectile:
                SoundController.PlaySound($"Mage_projectile_{BaseUtils.RandomInt(1, 3)}", .5f, true);
                break;
            case EffectType.ShamanProjectile:
                SoundController.PlaySound($"Boss_magic_projectile", .5f, true);
                break;
            case EffectType.RatBossProjectile:
                SoundController.PlaySound($"Boss_magic_projectile_v", .5f, true);
                break;
            case EffectType.RatSoldierProjectile:
                SoundController.PlaySound($"Tab_close", .5f, true);
                break;
            case EffectType.DemonProjectile:
                SoundController.PlaySound($"Fire_whip", .5f, true);
                break;
        }
    }

    public IEnumerator MeleeAttackCoroutine(bool critted, bool dodged, int damage, int lifeSteal, UnitController attackerController, UnitController targetController, float heightOffset = 0)
    {
        BaseUtils.InstantiateEffect(attackerController.gustEffect, attackerController.transform.position + Vector3.up * 30f.ToScale(), true);
        if (critted && !dodged)
        {
            cameraController.CritZoom(targetController.transform, .5f);
        }
        float timer = 0;
        Vector3 fromPos = attackerController.transform.position;
        int sideMult = attackerController.transform.position.x > 0 ? attackerController.attackRotations[2] : -attackerController.attackRotations[2];
        bool hitted = false;
        float fromScale = attackerController.pivotTransform.transform.localScale.x;
        while (timer <= 1)
        {
            Vector3 gotoScale = new Vector3(fromScale, 1, 1) * 1.1f;
            if (!attackerController.staticKnight && timer > .4f && timer < .75f)
            {
                gotoScale = new Vector3(gotoScale.x * -1, gotoScale.y, gotoScale.z);
            }
            attackerController.transform.position = Vector3.LerpUnclamped(fromPos, targetController.transform.position + Vector3.up * heightOffset, timer.Evaluate(CurveType.AttackCurve));
            attackerController.pivotTransform.localEulerAngles = Vector3.LerpUnclamped(Vector3.zero, Vector3.forward * sideMult, timer.Evaluate(CurveType.AttackCurve));
            attackerController.pivotTransform.localScale = Vector3.LerpUnclamped(new Vector3(fromScale, 1, 1), gotoScale, timer.Evaluate(CurveType.MeleeAttackCurve));
            if (timer >= .5f && !hitted)
            {
                hitted = true;
                targetController.OnDamage(critted, dodged, damage, lifeSteal, attackerController);
            }
            timer += Time.deltaTime * 3.5f;
            yield return null;
        }
        if (!hitted)
        {
            targetController.OnDamage(critted, dodged, damage, lifeSteal, attackerController);
        }
        attackerController.transform.position = fromPos;
        attackerController.pivotTransform.localEulerAngles = Vector3.zero;
        attackerController.pivotTransform.localScale = new Vector3(fromScale, 1, 1);
    }
    public void UnwarpTime()
    {
        StartCoroutine(UnwarpTimeCoroutine());
    }
    private IEnumerator UnwarpTimeCoroutine()
    {
        timeLocked = true;
        float timer = 0;
        while (timer <= 1)
        {
            Time.timeScale = Mathf.Lerp(0, 1, timer.Evaluate(CurveType.EaseIn));
            timer += Time.unscaledDeltaTime * .5f;
            yield return null;
        }
        Time.timeScale = 1;
        timeLocked = false;
    }
    public void OnLockButtonClick()
    {
        if (timeLocked)
        {
            return;
        }
        Database.databaseStruct.lockSpeed = Database.databaseStruct.lockSpeed == -1 ? (int)Time.timeScale : -1;
        lockButtonImage.sprite = Database.databaseStruct.lockSpeed == -1 ? lockSprites[0] : lockSprites[1];
        Database.SaveDatabase();
    }
    public void OnTimeButtonClick(bool advancing)
    {
        if (timeLocked)
        {
            return;
        }
        if (advancing)
        {
            Time.timeScale++;
        }
        else
        {
            Time.timeScale = 1;
        }
        Database.databaseStruct.lockSpeed = (int)Time.timeScale;
        Database.SaveDatabase();
        SetTimerProperties();
    }
    public void SetTimerProperties()
    {
        if (Time.timeScale >= 10)
        {
            Time.timeScale = 10;
            buttonImages[1].sprite = BaseUtils.buttonSprites[1];
            buttonArrows[1].color = BaseUtils.disabledColor;
        }
        else
        {
            buttonImages[1].sprite = BaseUtils.buttonSprites[0];
            buttonArrows[1].color = BaseUtils.enabledColor;
        }
        if (Time.timeScale <= 1)
        {
            buttonImages[0].sprite = BaseUtils.buttonSprites[1];
            buttonArrows[0].color = BaseUtils.disabledColor;
        }
        else
        {
            buttonImages[0].sprite = BaseUtils.buttonSprites[0];
            buttonArrows[0].color = BaseUtils.enabledColor;
        }
        timerText.SetString($"time speed: {(int)Time.timeScale}x");
    }
    public IEnumerator RewardXP(bool victory)
    {
        int loopAmount = 1;
        switch (difficulty)
        {
            case 0:
                loopAmount = 1;
                break;
            case 1:
                loopAmount = 2;
                break;
            case 2:
                loopAmount = 4;
                break;
            case 3:
                loopAmount = 6;
                break;
        }
        for (int i = 0; i < loopAmount; i++)
        {
            switch (difficulty)
            {
                case 0:
                    CharController.characterInfo.UpdateXpBar(10, BaseUtils.enabledColor);
                    break;
                case 1:
                    CharController.characterInfo.UpdateXpBar((!victory && i != 0) ? 9 : 10, BaseUtils.enabledColor);
                    break;
                case 2:
                    CharController.characterInfo.UpdateXpBar(!victory ? 17 : 20, BaseUtils.enabledColor);
                    break;
                case 3:
                    CharController.characterInfo.UpdateXpBar(!victory ? 28 : 40, BaseUtils.enabledColor);
                    break;
            }
            yield return new WaitForSeconds(.2f);
        }
        if (BaseUtils.offlineMode)
        {
            int restReduction = Database.HasStaminaPotion(classType) ? Database.GetStaminaPotion(classType).strength : 0;
            switch (difficulty)
            {
                case 0:
                    Database.SetInjured(classType, DateTime.Now.AddMinutes(30 - restReduction));
                    break;
                case 1:
                    Database.SetInjured(classType, DateTime.Now.AddMinutes(60 - restReduction));
                    break;
                case 2:
                    Database.SetInjured(classType, DateTime.Now.AddMinutes(240 - restReduction));
                    break;
                case 3:
                    Database.SetInjured(classType, DateTime.Now.AddMinutes(720 - restReduction));
                    break;
            }
            gameController.Setup();
            BaseUtils.InstantiateEffect(EffectType.InjuredEffect, CharController.transform.position + Vector3.up * 25f.ToScale(), true);
        }
        else
        {
            yield return new WaitForSeconds(1);
            nearHelper.dataGetState = DataGetState.AfterFight;
            StartCoroutine(nearHelper.GetPlayerData());
        }
    }
}

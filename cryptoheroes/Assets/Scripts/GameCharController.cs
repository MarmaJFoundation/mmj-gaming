using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharController : UnitController
{
    public Material charMaterial;
    public Material grayCharMaterial;
    public ClassType classType;
    public CharacterInfoController characterInfo;
    public GameObject lightObj;
    public GameObject[] bodyItems;
    public GameObject[] helmetItems;
    public GameObject[] weaponItems;
    public GameObject[] bootsItems;
    public GameObject[] weapon2Items;
    public SpriteRenderer shadowSprite;
    private float potionTimer;
    private readonly Dictionary<EquipType, GameObject[]> slotDict = new Dictionary<EquipType, GameObject[]>();
    private void Awake()
    {
        slotDict.Add(EquipType.Armor, bodyItems);
        slotDict.Add(EquipType.Helmet, helmetItems);
        slotDict.Add(EquipType.Weapon, weaponItems);
        slotDict.Add(EquipType.Boots, bootsItems);
    }
    public void Setup(bool fromGetData = false)
    {
        StopAllCoroutines();
        foreach (SpriteAnimator spriteAnimator in GetComponentsInChildren<SpriteAnimator>())
        {
            spriteAnimator.StopAllCoroutines();
        }
        pivotTransform.localScale = Vector3.one;
        bool unlockedChar = Database.HasClass(classType);
        bool injuredChar = unlockedChar && Database.GetCharStruct(classType).injuredTimer > DateTime.Now;
        if (!unlockedChar || injuredChar)
        {
            foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>(true))
            {
                sprite.material = grayCharMaterial;
            }
            foreach (SpriteAnimator spriteAnimator in GetComponentsInChildren<SpriteAnimator>())
            {
                spriteAnimator.StopAllCoroutines();
            }
            lightObj.SetActive(false);
            if (injuredChar && fromGetData)
            {
                BaseUtils.InstantiateEffect(EffectType.InjuredEffect, transform.position + Vector3.up * 25f.ToScale(), true);
            }
        }
        else
        {
            foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>(true))
            {
                sprite.material = charMaterial;
            }
            lightObj.SetActive(true);
            StartCoroutine(IdleCoroutine());
            foreach (SpriteAnimator spriteAnimator in GetComponentsInChildren<SpriteAnimator>())
            {
                spriteAnimator.Setup();
            }
        }
        characterInfo.Setup(this, classType, unlockedChar, injuredChar);
        if (unlockedChar)
        {
            UpdateItems();
        }
    }
    private void Update()
    {
        if (potionTimer < .5f)
        {
            potionTimer += Time.deltaTime * BaseUtils.RandomFloat(.5f, 1);
            return;
        }
        potionTimer = 0;
        if (Database.HasLuckPotion(classType) && BaseUtils.RandomBool())
        {
            BaseUtils.InstantiateEffect(EffectType.LuckPotionEffect, transform.position, true);
        }
        if (Database.HasStaminaPotion(classType) && BaseUtils.RandomBool())
        {
            BaseUtils.InstantiateEffect(EffectType.StaminaPotionEffect, transform.position, true);
        }
        if (Database.HasStrengthPotion(classType) && BaseUtils.RandomBool())
        {
            BaseUtils.InstantiateEffect(EffectType.StrengthPotionEffect, transform.position, true);
        }
    }
    public void FixHpBars()
    {
        characterInfo.FixHpBars();
    }
    public void SetCharStruct()
    {
        statStruct = BaseUtils.GenerateCharStruct(classType, Database.GetCharStruct(classType).level, Database.databaseStruct.ownedItems);
    }
    public void Setup(MainMenuController mainMenuController, string name, int height, StatStruct statStruct, List<ItemType> equippedItems)
    {
        this.statStruct = statStruct;
        deathEffect = EffectType.DeathExplosion;
        dyingEffect = EffectType.DyingExplosion;
        switch (classType)
        {
            case ClassType.Mage:
                attackEffect = EffectType.MageExplo;
                gustEffect = EffectType.MageGust;
                projectileEffect = EffectType.MageProjectile;
                break;
            case ClassType.Knight:
                gustEffect = EffectType.KnightGust;
                attackEffect = EffectType.KnightExplo;
                break;
            case ClassType.Ranger:
                gustEffect = EffectType.RangerGust;
                attackEffect = EffectType.RangerExplo;
                break;
        }
        bool[] equippedSlots = new bool[4];
        for (int i = 0; i < equippedItems.Count; i++)
        {
            ScriptableItem scriptableItem = BaseUtils.itemDict[equippedItems[i]];
            if (scriptableItem.equipType != EquipType.Necklace && scriptableItem.equipType != EquipType.Ring)
            {
                equippedSlots[(int)scriptableItem.equipType] = true;
                SetSpriteIndexes(scriptableItem.equipType, scriptableItem.slotIndex);
            }
        }
        for (int i = 0; i < 4; i++)
        {
            if (!equippedSlots[i])
            {
                SetSpriteIndexes((EquipType)i, 0);
            }
        }
        foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>(true))
        {
            sprite.material = charMaterial;
        }
        StartCoroutine(IdleCoroutine());
        foreach (SpriteAnimator spriteAnimator in GetComponentsInChildren<SpriteAnimator>())
        {
            spriteAnimator.Setup();
        }
        base.Setup(mainMenuController, name, height, false);
    }
    public override void Setup(MainMenuController mainMenuController, string name, int height, bool hasHealthbar = true)
    {
        SetCharStruct();
        deathEffect = EffectType.DeathExplosion;
        dyingEffect = EffectType.DyingExplosion;
        switch (classType)
        {
            case ClassType.Mage:
                attackEffect = EffectType.MageExplo;
                gustEffect = EffectType.MageGust;
                projectileEffect = EffectType.MageProjectile;
                break;
            case ClassType.Knight:
                gustEffect = EffectType.KnightGust;
                attackEffect = EffectType.KnightExplo;
                break;
            case ClassType.Ranger:
                gustEffect = EffectType.RangerGust;
                attackEffect = EffectType.RangerExplo;
                break;
        }
        base.Setup(mainMenuController, classType.ToString(), 4, hasHealthbar);
    }
    public override void OnDeathFinish()
    {
        StopAllCoroutines();
        if (!healthbarController.staticBar)
        {
            Destroy(healthbarController.gameObject);
        }
        else
        {
            healthbarController.gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }
    public IEnumerator IdleCoroutine()
    {
        yield return new WaitForSeconds(BaseUtils.RandomFloat(0, 1));
        while (true)
        {
            float timer = 0;
            while (timer <= 1)
            {
                float goScale = Mathf.Lerp(1, 1.06f, timer.Evaluate(CurveType.SmoothParabol));
                pivotTransform.localScale = new Vector3(pivotTransform.localScale.x, Mathf.Round(goScale * 150) / 150, 1);
                timer += Time.deltaTime * .2f;
                yield return null;
            }
        }
    }
    public void UpdateItems()
    {
        bool[] equippedSlots = new bool[4];
        for (int i = 0; i < Database.databaseStruct.ownedItems.Count; i++)
        {
            if (Database.databaseStruct.ownedItems[i].equipClass == classType)
            {
                ScriptableItem scriptableItem = BaseUtils.itemDict[Database.databaseStruct.ownedItems[i].itemType];
                if (scriptableItem.equipType != EquipType.Necklace && scriptableItem.equipType != EquipType.Ring)
                {
                    equippedSlots[(int)scriptableItem.equipType] = true;
                    SetSpriteIndexes(scriptableItem.equipType, scriptableItem.slotIndex);
                }
            }
        }
        for (int i = 0; i < 4; i++)
        {
            if (!equippedSlots[i])
            {
                SetSpriteIndexes((EquipType)i, 0);
            }
        }
    }
    private void SetSpriteIndexes(EquipType equipType, int slotIndex)
    {
        bool injuredChar = Database.GetCharStruct(classType).injuredTimer > DateTime.Now;
        for (int k = 0; k < slotDict[equipType].Length; k++)
        {
            slotDict[equipType][k].SetActive(k == slotIndex);
            if (injuredChar && slotDict[equipType][k].TryGetComponent(out SpriteAnimator spriteAnimator))
            {
                spriteAnimator.StopAllCoroutines();
            }
        }
        if (equipType == EquipType.Weapon && classType == ClassType.Mage)
        {
            for (int k = 0; k < weapon2Items.Length; k++)
            {
                weapon2Items[k].SetActive(k == slotIndex);
                if (injuredChar && weapon2Items[k].TryGetComponent(out SpriteAnimator spriteAnimator))
                {
                    spriteAnimator.StopAllCoroutines();
                }
            }
        }
    }
}

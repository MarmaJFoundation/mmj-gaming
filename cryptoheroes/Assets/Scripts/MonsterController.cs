using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : UnitController
{
    public SpriteRenderer shadowSprite;
    public SpriteRenderer monsterSprite;
    public SpriteRenderer specialSprite;
    public SpriteRenderer backSprite;
    public ParticleSystemRenderer[] backRenderers;
    public ParticleSystemRenderer[] frontRenderers;
    [HideInInspector]
    public MonsterType monsterType;
    [HideInInspector]
    public int index;
    public ScriptableMonster scriptableMonster;
    private DungeonController dungeonController;
    public void Setup(MainMenuController mainMenuController, DungeonController dungeonController, 
        StatStruct statStruct, MonsterType monsterType, ScriptableMonster scriptableMonster, int index, Vector3 startPos)
    {
        this.dungeonController = dungeonController;
        this.monsterType = monsterType;
        this.index = index;
        this.statStruct = statStruct;
        this.scriptableMonster = scriptableMonster;
        deathEffect = EffectType.DeathMonster;
        dyingEffect = EffectType.DyingMonster;
        int height = 4;
        switch (monsterType)
        {
            case MonsterType.Goblin1:
            case MonsterType.Rat1:
                attackEffect = EffectType.AssassinExplo;
                gustEffect = EffectType.AssassinGust;
                break;
            case MonsterType.Goblin2:
                attackEffect = EffectType.WarriorExplo;
                gustEffect = EffectType.WarriorGust;
                break;
            case MonsterType.Goblin3:
                attackEffect = EffectType.ShamanExplo;
                gustEffect = EffectType.ShamanGust;
                projectileEffect = EffectType.ShamanProjectile;
                break;
            case MonsterType.GoblinBoss:
                attackEffect = EffectType.BruteExplo;
                gustEffect = EffectType.BruteGust;
                deathEffect = EffectType.BruteDeath;
                break;
            case MonsterType.RatBoss:
                attackEffect = EffectType.RatBossExplo;
                gustEffect = EffectType.RatBossGust;
                projectileEffect = EffectType.RatBossProjectile;
                deathEffect = EffectType.RatBossDeath;
                break;
            case MonsterType.Rat2:
                attackEffect = EffectType.RatSoldierExplo;
                gustEffect = EffectType.RatSoldierGust;
                projectileEffect = EffectType.RatSoldierProjectile;
                break;
            case MonsterType.Demon1:
                height = 5;
                attackEffect = EffectType.DemonExplo;
                gustEffect = EffectType.DemonGust;
                deathEffect = EffectType.DemonDeath;
                break;
            case MonsterType.Demon2:
                height = 5;
                attackEffect = EffectType.DemonExplo;
                gustEffect = EffectType.DemonGust;
                projectileEffect = EffectType.DemonProjectile;
                deathEffect = EffectType.DemonDeath;
                break;
            case MonsterType.DemonBoss:
                height = 6;
                attackEffect = EffectType.DemonBossExplo;
                gustEffect = EffectType.DemonBossGust;
                deathEffect = EffectType.DemonBossDeath;
                break;
            case MonsterType.Reaper1:
            case MonsterType.Reaper2:
                height = 5;
                attackEffect = EffectType.ReaperExplo;
                gustEffect = EffectType.ReaperGust;
                deathEffect = EffectType.ReaperDeath;
                break;
            case MonsterType.ReaperBoss:
                height = 7;
                attackEffect = EffectType.ReaperBossExplo;
                gustEffect = EffectType.ReaperBossGust;
                deathEffect = EffectType.ReaperBossDeath;
                break;
        }
        switch (index)
        {
            //back
            case 0:
                if (specialSprite != null)
                {
                    specialSprite.sortingOrder = -6;
                }
                if (backSprite != null)
                {
                    backSprite.sortingOrder = -8;
                }
                monsterSprite.sortingOrder = -6;
                shadowSprite.sortingOrder = -7;
                for (int i = 0; i < frontRenderers.Length; i++)
                {
                    frontRenderers[i].sortingOrder = -5;
                }
                for (int i = 0; i < backRenderers.Length; i++)
                {
                    backRenderers[i].sortingOrder = -7;
                }
                break;
            //middle
            case 1:
                if (specialSprite != null)
                {
                    specialSprite.sortingOrder = -5;
                }
                if (backSprite != null)
                {
                    backSprite.sortingOrder = -7;
                }
                monsterSprite.sortingOrder = -5;
                shadowSprite.sortingOrder = -6;
                for (int i = 0; i < frontRenderers.Length; i++)
                {
                    frontRenderers[i].sortingOrder = -4;
                }
                for (int i = 0; i < backRenderers.Length; i++)
                {
                    backRenderers[i].sortingOrder = -6;
                }
                break;
            //front
            case 2:
                if (specialSprite != null)
                {
                    specialSprite.sortingOrder = -3;
                }
                if (backSprite != null)
                {
                    backSprite.sortingOrder = -5;
                }
                monsterSprite.sortingOrder = -3;
                shadowSprite.sortingOrder = -4;
                for (int i = 0; i < frontRenderers.Length; i++)
                {
                    frontRenderers[i].sortingOrder = -3;
                }
                for (int i = 0; i < backRenderers.Length; i++)
                {
                    backRenderers[i].sortingOrder = -4;
                }
                break;
        }
        monsterSprite.color = Color.clear;
        shadowSprite.color = Color.clear;
        transform.position = startPos;
        Setup(mainMenuController, scriptableMonster.monsterName, height);
    }
    public override void OnDeath()
    {
        if (!isBoss)
        {
            base.OnDeath();
            switch (monsterType)
            {
                case MonsterType.GoblinBoss:
                case MonsterType.RatBoss:
                case MonsterType.DemonBoss:
                case MonsterType.ReaperBoss:
                    dungeonController.UnwarpTime();
                    break;
            }
        }
    }
}

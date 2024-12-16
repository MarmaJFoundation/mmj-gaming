using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterType
{
    None = 0,
    Goblin1 = 1,
    Goblin2 = 2,
    Goblin3 = 3,
    GoblinBoss = 4,
    Rat1 = 5,
    Rat2 = 6,
    RatBoss = 7,
    Demon1 = 8,
    Demon2 = 9,
    DemonBoss = 10,
    Reaper1 = 11,
    Reaper2 = 12,
    ReaperBoss = 13,
    Boss1 = 14,
    Boss2 = 15,
    Boss3 = 16,
    Boss4 = 17
}
[CreateAssetMenu(fileName = "New Monster", menuName = "Scriptable/Monster")]
public class ScriptableMonster : ScriptableObject
{
    public MonsterType monsterType;
    public ClassType classType;
    public GameObject monsterPrefab;
    public string monsterName;
    public Vector2Int dexRange;
    public Vector2Int strRange;
    public Vector2Int endRange;
    public Vector2Int intRange;
    public Vector2Int lckRange;
}

using UnityEngine;

public enum ItemType
{
    None = 0,
    KnightWeapon1 = 1,
    KnightWeapon2 = 2,
    KnightWeapon3 = 3,
    KnightWeapon4 = 4,
    KnightHelmet1 = 5,
    KnightHelmet2 = 6,
    KnightHelmet3 = 7,
    KnightHelmet4 = 8,
    KnightArmor1 = 9,
    KnightArmor2 = 10,
    KnightArmor3 = 11,
    KnightArmor4 = 12,
    KnightBoots1 = 13,
    KnightBoots2 = 14,
    KnightBoots3 = 15,
    KnightBoots4 = 16,
    KnightRing1 = 17,
    KnightRing2 = 18,
    KnightRing3 = 19,
    KnightRing4 = 20,
    KnightAmulet1 = 21,
    KnightAmulet2 = 22,
    KnightAmulet3 = 23,
    KnightAmulet4 = 24,
    MageWeapon1 = 25,
    MageWeapon2 = 26,
    MageWeapon3 = 27,
    MageWeapon4 = 28,
    MageHelmet1 = 29,
    MageHelmet2 = 30,
    MageHelmet3 = 31,
    MageHelmet4 = 32,
    MageArmor1 = 33,
    MageArmor2 = 34,
    MageArmor3 = 35,
    MageArmor4 = 36,
    MageBoots1 = 37,
    MageBoots2 = 38,
    MageBoots3 = 39,
    MageBoots4 = 40,
    MageRing1 = 41,
    MageRing2 = 42,
    MageRing3 = 43,
    MageRing4 = 44,
    MageAmulet1 = 45,
    MageAmulet2 = 46,
    MageAmulet3 = 47,
    MageAmulet4 = 48,
    RangerWeapon1 = 49,
    RangerWeapon2 = 50,
    RangerWeapon3 = 51,
    RangerWeapon4 = 52,
    RangerHelmet1 = 53,
    RangerHelmet2 = 54,
    RangerHelmet3 = 55,
    RangerHelmet4 = 56,
    RangerArmor1 = 57,
    RangerArmor2 = 58,
    RangerArmor3 = 59,
    RangerArmor4 = 60,
    RangerBoots1 = 61,
    RangerBoots2 = 62,
    RangerBoots3 = 63,
    RangerBoots4 = 64,
    RangerRing1 = 65,
    RangerRing2 = 66,
    RangerRing3 = 67,
    RangerRing4 = 68,
    RangerAmulet1 = 69,
    RangerAmulet2 = 70,
    RangerAmulet3 = 71,
    RangerAmulet4 = 72,
    Potion1 = 73,
    Potion2 = 74,
    Potion3 = 75,
}
public enum RarityType
{
    Common = 0,
    Rare = 1,
    Epic = 2,
    Legendary = 3,
    None = 4
}
public enum ClassType
{
    None = 0,
    Mage = 1,
    Knight = 2,
    Ranger = 3
}
public enum EquipType
{
    Armor = 0,
    Helmet = 1,
    Weapon = 2,
    Boots = 3,
    Necklace = 4,
    Ring = 5,
    Empty = 6
}
[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable/Item")]
public class ScriptableItem : ScriptableObject
{
    public ItemType itemType;
    public RarityType rarityType;
    public EquipType equipType;
    public ClassType classType;
    public Sprite itemSprite;
    public string synonymString;
    public int slotIndex;
    public int strengthChance;
    public int enduranceChance;
    public int dexterityChance;
    public int intelligenceChance;
    public int luckChance;
}
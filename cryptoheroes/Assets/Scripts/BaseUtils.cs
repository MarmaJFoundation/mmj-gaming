using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseUtils : MonoBehaviour
{
    public Transform _restingCanvas;
    public Transform _mainCanvas;
    public Transform _mainGame;
    public static Transform restingCanvas;
    public static Transform mainCanvas;
    public static Transform mainGame;

    public Camera _mainCam;
    public static Camera mainCam;

    public bool _offlineMode;
    public static bool offlineMode;
    public static bool onFight;
    public static bool showingLoot;
    public static bool showingWarn;
    public static int itemViewState;

    public char[] letterChars;

    public Sprite[] mediumLetters;
    public Sprite[] smallLetters;
    public Sprite[] _itemBorders;
    public Sprite[] _toggleSprites;
    public Sprite[] _buttonSprites;
    public Sprite[] _knightSprites;
    public Sprite[] _rangerSprites;
    public Sprite[] _mageSprites;
    public Sprite[] _emptyBorders;
    public static Sprite[] emptyBorders;
    public static Sprite[] itemBorders;
    public static Sprite[] toggleSprites;
    public static Sprite[] buttonSprites;
    public static Sprite[] knightSprites;
    public static Sprite[] rangerSprites;
    public static Sprite[] mageSprites;

    public Material _normalUI;
    public Material _darklineUI;
    public Material _outlineUI;
    public Material _lightoutUI;
    public Material _highlightUI;
    public Material _highLineUI;
    public Material _grayscaleUI;
    public Material _graylightUI;
    public Material[] _rarityMaterials;
    public static Material normalUI;
    public static Material highlightUI;
    public static Material highLineUI;
    public static Material darkLineUI;
    public static Material outlineUI;
    public static Material grayscaleUI;
    public static Material graylightUI;
    public static Material[] rarityMaterials;

    public Color[] _rarityColors;
    public Color[] _stateColors;
    public Color[] _rankColors;
    public static Color[] rarityColors;
    public static Color[] stateColors;
    public static Color[] rankColors;
    public Color _disabledColor;
    public Color _enabledColor;
    public Color _offColor;
    public Color _textOffColor;
    public Color _damageColor;
    public Color _healColor;
    public static Color offColor;
    public static Color textOffColor;
    public static Color disabledColor;
    public static Color enabledColor;
    public static Color damageColor;
    public static Color healColor;
    public GameObject _loadingScreenObj;
    public GameObject _messageWindowPrefab;
    public GameObject _healthbarPrefab;
    public static GameObject loadingScreenObj;
    public static GameObject messageWindowPrefab;
    public static GameObject healthbarPrefab;

    public ScriptableCurve[] curves;
    public ScriptableItem[] items;
    public ScriptableMonster[] monsters;
    public ScriptableEffect[] effects;

    public static readonly Dictionary<CurveType, ScriptableCurve> curveDict = new Dictionary<CurveType, ScriptableCurve>();
    public static readonly Dictionary<ItemType, ScriptableItem> itemDict = new Dictionary<ItemType, ScriptableItem>();
    public static readonly Dictionary<MonsterType, ScriptableMonster> monsterDict = new Dictionary<MonsterType, ScriptableMonster>();
    public static readonly Dictionary<EffectType, ScriptableEffect> effectDict = new Dictionary<EffectType, ScriptableEffect>();

    public static readonly Dictionary<char, Sprite> mediumLetterDict = new Dictionary<char, Sprite>();
    public static readonly Dictionary<char, Sprite> smallLetterDict = new Dictionary<char, Sprite>();
    public static readonly Dictionary<char, Queue<Image>> mediumLetterPool = new Dictionary<char, Queue<Image>>();
    public static readonly Dictionary<char, Queue<Image>> smallLetterPool = new Dictionary<char, Queue<Image>>();

    public static readonly Dictionary<EffectType, Queue<EffectController>> effectPool = new Dictionary<EffectType, Queue<EffectController>>();
    public static readonly List<EffectController> activeEffects = new List<EffectController>();

    public static readonly Color transparentColor = new Color(1, 1, 1, .5f);
    public static readonly ItemType[] commonItems = new ItemType[18]
    {
        ItemType.KnightAmulet1,ItemType.KnightArmor1,ItemType.KnightBoots1,ItemType.KnightHelmet1,ItemType.KnightRing1,ItemType.KnightWeapon1,
        ItemType.MageAmulet1,ItemType.MageArmor1,ItemType.MageBoots1,ItemType.MageHelmet1,ItemType.MageRing1,ItemType.MageWeapon1,
        ItemType.RangerAmulet1,ItemType.RangerArmor1,ItemType.RangerBoots1,ItemType.RangerHelmet1,ItemType.RangerRing1,ItemType.RangerWeapon1
    };
    public static readonly ItemType[] rareItems = new ItemType[18]
    {
        ItemType.KnightAmulet2,ItemType.KnightArmor2,ItemType.KnightBoots2,ItemType.KnightHelmet2,ItemType.KnightRing2,ItemType.KnightWeapon2,
        ItemType.MageAmulet2,ItemType.MageArmor2,ItemType.MageBoots2,ItemType.MageHelmet2,ItemType.MageRing2,ItemType.MageWeapon2,
        ItemType.RangerAmulet2,ItemType.RangerArmor2,ItemType.RangerBoots2,ItemType.RangerHelmet2,ItemType.RangerRing2,ItemType.RangerWeapon2
    };
    public static readonly ItemType[] epicItems = new ItemType[18]
    {
        ItemType.KnightAmulet3,ItemType.KnightArmor3,ItemType.KnightBoots3,ItemType.KnightHelmet3,ItemType.KnightRing3,ItemType.KnightWeapon3,
        ItemType.MageAmulet3,ItemType.MageArmor3,ItemType.MageBoots3,ItemType.MageHelmet3,ItemType.MageRing3,ItemType.MageWeapon3,
        ItemType.RangerAmulet3,ItemType.RangerArmor3,ItemType.RangerBoots3,ItemType.RangerHelmet3,ItemType.RangerRing3,ItemType.RangerWeapon3
    };
    public static readonly ItemType[] legendaryItems = new ItemType[18]
    {
        ItemType.KnightAmulet4,ItemType.KnightArmor4,ItemType.KnightBoots4,ItemType.KnightHelmet4,ItemType.KnightRing4,ItemType.KnightWeapon4,
        ItemType.MageAmulet4,ItemType.MageArmor4,ItemType.MageBoots4,ItemType.MageHelmet4,ItemType.MageRing4,ItemType.MageWeapon4,
        ItemType.RangerAmulet4,ItemType.RangerArmor4,ItemType.RangerBoots4,ItemType.RangerHelmet4,ItemType.RangerRing4,ItemType.RangerWeapon4
    };
    private static readonly string[] adjectives = new string[31]
    {
        "bright","salvaged","sharp","dark","powerful","stunning","stormy","wicked","zealous","ashy","blessed","magical","enchanted","sacred",
        "honorful","proud","strong","resiliant","unbroken","twisted","burning","fiery","blazing","radiant","dazzling","shimmering","fervent",
        "luminous","gleaming","lucent","frosty"
    };
    private static readonly string[] nouns = new string[10]
    {
        "the flames","the darkness","the heavens","the depths","the ocean","the storm","the gods","the sky","fate","repentance"
    };
    private static readonly string[] colors = new string[10]
    {
        "red","bright","black","white","incandescent","opaque","obsidian","pale","ardent","jade"
    };
    private static readonly string[] helmetSynonyms = new string[5]
    {
        "helmet","cowl","cap","headgear","helm"
    };
    private static readonly string[] hoodSynonyms = new string[5]
    {
        "hood","coif","cowl","mantle","veil"
    };
    private static readonly string[] cloakSynonyms = new string[5]
    {
        "cloak","mantle","shawl","veneer","cape"
    };
    private static readonly string[] hatSynonyms = new string[5]
    {
        "hat","cap","tam","topper","straw"
    };
    private static readonly string[] armorSynonyms = new string[5]
    {
        "armor","guard","plate","mail","sheath"
    };
    private static readonly string[] robeSynonyms = new string[5]
    {
        "robe","frock","mantle","mail","coat"
    };
    private static readonly string[] bootSynonyms = new string[5]
    {
        "boots","waders","shoes","brogans","footwear"
    };
    private static readonly string[] ringSynonyms = new string[5]
    {
        "ring","band","brim","hoop","halo"
    };
    private static readonly string[] necklaceSynonyms = new string[5]
    {
        "necklace","amulet","pendant","locket","strand"
    };
    private static readonly string[] swordSynonyms = new string[5]
    {
         "sword","claymore","blade","glaive","sabre"
    };
    private static readonly string[] staffSynonyms = new string[5]
    {
        "staff","pole","rod","wand","cane"
    };
    private static readonly string[] daggerSynonyms = new string[5]
    {
        "daggers","dirks","skeans","bodkins","knives"
    };
    private static readonly Dictionary<string, string[]> synonymDict = new Dictionary<string, string[]>();

    public static readonly KeyCode[] modifierKeys = new KeyCode[44]
    {
        KeyCode.LeftShift, KeyCode.RightShift, KeyCode.LeftControl, KeyCode.RightControl,
        KeyCode.AltGr, KeyCode.LeftAlt, KeyCode.RightAlt, KeyCode.Tab, KeyCode.CapsLock,
        KeyCode.Numlock, KeyCode.Print, KeyCode.End, KeyCode.PageDown, KeyCode.PageUp,
        KeyCode.Pause, KeyCode.Insert, KeyCode.ScrollLock, KeyCode.Break, KeyCode.DownArrow,
        KeyCode.LeftArrow,KeyCode.RightArrow,KeyCode.UpArrow,KeyCode.F1,KeyCode.F2,KeyCode.F3,
        KeyCode.F4,KeyCode.F5,KeyCode.F6,KeyCode.F7,KeyCode.F8,KeyCode.F9,KeyCode.F10,KeyCode.F11,
        KeyCode.F12,KeyCode.LeftWindows,KeyCode.RightWindows,KeyCode.SysReq, KeyCode.Mouse0, KeyCode.Mouse1,
        KeyCode.Mouse2,KeyCode.Mouse3,KeyCode.Mouse4,KeyCode.Mouse5,KeyCode.Mouse6
    };
    private static readonly Vector2Int[] rarityBases = new Vector2Int[4] { new Vector2Int(75, 175), new Vector2Int(150, 350), new Vector2Int(300, 500), new Vector2Int(500, 800) };
    private void Awake()
    {
        Setup();
    }
    public virtual void Setup()
    {
        Physics.autoSimulation = false;
        offlineMode = _offlineMode;
        normalUI = _normalUI;
        highlightUI = _highlightUI;
        highLineUI = _highLineUI;
        darkLineUI = _darklineUI;
        outlineUI = _outlineUI;
        grayscaleUI = _grayscaleUI;
        graylightUI = _graylightUI;
        rarityMaterials = _rarityMaterials;
        restingCanvas = _restingCanvas;
        mainCanvas = _mainCanvas;
        mainGame = _mainGame;
        itemBorders = _itemBorders;
        emptyBorders = _emptyBorders;
        toggleSprites = _toggleSprites;
        messageWindowPrefab = _messageWindowPrefab;
        healthbarPrefab = _healthbarPrefab;
        rarityColors = _rarityColors;
        stateColors = _stateColors;
        rankColors = _rankColors;
        mainCam = _mainCam;
        buttonSprites = _buttonSprites;
        knightSprites = _knightSprites;
        rangerSprites = _rangerSprites;
        mageSprites = _mageSprites;
        disabledColor = _disabledColor;
        enabledColor = _enabledColor;
        damageColor = _damageColor;
        healColor = _healColor;
        offColor = _offColor;
        textOffColor = _textOffColor;
        loadingScreenObj = _loadingScreenObj;
        SetLetterDicts();
        SetDicts();
    }
    public void SetLetterDicts()
    {
        mediumLetterDict.Clear();
        smallLetterDict.Clear();
        mediumLetterPool.Clear();
        smallLetterPool.Clear();
        for (int i = 0; i < letterChars.Length; i++)
        {
            mediumLetterDict.Add(letterChars[i], mediumLetters[i]);
            smallLetterDict.Add(letterChars[i], smallLetters[i]);
            mediumLetterPool.Add(letterChars[i], new Queue<Image>());
            smallLetterPool.Add(letterChars[i], new Queue<Image>());
        }
    }
    public virtual void SetDicts()
    {
        curveDict.Clear();
        itemDict.Clear();
        monsterDict.Clear();
        effectDict.Clear();
        effectPool.Clear();
        for (int i = 0; i < curves.Length; i++)
        {
            curveDict.Add(curves[i].curveType, curves[i]);
        }
        for (int i = 0; i < items.Length; i++)
        {
            itemDict.Add(items[i].itemType, items[i]);
        }
        for (int i = 0; i < monsters.Length; i++)
        {
            monsterDict.Add(monsters[i].monsterType, monsters[i]);
        }
        for (int i = 0; i < effects.Length; i++)
        {
            effectDict.Add(effects[i].effectType, effects[i]);
            effectPool.Add(effects[i].effectType, new Queue<EffectController>());
        }
    }
    public static void InstantiateEffect(EffectType effectType, Vector3 fromPosition, Vector3 goPosition, bool fromGame, float scaleModifier = 1)
    {
        EffectController effectController;
        if (effectPool[effectType].Count > 0)
        {
            effectController = effectPool[effectType].Dequeue();
        }
        else
        {
            GameObject effectObj = Instantiate(effectDict[effectType].effectPrefab, fromGame ? mainGame : mainCanvas);
            effectController = effectObj.GetComponent<EffectController>();
        }
        effectController.Setup(effectType, fromPosition, goPosition, scaleModifier);
    }
    public static void InstantiateEffect(EffectType effectType, Vector3 goPosition, bool fromGame, float scaleModifier = 1)
    {
        EffectController effectController;
        if (effectPool[effectType].Count > 0)
        {
            effectController = effectPool[effectType].Dequeue();
        }
        else
        {
            GameObject effectObj = Instantiate(effectDict[effectType].effectPrefab, fromGame ? mainGame : mainCanvas);
            effectController = effectObj.GetComponent<EffectController>();
        }
        effectController.Setup(effectType, goPosition, scaleModifier);
    }
    public static void InstantiateEffect(EffectType effectType, Vector3 goPosition, SpriteRenderer spriteRenderer, float scaleModifier = 1)
    {
        EffectController effectController;
        if (effectPool[effectType].Count > 0)
        {
            effectController = effectPool[effectType].Dequeue();
        }
        else
        {
            GameObject effectObj = Instantiate(effectDict[effectType].effectPrefab, mainGame);
            effectController = effectObj.GetComponent<EffectController>();
        }
        effectController.Setup(effectType, goPosition, spriteRenderer, scaleModifier);
    }
    public static void StopAllEffects()
    {
        for (int i = 0; i < activeEffects.Count; i++)
        {
            activeEffects[i].Dispose();
        }
    }
    public static void ShowLoading()
    {
        loadingScreenObj.SetActive(true);
    }
    public static void HideLoading()
    {
        loadingScreenObj.SetActive(false);
    }
    public static void ShowWarningMessage(string title, string[] message, ItemData itemData, Action OnAcceptCallback)
    {
        GameObject windowObj = Instantiate(messageWindowPrefab, mainCanvas.transform);
        windowObj.GetComponent<MessageWindowController>().Setup(title, message, itemData, OnAcceptCallback);
    }
    public static GameObject ShowWarningMessage(string title, string[] message, ItemData itemData)
    {
        GameObject windowObj = Instantiate(messageWindowPrefab, mainCanvas.transform);
        windowObj.GetComponent<MessageWindowController>().Setup(title, message, itemData);
        return windowObj;
    }
    public static void ShowWarningMessage(string title, string[] message, Action OnAcceptCallback)
    {
        GameObject windowObj = Instantiate(messageWindowPrefab, mainCanvas.transform);
        windowObj.GetComponent<MessageWindowController>().Setup(title, message, OnAcceptCallback);
    }
    public static void ShowWarningMessage(string title, string[] message)
    {
        GameObject windowObj = Instantiate(messageWindowPrefab, mainCanvas.transform);
        windowObj.GetComponent<MessageWindowController>().Setup(title, message);
    }
    public static void ShowWarningMessage(string title, string[] message, bool lockWindow)
    {
        GameObject windowObj = Instantiate(messageWindowPrefab, mainCanvas.transform);
        windowObj.GetComponent<MessageWindowController>().Setup(title, message, lockWindow);
    }
    public static bool RandomBool()
    {
        return UnityEngine.Random.Range(0, 100) > 50;
    }
    public static int RandomSign()
    {
        if (RandomBool())
        {
            return -1;
        }
        return 1;
    }
    public static int RandomInt(int rangeX, int rangeY)
    {
        return UnityEngine.Random.Range(rangeX, rangeY);
    }
    public static int RandomInt(Vector2Int range)
    {
        return UnityEngine.Random.Range(range.x, range.y);
    }
    public static float RandomFloat(float fromFloat, float gotoFloat)
    {
        return RandomInt(Mathf.RoundToInt(fromFloat * 100), Mathf.RoundToInt(gotoFloat * 100)) / 100f;
    }
    public static int GetExpForNextLevel(int level)
    {
        return Mathf.RoundToInt(50 * level * (1 + level * .2f));
    }
    public static void OnAcceptTradeToken()
    {
        Application.OpenURL("https://app.ref.finance/#wrap.near%7Cpixeltoken.near");
    }
    public static string GenerateItemName(string itemType, RarityType rarityType, int seed = -1)
    {
        if (seed != -1)
        {
            UnityEngine.Random.InitState(seed);
        }
        switch (rarityType)
        {
            case RarityType.Rare:
                return $"{adjectives[RandomInt(0, adjectives.Length)]} {colors[RandomInt(0, colors.Length)]} {synonymDict[itemType][RandomInt(0, synonymDict[itemType].Length)]}";
            case RarityType.Epic:
                return $"{colors[RandomInt(0, colors.Length)]} {synonymDict[itemType][RandomInt(0, synonymDict[itemType].Length)]} of {nouns[RandomInt(0, nouns.Length)]}";
            case RarityType.Legendary:
                return $"{adjectives[RandomInt(0, adjectives.Length)]} {synonymDict[itemType][RandomInt(0, synonymDict[itemType].Length)]} of {nouns[RandomInt(0, nouns.Length)]}";
            default:
                return $"{adjectives[RandomInt(0, adjectives.Length)]} {synonymDict[itemType][RandomInt(0, synonymDict[itemType].Length)]}";
        }
    }
    private static void SetSynonymDict()
    {
        if (synonymDict.Count > 0)
        {
            return;
        }
        synonymDict.Add("Armor", armorSynonyms);
        synonymDict.Add("Robe", robeSynonyms);
        synonymDict.Add("Dagger", daggerSynonyms);
        synonymDict.Add("Sword", swordSynonyms);
        synonymDict.Add("Staff", staffSynonyms);
        synonymDict.Add("Boots", bootSynonyms);
        synonymDict.Add("Cloak", cloakSynonyms);
        synonymDict.Add("Hat", hatSynonyms);
        synonymDict.Add("Hood", hoodSynonyms);
        synonymDict.Add("Ring", ringSynonyms);
        synonymDict.Add("Necklace", necklaceSynonyms);
        synonymDict.Add("Helmet", helmetSynonyms);
    }
    public static ItemData GenerateRandomItem(RarityType rarityType, ItemType itemType = ItemType.None, int price = 0)
    {
        SetSynonymDict();
        int itemID = RandomInt(-9000000, 9000000);
        if (itemType == ItemType.None)
        {
            switch (rarityType)
            {
                case RarityType.Common:
                    itemType = commonItems[RandomInt(0, commonItems.Length)];
                    break;
                case RarityType.Rare:
                    itemType = rareItems[RandomInt(0, rareItems.Length)];
                    break;
                case RarityType.Epic:
                    itemType = epicItems[RandomInt(0, epicItems.Length)];
                    break;
                case RarityType.Legendary:
                    itemType = legendaryItems[RandomInt(0, legendaryItems.Length)];
                    break;
            }
        }
        ScriptableItem scriptableItem = itemDict[itemType];
        Vector2Int rarityBaseStat = rarityBases[(int)rarityType];
        int strength = RandomInt(rarityBaseStat);
        int endurance = RandomInt(rarityBaseStat);
        int dexterity = RandomInt(rarityBaseStat);
        int intelligence = RandomInt(rarityBaseStat);
        int luck = RandomInt(rarityBaseStat);
        for (int i = 0; i < 10; i++)
        {
            if ((strength + endurance + dexterity + intelligence + luck) > rarityBaseStat.y)
            {
                strength -= RandomInt(10, 50);
                endurance -= RandomInt(10, 50);
                dexterity -= RandomInt(10, 50);
                intelligence -= RandomInt(10, 50);
                luck -= RandomInt(10, 50);
            }
        }
        if (strength < 0)
        {
            strength = 0;
        }
        if (endurance < 0)
        {
            endurance = 0;
        }
        if (dexterity < 0)
        {
            dexterity = 0;
        }
        if (intelligence < 0)
        {
            intelligence = 0;
        }
        if (luck < 0)
        {
            luck = 0;
        }
        string itemName = GenerateItemName(scriptableItem.synonymString, rarityType);
        return new ItemData(DateTime.Now, strength, dexterity, endurance, intelligence, luck, itemName, itemID, price, itemType, ClassType.None);
    }
    public static StatStruct GenerateCharStruct(ClassType classType, int charLevel, List<ItemData> itemData, bool fromLeaderboard = false)
    {
        int damage = 0;
        float defense = 0;
        float critChance = 0;
        float lifeSteal = 0;
        float dodge = 0;
        int maxHealth = 0;
        //set base stats
        switch (classType)
        {
            case ClassType.Mage:
                damage = 15;
                defense = 3;
                critChance = 3;
                lifeSteal = 4;
                dodge = 3;
                maxHealth = 85;
                break;
            case ClassType.Knight:
                damage = 13;
                defense = 5;
                critChance = 3;
                lifeSteal = 3;
                dodge = 3;
                maxHealth = 120;
                break;
            case ClassType.Ranger:
                damage = 14;
                defense = 3;
                critChance = 4;
                lifeSteal = 3;
                dodge = 3;
                maxHealth = 90;
                break;
        }
        //add stats from items
        for (int i = 0; i < itemData.Count; i++)
        {
            if (itemData[i].equipClass == classType || fromLeaderboard)
            {
                StatStruct itemStatStruct = GenerateItemStatStruct(itemData[i], classType, itemDict[itemData[i].itemType].equipType);
                damage += itemStatStruct.damage;
                defense += itemStatStruct.defense;
                critChance += itemStatStruct.critChance;
                lifeSteal += itemStatStruct.lifeSteal;
                dodge += itemStatStruct.dodge;
                maxHealth += itemStatStruct.maxHealth;
            }
        }
        float potionMultiplier = Database.HasStrengthPotion(classType) ? 1 + Database.GetStrengthPotion(classType).strength * .01f : 1;
        //multiply by level & potion
        damage = Mathf.RoundToInt(damage * (1 + (charLevel * .01f)) * potionMultiplier);
        defense = Mathf.RoundToInt(defense * (1 + (charLevel * .01f)) * potionMultiplier);
        critChance = Mathf.RoundToInt(critChance * (1 + (charLevel * .01f)) * potionMultiplier);
        lifeSteal = Mathf.RoundToInt(lifeSteal * (1 + (charLevel * .01f)) * potionMultiplier);
        dodge = Mathf.RoundToInt(dodge * (1 + (charLevel * .01f)) * potionMultiplier);
        maxHealth = Mathf.RoundToInt(maxHealth * (1 + (charLevel * .01f)) * potionMultiplier);
        //clamping to not exceed 100%
        defense = Mathf.Clamp(defense, 0, 100);
        critChance = Mathf.Clamp(critChance, 0, 100);
        lifeSteal = Mathf.Clamp(lifeSteal, 0, 100);
        dodge = Mathf.Clamp(dodge, 0, 100);
        return new StatStruct(maxHealth, damage, defense, dodge, lifeSteal, critChance);
    }
    public static StatStruct GenerateMonsterStatStruct(ClassType classType, int dexterity, int strength, int intelligence, int endurance, int luck)
    {
        int damage = 0;
        float defense = 0;
        float critChance = 0;
        float lifeSteal = 0;
        float dodge = 0;
        int maxHealth = 0;
        switch (classType)
        {
            case ClassType.Ranger:
                damage = Mathf.RoundToInt((luck + dexterity * .3f + strength * .3f) * .25f);
                defense = (strength + endurance) * .005f;
                critChance = (dexterity + luck) * .008f;
                lifeSteal = (intelligence + luck) * .005f;
                dodge = (dexterity + luck) * .005f;
                maxHealth = Mathf.RoundToInt(endurance * 1.4f);
                break;
            case ClassType.Mage:
                damage = Mathf.RoundToInt((luck + intelligence * .7f) * .25f);
                defense = (strength + endurance) * .004f;
                critChance = (intelligence + luck) * .006f;
                lifeSteal = (intelligence + luck) * .006f;
                dodge = (dexterity + luck) * .006f;
                maxHealth = Mathf.RoundToInt(endurance * 1.2f);
                break;
            case ClassType.Knight:
                damage = Mathf.RoundToInt((luck + strength * .5f) * .25f);
                defense = (strength + endurance) * .008f;
                critChance = (strength + luck) * .004f;
                lifeSteal = (intelligence + luck) * .004f;
                dodge = (dexterity + luck) * .003f;
                maxHealth = Mathf.RoundToInt(endurance * 1.75f);
                break;
        }
        //clamping to not exceed 100%
        defense = Mathf.Clamp(defense, 0, 100);
        critChance = Mathf.Clamp(critChance, 0, 100);
        lifeSteal = Mathf.Clamp(lifeSteal, 0, 100);
        dodge = Mathf.Clamp(dodge, 0, 100);
        return new StatStruct(maxHealth, damage, defense, dodge, lifeSteal, critChance);
    }
    public static StatStruct GenerateItemStatStruct(ItemData itemData, ClassType classType, EquipType equipType)
    {
        int damage = 0;
        float defense = 0;
        float critChance = 0;
        float lifeSteal = 0;
        float dodge = 0;
        int maxHealth = 0;
        switch (classType)
        {
            case ClassType.Ranger:
                switch (equipType)
                {
                    case EquipType.Armor:
                        dodge = (itemData.dexterity + itemData.intelligence) * .01f;
                        maxHealth = 1 + Mathf.RoundToInt(itemData.endurance * 2 + itemData.dexterity + itemData.luck);
                        defense = (itemData.strength + itemData.endurance) * .01f;
                        break;
                    case EquipType.Helmet:
                        defense = .01f + (itemData.strength + itemData.endurance) * .01f;
                        dodge = (itemData.dexterity + itemData.luck) * .01f;
                        break;
                    case EquipType.Weapon:
                        damage = 1 + Mathf.RoundToInt((itemData.luck + itemData.dexterity + itemData.strength + itemData.intelligence) * .25f);
                        critChance = (itemData.dexterity + itemData.luck) * .01f;
                        lifeSteal = (itemData.intelligence + itemData.luck) * .005f;
                        break;
                    case EquipType.Boots:
                        defense = (itemData.strength + itemData.endurance) * .01f;
                        dodge = .01f + (itemData.dexterity + itemData.luck) * .01f;
                        break;
                    case EquipType.Necklace:
                        maxHealth = 1 + Mathf.RoundToInt((itemData.endurance + itemData.dexterity) * .5f);
                        critChance = (itemData.dexterity + itemData.luck) * .01f;
                        break;
                    case EquipType.Ring:
                        damage = 1 + Mathf.RoundToInt((itemData.luck + itemData.dexterity) * .2f);
                        lifeSteal = (itemData.intelligence + itemData.luck) * .005f;
                        break;
                }
                break;
            case ClassType.Mage:
                switch (equipType)
                {
                    case EquipType.Armor:
                        defense = (itemData.strength + itemData.endurance) * .01f;
                        maxHealth = 1 + Mathf.RoundToInt(itemData.endurance * 1.75f + itemData.intelligence + itemData.luck);
                        dodge = (itemData.dexterity + itemData.luck) * .01f;
                        break;
                    case EquipType.Helmet:
                        defense = (itemData.strength + itemData.endurance) * .01f;
                        damage = 1 + Mathf.RoundToInt((itemData.luck + itemData.intelligence) * .2f);
                        break;
                    case EquipType.Weapon:
                        damage = 1 + Mathf.RoundToInt((itemData.luck + itemData.intelligence + itemData.dexterity + itemData.strength) * .25f);
                        critChance = (itemData.intelligence + itemData.luck) * .01f;
                        lifeSteal = (itemData.intelligence + itemData.luck) * .005f;
                        break;
                    case EquipType.Boots:
                        dodge = .01f + (itemData.dexterity + itemData.luck) * .01f;
                        defense = (itemData.strength + itemData.endurance) * .01f;
                        break;
                    case EquipType.Necklace:
                        maxHealth = 1 + Mathf.RoundToInt((itemData.endurance + itemData.intelligence) * .5f);
                        lifeSteal = (itemData.intelligence + itemData.luck) * .005f;
                        break;
                    case EquipType.Ring:
                        critChance = (itemData.intelligence + itemData.luck) * .01f;
                        lifeSteal = .01f + (itemData.intelligence + itemData.luck) * .005f;
                        break;
                }
                break;
            case ClassType.Knight:
                switch (equipType)
                {
                    case EquipType.Armor:
                        defense = (itemData.strength + itemData.endurance) * .015f;
                        maxHealth = 1 + Mathf.RoundToInt(itemData.endurance * 2.25f + itemData.strength + itemData.luck);
                        lifeSteal = (itemData.intelligence + itemData.luck) * .005f;
                        break;
                    case EquipType.Helmet:
                        defense = (itemData.strength + itemData.endurance) * .01f;
                        maxHealth = 1 + Mathf.RoundToInt(itemData.endurance * 1.5f + itemData.strength);
                        break;
                    case EquipType.Weapon:
                        damage = 1 + Mathf.RoundToInt((itemData.luck + itemData.strength + itemData.dexterity + itemData.intelligence) * .25f);
                        critChance = (itemData.strength + itemData.luck) * .01f;
                        lifeSteal = (itemData.intelligence + itemData.luck) * .005f;
                        break;
                    case EquipType.Boots:
                        defense = .01f + (itemData.strength + itemData.endurance) * .01f;
                        dodge = (itemData.dexterity + itemData.luck) * .01f;
                        break;
                    case EquipType.Necklace:
                        lifeSteal = (itemData.intelligence + itemData.luck) * .005f;
                        defense = .01f + (itemData.strength + itemData.endurance) * .01f;
                        break;
                    case EquipType.Ring:
                        critChance = (itemData.strength + itemData.luck) * .01f;
                        maxHealth = 1 + Mathf.RoundToInt((itemData.endurance + itemData.luck) * .75f);
                        break;
                }
                break;
        }
        //clamping to not exceed 100%
        defense = Mathf.Clamp(defense, 0, 100);
        critChance = Mathf.Clamp(critChance, 0, 100);
        lifeSteal = Mathf.Clamp(lifeSteal, 0, 100);
        dodge = Mathf.Clamp(dodge, 0, 100);
        return new StatStruct(maxHealth, damage, defense, dodge, lifeSteal, critChance);
    }
    public static RoomData GenerateRandomRoomInfo(int difficulty, ClassType selectedClass)
    {
        List<string> playerNames = new List<string>() { "anon.near", "nelphion.near", "bubruno.near", "messagebox.near", "danzo.near", "barack_obama.near", "elon_musk.near", "edzo.near" };
        List<ClassType> playerClasses = new List<ClassType>();
        List<int> playerLevels = new List<int>();
        List<int> playerRanks = new List<int>();
        List<StatStruct> playerStats = new List<StatStruct>();
        List<List<ItemType>> playerEquippedItems = new List<List<ItemType>>();
        List<DateTime> playerTimes = new List<DateTime>();
        //playerNames.Add(Database.databaseStruct.playerAccount);
        playerClasses.Add(selectedClass);
        playerLevels.Add(Database.GetCharStruct(selectedClass).level);
        playerStats.Add(GenerateCharStruct(selectedClass, Database.GetCharStruct(selectedClass).level, Database.databaseStruct.ownedItems));
        playerEquippedItems.Add(Database.GetEquippedItemTypes(selectedClass));
        playerRanks.Add(CalculateRank(playerLevels[0], playerStats[0]));
        for (int i = 0; i < 7; i++)
        {
            int level = RandomInt(10, 15) * (difficulty + 1);
            ClassType classType = (ClassType)RandomInt(1, 4);
            Enum.TryParse($"{classType}Helmet{difficulty + 1}", out ItemType helmetType);
            ItemData helmet = GenerateRandomItem((RarityType)difficulty, helmetType);
            Enum.TryParse($"{classType}Boots{difficulty + 1}", out ItemType bootsType);
            ItemData boots = GenerateRandomItem((RarityType)difficulty, bootsType);
            Enum.TryParse($"{classType}Weapon{difficulty + 1}", out ItemType weaponType);
            ItemData weapon = GenerateRandomItem((RarityType)difficulty, weaponType);
            Enum.TryParse($"{classType}Armor{difficulty + 1}", out ItemType armorType);
            ItemData armor = GenerateRandomItem((RarityType)difficulty, armorType);
            Enum.TryParse($"{classType}Necklace{difficulty + 1}", out ItemType necklaceType);
            ItemData necklace = GenerateRandomItem((RarityType)difficulty, necklaceType);
            Enum.TryParse($"{classType}Ring{difficulty + 1}", out ItemType ringType);
            ItemData ring = GenerateRandomItem((RarityType)difficulty, ringType);
            List<ItemType> equippedItems = new List<ItemType>()
            {
               armor.itemType,
               helmet.itemType,
               weapon.itemType,
               boots.itemType,
               necklace.itemType,
               ring.itemType
            };
            StatStruct charStats = GenerateCharStruct(classType, level, new List<ItemData>() { armor, helmet, weapon, boots, necklace, ring });
            //playerNames.Add("Offline " + classType);
            playerClasses.Add(classType);
            playerLevels.Add(level);
            playerStats.Add(charStats);
            playerEquippedItems.Add(equippedItems);
            playerRanks.Add(CalculateRank(level, charStats));
            playerTimes.Add(DateTime.Now);
        }
        return new RoomData(0, playerNames, playerClasses, playerLevels, playerRanks, playerStats, playerEquippedItems, playerTimes);
    }
    public static int CalculateRank(int level, StatStruct statStruct)
    {
        int statRank = 0;
        statRank += Mathf.RoundToInt(level * 100 +
            statStruct.critChance * 100 + statStruct.defense * 100 + statStruct.dodge * 100 +
            statStruct.maxHealth / 4 + statStruct.lifeSteal * 100 + statStruct.damage);
        return Mathf.Clamp(statRank / 5000, 0, 3);
    }
}
public static class Extensions
{
    public static int ToStatRank(this StatStruct statStruct)
    {
        return Mathf.RoundToInt(
            statStruct.critChance * 100 + statStruct.defense * 100 + statStruct.dodge * 100 +
            statStruct.maxHealth / 4 + statStruct.lifeSteal * 100 + statStruct.damage);
    }
    public static Vector3 ToScale(this Vector3 scale)
    {
        return new Vector3(scale.x.ToScale(), scale.y.ToScale(), scale.z.ToScale());
    }
    public static float ToEffectScale(this int damage)
    {
        if (damage == 0)
        {
            return 1;
        }
        return 1 + Mathf.Clamp(damage / damage * .1f, 0, 2);
    }
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    //public static bool capsLockOn;
    public static readonly KeyCode[] acceptKeycodes = new KeyCode[26]
    {
            KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J,
            KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R,
            KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.X, KeyCode.W, KeyCode.Y, KeyCode.Z
    };
    public static string ToNumberBigChar(this int number)
    {
        string finalString = "";
        foreach (char character in number.ToString())
        {
            switch (character)
            {
                case '0':
                    finalString += "@";
                    break;
                case '1':
                    finalString += "%";
                    break;
                case '2':
                    finalString += "¨";
                    break;
                case '3':
                    finalString += "¬";
                    break;
                case '4':
                    finalString += "£";
                    break;
                case '5':
                    finalString += "§";
                    break;
                case '6':
                    finalString += "³";
                    break;
                case '7':
                    finalString += "²";
                    break;
                case '8':
                    finalString += "º";
                    break;
                case '9':
                    finalString += "¹";
                    break;
            }
        }

        return finalString;
    }
    public static char ToCorrectChar(this KeyCode keyCode)
    {
        if (Array.IndexOf(acceptKeycodes, keyCode) != -1)
        {
            return (char)keyCode;
        }
        bool upperCheck = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        switch (keyCode)
        {
            case KeyCode.Alpha0:
            case KeyCode.Keypad0:
                return upperCheck ? ')' : '0';
            case KeyCode.Alpha1:
            case KeyCode.Keypad1:
                return upperCheck ? '!' : '1';
            case KeyCode.Alpha2:
            case KeyCode.Keypad2:
                return upperCheck ? '@' : '2';
            case KeyCode.Alpha3:
            case KeyCode.Keypad3:
                return upperCheck ? '#' : '3';
            case KeyCode.Alpha4:
            case KeyCode.Keypad4:
                return upperCheck ? '$' : '4';
            case KeyCode.Alpha5:
            case KeyCode.Keypad5:
                return upperCheck ? '%' : '5';
            case KeyCode.Alpha6:
            case KeyCode.Keypad6:
                return upperCheck ? '¨' : '6';
            case KeyCode.Alpha7:
            case KeyCode.Keypad7:
                return upperCheck ? '&' : '7';
            case KeyCode.Alpha8:
            case KeyCode.Keypad8:
                return upperCheck ? '*' : '8';
            case KeyCode.Alpha9:
            case KeyCode.Keypad9:
                return upperCheck ? '(' : '9';
            case KeyCode.KeypadPeriod:
                return upperCheck ? '>' : '.';
            case KeyCode.KeypadDivide:
                return upperCheck ? '?' : '/';
            case KeyCode.KeypadMultiply:
                return upperCheck ? '8' : '*';
            case KeyCode.KeypadMinus:
                return upperCheck ? '_' : '-';
            case KeyCode.KeypadPlus:
                return upperCheck ? '=' : '+';
            case KeyCode.KeypadEquals:
                return upperCheck ? '+' : '=';
            case KeyCode.Exclaim:
                return '!';
            case KeyCode.DoubleQuote:
                return '"';
            case KeyCode.Hash:
                return '#';
            case KeyCode.Dollar:
                return '$';
            case KeyCode.Percent:
                return '%';
            case KeyCode.Ampersand:
                return '&';
            case KeyCode.Quote:
                return '\'';
            case KeyCode.LeftParen:
                return '(';
            case KeyCode.RightParen:
                return ')';
            case KeyCode.Asterisk:
                return '*';
            case KeyCode.Plus:
                return '+';
            case KeyCode.Comma:
                return upperCheck ? '<' : ',';
            case KeyCode.Minus:
                return upperCheck ? '_' : '-';
            case KeyCode.Period:
                return upperCheck ? '>' : '.';
            case KeyCode.Slash:
                return upperCheck ? '?' : '/';
            case KeyCode.Colon:
                return upperCheck ? ':' : ';';
            case KeyCode.Semicolon:
                return upperCheck ? ':' : ';';
            case KeyCode.Less:
                return upperCheck ? ',' : '<';
            case KeyCode.Equals:
                return '=';
            case KeyCode.Greater:
                return '>';
            case KeyCode.Question:
                return '?';
            case KeyCode.At:
                return '@';
            case KeyCode.LeftBracket:
                return upperCheck ? '{' : '[';
            case KeyCode.Backslash:
                return '\\';
            case KeyCode.RightBracket:
                return upperCheck ? '}' : ']';
            case KeyCode.Caret:
                return '^';
            case KeyCode.Underscore:
                return '_';
            case KeyCode.BackQuote:
                return '`';
            case KeyCode.LeftCurlyBracket:
                return '{';
            case KeyCode.Pipe:
                return '|';
            case KeyCode.RightCurlyBracket:
                return '}';
            case KeyCode.Tilde:
                return '~';
            case KeyCode.Space:
                return ' ';
            default:
                return '?';
        }
    }
    public static float ToScale(this float value)
    {
        return value * BaseUtils.mainCanvas.parent.localScale.x;
    }
    public static float ToFloatTime(this DateTime dateTime)
    {
        double elapsedTicks = (dateTime - DateTime.Now).TotalSeconds;
        return (float)elapsedTicks;
    }
    public static string ToDetailedDaysTime(this DateTime dateTime)
    {
        double elapsedTicks = (dateTime - DateTime.Now).TotalSeconds;
        return GetDetailedDaysTime((float)elapsedTicks);
    }
    public static string GetDetailedDaysTime(this float currentTime)
    {
        int minutes = Mathf.FloorToInt((currentTime / 60f) % 60f);
        int hours = Mathf.FloorToInt(currentTime / 3600f % 24f);
        int days = Mathf.FloorToInt(currentTime / 3600f / 24f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        return $"{days} days, {hours} hours, {minutes} minutes, {seconds} seconds.";
    }
    public static string ToDaysTime(this DateTime dateTime)
    {
        double elapsedTicks = (dateTime - DateTime.Now).TotalSeconds;
        return GetDaysTime((float)elapsedTicks);
    }
    public static string GetDaysTime(this float currentTime)
    {
        int minutes = Mathf.FloorToInt((currentTime / 60f) % 60f);
        int hours = Mathf.FloorToInt(currentTime / 3600f % 24f);
        int days = Mathf.FloorToInt(currentTime / 3600f / 24f);
        return $"{days}d {hours}h {minutes}m";
    }
    public static string ToHoursTime(this DateTime dateTime, bool noHours = false)
    {
        double elapsedTicks = (dateTime - DateTime.Now).TotalSeconds;
        return GetHoursTime((float)elapsedTicks, noHours);
    }
    public static string GetHoursTime(this float currentTime, bool noHours = false)
    {
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        int minutes = Mathf.FloorToInt((currentTime / 60f) % 60f);
        int hours = Mathf.FloorToInt(currentTime / 3600f);
        return noHours ? $"{minutes:00}:{seconds:00}" : $"{hours:00}:{minutes:00}:{seconds:00}";
    }
    public static float Evaluate(this float value, CurveType curveType)
    {
        return BaseUtils.curveDict[curveType].animationCurve.Evaluate(value);
    }
    public static float ToDropChance(this float value, int luck)
    {
        return value + (value * luck * 0.01f);
    }

    public static ItemData ToItemData(this ItemToken itemToken)
    {
        int.TryParse(itemToken.token_id, out int itemID);
        long.TryParse(itemToken.price, out long rawPrice);
        ItemData itemData = new ItemData(
            TimestampHelper.GetDateTimeFromTimestamp(itemToken.cd / 1000 / 1000),
            itemToken.strength,
            itemToken.dexterity,
            itemToken.endurance,
            itemToken.intelligence,
            itemToken.luck,
            BaseUtils.GenerateItemName(BaseUtils.itemDict[(ItemType)itemToken.item_type].synonymString, (RarityType)itemToken.rarity_type, itemID),
            itemID,
            (int)(rawPrice / 1000000),
            (ItemType)itemToken.item_type,
            ClassType.None
            );
        return itemData;
    }
    public static RoomData ToRoomData(this RoomWrapper roomWrapper)
    {
        List<ClassType> convertedClasses = new List<ClassType>();
        for (int i = 0; i < roomWrapper.playerClasses.Count; i++)
        {
            convertedClasses.Add((ClassType)roomWrapper.playerClasses[i]);
        }
        List<List<ItemType>> convertedEquips = new List<List<ItemType>>();
        for (int i = 0; i < roomWrapper.playerEquippedItems.Count; i++)
        {
            convertedEquips.Add(new List<ItemType>());
            for (int k = 0; k < roomWrapper.playerEquippedItems[i].Count; k++)
            {
                convertedEquips[i].Add((ItemType)roomWrapper.playerEquippedItems[i][k]);
            }
        }
        List<int> convertedRanks = new List<int>();
        for (int i = 0; i < roomWrapper.playerRanks.Count; i++)
        {
            convertedRanks.Add(Mathf.Clamp(roomWrapper.playerRanks[i] / 5000, 0, 3));
        }
        List<DateTime> playerTimes = new List<DateTime>();
        for (int i = 0; i < roomWrapper.playerJoinTimestamps.Length; i++)
        {
            playerTimes.Add(TimestampHelper.GetDateTimeFromTimestamp(roomWrapper.playerJoinTimestamps[i]));
        }
        RoomData roomData = new RoomData(
            roomWrapper.boss_kills,
            roomWrapper.playerNames,
            convertedClasses,
            roomWrapper.playerLevels,
            convertedRanks,
            roomWrapper.playerStatStructs,
            convertedEquips,
            playerTimes);
        return roomData;
    }
}

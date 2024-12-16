using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class XMLGenerator : MonoBehaviour
{
    public static string UTF8ByteArrayToString(byte[] characters)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        string constructedString = encoding.GetString(characters);
        return (constructedString);
    }
    public static byte[] StringToUTF8ByteArray(string pXmlString)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        byte[] byteArray = encoding.GetBytes(pXmlString);
        return byteArray;
    }
    public static string SerializeObject(object pObject, Type classType)
    {
        MemoryStream memoryStream = new MemoryStream();
        XmlSerializer xs = new XmlSerializer(classType);
        XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
        xs.Serialize(xmlTextWriter, pObject);
        memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
        string XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
        return XmlizedString;
    }
    public static object DeserializeObject(string pXmlizedString, Type classType)
    {
        XmlSerializer xs = new XmlSerializer(classType);
        MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
        //XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
        return xs.Deserialize(memoryStream);
    }
    public static void CreateXML(string rawData, string fileName, string fileLocation = "")
    {
        StreamWriter writer;
        if (fileLocation == "")
        {
            fileLocation = Application.dataPath + "/XMLs/";
        }
        FileInfo t = new FileInfo(fileLocation + "\\" + fileName);
        if (!t.Exists)
        {
            writer = t.CreateText();
        }
        else
        {
            t.Delete();
            writer = t.CreateText();
        }
        writer.Write(rawData);
        writer.Close();
    }
    public static string LoadXML(string fileName, string fileLocation = "")
    {
        if (fileLocation == "")
        {
            fileLocation = Application.dataPath + "/XMLs/";
        }
        if (!File.Exists(fileLocation + "\\" + fileName))
        {
            return "";
        }
        StreamReader r = File.OpenText(fileLocation + "\\" + fileName);
        string _info = r.ReadToEnd();
        r.Close();
        return _info;
    }
    public void GenerateXMLs()
    {
        BaseUtils baseUtils = FindObjectOfType<BaseUtils>();
        ItemValues[] itemValues = new ItemValues[baseUtils.items.Length];
        for (int i = 0; i < baseUtils.items.Length; i++)
        {
            itemValues[i].itemType = baseUtils.items[i].itemType;
            itemValues[i].rarityType = baseUtils.items[i].rarityType;
            itemValues[i].equipType = baseUtils.items[i].equipType;
            itemValues[i].classType = baseUtils.items[i].classType;
            itemValues[i].slotIndex = baseUtils.items[i].slotIndex;
            itemValues[i].strengthChance = baseUtils.items[i].strengthChance;
            itemValues[i].enduranceChance = baseUtils.items[i].enduranceChance;
            itemValues[i].dexterityChance = baseUtils.items[i].dexterityChance;
            itemValues[i].intelligenceChance = baseUtils.items[i].intelligenceChance;
            itemValues[i].luckChance = baseUtils.items[i].luckChance;
        }
        string rawItemData = SerializeObject(itemValues, typeof(ItemValues[]));
        CreateXML(rawItemData, "Items.xml");

        MonsterValues[] monsterValues = new MonsterValues[baseUtils.monsters.Length];
        for (int i = 0; i < baseUtils.monsters.Length; i++)
        {
            monsterValues[i].monsterType = baseUtils.monsters[i].monsterType;
            monsterValues[i].classType = baseUtils.monsters[i].classType;
            monsterValues[i].dexRange = baseUtils.monsters[i].dexRange;
            monsterValues[i].strRange = baseUtils.monsters[i].strRange;
            monsterValues[i].endRange = baseUtils.monsters[i].endRange;
            monsterValues[i].intRange = baseUtils.monsters[i].intRange;
            monsterValues[i].lckRange = baseUtils.monsters[i].lckRange;
        }
        string rawMonsterData = SerializeObject(monsterValues, typeof(MonsterValues[]));
        CreateXML(rawMonsterData, "Monsters.xml");
    }
}
public struct ItemValues
{
    public ItemType itemType;
    public RarityType rarityType;
    public EquipType equipType;
    public ClassType classType;
    public int slotIndex;
    public int strengthChance;
    public int enduranceChance;
    public int dexterityChance;
    public int intelligenceChance;
    public int luckChance;
}
public struct MonsterValues
{
    public MonsterType monsterType;
    public ClassType classType;
    public Vector2Int dexRange;
    public Vector2Int strRange;
    public Vector2Int endRange;
    public Vector2Int intRange;
    public Vector2Int lckRange;
}
#if UNITY_EDITOR
[CustomEditor(typeof(XMLGenerator))]
public class XMLGeneratorUI : Editor
{
    public override void OnInspectorGUI()
    {
        XMLGenerator generator = (XMLGenerator)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Generate XMLs"))
        {
            EditorUtility.SetDirty(generator);
            generator.GenerateXMLs();
            EditorUtility.ClearDirty(generator);
        }
    }
}
#endif

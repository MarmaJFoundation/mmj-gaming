using System;
using System.Collections.Generic;
using System.Net.Http;

public class KeyPair
{
    public string publicKey;
    public string privateKey;
}

[System.Serializable]
public class RequestParamsBuilder
{
    static public RequestParams CreateFunctionCallRequest(string contract_id, string methodName, string args, string accountId = null, string privatekey = null, bool attachYoctoNear = false, bool raise_gas = false)
    {
        return new RequestParams
        {
            contract_id = contract_id,
            account_id = accountId,
            method_name = methodName,
            args = args,
            privatekey = privatekey,
            attachYoctoNear = attachYoctoNear,
            raise_gas = raise_gas,
        };

    }

    static public RequestParams CreateFunctionViewRequest<T>(string contract_id, string methodName, string args)
    {
        return new RequestParams
        {
            contract_id = contract_id,
            method_name = methodName,
            args = args,
        };

    }

    static public RequestParams CreateAccessKeyCheckRequest(string accountId, string publickey)
    {
        return new RequestParams
        {
            account_id = accountId,
            publickey = publickey,
        };

    }
}

[Serializable]
public class WalletAuth
{
    public string account_id;
    public string privatekey;
}
[Serializable]
public class OfferAuth : WalletAuth
{
    public string token_id;
    public string price;
    public OfferWrapper itemdata;
}
[Serializable]
public class OfferWrapper
{
    public string token_id;
}
[Serializable]
public class OfferSellAuth : WalletAuth
{
    public string token_id;
    public string price;
    public OfferSellWrapper itemdata;
}
[Serializable]
public class OfferSellWrapper
{
    public string token_id;
    public string price;
}
[Serializable]
public class RequestParams
{
    public string account_id;
    public string contract_id;
    public string method_name;
    public string args;
    public string privatekey;
    public string publickey;
    public bool attachYoctoNear;
    public bool raise_gas;
}
[Serializable]
public class ProxyAccessKeyResponse
{
    public string privateKey;
    public string publicKey;
}

[Serializable]
public class ProxyCheckAccessKeyResponse
{
    public bool valid;
    public string allowance;
    public bool fullAccess;
    public bool player_registered;
}

[Serializable]
public class PlayerDataRequest
{
    public string account_id;
}
[Serializable]
public class ItemWrapper
{
    public string token_id;
    public int item_type;
    public int rarity_type;
    public int strength;
    public int dexterity;
    public int endurance;
    public int intelligence;
    public int luck;
}
[Serializable]
public class LeaderboardCharacter
{
    public string account_id;
    public int position;
    public int class_Type;
    public int char_level;
    public List<ItemWrapper> inventory;
}
[Serializable]
public class LeaderboardWrapper
{
    public List<LeaderboardCharacter> mage;
    public List<LeaderboardCharacter> knight;
    public List<LeaderboardCharacter> ranger;
}
public class TimestampHelper
{
    public static DateTime GetDateTimeFromTimestamp(long timestamp)
    {
        if (timestamp == 0)
        {
            return DateTime.Now;
        }
        DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddMilliseconds(timestamp).ToLocalTime();
        return dtDateTime;
    }
}
[Serializable]
public class BackendResponse<T>
{
    public bool success;
    public string error;
    public T data;
}
[Serializable]
public class PlayerDataResponse
{
    public PlayerBalance balance;
    public PlayerData playerdata;
    public List<ItemToken> items;
    public List<CharacterData> characters;
    //public bool owns_genesis;
    public bool maintenance;
}
[Serializable]
public class PlayerBalance
{
    public string pixeltoken;
    public long titan_timer;
    public int titan;
    public int common;
    public int rare;
    public int epic;
    public int legendary;
}
[Serializable]
public class PlayerData
{
    public int fight_balance;
}
[Serializable]
public class CharacterData
{
    public int class_type;
    public int experience;
    public int level;
    public long injured_timer;
    public List<int> inventory;
    public PotionData strengthPotion;
    public PotionData staminaPotion;
    public PotionData luckPotion;
}
[Serializable]
public class ItemToken
{
    public long cd;
    public string token_id;
    public int item_type;
    public int rarity_type;
    public int strength;
    public int dexterity;
    public int endurance;
    public int intelligence;
    public int luck;
    public string owner;
    public string price;
}
[Serializable]
public class PlayerBattleRequest : WalletAuth
{
    public PlayerFightData playerdata;
}
[Serializable]
public class PlayerFightData
{
    public int difficulty;
    public int class_type;
    public string[] inventory;
}
[Serializable]
public class UnlockRequest
{
    public int class_type;
}
[Serializable]
public class MarketRequest
{
    public string account_id;
    public MarketRequestItem itemdata;
}
[Serializable]
public class MarketWrapper
{
    public string token_id;
    public ulong price;
    public ItemToken item_data;
}
[Serializable]
public class MarketRequestItem
{
    public int class_type;
    public int equip_type;
    public int rarity_type;
    public int minStat;
}
[Serializable]
public class ItemEquipInfo
{
    public ClassType classType;
    public int itemID;
}
[Serializable]
public class ForgeRequest : WalletAuth
{
    public ForgeItems itemsdata;
}
[Serializable]
public class ForgeItems
{
    public string left_token_id;
    public string right_token_id;
}
[Serializable]
public class GenesisWrapper
{
    public long timestamp;
    public List<ItemToken> item_tokens;
}
[Serializable]
public class RoomWrapper
{
    public string account_id;
    public string week_code;
    public int difficulty;
    public int boss_kills;
    public List<string> playerNames;
    public List<int> playerClasses;
    public List<int> playerLevels;
    public List<int> playerRanks;
    public List<List<int>> playerEquippedItems;
    public List<StatStruct> playerStatStructs;
    public long[] playerJoinTimestamps;
}
[Serializable]
public class JoinRoomRequest : PlayerBattleRequest
{
    public RoomJoinData roomdata;
}
[Serializable]
public class RoomJoinData
{
    public string leader_id;
}
[Serializable]
public class RaidLeaderboardRequest : PlayerDataRequest
{
    public RaidLeaderboardData raiddata;
}
[Serializable]
public class RaidLeaderboardData
{
    public int difficulty;
}
[Serializable]
public class RaidLeaderboardWrapper
{
    public string account_id;
    public List<string> playerNames;
    public List<int> playerRanks;
    public List<int> playerClasses;
    public int boss_kills;
    public int position;
}
[Serializable]
public class LootboxRequest
{
    public string account_id;
    public int rarity_type;
}
[Serializable]
public class LootboxItemRequest : LootboxRequest
{
    public int item1_index;
    public int item2_index;
}
[Serializable]
public class KickData
{
    public string account_id;
}
[Serializable]
public class KickPlayerRequest : WalletAuth
{
    public string publickey;
    public KickData playerdata;
}
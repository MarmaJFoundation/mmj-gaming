using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using UnityEngine.UI;

public enum DataGetState
{
    Login = 0,
    MarketPurchase = 1,
    RefillPurchase = 2,
    AfterFight = 3,
    AfterForge = 4,
    AfterTrash = 5,
    AfterGenesis = 6,
    RaidPurchase = 7,
    AfterLootbox = 8
}

public class NearHelper : MonoBehaviour
{
    private string WalletUri = "https://testnet.mynearwallet.com/";
    private string ContractId = "pixeltoken.testnet";
    [HideInInspector]
    public string WalletSuffix = ".testnet";
    private string BackendUri = "https://intern-testnet.pixeldapps.co/api";
    public bool Testnet = true;
    private bool firstLogin = true;
    public GameController gameController;
    public RaidController raidController;
    public LoginController loginController;
    public MainMenuController mainMenuController;
    public GenesisController genesisController;
    public ForgeController forgeController;
    public BackpackController backpackController;
    public LeaderboardController leaderboardController;
    public DifficultyController difficultyController;
    public AuctionHallController auctionHallController;
    public PresaleController presaleController;
    public PotionController potionController;
    public CharacterInfoController[] characterInfoControllers;
    [HideInInspector]
    public DataGetState dataGetState;
    private int classIndex;
    private void Start()
    {
        loginController.Setup();
        if (BaseUtils.offlineMode)
        {
            loginController.OnReceiveLoginAuthorise();
            return;
        }
        if (!Testnet)
        {
            BackendUri = "https://ecosystem.pixeldapps.co/api";
            WalletUri = "https://www.mynearwallet.com/";
            ContractId = "pixeltoken.near";
            WalletSuffix = ".near";
        }
        if (Database.databaseStruct.playerAccount != null && Database.databaseStruct.privateKey != null && Database.databaseStruct.publicKey != null)
        {
            loginController.windowObj.SetActive(false);
            CheckForValidLogin();
        }
        else
        {
            Debug.Log("No login found, please login");
        }
    }
    public void CheckForValidLogin()
    {
        StartCoroutine(CheckLoginValid(Database.databaseStruct.playerAccount, Database.databaseStruct.publicKey));
    }
    public IEnumerator CallViewMethod<T>(string methodName, string args, Action<T> callbackFunction)
    {
        var requestData = RequestParamsBuilder.CreateFunctionCallRequest(this.ContractId, methodName, args);
        var content = JsonUtility.ToJson(requestData);
        return WebHelper.SendPost<T>(BackendUri + "/proxy/call-view-function", content, callbackFunction);
    }

    public IEnumerator CallChangeMethod<T>(string methodName, string args, Action<T> callbackFunction, string accountId, string privatekey, bool attachYoctoNear, bool raise_gas = false)
    {
        var requestData = RequestParamsBuilder.CreateFunctionCallRequest(this.ContractId, methodName, args, accountId, privatekey, attachYoctoNear, raise_gas);
        var content = JsonUtility.ToJson(requestData);

        return WebHelper.SendPost<T>(BackendUri + "/proxy/call-change-function", content, callbackFunction);
    }

    public void Login(string account_id)
    {
        Database.databaseStruct.playerAccount = account_id + WalletSuffix;
        StartCoroutine(WebHelper.SendGet<ProxyAccessKeyResponse>(BackendUri + "/proxy/get-ed25519pair", LoginCallback));
    }

    private void LoginCallback(ProxyAccessKeyResponse res)
    {
        Database.databaseStruct.privateKey = res.privateKey;
        Database.databaseStruct.publicKey = res.publicKey;
        Application.OpenURL(WalletUri + "/login/?success_url=https%3A%2F%2Fecosystem.pixeldapps.co%2Fcallback%3Fpage%3Dlogin_success&failure_url=https%3A%2F%2Fecosystem.pixeldapps.co%2Fcallback%3Fpage%3Dlogin_fail&public_key=" + res.publicKey + "&contract_id=" + ContractId);
        //Application.OpenURL(WalletUri + "/login/?referrer=CryptoHero%20Client&public_key=" + res.publicKey + "&contract_id=" + ContractId);
    }
    public void Logout(bool fromPlayer = false)
    {
        Database.databaseStruct.playerAccount = null;
        Database.databaseStruct.publicKey = null;
        Database.databaseStruct.privateKey = null;
        if (!fromPlayer)
        {
            if (!firstLogin)
            {
                BaseUtils.ShowWarningMessage("Invalid Login", new string[1] { "Login has been invalidated, please try again." });
                firstLogin = false;
            }
            loginController.ResetLogin();
        }
    }
    private IEnumerator CheckLoginValid(string accountId, string publickey)
    {
        BaseUtils.ShowLoading();
        var requestData = RequestParamsBuilder.CreateAccessKeyCheckRequest(accountId, publickey);
        var content = JsonUtility.ToJson(requestData);

        return WebHelper.SendPost<BackendResponse<ProxyCheckAccessKeyResponse>>(BackendUri + "/cryptohero/is-valid-login", content, CheckLoginValidCallback);
    }

    private void CheckLoginValidCallback(BackendResponse<ProxyCheckAccessKeyResponse> res)
    {

        //Debug.Log(res.data.allowance);
        //Debug.Log("Is player registered: " + res.data.player_registered);
        //Debug.Log("Is key valid: " + res.data.valid);

        BaseUtils.HideLoading();
        if (!res.success)
        {
            BaseUtils.ShowWarningMessage("Error on login", new string[2] { "Login was not possible!", "please check if your wallet has enough balance to cover transaction fees." });
            loginController.Setup();
            return;
        }
        if (!res.data.valid)
        {
            BaseUtils.ShowWarningMessage("Error on login", new string[2] { "Login was not possible!", "Data was not valid." });
            Logout();
            Database.databaseStruct.validCredentials = false;
            loginController.Setup();
            return;
        }
        string cropAllowance = res.data.allowance.Substring(0, 4);
        float.TryParse(cropAllowance, out float allowanceFloat);
        Database.databaseStruct.allowance = allowanceFloat;
        if (!res.data.player_registered)
        {
            StartCoroutine(RegisterPlayerCoroutine());
        }
        else
        {
            Database.databaseStruct.validCredentials = true;
            dataGetState = DataGetState.Login;
            StartCoroutine(GetPlayerData());
        }
    }

    public IEnumerator RegisterPlayerCoroutine()
    {
        BaseUtils.ShowLoading();
        return CallChangeMethod<BackendResponse<object>>("ch_register_player", null, GetPlayerDataCoroutine, Database.databaseStruct.playerAccount, Database.databaseStruct.privateKey, false);
    }

    public void GetPlayerDataCoroutine(BackendResponse<object> res)
    {
        BaseUtils.HideLoading();
        if (res.success)
        {
            dataGetState = DataGetState.Login;
            StartCoroutine(GetPlayerData());
        }
        else
        {
            loginController.Setup();
            BaseUtils.ShowWarningMessage("Error on login", new string[2] { "Login was not possible!", "please check if your wallet has enough balance to cover transaction fees." });
        }
    }

    public IEnumerator GetPlayerData()
    {
        BaseUtils.ShowLoading();
        var d = new PlayerDataRequest
        {
            account_id = Database.databaseStruct.playerAccount
        };
        var dString = JsonUtility.ToJson(d);

        return WebHelper.SendPost<BackendResponse<PlayerDataResponse>>(BackendUri + "/cryptohero/get-playerdata", dString, GetPlayerDataCallback);
    }

    public void GetPlayerDataCallback(BackendResponse<PlayerDataResponse> res)
    {
        BaseUtils.HideLoading();
        if (res.data.maintenance)
        {
            mainMenuController.SetMaintenanceMode();
            return;
        }
        CryptoHero.FillDatabaseFromPlayerResponse(res.data);
        switch (dataGetState)
        {
            case DataGetState.Login:
                loginController.OnReceiveLoginAuthorise();
                break;
            case DataGetState.MarketPurchase:
                auctionHallController.OnReceiveNewPlayerData();
                break;
            case DataGetState.RefillPurchase:
                difficultyController.OnReceiveNewPlayerData();
                break;
            case DataGetState.AfterFight:
                gameController.Setup(true);
                break;
            case DataGetState.AfterForge:
                forgeController.OnReceiveNewPlayerData();
                break;
            case DataGetState.AfterTrash:
                backpackController.OnReceiveNewPlayerData();
                break;
            case DataGetState.AfterGenesis:
                genesisController.OnReceiveNewPlayerData();
                break;
            case DataGetState.RaidPurchase:
                raidController.OnReceiveNewPlayerData();
                break;
            case DataGetState.AfterLootbox:
                presaleController.OnReceiveNewPlayerData();
                break;
        }
        dataGetState = DataGetState.Login;
    }
    public IEnumerator RequestSellItem(int creatureID, long price)
    {
        BaseUtils.ShowLoading();
        var d = new OfferSellAuth
        {
            account_id = Database.databaseStruct.playerAccount,
            privatekey = Database.databaseStruct.privateKey,
            itemdata = new OfferSellWrapper()
            {
                token_id = creatureID.ToString(),
                price = (price * 1000000).ToString()
            }
        };
        var dString = JsonUtility.ToJson(d);
        return WebHelper.SendPost<BackendResponse<object>>(BackendUri + "/cryptohero/marketplace/offer-item", dString, SellItemCallback);
    }
    public void SellItemCallback(BackendResponse<object> res)
    {
        RemoveAllowanceAndCheck();
        BaseUtils.HideLoading();
        if (res.success)
        {
            auctionHallController.OnAcceptItemSell();
        }
        else
        {
            SendErrorMessage(res.error);
        }
    }
    public IEnumerator RequestSellItem2(int creatureID, long price)
    {
        BaseUtils.ShowLoading();
        var d = new OfferSellAuth
        {
            account_id = Database.databaseStruct.playerAccount,
            privatekey = Database.databaseStruct.privateKey,
            itemdata = new OfferSellWrapper()
            {
                token_id = creatureID.ToString(),
                price = (price * 1000000).ToString()
            }
        };
        var dString = JsonUtility.ToJson(d);
        return WebHelper.SendPost<BackendResponse<object>>(BackendUri + "/cryptohero/marketplace/offer-item", dString, SellItemCallback2);
    }
    public void SellItemCallback2(BackendResponse<object> res)
    {
        RemoveAllowanceAndCheck();
        BaseUtils.HideLoading();
        if (res.success)
        {
            backpackController.OnAcceptItemSell();
        }
        else
        {
            SendErrorMessage(res.error);
        }
    }
    public IEnumerator RequestUnlockCharacter(ClassType classType)
    {
        BaseUtils.ShowLoading();
        var d = new UnlockRequest
        {
            class_type = (int)classType
        };
        var dString = JsonUtility.ToJson(d);
        classIndex = (int)classType - 1;
        return CallChangeMethod<BackendResponse<object>>("ch_unlock_character", dString, UnlockCallback, Database.databaseStruct.playerAccount, Database.databaseStruct.privateKey, false);
    }
    public void UnlockCallback(BackendResponse<object> res)
    {
        BaseUtils.HideLoading();
        RemoveAllowanceAndCheck();
        if (res.success)
        {
            characterInfoControllers[classIndex].OnSuccessfulUnlock();
        }
        else
        {
            SendErrorMessage(res.error);
        }
    }
    public IEnumerator RequestMarketData(ClassType classType, EquipType equipType, RarityType rarityType, int minStat)
    {
        BaseUtils.ShowLoading();
        var d = new MarketRequest
        {
            account_id = Database.databaseStruct.playerAccount,
            itemdata = new MarketRequestItem()
            {
                class_type = (int)classType,
                equip_type = (int)equipType,
                rarity_type = (int)rarityType,
                minStat = minStat
            }
        };
        var dString = JsonUtility.ToJson(d);
        return WebHelper.SendPost<BackendResponse<List<MarketWrapper>>>(BackendUri + "/cryptohero/marketplace/advanced-search", dString, MarketDataCallback);
    }
    public void MarketDataCallback(BackendResponse<List<MarketWrapper>> res)
    {
        BaseUtils.HideLoading();
        if (res.success)
        {
            auctionHallController.OnReceiveAuctionHallItems(res.data);
        }
        else
        {
            SendErrorMessage(res.error);
        }
    }
    public IEnumerator RequestCancelItemSell(int itemID)
    {
        BaseUtils.ShowLoading();
        var d = new OfferAuth
        {
            account_id = Database.databaseStruct.playerAccount,
            privatekey = Database.databaseStruct.privateKey,
            itemdata = new OfferWrapper
            {
                token_id = itemID.ToString()
            }
        };
        var dString = JsonUtility.ToJson(d);
        return WebHelper.SendPost<BackendResponse<object>>(BackendUri + "/cryptohero/marketplace/cancel-offer-item", dString, CancelItemSellCallback);
    }
    public void CancelItemSellCallback(BackendResponse<object> res)
    {
        RemoveAllowanceAndCheck();
        BaseUtils.HideLoading();
        if (res.success)
        {
            auctionHallController.OnCancelItemSell();
        }
        else
        {
            SendErrorMessage(res.error);
        }
    }
    public IEnumerator RequestCancelItemSell2(int itemID)
    {
        BaseUtils.ShowLoading();
        var d = new OfferAuth
        {
            account_id = Database.databaseStruct.playerAccount,
            privatekey = Database.databaseStruct.privateKey,
            itemdata = new OfferWrapper
            {
                token_id = itemID.ToString()
            }
        };
        var dString = JsonUtility.ToJson(d);
        return WebHelper.SendPost<BackendResponse<object>>(BackendUri + "/cryptohero/marketplace/cancel-offer-item", dString, CancelItemSellCallback2);
    }
    public void CancelItemSellCallback2(BackendResponse<object> res)
    {
        RemoveAllowanceAndCheck();
        BaseUtils.HideLoading();
        if (res.success)
        {
            backpackController.OnCancelItemSell();
        }
        else
        {
            SendErrorMessage(res.error);
        }
    }
    public IEnumerator RequestBuyItem(int itemID)
    {
        BaseUtils.ShowLoading();
        var d = new OfferAuth
        {
            account_id = Database.databaseStruct.playerAccount,
            privatekey = Database.databaseStruct.privateKey,
            itemdata = new OfferWrapper
            {
                token_id = itemID.ToString()
            }
        };
        var dString = JsonUtility.ToJson(d);
        return WebHelper.SendPost<BackendResponse<string>>(BackendUri + "/cryptohero/marketplace/buy-item", dString, BuyItemCallback);
    }
    public void BuyItemCallback(BackendResponse<string> res)
    {
        RemoveAllowanceAndCheck();
        BaseUtils.HideLoading();
        if (res.success)
        {
            Application.OpenURL(res.data);
            auctionHallController.ShowBuyAuthorize();
        }
        else
        {
            SendErrorMessage(res.error);
        }
    }
    public void RemoveAllowanceAndCheck()
    {
        Database.databaseStruct.allowance -= 50;
        //mainMenuController.allowanceText.SetString($"allowance: {Database.databaseStruct.allowance}");
        if (Database.databaseStruct.allowance <= 0)
        {
            BaseUtils.ShowWarningMessage("Not enough allowance", new string[3] { "Your allowance has ran out", "to continue operating smoothly, you need to re-login.", "Press Ok to logout and then login again" }, OnAcceptRelogin);
            //mainMenuController.ShowAllowanceWindow();
        }
    }
    public IEnumerator RequestFightRefill()
    {
        BaseUtils.ShowLoading();
        var d = new WalletAuth
        {
            account_id = Database.databaseStruct.playerAccount,
            privatekey = Database.databaseStruct.privateKey
        };
        var dString = JsonUtility.ToJson(d);
        return WebHelper.SendPost<BackendResponse<string>>(BackendUri + "/cryptohero/refill-fightpoints", dString, FightRefillCallback);
    }
    public void FightRefillCallback(BackendResponse<string> res)
    {
        RemoveAllowanceAndCheck();
        BaseUtils.HideLoading();
        Application.OpenURL(res.data);
        difficultyController.ShowRefillAuthorize();
    }
    public IEnumerator RequestRaidRefill()
    {
        BaseUtils.ShowLoading();
        var d = new WalletAuth
        {
            account_id = Database.databaseStruct.playerAccount,
            privatekey = Database.databaseStruct.privateKey
        };
        var dString = JsonUtility.ToJson(d);
        return WebHelper.SendPost<BackendResponse<string>>(BackendUri + "/cryptohero/refill-fightpoints", dString, RaidRefillCallback);
    }
    public void RaidRefillCallback(BackendResponse<string> res)
    {
        RemoveAllowanceAndCheck();
        BaseUtils.HideLoading();
        Application.OpenURL(res.data);
        raidController.ShowRefillAuthorize();
    }
    public IEnumerator RequestLeaderboardData()
    {
        BaseUtils.ShowLoading();
        var d = new WalletAuth
        {
            account_id = Database.databaseStruct.playerAccount,
            privatekey = Database.databaseStruct.privateKey
        };
        var dString = JsonUtility.ToJson(d);
        return WebHelper.SendPost<BackendResponse<LeaderboardWrapper>>(BackendUri + "/cryptohero/get-leaderboard", dString, LeaderboardDataCallback);
    }
    public void LeaderboardDataCallback(BackendResponse<LeaderboardWrapper> res)
    {
        BaseUtils.HideLoading();
        if (res.success)
        {
            leaderboardController.OnReceiveRankData(res.data);
        }
        else
        {
            SendErrorMessage(res.error);
        }
    }
    public IEnumerator RequestTrashItem(int itemID)
    {
        BaseUtils.ShowLoading();
        var d = new OfferWrapper
        {
            token_id = itemID.ToString()
        };
        var dString = JsonUtility.ToJson(d);

        return CallChangeMethod<BackendResponse<object>>("ch_delete_item", dString, TrashItemCallback, Database.databaseStruct.playerAccount, Database.databaseStruct.privateKey, false);
    }
    public void TrashItemCallback(BackendResponse<object> res)
    {
        RemoveAllowanceAndCheck();
        BaseUtils.HideLoading();
        if (res.success)
        {
            backpackController.OnTrashItemCallback();
        }
        else
        {
            SendErrorMessage(res.error);
        }
    }
    public IEnumerator RequestForgeItem(int itemID1, int itemID2)
    {
        BaseUtils.ShowLoading();
        var d = new ForgeItems
        {
            left_token_id = itemID1.ToString(),
            right_token_id = itemID2.ToString()
        };
        /*var d = new ForgeRequest
        {
            account_id = Database.databaseStruct.playerAccount,
            privatekey = Database.databaseStruct.privateKey,
            itemsdata = new ForgeItems()
            {
                left_token_id = itemID1.ToString(),
                right_token_id = itemID2.ToString()
            }
        };*/
        var dString = JsonUtility.ToJson(d);
        return CallChangeMethod<BackendResponse<ItemToken>>("ch_forge_items", dString, ForgeItemCallback, Database.databaseStruct.playerAccount, Database.databaseStruct.privateKey, false, true);
        //return WebHelper.SendPost<BackendResponse<ItemToken>>(BackendUri + "/cryptohero/forge-items", dString, ForgeItemCallback);
    }
    public void ForgeItemCallback(BackendResponse<ItemToken> res)
    {
        RemoveAllowanceAndCheck();
        BaseUtils.HideLoading();
        if (res.success)
        {
            forgeController.OnForgeItemCallback(res.data);
        }
        else
        {
            SendErrorMessage(res.error);
        }
    }
    public IEnumerator RequestDungeonFight(int difficulty, ClassType classType, List<ItemData> inventory)
    {
        BaseUtils.ShowLoading();
        var fr = new PlayerBattleRequest
        {
            account_id = Database.databaseStruct.playerAccount,
            privatekey = Database.databaseStruct.privateKey,
            playerdata = new PlayerFightData
            {
                difficulty = difficulty,
                class_type = (int)classType,
                inventory = GenerateIDInventory(inventory)
            }
        };
        var dString = JsonUtility.ToJson(fr);
        return WebHelper.SendPost<BackendResponse<DungeonFightStruct>>(BackendUri + "/cryptohero/simulate-dungeon", dString, DungeonFightCallback);
    }
    private string[] GenerateIDInventory(List<ItemData> inventory)
    {
        List<string> idInventory = new List<string>();
        for (int i = 0; i < inventory.Count; i++)
        {
            idInventory.Add(inventory[i].itemID.ToString());
        }
        return idInventory.ToArray();
    }
    private void DungeonFightCallback(BackendResponse<DungeonFightStruct> res)
    {
        BaseUtils.HideLoading();
        if (res.success)
        {
            difficultyController.OnReceiveDungeonStruct(res.data);
        }
        else
        {
            SendErrorMessage(res.error);
        }
    }
    public IEnumerator RequestGenesis()
    {
        BaseUtils.ShowLoading();
        return CallChangeMethod<BackendResponse<GenesisWrapper>>("ch_claim_titan_reward", null, GenesisCallback, Database.databaseStruct.playerAccount, Database.databaseStruct.privateKey, false, true);
    }
    public void GenesisCallback(BackendResponse<GenesisWrapper> res)
    {
        RemoveAllowanceAndCheck();
        BaseUtils.HideLoading();
        if (res.success)
        {
            genesisController.OnGenesisCallback(res.data);
        }
        else
        {
            SendErrorMessage(res.error);
        }
    }
    public IEnumerator RequestCreateRoom(int difficulty, ClassType classType, List<ItemData> inventory)
    {
        BaseUtils.ShowLoading();
        var fr = new PlayerBattleRequest
        {
            account_id = Database.databaseStruct.playerAccount,
            privatekey = Database.databaseStruct.privateKey,
            playerdata = new PlayerFightData
            {
                difficulty = difficulty,
                class_type = (int)classType,
                inventory = GenerateIDInventory(inventory)
            }
        };
        var dString = JsonUtility.ToJson(fr);
        return WebHelper.SendPost<BackendResponse<RoomWrapper>>(BackendUri + "/cryptohero/raid/create-room", dString, CreateRoomCallback);
    }
    private void CreateRoomCallback(BackendResponse<RoomWrapper> res)
    {
        BaseUtils.HideLoading();
        if (res.success)
        {
            raidController.JoinRoom(res.data);
        }
        else
        {
            SendErrorMessage(res.error);
        }
    }
    public IEnumerator RequestKickMember(string memberWallet)
    {
        BaseUtils.ShowLoading();
        var fr = new KickPlayerRequest
        {
            account_id = Database.databaseStruct.playerAccount,
            privatekey = Database.databaseStruct.privateKey,
            publickey = Database.databaseStruct.publicKey,
            playerdata = new KickData
            {
                account_id = memberWallet
            },
        };
        var dString = JsonUtility.ToJson(fr);
        return WebHelper.SendPost<BackendResponse<RoomWrapper>>(BackendUri + "/cryptohero/raid/kick-player", dString, KickMemberCallback);
    }
    public void KickMemberCallback(BackendResponse<RoomWrapper> res)
    {
        BaseUtils.HideLoading();
        if (res.success)
        {
            raidController.KickMemberCallback(res.data);
        }
        else
        {
            if (res.error != "LackBalanceForState" && res.error != "NotEnoughAllowance")
            {
                BaseUtils.ShowWarningMessage("Error on kicking member", new string[2] { "There was an error kicking that member", "please make sure you are inputting the correct wallet adress"});
            }
            else
            {
                SendErrorMessage(res.error);
            }
        }
    }

    public IEnumerator RequestJoinRoom(int difficulty, ClassType classType, List<ItemData> inventory, string leaderWallet)
    {
        BaseUtils.ShowLoading();
        var fr = new JoinRoomRequest
        {
            account_id = Database.databaseStruct.playerAccount,
            privatekey = Database.databaseStruct.privateKey,
            playerdata = new PlayerFightData
            {
                difficulty = difficulty,
                class_type = (int)classType,
                inventory = GenerateIDInventory(inventory)
            },
            roomdata = new RoomJoinData
            {
                leader_id = leaderWallet
            }
        };
        var dString = JsonUtility.ToJson(fr);
        return WebHelper.SendPost<BackendResponse<RoomWrapper>>(BackendUri + "/cryptohero/raid/join-room", dString, JoinRoomCallback);
    }
    private void JoinRoomCallback(BackendResponse<RoomWrapper> res)
    {
        BaseUtils.HideLoading();
        if (res.success)
        {
            raidController.JoinRoom(res.data);
        }
        else
        {
            if (res.error != "LackBalanceForState" && res.error != "NotEnoughAllowance")
            {
                BaseUtils.ShowWarningMessage("Error on joining group", new string[3] { "There was an error joining that group", "please make sure you are inputting the correct wallet adress with .near at the end!", "if it persists, refresh your page." });
            }
            else
            {
                SendErrorMessage(res.error);
            }
        }
    }
    public IEnumerator RequestRoomData()
    {
        BaseUtils.ShowLoading();
        var fr = new PlayerDataRequest
        {
            account_id = Database.databaseStruct.playerAccount,
        };
        var dString = JsonUtility.ToJson(fr);
        return WebHelper.SendPost<BackendResponse<RoomWrapper>>(BackendUri + "/cryptohero/raid/get-room-info", dString, RoomDataCallback);
    }
    private void RoomDataCallback(BackendResponse<RoomWrapper> res)
    {
        BaseUtils.HideLoading();
        if (res.success)
        {
            raidController.OnReceiveRoomData(res.data);
        }
        else
        {
            SendErrorMessage(res.error);
        }
    }
    public IEnumerator RequestSimulateRaidFight(ClassType classType, List<ItemData> inventory, string leaderWallet)
    {
        BaseUtils.ShowLoading();
        var fr = new JoinRoomRequest
        {
            account_id = Database.databaseStruct.playerAccount,
            privatekey = Database.databaseStruct.privateKey,
            playerdata = new PlayerFightData
            {
                class_type = (int)classType,
                inventory = GenerateIDInventory(inventory)
            },
            roomdata = new RoomJoinData
            {
                leader_id = leaderWallet
            }
        };
        var dString = JsonUtility.ToJson(fr);
        return WebHelper.SendPost<BackendResponse<BossFightStruct>>(BackendUri + "/cryptohero/raid/simulate-fight", dString, RaidFightCallback);
    }
    private void RaidFightCallback(BackendResponse<BossFightStruct> res)
    {
        BaseUtils.HideLoading();
        if (res.success)
        {
            raidController.OnRaidStartCallback(res.data);
        }
        else
        {
            SendErrorMessage(res.error);
        }
    }
    public IEnumerator RequestRaidLeaderboardData(int difficulty)
    {
        BaseUtils.ShowLoading();
        var d = new RaidLeaderboardRequest
        {
            account_id = Database.databaseStruct.playerAccount,
            raiddata = new RaidLeaderboardData
            {
                difficulty = difficulty
            }
        };
        var dString = JsonUtility.ToJson(d);
        return WebHelper.SendPost<BackendResponse<List<RaidLeaderboardWrapper>>>(BackendUri + "/cryptohero/raid/get-highscores", dString, RaidLeaderboardDataCallback);
    }
    public void RaidLeaderboardDataCallback(BackendResponse<List<RaidLeaderboardWrapper>> res)
    {
        BaseUtils.HideLoading();
        if (res.success)
        {
            raidController.OnReceiveLeaderboardData(res.data);
        }
        else
        {
            SendErrorMessage(res.error);
        }
    }
    public IEnumerator RequestLootbox(RarityType rarityType)
    {
        BaseUtils.ShowLoading();
        var fr = new LootboxRequest
        {
            account_id = Database.databaseStruct.playerAccount,
            rarity_type = (int)rarityType
        };
        var dString = JsonUtility.ToJson(fr);
        return WebHelper.SendPost<BackendResponse<List<ItemToken>>>(BackendUri + "/cryptohero/presale/request-lootbox", dString, LootboxCallback);
    }
    public void LootboxCallback(BackendResponse<List<ItemToken>> res)
    {
        BaseUtils.HideLoading();
        if (res.success)
        {
            presaleController.OnReceivePresaleChestItems(res.data);
        }
        else
        {
            SendErrorMessage(res.error);
        }
    }
    public IEnumerator RequestItemLootbox(RarityType rarityType, int itemIndex1, int itemIndex2)
    {
        BaseUtils.ShowLoading();
        var fr = new LootboxItemRequest
        {
            account_id = Database.databaseStruct.playerAccount,
            rarity_type = (int)rarityType,
            item1_index = itemIndex1,
            item2_index = itemIndex2
        };
        var dString = JsonUtility.ToJson(fr);
        return WebHelper.SendPost<BackendResponse<List<object>>>(BackendUri + "/cryptohero/presale/open-lootbox", dString, LootboxItemCallback);
    }
    public void LootboxItemCallback(BackendResponse<List<object>> res)
    {
        BaseUtils.HideLoading();
        if (res.success)
        {
            dataGetState = DataGetState.AfterLootbox;
            StartCoroutine(GetPlayerData());
        }
        else
        {
            SendErrorMessage(res.error);
        }
    }
    private void SendErrorMessage(string error)
    {
        switch (error)
        {
            case "LackBalanceForState":
                BaseUtils.ShowWarningMessage("Lack of balance", new string[2] { "Not enough funds to cover simple transactions", "please fill your wallet with some near." });
                break;
            case "NotEnoughAllowance":
                //mainMenuController.ShowAllowanceWindow();
                BaseUtils.ShowWarningMessage("Not enough allowance", new string[3] { "Your allowance has ran out", "to continue operating smoothly, you need to re-login.", "Press Ok to logout and then login again" }, OnAcceptRelogin);
                break;
            default:
                BaseUtils.ShowWarningMessage("Error on loading info", new string[2] { "There was an error with your connection", "if it persists, refresh your page." });
                break;
        }
    }
    private void OnAcceptRelogin()
    {
        mainMenuController.OnLogoutClick();
    }
    public IEnumerator RequestPotionBuy(int classType, int potionToBuy)
    {
        BaseUtils.ShowLoading();
        var d = new
        {
            class_type = classType,
            potion_type = potionToBuy,
        };
        var dString = JsonConvert.SerializeObject(d);
        return CallChangeMethod<BackendResponse<PotionData>>("ch_character_buy_potion", dString, PotionBuyCallback, Database.databaseStruct.playerAccount, Database.databaseStruct.privateKey, false);
    }
    public void PotionBuyCallback(BackendResponse<PotionData> res)
    {
        RemoveAllowanceAndCheck();
        BaseUtils.HideLoading();
        if (res.success)
        {
            potionController.OnPotionBuyCallback(res.data);
        }
        else
        {
            SendErrorMessage(res.error);
        }
    }
}

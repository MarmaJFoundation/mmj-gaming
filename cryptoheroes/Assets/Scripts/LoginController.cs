using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    public NearHelper nearhelper;
    public GameController gameController;
    public MainMenuController mainMenuController;
    public GenesisController genesisController;
    public PresaleController presaleController;
    public CustomInput customInput;
    public CustomText nearText;
    public GameObject loginButtonObj;
    public GameObject authButtonObj;
    public GameObject introObj;
    public GameObject windowObj;
    public EventSystem eventSystem;
    public Canvas loginCanvas;
    private string playerAccountName;
    public void Setup()
    {
        loginCanvas.enabled = true;
        loginCanvas.gameObject.SetActive(true);
        windowObj.SetActive(true);
        customInput.ResetInput();
        customInput.OnPointerClick(null);
        eventSystem.SetSelectedGameObject(customInput.gameObject);
        nearText.SetString(nearhelper.Testnet ? ".testnet" : ".near");
    }
    public void ResetLogin()
    {
        Setup();
        customInput.enabled = true;
        customInput.ResetInput();
        customInput.OnPointerClick(null);
        eventSystem.SetSelectedGameObject(customInput.gameObject);
        authButtonObj.SetActive(false);
        loginButtonObj.SetActive(true);
    }
    //#protocol
    public void OnLoginClick()
    {
        if (customInput.typeString == "")
        {
            BaseUtils.ShowWarningMessage("Error!", new string[2] { "Address cannot be empty.", "Enter your Near Wallet adress." });
            return;
        }
        if (customInput.typeString.Length >= 64)
        {
            BaseUtils.ShowWarningMessage("Error!", new string[2] { "Address not supported. Contact support", "If you wish to use non near accounts." });
            return;
        }
        playerAccountName = customInput.typeString.ToLower();
        if (playerAccountName.Contains(".near"))
        {
            playerAccountName = playerAccountName.Remove(playerAccountName.IndexOf('.'), 5);
        }
        customInput.enabled = false;
        if (BaseUtils.offlineMode)
        {
            BaseUtils.ShowLoading();
        }
        else
        {
            nearhelper.Login(playerAccountName);
        }
        StartCoroutine(WaitAndShowAuthButton());
    }
    private IEnumerator WaitAndShowAuthButton()
    {
        loginButtonObj.SetActive(false);
        BaseUtils.ShowLoading();
        yield return new WaitForSeconds(5);
        BaseUtils.HideLoading();
        authButtonObj.SetActive(true);
    }
    //player clicked the authorized button after the login
    public void OnAuthClick()
    {
        windowObj.SetActive(false);
        if (BaseUtils.offlineMode)
        {
            OnReceiveLoginAuthorise();
        }
        else
        {
            nearhelper.CheckForValidLogin();
        }
    }
    public void OnLoginError(int errorType)
    {
        switch (errorType)
        {
            //not authorized in near page
            case 0:
                BaseUtils.ShowWarningMessage("Error!", new string[2] { "Login info was not authorized.", "Please try again!" });
                break;
            //wrong account name
            case 1:
                BaseUtils.ShowWarningMessage("Error!", new string[2] { "Wallet adress is not correct.", "Please try again!" });
                break;
            //uknown/server error
            case 2:
                BaseUtils.ShowWarningMessage("Error!", new string[2] { "The servers are acting weird!", "Please try again later." });
                break;
        }
        BaseUtils.HideLoading();
        loginButtonObj.SetActive(true);
        authButtonObj.SetActive(false);
        customInput.enabled = true;
        customInput.ResetInput();
        customInput.OnPointerClick(null);
        eventSystem.SetSelectedGameObject(customInput.gameObject);
    }
    public void OnReceiveLoginAuthorise()
    {
        loginCanvas.enabled = false;
        loginCanvas.gameObject.SetActive(false);
        BaseUtils.HideLoading();
        if (BaseUtils.offlineMode)
        {
            Database.databaseStruct.playerAccount = "anon.near";
            Database.databaseStruct.pixelTokens = 6500;
            Database.databaseStruct.presaleChests = new List<RarityType>() { RarityType.Common, RarityType.Rare, RarityType.Epic, RarityType.Legendary };
            Database.databaseStruct.genesisAmount = 5;
            Database.databaseStruct.genesisTime = System.DateTime.Now.AddDays(30);
        }
        if (!Database.databaseStruct.firstLogin)
        {
            Database.databaseStruct.firstLogin = true;
            StartCoroutine(IntroCoroutine());
        }
        else
        {
            gameController.Setup();
            presaleController.Setup();
            genesisController.Setup();
            mainMenuController.Setup();
        }
    }
    private IEnumerator IntroCoroutine()
    {
        mainMenuController.blackScreen.enabled = true;
        mainMenuController.blackScreen.color = Color.black;
        introObj.SetActive(true);
        SoundController.PlaySound("CryptoHeroesIntro_1");
        float timer = 0;
        while (timer <= 1)
        {
            foreach (Image image in introObj.GetComponentsInChildren<Image>())
            {
                image.color = Color.Lerp(Color.clear, Color.white, timer.Evaluate(CurveType.EaseOut));
            }
            timer += Time.deltaTime * 4f;
            yield return null;
        }
        yield return new WaitForSeconds(3);
        gameController.Setup();
        presaleController.Setup();
        genesisController.Setup();
        mainMenuController.Setup();
        timer = 0;
        while (timer <= 1)
        {
            foreach (Image image in introObj.GetComponentsInChildren<Image>())
            {
                image.color = Color.Lerp(Color.white, Color.clear, timer.Evaluate(CurveType.EaseIn));
            }
            timer += Time.deltaTime * 4f;
            yield return null;
        }
        introObj.SetActive(false);
        yield return new WaitForSeconds(.1f);
        BaseUtils.ShowWarningMessage("Welcome to Crypto Heroes!", new string[6] {
            "Here are some useful tips for you before you start playing Crypto hero:",
            "1. Look for cheap items on the marketplace, these will be useful in order to grind dungeons",
            "2. Focus on a single hero at the beginning, once you have enough items, look into other heroes",
            "3. raids are the main form of earning in the game, but in order to raid, you need good items",
            "4. focus on dungeon grinding in order to form an economy and start trading/forging new items",
            "5. join our discord to meet more players who can help you with the game, as well as raiding!"});
    }
}

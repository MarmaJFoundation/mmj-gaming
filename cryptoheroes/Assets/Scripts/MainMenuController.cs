using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public enum WindowType
{
    MainScreen = 0,
    AuctionHall = 1,
    Inventory = 2,
    Difficulty = 3,
    Leaderboard = 4,
    Forge = 5,
    Raid = 6,
    Potion = 7
}

public class MainMenuController : MonoBehaviour
{
    public AuctionHallController auctionHallController;
    public InventoryController inventoryController;
    public GameController gameController;
    public DungeonController dungeonController;
    public DifficultyController difficultyController;
    public LeaderboardController leaderboardController;
    public PotionController potionController;
    public CameraController cameraController;
    public RaidController raidController;
    public ForgeController forgeController;
    public NearHelper nearHelper;
    private RectTransform tooltipTarget;
    private RectTransform textTooltipTarget;
    public Canvas mainCanvas;
    public CanvasScaler canvasScaler;
    public ItemTooltip itemTooltip;
    public Image blackScreen;
    //public GameObject backScreen;
    public GameObject authWindow;
    public GameObject textPrefab;
    public GameObject buttonsWindow;
    public GameObject maintenanceWindow;
    public GameObject rainObj;
    public RectTransform textTooltip;
    public CustomText[] textTooltipTexts;
    public CustomButton rainButton;
    public CustomButton soundButton;
    public CustomButton musicButton;
    public GraphicRaycaster[] canvasRaycasters;
    public readonly Queue<TextController> textPool = new Queue<TextController>();
    public readonly List<TextController> activeTexts = new List<TextController>();
    private Coroutine tooltipCoroutine;
    [HideInInspector]
    public WindowType currentWindow;
    private TooltipPosition tooltipPosition;
    public void Setup()
    {
        blackScreen.enabled = true;
        blackScreen.color = Color.black;
        soundButton.SetDeactivated(Database.databaseStruct.soundVolume == 100);
        musicButton.SetDeactivated(Database.databaseStruct.musicVolume == 100);
        rainButton.SetDeactivated(Database.databaseStruct.rainOff);
        rainObj.SetActive(!Database.databaseStruct.rainOff);
        SoundController.PlayMusic("CHMenu");
        FadeOutBlack();
    }
    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Database.SetInjured(ClassType.Knight, System.DateTime.Now);
            Database.SetInjured(ClassType.Mage, System.DateTime.Now);
            Database.SetInjured(ClassType.Ranger, System.DateTime.Now);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            raidController.OnRaidStartCallback(raidController.bossFightController.GenerateFightStruct(raidController.selectedDifficulty, raidController.roomDatas[raidController.selectedDifficulty]));
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (Database.GetCharStruct(ClassType.Knight).experience >= BaseUtils.GetExpForNextLevel(Database.GetCharStruct(ClassType.Knight).level))
            {
                Database.AddLevel(ClassType.Knight);
            }
        }
        if (textTooltip.gameObject.activeSelf)
        {
            Vector3 offsetPos = Vector3.zero;
            switch (tooltipPosition)
            {
                case TooltipPosition.BottomCenter:
                    offsetPos += Vector3.down * (textTooltip.sizeDelta.y / 2 + textTooltipTarget.sizeDelta.y / 2);
                    break;
                case TooltipPosition.BottomRight:
                    offsetPos += Vector3.down * (textTooltip.sizeDelta.y / 2 + textTooltipTarget.sizeDelta.y / 2);
                    offsetPos += Vector3.right * (textTooltip.sizeDelta.x / 2 + textTooltipTarget.sizeDelta.x / 2);
                    break;
                case TooltipPosition.BottomLRight:
                    offsetPos += Vector3.down * (textTooltip.sizeDelta.y / 2 + textTooltipTarget.sizeDelta.y / 2);
                    offsetPos += Vector3.right * textTooltipTarget.sizeDelta.x / 2;
                    break;
                case TooltipPosition.BottomLLeft:
                    offsetPos += Vector3.down * (textTooltip.sizeDelta.y / 2 + textTooltipTarget.sizeDelta.y / 2);
                    offsetPos += Vector3.left * textTooltipTarget.sizeDelta.x / 2;
                    break;
                case TooltipPosition.TopCenter:
                    offsetPos += Vector3.up * (textTooltip.sizeDelta.y / 2 + textTooltipTarget.sizeDelta.y / 2);
                    break;
                case TooltipPosition.TopRight:
                    offsetPos += Vector3.up * (textTooltip.sizeDelta.y / 2 + textTooltipTarget.sizeDelta.y / 2);
                    offsetPos += Vector3.right * (textTooltip.sizeDelta.x / 2 + textTooltipTarget.sizeDelta.x / 2);
                    break;
                case TooltipPosition.TopLRight:
                    offsetPos += Vector3.up * (textTooltip.sizeDelta.y / 2 + textTooltipTarget.sizeDelta.y / 2);
                    offsetPos += Vector3.right * textTooltipTarget.sizeDelta.x / 2;
                    break;
                case TooltipPosition.TopLeft:
                    offsetPos += Vector3.up * (textTooltip.sizeDelta.y / 2 + textTooltipTarget.sizeDelta.y / 2);
                    offsetPos += Vector3.left * (textTooltip.sizeDelta.x / 2 + textTooltipTarget.sizeDelta.x / 2);
                    break;
                case TooltipPosition.Right:
                    offsetPos += Vector3.right * (textTooltip.sizeDelta.x / 2 + textTooltipTarget.sizeDelta.x / 2);
                    break;
            }
            offsetPos *= canvasScaler.scaleFactor;
            offsetPos *= cameraController.OrthoDiff;
            textTooltip.position = BaseUtils.mainCam.ScreenToWorldPoint(BaseUtils.mainCam.WorldToScreenPoint(textTooltipTarget.position) + offsetPos);
            if (!textTooltipTarget.gameObject.activeSelf || !textTooltipTarget.parent.gameObject.activeSelf)
            {
                HideTextTooltip();
            }
        }
        if (BaseUtils.showingLoot)
        {
            return;
        }
        if (itemTooltip.gameObject.activeSelf && !BaseUtils.onFight)
        {
            Vector3 offsetPos = Vector3.zero;
            Vector3 screenPos = BaseUtils.mainCam.WorldToScreenPoint(tooltipTarget.position);
            if (screenPos.x > Screen.width / 2)
            {
                offsetPos += Vector3.left * (tooltipTarget.sizeDelta.x / 2 + itemTooltip.rectTransform.sizeDelta.x / 2);
            }
            else
            {
                offsetPos += Vector3.right * (tooltipTarget.sizeDelta.x / 2 + itemTooltip.rectTransform.sizeDelta.x / 2);
            }
            if (screenPos.y < Screen.height / 2)
            {
                offsetPos += Vector3.up * (tooltipTarget.sizeDelta.y / 2 + itemTooltip.rectTransform.sizeDelta.y / 2);
            }
            else
            {
                offsetPos += Vector3.down * (tooltipTarget.sizeDelta.y / 2 + itemTooltip.rectTransform.sizeDelta.y / 2);
                if (screenPos.x > Screen.width / 2)
                {
                    offsetPos += Vector3.left * 10;
                }
                else
                {
                    offsetPos += Vector3.right * 10;
                }
            }
            offsetPos *= canvasScaler.scaleFactor;
            offsetPos *= cameraController.OrthoDiff;
            itemTooltip.rectTransform.position = BaseUtils.mainCam.ScreenToWorldPoint(BaseUtils.mainCam.WorldToScreenPoint(tooltipTarget.position) + offsetPos);
            if (!tooltipTarget.gameObject.activeSelf || !tooltipTarget.parent.gameObject.activeSelf)
            {
                HideTooltip();
            }
        }
    }
    public void OnSoundClick()
    {
        if (Database.databaseStruct.soundVolume == 0)
        {
            Database.databaseStruct.soundVolume = 100;
        }
        else
        {
            Database.databaseStruct.soundVolume = 0;
        }
        soundButton.SetDeactivated(Database.databaseStruct.soundVolume == 100);
        Database.SaveDatabase();
    }
    public void OnMusicClick()
    {
        if (Database.databaseStruct.musicVolume == 0)
        {
            Database.databaseStruct.musicVolume = 100;
        }
        else
        {
            Database.databaseStruct.musicVolume = 0;
        }
        musicButton.SetDeactivated(Database.databaseStruct.musicVolume == 100);
        SoundController.OnChangeMusicVolume();
        Database.SaveDatabase();
    }
    public void OnAuctionHallClick()
    {
        HideTextTooltip();
        gameController.Dispose();
        auctionHallController.Setup();
        currentWindow = WindowType.AuctionHall;
    }
    public void OnLeaderboardClick()
    {
        HideTextTooltip();
        gameController.Dispose();
        leaderboardController.Setup();
        currentWindow = WindowType.Leaderboard;
    }
    public void OnForgeClick()
    {
        HideTextTooltip();
        gameController.Dispose();
        forgeController.Setup();
        currentWindow = WindowType.Forge;
    }
    public void OnRaidClick()
    {
        HideTextTooltip();
        gameController.Dispose();
        raidController.Setup();
        currentWindow = WindowType.Raid;
    }
    public void OnInventoryClick(int classType)
    {
        if (BaseUtils.onFight)
        {
            return;
        }
        HideTextTooltip();
        inventoryController.Setup((ClassType)classType);
        if (currentWindow == WindowType.Difficulty)
        {
            difficultyController.Dispose(true);
        }
        else if (currentWindow == WindowType.Potion)
        {
            potionController.Dispose(true);
        }
        currentWindow = WindowType.Inventory;
    }
    public void OnPotionClick(int classType)
    {
        if (BaseUtils.onFight)
        {
            return;
        }
        HideTextTooltip();
        potionController.Setup((ClassType)classType);
        if (currentWindow == WindowType.Difficulty)
        {
            difficultyController.Dispose(true);
        }
        else if (currentWindow == WindowType.Inventory)
        {
            inventoryController.Dispose(true);
        }
        currentWindow = WindowType.Potion;
    }
    public void OnDungeonGrindClick(int classType)
    {
        if (BaseUtils.onFight)
        {
            return;
        }
        HideTextTooltip();
        difficultyController.Setup((ClassType)classType);
        if (currentWindow == WindowType.Inventory)
        {
            inventoryController.Dispose(true);
        }
        else if (currentWindow == WindowType.Potion)
        {
            potionController.Dispose(true);
        }
        currentWindow = WindowType.Difficulty;
    }
    public void OnAddTokenClick()
    {
        HideTextTooltip();
        Application.OpenURL("https://app.ref.finance/#wrap.near%7Cpixeltoken.near");
    }
    public void OnLogoutClick()
    {
        nearHelper.Logout(true);
        Database.SaveDatabase();
        SceneManager.LoadScene(0);
    }
    public void OnBackToMainScreenClick(bool forceUpdate = false)
    {
        SoundController.PlaySound("Menu_close", 1, true);
        bool updateGame = true;
        switch (currentWindow)
        {
            case WindowType.AuctionHall:
                auctionHallController.Dispose();
                break;
            case WindowType.Difficulty:
                updateGame = false;
                difficultyController.Dispose();
                break;
            case WindowType.Potion:
                updateGame = false;
                potionController.Dispose();
                break;
            case WindowType.Inventory:
                updateGame = false;
                inventoryController.Dispose();
                break;
            case WindowType.Leaderboard:
                leaderboardController.Dispose();
                break;
            case WindowType.Forge:
                forgeController.Dispose();
                break;
            case WindowType.Raid:
                raidController.Dispose();
                break;
        }
        if (updateGame || forceUpdate)
        {
            gameController.Setup();
        }
        HideTextTooltip();
        rainButton.SetDeactivated(Database.databaseStruct.rainOff);
        rainObj.SetActive(!Database.databaseStruct.rainOff);
        Database.SaveDatabase();
        //buttonsWindow.SetActive(!BaseUtils.onFight);
    }
    public void ShowTooltip(RectTransform target, ItemData itemData)
    {
        if (BaseUtils.showingLoot)
        {
            return;
        }
        if (tooltipCoroutine != null)
        {
            StopCoroutine(tooltipCoroutine);
        }
        tooltipCoroutine = StartCoroutine(WaitAndShowTooltip(target, itemData));
    }
    public void ShowTooltip(RectTransform target, ItemData itemData, ItemData equipData)
    {
        if (BaseUtils.showingLoot)
        {
            return;
        }
        if (tooltipCoroutine != null)
        {
            StopCoroutine(tooltipCoroutine);
        }
        tooltipCoroutine = StartCoroutine(WaitAndShowTooltip(target, itemData, equipData));
    }
    private IEnumerator WaitAndShowTooltip(RectTransform target, ItemData itemData)
    {
        float timer = 0;
        Vector3 mousePos = Input.mousePosition;
        while (timer <= 1)
        {
            if (Vector3.Distance(Input.mousePosition, mousePos) > .3f)
            {
                tooltipCoroutine = StartCoroutine(WaitAndShowTooltip(target, itemData));
                yield break;
            }
            timer += Time.deltaTime * 7;
            yield return null;
        }
        tooltipTarget = target;
        itemTooltip.Setup(itemData);
    }
    private IEnumerator WaitAndShowTooltip(RectTransform target, ItemData itemData, ItemData equipData)
    {
        float timer = 0;
        Vector3 mousePos = Input.mousePosition;
        while (timer <= 1)
        {
            if (Vector3.Distance(Input.mousePosition, mousePos) > .3f)
            {
                tooltipCoroutine = StartCoroutine(WaitAndShowTooltip(target, itemData, equipData));
                yield break;
            }
            timer += Time.deltaTime * 7;
            yield return null;
        }
        tooltipTarget = target;
        itemTooltip.Setup(itemData, equipData, target.position);
    }
    public void HideTooltip()
    {
        if (BaseUtils.showingLoot)
        {
            return;
        }
        if (tooltipCoroutine != null)
        {
            StopCoroutine(tooltipCoroutine);
        }
        itemTooltip.Dispose();
    }
    public void FadeInBlack()
    {
        StartCoroutine(FadeInBlackCoroutine(Color.black));
    }
    private IEnumerator FadeInBlackCoroutine(Color goColor)
    {
        blackScreen.enabled = true;
        float timer = 0;
        while (timer <= 1)
        {
            blackScreen.color = Color.Lerp(Color.clear, goColor, timer);
            timer += Time.deltaTime * 4;
            yield return null;
        }
        blackScreen.color = goColor;
    }
    public void FadeOutBlack()
    {
        StartCoroutine(FadeOutBlackCoroutine());
    }
    private IEnumerator FadeOutBlackCoroutine()
    {
        float timer = 0;
        Color fromColor = blackScreen.color;
        while (timer <= 1)
        {
            blackScreen.color = Color.Lerp(fromColor, Color.clear, timer);
            timer += Time.deltaTime * 4;
            yield return null;
        }
        blackScreen.color = Color.clear;
        blackScreen.enabled = false;
    }
    public bool GetCanvasCasts(out List<RaycastResult> raycastResults)
    {
        raycastResults = new List<RaycastResult>();
        PointerEventData pointerEvent = new PointerEventData(null)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        for (int i = 0; i < canvasRaycasters.Length; i++)
        {
            canvasRaycasters[i].Raycast(pointerEvent, raycastResults);
        }
        return raycastResults.Count > 0;
    }
    public void InstantiateText(Transform fromTransform, string gotoString, int damage, Color color, bool followTransform)
    {
        TextController textController;
        if (textPool.Count > 0)
        {
            textController = textPool.Dequeue();
        }
        else
        {
            GameObject textObj = Instantiate(textPrefab, BaseUtils.mainCanvas);
            textController = textObj.GetComponent<TextController>();
        }
        textController.Setup(this, fromTransform, gotoString, color, damage, followTransform);
    }
    public void StopAllTexts()
    {
        for (int i = 0; i < activeTexts.Count; i++)
        {
            activeTexts[i].StopAllCoroutines();
            activeTexts[i].Dispose();
        }
    }
    public void SetupBlackScreen(float intensity = .3f)
    {
        blackScreen.enabled = true;
        blackScreen.raycastTarget = true;
        blackScreen.rectTransform.SetAsFirstSibling();
        StartCoroutine(FadeInBlackCoroutine(new Color(0, 0, 0, intensity)));
    }
    public void RemoveBlackScreen()
    {
        blackScreen.rectTransform.SetAsLastSibling();
        blackScreen.raycastTarget = false;
        StartCoroutine(FadeOutBlackCoroutine());
    }
    public void SetMaintenanceMode()
    {
        maintenanceWindow.SetActive(true);
    }
    public void OnDiscordClick()
    {
        Application.OpenURL("https://discord.gg/xFAAa8Db6f");
    }
    public void OnTelegramClick()
    {
        Application.OpenURL("https://twitter.com/PixelDapps");
    }
    public void OnRainClick()
    {
        Database.databaseStruct.rainOff = !Database.databaseStruct.rainOff;
        rainButton.SetDeactivated(Database.databaseStruct.rainOff);
        rainObj.SetActive(!Database.databaseStruct.rainOff);
        Database.SaveDatabase();
    }
    public void ShowTextTooltip(RectTransform target, string[] tooltipText, TooltipPosition tooltipPosition)
    {
        this.tooltipPosition = tooltipPosition;
        textTooltipTarget = target;
        for (int i = 0; i < textTooltipTexts.Length; i++)
        {
            textTooltipTexts[i].gameObject.SetActive(i < tooltipText.Length);
            if (i < tooltipText.Length)
            {
                textTooltipTexts[i].SetString(tooltipText[i]);
            }
        }
        textTooltip.gameObject.SetActive(true);
        //SoundController.PlaySound("Button_hover");
    }
    public void HideTextTooltip()
    {
        textTooltip.gameObject.SetActive(false);
    }
    public void OnAuthorizedClick()
    {
        switch (nearHelper.dataGetState)
        {
            case DataGetState.MarketPurchase:
                auctionHallController.OnAuthorizedClick();
                break;
            case DataGetState.RefillPurchase:
                difficultyController.OnAuthorizedClick();
                break;
            case DataGetState.RaidPurchase:
                raidController.OnAuthorizedClick();
                break;
        }
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;



public class UiManager : MonoBehaviour
{
    [Header("HUD - 인게임 게이지 및 텍스트")]
    [SerializeField] private GameObject hudPrefab;
    public Image playerHp;
    public Image expBar;
    public TMP_Text levelText;
    public TMP_Text timerText;
    public TMP_Text killCountText;
    public TMP_Text coinText;
    [SerializeField] private bool showRunGoldUnderKills;

    [Header("패널 UI")]
    public GameObject gameOverPanel;
    public GameObject optionPanel;
    public GameObject startPanel;
    public GameObject treasurePanel;
    [SerializeField] private ShopManager shopManager;

    [Header("GameOver 결과창 텍스트")]
    public TMP_Text gameOverTitleText;
    public TMP_Text victoryTitleText;
    public TMP_Text gameOverTimerText;
    public TMP_Text gameOverKillCountText;

    [Header("GameOver 버튼")]
    public Button restartButton;
    public Button quitButton;

    [Header("플레이어 연출 (테스트 및 사망 연출용)")]
    public Animator playerAnimator;
    public float gameOverDelay = 1.5f;

    [Header("시작 화면 버튼")]
    public Button startButton;
    public Button exitButton;

    [Header("입력 제한 UI")]
    [SerializeField] private GameObject passiveUpgradePanel;

    public bool IsRewardUIOpen => (treasurePanel != null && treasurePanel.activeInHierarchy) || (passiveUpgradePanel != null && passiveUpgradePanel.activeInHierarchy);
    public bool IsRunOver => isGameOver;

    [System.Serializable]
    public struct SlotEntry
    {
        public string key;
        public GameObject iconObj;
    }
    public static UiManager Instance { get; private set; }

    [Header("아이콘 슬롯")]
    [SerializeField] private List<SlotEntry> skillSlots = new List<SlotEntry>();
    [SerializeField] private List<SlotEntry> passiveSlots = new List<SlotEntry>();

    private readonly Dictionary<string, GameObject> skillMap = new Dictionary<string, GameObject>();
    private readonly Dictionary<string, GameObject> passiveMap = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (hudPrefab != null) BindHud();
        InitIconMap(skillSlots, skillMap);
        InitIconMap(passiveSlots, passiveMap);
    }

    private void BindHud()
    {
        Transform hud = Instantiate(hudPrefab, transform, false).transform;
        hud.SetAsFirstSibling();

        expBar = FindHudComponent<Image>(hud, "ExpBar/ExpBarFrame/EXP");
        levelText = FindHudComponent<TMP_Text>(hud, "ExpBar/Level");
        timerText = FindHudComponent<TMP_Text>(hud, "Timer/Text");
        killCountText = FindHudComponent<TMP_Text>(hud, "Kill (1)/Kill/KillCount");

        Transform result = FindHudComponent<Transform>(hud, "GamOverBackground");
        if (result != null)
        {
            gameOverPanel = result.gameObject;
            gameOverPanel.SetActive(false);
            const string frame = "GamOver/GamOverFrame/";
            gameOverTitleText = FindHudComponent<TMP_Text>(result, frame + "GameOver");
            victoryTitleText = gameOverTitleText;
            gameOverTimerText = FindHudComponent<TMP_Text>(result, frame + "OverTime/Time (1)/Text");
            gameOverKillCountText = FindHudComponent<TMP_Text>(result, frame + "Total KillCount/GameObject (1)/KillCount");
            restartButton = FindHudComponent<Button>(result, frame + "Buttons/Restart");
            quitButton = FindHudComponent<Button>(result, frame + "Buttons/Quit");
        }

        if (playerHp == null && playerAnimator != null)
        {
            foreach (Image image in playerAnimator.GetComponentsInChildren<Image>(true))
            {
                if (image.name != "PlayerHp" || image.transform.parent.name != "PlayerUnderHp") continue;
                playerHp = image;
                break;
            }
        }

        skillSlots.Clear();
        AddHudSlot(skillSlots, hud, "Gun", "SkillBox/Gun Frame");
        AddHudSlot(skillSlots, hud, "Lightning", "SkillBox/Lightning Frame");
        AddHudSlot(skillSlots, hud, "Spear", "SkillBox/SpearSkill");
        AddHudSlot(skillSlots, hud, "HolyWater", "SkillBox/HolyWaterSkill");
        AddHudSlot(skillSlots, hud, "Whip", "SkillBox/RosaryWhipSkill");
        AddHudSlot(skillSlots, hud, "Bell", "SkillBox/JudgmentBookSkill");
        AddHudSlot(skillSlots, hud, "Garlic", "SkillBox/SanctumCenserSkill");

        passiveSlots.Clear();
        foreach (string key in new[] { "Damage", "Magnet", "MaxHp", "Speed", "Area", "Count", "Duration" })
            AddHudSlot(passiveSlots, hud, key, "PassiveBox/" + key);
    }

    private void AddHudSlot(List<SlotEntry> slots, Transform hud, string key, string path)
    {
        Transform slot = FindHudComponent<Transform>(hud, path);
        if (slot != null) slots.Add(new SlotEntry { key = key, iconObj = slot.gameObject });
    }

    private T FindHudComponent<T>(Transform parent, string path) where T : Component
    {
        Transform child = parent.Find(path);
        if (child != null && child.TryGetComponent(out T component)) return component;

        Debug.LogError($"[UiManager] Missing {typeof(T).Name} at {path} in {parent.name}.", this);
        return null;
    }


    private float elapsedTime = 0f;
    private bool isGameOver = false;
    private int currentKillCount = 0;
    private int currentCoinCount = 0;
    private float targetExpRatio = 0f;
    private int lastLevel = 1;

    private static bool skipStartPanel = false;
    private GameLanguage language;

    private void Start()
    {
        if (startPanel != null)
        {
            startPanel.SetActive(!skipStartPanel);
            Time.timeScale = skipStartPanel ? 1f : 0f;
        }
        GameAudio.PlayMusic(startPanel != null && startPanel.activeSelf ? GameAudio.Track.Menu : GameAudio.Track.Battle);
        skipStartPanel = false;
        language = new GameLanguage(transform, shopManager != null ? shopManager.Panel : null);
        language.AddButton(startPanel, startButton);
        language.AddButton(optionPanel, startButton);
        language.Changed += RefreshHudLanguage;
        language.Refresh();

        if (TryGetComponent(out GameAudio audio))
        {
            audio.BindSettings(optionPanel);
            audio.BindButtonHover();
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (optionPanel != null)
        {
            optionPanel.SetActive(false);
            foreach (Button button in optionPanel.GetComponentsInChildren<Button>(true))
            {
                if (button.name != "Continue") continue;
                button.onClick.AddListener(ResumeFromOptionMenu);
                break;
            }
        }
        if (expBar != null) expBar.fillAmount = 0f;
        if (gameOverTitleText != null) gameOverTitleText.gameObject.SetActive(false);
        if (victoryTitleText != null) victoryTitleText.gameObject.SetActive(false);
        currentKillCount = 0;
        currentCoinCount = 0;
        UpdateCoinCount(0);
        if (restartButton != null) restartButton.onClick.AddListener(OnClickRestart);
        if (quitButton != null) quitButton.onClick.AddListener(OnClickQuit);
        if (startButton != null) startButton.onClick.AddListener(OnClickStart);
        if (exitButton != null) exitButton.onClick.AddListener(OnClickExit);
    }

    private void Update()
    {
        if (!isGameOver)
        {
            UpdateTimer();
        }

        if (expBar != null)
        {
            expBar.fillAmount = Mathf.Lerp(expBar.fillAmount, targetExpRatio, Time.deltaTime * 10f);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (shopManager != null && shopManager.IsOpen)
                shopManager.CloseShop();
            else
                ToggleOptionUI();
        }

#if UNITY_EDITOR
        if (!isGameOver) RunTestInputs();
#endif
    }

    private void InitIconMap(List<SlotEntry> entries, Dictionary<string, GameObject> map)
    {
        map.Clear();
        if (entries == null) return;

        foreach (var entry in entries)
        {
            if (string.IsNullOrEmpty(entry.key) || entry.iconObj == null) continue;

            // 처음엔 꺼진 상태로 시작
            entry.iconObj.SetActive(false);

            if (!map.ContainsKey(entry.key))
            {
                map.Add(entry.key, entry.iconObj);
            }
            else
            {
                Debug.LogWarning($"[UiManager] 중복된 아이콘 키입니다: {entry.key}");
            }
        }
    }

    public void ShowSkillIcon(string key)
    {
        if (string.IsNullOrEmpty(key)) return;

        if (skillMap.TryGetValue(key, out GameObject obj) && obj != null && !obj.activeSelf)
        {
            obj.transform.SetAsLastSibling();
            obj.SetActive(true);
        }
    }

    public void ShowPassiveIcon(string key, Sprite sprite)
    {
        if (string.IsNullOrEmpty(key)) return;

        if (passiveMap.TryGetValue(key, out GameObject obj) && obj != null)
        {
            if (sprite != null && obj.TryGetComponent(out Image image))
                image.sprite = sprite;

            if (!obj.activeSelf)
            {
                obj.transform.SetAsLastSibling();
                obj.SetActive(true);
            }
        }
    }


    public void OnClickRestart()
    {
        skipStartPanel = true;

        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void OnClickExit()
    {
        Application.Quit();
    }

    public void OnClickStart()
    {
        GameAudio.PlayMusic(GameAudio.Track.Battle);
        GameAudio.Play("game_start");
        if (startPanel != null) startPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnClickQuit()
    {
        skipStartPanel = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void UpdateHp(float currentHp, float maxHp)
    {
        if (playerHp != null && maxHp > 0f)
            playerHp.fillAmount = Mathf.Clamp01(currentHp / maxHp);

        if (currentHp <= 0f && !isGameOver)
        {
            isGameOver = true;
            StartCoroutine(DeathSequenceRoutine());
        }
    }

    public void UpdateExp(float currentExp, float maxExp, int currentLevel)
    {
        if (currentLevel > lastLevel && expBar != null)
        {
            expBar.fillAmount = 0f;
        }

        lastLevel = currentLevel;
        targetExpRatio = Mathf.Clamp01(currentExp / maxExp);

        RefreshLevelLabel();
    }

    private void OnDestroy()
    {
        language?.Dispose();
    }

    private void RefreshHudLanguage()
    {
        RefreshLevelLabel();
        RefreshKillCount();
    }

    private void RefreshLevelLabel()
    {
        if (levelText != null)
            levelText.text = GameLanguage.Choose($"LV. {lastLevel}", $"레벨 {lastLevel}");
    }

    public void UpdateKillCount(int killCount)
    {
        currentKillCount += killCount;
        RefreshKillCount();
    }

    private void RefreshKillCount()
    {

        if (killCountText != null)
            killCountText.text = showRunGoldUnderKills
                ? GameLanguage.Choose($"X: {currentKillCount}\nGold: {currentCoinCount:N0}", $"처치: {currentKillCount}\n골드: {currentCoinCount:N0}")
                : GameLanguage.Choose($"X: {currentKillCount}", $"처치: {currentKillCount}");
    }

    public void UpdateCoinCount(int coinCount)
    {
        currentCoinCount += coinCount;
        if (showRunGoldUnderKills) RefreshKillCount();

        if (coinText != null)
        {
            coinText.text = currentCoinCount.ToString();
        }
    }

    private IEnumerator DeathSequenceRoutine()
    {
        GameAudio.Play("player_death");
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Die");
        }

        yield return new WaitForSecondsRealtime(gameOverDelay);
        GameAudio.PlayMusic(GameAudio.Track.GameOver);
        GameAudio.Play("game_over", 0.65f);
        if (gameOverTitleText != null)
        {
            gameOverTitleText.gameObject.SetActive(true);
            gameOverTitleText.text = GameLanguage.Translate("Game Over!");
        }

        if (gameOverTimerText != null)
        {
            gameOverTimerText.text = FormatTime(elapsedTime);
        }
        if (gameOverKillCountText != null)
        {
            gameOverKillCountText.text = GameLanguage.Choose($"X: {currentKillCount}", $"처치: {currentKillCount}");
        }
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }
    private IEnumerator VictorySequenceRoutine()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        GameAudio.PlayMusic(GameAudio.Track.Victory);
        GameAudio.Play("victory", 0.65f);
        if (victoryTitleText != null)
        {
            victoryTitleText.gameObject.SetActive(true);
            victoryTitleText.text = GameLanguage.Translate("Victory!");
        }

        if (gameOverTimerText != null)
        {
            gameOverTimerText.text = FormatTime(elapsedTime);
        }
        if (gameOverKillCountText != null)
        {
            gameOverKillCountText.text = GameLanguage.Choose($"X: {currentKillCount}", $"처치: {currentKillCount}");
        }
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void ToggleOptionUI()
    {
        if (shopManager != null && shopManager.IsOpen) return;
        if (IsRewardUIOpen) return;
        if (isGameOver) return;
        if (startPanel != null && startPanel.activeSelf) return;
        if (optionPanel == null) return;

        bool nextState = !optionPanel.activeSelf;
        GameAudio.Play(nextState ? "pause" : "resume");
        optionPanel.SetActive(nextState);
        Time.timeScale = nextState ? 0f : 1f;
    }

    private void ResumeFromOptionMenu()
    {
        if (optionPanel != null && optionPanel.activeInHierarchy)
            ToggleOptionUI();
    }

    private void UpdateTimer()
    {
        elapsedTime += Time.deltaTime;

        if (timerText != null)
        {
            timerText.text = FormatTime(elapsedTime);
        }
    }

    public void Victory()
    {
        if (isGameOver) return;

        isGameOver = true;

        StartCoroutine(VictorySequenceRoutine());
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return $"{minutes:00} : {seconds:00}";
    }

    public void ChangeSkillIcon(string key, Sprite sprite)
    {
        if (string.IsNullOrEmpty(key) || sprite == null) return;

        if (skillMap.TryGetValue(key, out GameObject obj) && obj != null)
        {
            if (obj.transform.childCount == 0) return;

            Image image = obj.transform.GetChild(0).GetComponent<Image>();

            if (image != null)
            {
                image.sprite = sprite;
            }
        }
    }

#if UNITY_EDITOR
    // ==============================================================
    // [테스트용 변수 & 메서드] New Input System 방식 (협업 병합 전 삭제)
    // ==============================================================
    private float testHp = 100f;
    private float testMaxHp = 100f;
    private float testExp = 0f;
    private float testMaxExp = 100f;
    private int testLevel = 1;
    private int testKills = 0;

    private void RunTestInputs()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            testHp -= 25f;
            UpdateHp(testHp, testMaxHp);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            testHp = Mathf.Min(testHp + 25f, testMaxHp);
            UpdateHp(testHp, testMaxHp);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UpdateKillCount(testKills);

            testExp += 35f;
            while (testExp >= testMaxExp)
            {
                testExp -= testMaxExp;
                testMaxExp = Mathf.Round(testMaxExp * 1.2f);
                testLevel++;
            }
            UpdateExp(testExp, testMaxExp, testLevel);
        }


    }
#endif
}

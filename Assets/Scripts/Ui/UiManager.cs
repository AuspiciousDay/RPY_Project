using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;



public class UiManager : MonoBehaviour
{
    [Header("HUD - 인게임 게이지 및 텍스트")]
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

        InitIconMap(skillSlots, skillMap);
        InitIconMap(passiveSlots, passiveMap);
    }


    private float elapsedTime = 0f;
    private bool isGameOver = false;
    private int currentKillCount = 0;
    private int currentCoinCount = 0;
    private float targetExpRatio = 0f;
    private int lastLevel = 1;

    private static bool skipStartPanel = false;

    private void Start()
    {
        if (startPanel != null)
        {
            startPanel.SetActive(!skipStartPanel);
            Time.timeScale = skipStartPanel ? 1f : 0f;
        }
        skipStartPanel = false;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (optionPanel != null) optionPanel.SetActive(false);
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
        RunTestInputs();
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

        if (levelText != null)
            levelText.text = $"LV. {currentLevel}";
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
                ? $"X: {currentKillCount}\nGold: {currentCoinCount:N0}"
                : $"X: {currentKillCount}";
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
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Die");
        }

        yield return new WaitForSecondsRealtime(gameOverDelay);
        if (gameOverTitleText != null)
        {
            gameOverTitleText.gameObject.SetActive(true);
            gameOverTitleText.text = "Game Over!";
        }

        if (gameOverTimerText != null)
        {
            gameOverTimerText.text = FormatTime(elapsedTime);
        }
        if (gameOverKillCountText != null)
        {
            gameOverKillCountText.text = $"X: {currentKillCount}";
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
        if (victoryTitleText != null)
        {
            victoryTitleText.gameObject.SetActive(true);
            victoryTitleText.text = "Victory!";
        }

        if (gameOverTimerText != null)
        {
            gameOverTimerText.text = FormatTime(elapsedTime);
        }
        if (gameOverKillCountText != null)
        {
            gameOverKillCountText.text = $"X: {currentKillCount}";
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
        optionPanel.SetActive(nextState);
        Time.timeScale = nextState ? 0f : 1f;
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

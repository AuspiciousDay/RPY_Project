using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class UiManager : MonoBehaviour
{
    [Header("HUD - 인게임 게이지 및 텍스트")]
    public Image playerHp;
    public Image expBar;
    public TMP_Text levelText;
    public TMP_Text timerText;
    public TMP_Text killCountText;

    [Header("패널 UI")]
    public GameObject gameOverPanel;
    public GameObject optionPanel;

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

    private float elapsedTime = 0f;
    private bool isGameOver = false;
    private int currentKillCount = 0;

    private float targetExpRatio = 0f;
    private int lastLevel = 1;

    private void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (optionPanel != null) optionPanel.SetActive(false);
        if (expBar != null) expBar.fillAmount = 0f;
        gameOverTitleText.gameObject.SetActive(false);
        victoryTitleText.gameObject.SetActive(false);
        currentKillCount = 0;
        if (restartButton != null) restartButton.onClick.AddListener(OnClickRestart);
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

#if UNITY_EDITOR
        RunTestInputs();
#endif
    }

    public void OnClickRestart()
    {
        // 혹시 Time.timeScale이 0으로 멈춰있을 경우를 대비해 1로 복구 후 현재 씬 재시작
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void UpdateHp(float currentHp, float maxHp)
    {
        if (playerHp == null) return;

        float ratio = Mathf.Clamp01(currentHp / maxHp);
        playerHp.fillAmount = ratio;

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

        if (killCountText != null)
            killCountText.text = $"X: {currentKillCount}";
    }

    private IEnumerator DeathSequenceRoutine()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Die");
        }

        yield return new WaitForSeconds(gameOverDelay);
        if (gameOverTitleText != null )
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
        yield return new WaitForSeconds(0.1f);
        if (victoryTitleText != null )
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
        if (optionPanel == null) return;

        bool nextState = !optionPanel.activeSelf;
        optionPanel.SetActive(nextState);
        Time.timeScale = nextState ? 0f : 1f;
    }

    private void UpdateTimer()
    {
        elapsedTime += Time.deltaTime;

        if (timerText != null)
            timerText.text = FormatTime(elapsedTime);
        if(elapsedTime >= 300f && !isGameOver)
        {
            isGameOver = true;
            StartCoroutine(VictorySequenceRoutine());
        }
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return $"{minutes:00} : {seconds:00}";
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleOptionUI();
        }
    }
#endif
}
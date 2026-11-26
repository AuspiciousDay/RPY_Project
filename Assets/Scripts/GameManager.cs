using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerHealth PlayerHealth;
    public GameObject GameOverUi;


    private void Start()
    {
        if (GameOverUi != null) GameOverUi.SetActive(false);
        if (PlayerHealth != null) PlayerHealth.OnDeath += GameOver;
    }

    public void StartGame()
    {
        PauseGame(1f);
    }

    private void GameOver()
    {
        if (GameOverUi == null) return;
        if (PlayerHealth.uiManager != null && PlayerHealth.uiManager.gameOverPanel == GameOverUi) return;
        GameOverUi.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OnDestroy()
    {
        if (PlayerHealth != null) PlayerHealth.OnDeath -= GameOver;
    }

    public static void PauseGame(float timeScale = 1f)
    {
        Time.timeScale = timeScale;
    }
}

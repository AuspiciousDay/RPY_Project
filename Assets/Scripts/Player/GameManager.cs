using UnityEngine;


public class GameManager : MonoBehaviour
{
    public PlayerHealth PlayerHealth;
    public GameObject GameOverUi;

    private void Start()
    {
        GameOverUi.SetActive(false);
        PlayerHealth.OnDeath += GameOver;
    }
    private void GameOver()
    {
        GameOverUi.SetActive(true);
    }
    private void OnDestroy()
    {
        PlayerHealth.OnDeath -= GameOver;
    }
    
    public static void PauseGame(float timeScale = 0f)
    {
        Time.timeScale = timeScale;
    }
}

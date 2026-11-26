
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public GameObject MenuWindow;

    private void Start()
    {
        MenuWindow.SetActive(false);
        Time.timeScale = 1f;
    }

   private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            
            if (MenuWindow.activeSelf == true)
            {
                MenuWindow.SetActive(false);
                Time.timeScale = 1f;
            }
            else
            {
                MenuWindow.SetActive(true);
                Time.timeScale = 0f;
            }
        }
    }

    // Continue 버튼
    public void Continue()
    {
        MenuWindow.SetActive(false);
        Time.timeScale = 1f;
    }

    // Quit 버튼
    public void Quit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject menuPanel;

    public GameObject Panel => shopPanel;

    public bool IsOpen => shopPanel != null && shopPanel.activeInHierarchy;

    public void OpenShop()
    {
        GameAudio.Play("ui_click");
        Time.timeScale = 0f;
        menuPanel.SetActive(false);
        shopPanel.SetActive(true);
    }

    public void CloseShop()
    {
        GameAudio.Play("ui_back");
        Time.timeScale = 0f;
        shopPanel.SetActive(false);
        menuPanel.SetActive(true);
    }
}

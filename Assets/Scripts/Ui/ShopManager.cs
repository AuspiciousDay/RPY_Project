using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject menuPanel;

    public bool IsOpen => shopPanel != null && shopPanel.activeInHierarchy;

    public void OpenShop()
    {
        Time.timeScale = 0f;
        menuPanel.SetActive(false);
        shopPanel.SetActive(true);
    }

    public void CloseShop()
    {
        Time.timeScale = 0f;
        shopPanel.SetActive(false);
        menuPanel.SetActive(true);
    }
}

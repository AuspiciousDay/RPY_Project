using UnityEngine;

public class Coin : MonoBehaviour
{
    public int CoinAmount = 1;

    public static int BonusCoin = 0;

    private UiManager uiManager;

    private void Awake()
    {
        uiManager = FindFirstObjectByType<UiManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            int amount = CoinAmount + BonusCoin;
            
            if (uiManager != null)
            {
                uiManager.UpdateCoinCount(amount);
            }

            Debug.Log("코인 획득 : " + amount);

            Destroy(gameObject);
        }
    }
}
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int goldAmount = 10;

    private bool isCollected = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected) return;
        if (!collision.CompareTag("Player")) return;
        if (GoldManager.Instance == null) return;

        isCollected = true;

        GoldManager.Instance.AddGold(goldAmount);
        UiManager.Instance?.UpdateCoinCount(goldAmount);

        Destroy(gameObject);
    }
}
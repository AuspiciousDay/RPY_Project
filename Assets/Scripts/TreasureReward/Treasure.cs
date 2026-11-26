using UnityEngine;

public class Treasure : MonoBehaviour
{
    private TreasureUI treasureUI;
    private bool isCollected;

    private void Awake()
    {
        treasureUI = FindFirstObjectByType<TreasureUI>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        TryCollect();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        TryCollect();
    }

    private void TryCollect()
    {
        if (isCollected) return;
        if (Time.timeScale == 0f) return;
        if (UiManager.Instance != null && UiManager.Instance.IsRunOver) return;
        if (treasureUI == null) return;
        if (treasureUI.IsTreasureOpen) return;

        isCollected = true;

        treasureUI.ShowTreasure();

        Destroy(gameObject);
    }
}

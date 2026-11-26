using UnityEngine;


public class Treasure : MonoBehaviour
{
    private TreasureUI treasureUI;

    private void Awake()
    {
        treasureUI = FindFirstObjectByType<TreasureUI>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            treasureUI.ShowTreasure();

            Destroy(gameObject);
        }
    }
}
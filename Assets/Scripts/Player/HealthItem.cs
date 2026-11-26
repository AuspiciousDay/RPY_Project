
using UnityEngine;

public class HealthItem : MonoBehaviour
{
    public int HealAmount = 20;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.Heal(HealAmount);
                Debug.Log("회복 아이템 획득 : " + HealAmount);
                Destroy(gameObject);
            }
        }
    }
}

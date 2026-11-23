
using UnityEngine;

public class Monster : MonoBehaviour
{
    public int Health = 100;
    public int Damage = 10;

    public void TakeDamage(int Damage)
    {
        Health -= Damage;

        Debug.Log("Monster Health : " + Health);

        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        if(playerHealth != null)
        {
            playerHealth.Damage(Damage);
        }
    }
}

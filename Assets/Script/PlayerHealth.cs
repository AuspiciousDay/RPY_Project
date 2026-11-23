
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int Health = 100;

    private bool IsDead = false;

    public void Damage(int Damage)
    {
        if (IsDead)
            return;

        Health -= Damage;

        if (Health <= 0)
        {
            Health = 0;
            Die();
        }

        Debug.Log("Health");
    }

    private void Die()
    {
        IsDead = true;

        Debug.Log("Die");
    }
}

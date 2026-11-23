
using UnityEngine;
using System.Collections;
using System;

public class PlayerHealth : MonoBehaviour
{
    public int Health = 100;
    public int HealthMax = 100;

    public float InvincibilityTime = 2f; // 무적시간
    private bool Invincibility = false; 
    private bool IsDead = false;
    public event Action OnDeath;
    private void Start()
    {
        Health = HealthMax;
    }
    public void Damage(int Damage)
    {
        if (IsDead || Invincibility)
            return;

        Health -= Damage;

        if (Health <= 0)
        {
            Health = 0;
            Die();
            return;
        }
        StartCoroutine(Invincible());

        Debug.Log("Player Health : " + Health);
    }
    private IEnumerator Invincible()
    {
        Invincibility = true;

        yield return new WaitForSeconds(InvincibilityTime);

        Invincibility = false;
    }

    private void Die()
    {
        IsDead = true;

        Debug.Log("Die");

        OnDeath?.Invoke();
    }
}

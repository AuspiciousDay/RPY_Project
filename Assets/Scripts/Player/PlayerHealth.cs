
using UnityEngine;
using System.Collections;
using System;

public class PlayerHealth : MonoBehaviour
{
    public int Health = 100;
    public int HealthMax = 100;

    public float InvincibilityTime = 2f;
    private bool Invincibility = false;
    private bool IsDead = false;
    public event Action OnDeath;

    public UiManager uiManager;
    private SpriteRenderer spriteRenderer;

    private Color originalColor;
    private Coroutine hitCoroutine;

    private void Start()
    {
        Health = HealthMax;
        uiManager.UpdateHp(Health, HealthMax);

        spriteRenderer = GetComponent<SpriteRenderer>();    // 피격 당할시 
        originalColor = spriteRenderer.color;
    }

    public void Damage(int Damage)
    {
        if (IsDead || Invincibility)
            return;

        Health -= Damage;
        uiManager.UpdateHp(Health, HealthMax);

        if (Health <= 0)
        {
            Health = 0;
            Die();
            return;
        }

        if (hitCoroutine != null)
            StopCoroutine(hitCoroutine);

        hitCoroutine = StartCoroutine(HitEffect());

        StartCoroutine(Invincible());

        Debug.Log("Player Health : " + Health);
    }

    private IEnumerator HitEffect() // 색
    {
        spriteRenderer.color = Color.softRed;

        yield return new WaitForSeconds(0.2f);

        spriteRenderer.color = originalColor;
        hitCoroutine = null;
    }

    private IEnumerator Invincible()
    {
        Invincibility = true;

        yield return new WaitForSeconds(InvincibilityTime);

        Invincibility = false;
    }
    public void Heal(int Amount)
    {
        if (IsDead)
            return;

        Health += Amount;

        if (Health > HealthMax)
        {
            Health = HealthMax;
        }

        uiManager.UpdateHp(Health, HealthMax);

        Debug.Log("Player Heal : " + Health);
    }

    private void Die()
    {
        IsDead = true;

        Debug.Log("Die");
        OnDeath?.Invoke();
    }

}

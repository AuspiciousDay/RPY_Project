
using UnityEngine;
using System.Collections;
using System;

public class PlayerHealth : MonoBehaviour
{
    public float Health = 100f;
    public float HealthMax = 100f;
    private bool IsDead = false;
    public event Action OnDeath;

    public UiManager uiManager;

    private void Start()
    {
        Health = HealthMax;
        uiManager.UpdateHp(Health, HealthMax);
    }
    public void Damage(float Damage)
    {
        if (IsDead)
            return;

#if UNITY_EDITOR
        if (Damage > 0f && EditorCombatModes.Invincible)
            return;
#endif

        Health -= Damage;
        uiManager.UpdateHp(Health, HealthMax);
        if (Health <= 0)
        {
            Health = 0;
            Die();
            return;
        }
    }

    private void Die()
    {
        IsDead = true;
        OnDeath?.Invoke();
    }

    public void Heal(float amount)
    {
        if (IsDead)
            return;

        Health += amount;
        if (Health > HealthMax)
        {
            Health = HealthMax;
        }
        uiManager.UpdateHp(Health, HealthMax);
    }
}

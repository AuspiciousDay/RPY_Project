
using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    public float Armor = 0f;
    public float Health = 100f;
    public float HealthMax = 100f;
    private bool IsDead = false;
    public event Action OnDeath;

    public UiManager uiManager;

    [SerializeField] private PlayerInput playerInput;

    private void Start()
    {
        Health = HealthMax;
        uiManager.UpdateHp(Health, HealthMax);
    }
    public void Damage(float Damage)
    {
#if UNITY_EDITOR
        if (DebugManager.Instance != null && DebugManager.Instance.godMode)
        {
            return;
        }
#endif

        if (IsDead || (uiManager != null && uiManager.IsRunOver)) return;

        Health -= Mathf.Max(1f, Damage - Armor);

        uiManager.UpdateHp(Health, HealthMax);

        if (Health <= 0)
        {
            Health = 0;
            Die();
            return;
        }

        GameAudio.Play("player_hit", 0.8f, 0.2f);
    }

    private void Die()
    {
        IsDead = true;

        playerInput.enabled = false;

        OnDeath?.Invoke();
    }
    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f) return;

        Health = Mathf.Min(Health + amount, HealthMax);

        uiManager.UpdateHp(Health, HealthMax);
    }
}

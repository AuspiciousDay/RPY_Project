using UnityEngine;

public class PermanentPowerUp : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerLevel playerLevel;
    [SerializeField] private CircleCollider2D magnetCollider;

    [Header("Weapons")]
    [SerializeField] private PlayerShooter playerShooter;
    [SerializeField] private SpearWeapon spearWeapon;
    [SerializeField] private HolyWaterWeapon holyWaterWeapon;
    [SerializeField] private WhipWeapon whipWeapon;
    [SerializeField] private LightningWeapon lightningWeapon;
    [SerializeField] private BellWeapon bellWeapon;
    [SerializeField] private GarlicAbility garlicAbility;
    public float LuckMultiplier { get; private set; } = 1f;

    private bool isApplied;

    public void Apply()
    {
        if (isApplied) return;

        PowerUpManager manager = PowerUpManager.Instance;

        if (manager == null) return;

        playerMovement.MoveSpeed *= 1f + manager.GetValue(PowerUpType.Speed);

        playerHealth.HealthMax *= 1f + manager.GetValue(PowerUpType.MaxHealth);

        playerHealth.Armor += manager.GetValue(PowerUpType.Armor);

        playerLevel.ExpMultiplier *= 1f + manager.GetValue(PowerUpType.Growth);

        magnetCollider.radius *= 1f + manager.GetValue(PowerUpType.Magnet);

        ApplyMight(manager.GetValue(PowerUpType.Might));
        ApplyAmount(manager.GetValue(PowerUpType.Amount));
        ApplyCooldown(manager.GetValue(PowerUpType.Cooldown));
        ApplyArea(manager.GetValue(PowerUpType.Area));
        ApplyVelocity(manager.GetValue(PowerUpType.Velocity));
        ApplyDuration(manager.GetValue(PowerUpType.Duration));
        ApplyLuck(manager.GetValue(PowerUpType.Luck));

        playerHealth.Health = playerHealth.HealthMax;

        playerHealth.uiManager.UpdateHp(playerHealth.Health, playerHealth.HealthMax);

        isApplied = true;
    }

    private void Update()
    {
        if (isApplied || Time.timeScale <= 0f) return;

        Apply();
    }
    private void ApplyLuck(float value)
    {
        LuckMultiplier = 1f + value;
    }

    private void ApplyDuration(float value)
    {
        float multiplier = 1f + value;

        spearWeapon.lifeTime *= multiplier;
        holyWaterWeapon.duration *= multiplier;
        bellWeapon.duration *= multiplier;
        lightningWeapon.MultiplyAreaDuration(multiplier);
    }

    private void ApplyVelocity(float value)
    {
        float multiplier = 1f + value;

        playerShooter.ProjectileSpeedMultiplier *= multiplier;
        spearWeapon.speed *= multiplier;
        holyWaterWeapon.ProjectileSpeedMultiplier *= multiplier;
        bellWeapon.rotateSpeed *= multiplier;
    }

    private void ApplyArea(float value)
    {
        float multiplier = 1f + value;

        spearWeapon.sizeMultiplier *= multiplier;
        holyWaterWeapon.radius *= multiplier;
        whipWeapon.attackWidth *= multiplier;
        whipWeapon.attackHeight *= multiplier;
        bellWeapon.radius *= multiplier;
        lightningWeapon.MultiplyAreaRadius(multiplier);
        garlicAbility.MultiplyRange(multiplier);
    }

    private void ApplyMight(float value)
    {
        float multiplier = 1f + value;

        playerShooter.AttackDamage *= multiplier;
        spearWeapon.damage *= multiplier;
        holyWaterWeapon.damage *= multiplier;
        whipWeapon.damage *= multiplier;
        lightningWeapon.damage *= multiplier;
        bellWeapon.damage *= multiplier;
        garlicAbility.Damage *= multiplier;
    }
    private void ApplyAmount(float value)
    {
        int amount = Mathf.RoundToInt(value);

        playerShooter.AttackCount += amount;
        spearWeapon.attackCount += amount;
        holyWaterWeapon.attackCount += amount;
        whipWeapon.attackCount += amount;
        lightningWeapon.maxCount += amount;
        bellWeapon.attackCount += amount;
    }
    private void ApplyCooldown(float value)
    {
        float multiplier = 1f - value;

        playerShooter.MultiplyCooldown(multiplier);
        spearWeapon.attackInterval *= multiplier;
        holyWaterWeapon.attackInterval *= multiplier;
        whipWeapon.attackInterval *= multiplier;
        lightningWeapon.attackInterval *= multiplier;
        bellWeapon.attackInterval *= multiplier;
        garlicAbility.DamageInterval *= multiplier;
    }
}
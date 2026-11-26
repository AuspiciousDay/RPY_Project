using UnityEngine;

public class PassiveAddDamage : PassiveController
{
    [SerializeField] private PlayerShooter playerShooter;
    [SerializeField] private LightningWeapon lightningWeapon;

    [SerializeField] private float addDamage = 5f;

    protected override void Apply()
    {
        if (playerShooter != null)
        {
            playerShooter.AttackDamage += addDamage;
        }
        if (lightningWeapon != null)
        {
            lightningWeapon.damage += addDamage;
        }
    }
}
using UnityEngine;

public class PassiveAddDamage : PassiveController
{
    [SerializeField] private PlayerShooter playerShooter;
    [SerializeField] private LightningWeapon lightningWeapon;
    [SerializeField] private GarlicAbility garlic;

    [SerializeField] private float addDamage = 5f;

    protected override void Apply()
    {
        UiManager.Instance?.ShowPassiveIcon("Damage");
        if (playerShooter != null)
        {
            playerShooter.AttackDamage += addDamage;
        }
        if (lightningWeapon != null)
        {
            lightningWeapon.damage += addDamage;
        }
        if (garlic != null)
        {
            garlic.Damage += addDamage;
        }
    }
}
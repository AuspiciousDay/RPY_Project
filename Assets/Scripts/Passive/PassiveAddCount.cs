using UnityEngine;

public class PassiveAddCount : PassiveController
{
    [SerializeField] private PlayerShooter playerShooter;
    [SerializeField] private SpearWeapon spearWeapon;
    [SerializeField] private HolyWaterWeapon holyWaterWeapon;
    [SerializeField] private LightningWeapon lightningWeapon;
    [SerializeField] private WhipWeapon whipWeapon;
    [SerializeField] private BellWeapon bellWeapon;

    [SerializeField] private int addCount = 1;

    protected override void Apply()
    {
        if (playerShooter != null)
            playerShooter.AttackCount += addCount;

        if (spearWeapon != null)
            spearWeapon.attackCount += addCount;

        if (holyWaterWeapon != null)
            holyWaterWeapon.attackCount += addCount;

        if (lightningWeapon != null)
            lightningWeapon.maxCount += addCount;

        if (whipWeapon != null)
            whipWeapon.attackCount += addCount;

        if (bellWeapon != null)
            bellWeapon.attackCount += addCount;
    }
}
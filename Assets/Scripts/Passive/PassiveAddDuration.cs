using UnityEngine;

public class PassiveAddDuration : PassiveController
{
    [SerializeField] private HolyWaterWeapon holyWaterWeapon;
    [SerializeField] private BellWeapon bellWeapon;

    [SerializeField] private float durationMultiplier = 1.1f;

    protected override void Apply()
    {
        if (TryGetComponent(out LightningWeapon lightningWeapon))
            lightningWeapon.MultiplyAreaDuration(durationMultiplier);

        if (holyWaterWeapon != null)
            holyWaterWeapon.duration *= durationMultiplier;

        if (bellWeapon != null)
            bellWeapon.duration *= durationMultiplier;
    }
}
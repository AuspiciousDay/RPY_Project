using UnityEngine;

public class PassiveAddArea : PassiveController
{
    [SerializeField] private SpearWeapon spearWeapon;
    [SerializeField] private HolyWaterWeapon holyWaterWeapon;
    [SerializeField] private WhipWeapon whipWeapon;
    [SerializeField] private BellWeapon bellWeapon;
    [SerializeField] private GarlicAbility garlic;

    [SerializeField] private float areaMultiplier = 1.1f;

    protected override void Apply()
    {
        UiManager.Instance?.ShowPassiveIcon("Area");
        if (spearWeapon != null)
            spearWeapon.sizeMultiplier *= areaMultiplier;

        if (holyWaterWeapon != null)
            holyWaterWeapon.radius *= areaMultiplier;

        if (whipWeapon != null)
        {
            whipWeapon.attackWidth *= areaMultiplier;
            whipWeapon.attackHeight *= areaMultiplier;
        }

        if (bellWeapon != null)
            bellWeapon.radius *= areaMultiplier;
        if (garlic != null)
        {
            garlic.IncreaseRange(0.2f);
        }
    }
}
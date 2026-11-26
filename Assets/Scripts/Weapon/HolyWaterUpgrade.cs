using UnityEngine;

public class HolyWaterUpgrade : WeaponController
{
    [SerializeField] private HolyWaterWeapon holyWaterWeapon;

    private void Awake()
    {
        holyWaterWeapon.enabled = false;
    }

    protected override void Apply()
    {
        switch (Level)
        {
            case 1:
                holyWaterWeapon.enabled = true;
                break;

            case 2:
                holyWaterWeapon.attackCount++;
                break;

            case 3:
                holyWaterWeapon.damage += 5f;
                break;

            case 4:
                holyWaterWeapon.radius *= 1.25f;
                holyWaterWeapon.duration += 1f;
                break;

            case 5:
                holyWaterWeapon.attackCount++;
                holyWaterWeapon.attackInterval -= 0.5f;
                break;
        }
    }

    public override string GetDescription()
    {
        switch (Level + 1)
        {
            case 1:
                return "Throws holy water that creates damaging zones.";

            case 2:
                return "Throws 1 additional holy water bottle.";

            case 3:
                return "Increases damage by 5.";

            case 4:
                return "Increases area by 25% and duration by 1s.";

            case 5:
                return "Throws 1 additional bottle and reduces cooldown by 0.5s.";

            default:
                return "Max Level";
        }
    }

    protected override void ApplyEvolution()
    {

    }
}
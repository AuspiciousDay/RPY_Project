using UnityEngine;

public class SpearUpgrade : WeaponController
{
    [SerializeField] private SpearWeapon spearWeapon;

    private void Awake()
    {
        spearWeapon.enabled = false;
    }

    protected override void Apply()
    {
        switch (Level)
        {
            case 1:
                spearWeapon.enabled = true;
                break;

            case 2:
                spearWeapon.attackCount++;
                break;

            case 3:
                spearWeapon.damage += 15f;
                break;

            case 4:
                spearWeapon.speed *= 1.3f;
                spearWeapon.sizeMultiplier *= 1.2f;
                break;

            case 5:
                spearWeapon.attackCount++;
                spearWeapon.attackInterval -= 0.5f;
                break;
        }
    }

    public override string GetDescription()
    {
        switch (Level + 1)
        {
            case 1:
                return "Fires piercing spears in random directions.";

            case 2:
                return "Fires 1 additional spear.";

            case 3:
                return "Increases damage by 15.";

            case 4:
                return "Increases projectile speed by 30% and size by 20%.";

            case 5:
                return "Fires 1 additional spear and reduces cooldown by 0.5s.";

            default:
                return "Max Level";
        }
    }

    protected override void ApplyEvolution()
    {

    }
}
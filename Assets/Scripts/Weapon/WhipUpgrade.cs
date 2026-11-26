using UnityEngine;

public class WhipUpgrade : WeaponController
{
    [SerializeField] private WhipWeapon whipWeapon;

    private void Awake()
    {
        whipWeapon.enabled = false;
    }

    protected override void Apply()
    {
        switch (Level)
        {
            case 1:
                whipWeapon.enabled = true;
                break;

            case 2:
                whipWeapon.attackCount++;
                break;

            case 3:
                whipWeapon.damage += 10f;
                break;

            case 4:
                whipWeapon.attackWidth *= 1.25f;
                whipWeapon.attackHeight *= 1.25f;
                break;

            case 5:
                whipWeapon.attackInterval -= 0.3f;
                break;
        }
    }

    public override string GetDescription()
    {
        switch (Level + 1)
        {
            case 1:
                return "Slashes enemies in front of the player.";

            case 2:
                return "Attacks in both directions.";

            case 3:
                return "Increases damage by 10.";

            case 4:
                return "Increases attack dimensions by 25%.";

            case 5:
                return "Reduces attack interval by 0.3s.";

            default:
                return "Max Level";
        }
    }

    protected override void ApplyEvolution()
    {

    }
}
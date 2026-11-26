using UnityEngine;

public class BellUpgrade : WeaponController
{
    [SerializeField] private BellWeapon bellWeapon;

    private void Awake()
    {
        bellWeapon.enabled = false;
    }

    protected override void Apply()
    {
        switch (Level)
        {
            case 1:
                bellWeapon.enabled = true;
                break;

            case 2:
                bellWeapon.attackCount++;
                break;

            case 3:
                bellWeapon.damage += 10f;
                break;

            case 4:
                bellWeapon.duration += 1f;
                break;

            case 5:
                bellWeapon.attackCount++;
                bellWeapon.rotateSpeed *= 1.3f;
                break;
        }
    }

    public override string GetDescription()
    {
        switch (Level + 1)
        {
            case 1:
                return "Summons holy books that orbit the player.";

            case 2:
                return "Adds 1 additional holy book.";

            case 3:
                return "Increases damage by 10.";

            case 4:
                return "Increases duration by 1s.";

            case 5:
                return "Adds 1 book and increases rotation speed by 30%.";

            default:
                return "Max Level";
        }
    }
}
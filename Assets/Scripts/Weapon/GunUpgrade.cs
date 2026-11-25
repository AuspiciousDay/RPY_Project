using UnityEngine;

public class GunUpgrade : WeaponController
{
    [SerializeField] private PlayerShooter playerShooter;

    private void Awake()
    {
        Upgrade();
    }

    protected override void Apply()
    {
        switch (Level)
        {
            case 1:
                playerShooter.enabled = true;
                break;

            case 2:
                playerShooter.AttackDamage += 5;
                break;

            case 3:
                playerShooter.AttackCount++;
                break;

            case 4:
                playerShooter.PenetrateCount++;
                break;

            case 5:
                playerShooter.AttackInterval -= 0.2f;
                break;
        }
    }

    public override string GetDescription()
    {
        switch (Level + 1)
        {
            case 1:
                return "Fires bullets at the nearest enemy.";

            case 2:
                return "Increases damage by 5.";

            case 3:
                return "Fires 1 additional projectile.";

            case 4:
                return "Increases penetration by 1.";

            case 5:
                return "Reduces attack interval by 0.2s.";

            default:
                return "Max Level";
        }
    }
}
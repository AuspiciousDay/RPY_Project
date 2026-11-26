using UnityEngine;

public class LightningUpgrade : WeaponController
{
    [SerializeField] private LightningWeapon lightningWeapon;

    private void Awake()
    {
        lightningWeapon.enabled = false;
    }

    protected override void Apply()
    {
        switch (Level)
        {
            case 1:
                lightningWeapon.enabled = true;
                break;

            case 2:
                lightningWeapon.damage += 10;
                break;

            case 3:
                lightningWeapon.maxCount++;
                break;

            case 4:
                lightningWeapon.attackInterval -= 1f;
                break;

            case 5:
                lightningWeapon.maxCount += 2;
                break;
        }
    }

    public override string GetDescription()
    {
        switch (Level + 1)
        {
            case 1:
                return "Strikes random enemies with lightning.";

            case 2:
                return "Increases damage by 10.";

            case 3:
                return "Strikes 1 additional enemy.";

            case 4:
                return "Reduces attack interval by 1s.";

            case 5:
                return "Strikes 2 additional enemies.";

            default:
                return "Max Level";
        }
    }

    protected override void ApplyEvolution()
    {
        lightningWeapon.Evolve();
    }
}
using UnityEngine;

public class CoinUpgrade : WeaponController
{
    private void Awake()
    {
        Coin.BonusCoin = 0;
    }

    protected override void Apply()
    {
        Coin.BonusCoin++;
    }

    public override string GetDescription()
    {
        switch (Level + 1)
        {
            case 1:
                return "Collects 2 coins.";

            case 2:
                return "Collects 3 coins.";

            case 3:
                return "Collects 4 coins.";

            case 4:
                return "Collects 5 coins.";

            case 5:
                return "Collects 6 coins.";

            default:
                return "Max Level";
        }
    }
}
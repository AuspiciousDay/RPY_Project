
using UnityEngine;

public class GarlicUpgrade : WeaponController
{
    [SerializeField] private GameObject garlicObject;
    [SerializeField] private GarlicAbility garlic;

    private void Awake()
    {
        garlicObject.SetActive(false);
    }

    protected override void Apply()
    {
        switch (Level)
        {
            case 1:
                garlicObject.SetActive(true);
                break;

            case 2:
                garlic.Damage += 10;
                break;

            case 3:
                garlic.IncreaseRange(0.5f);
                break;

            case 4:
                garlic.Damage += 20;
                break;

            case 5:
                garlic.IncreaseRange(0.5f);
                break;
        }
    }

    public override string GetDescription()
    {
        switch (Level + 1)
        {
            case 1:
                return "Unlocks Garlic.";

            case 2:
                return "Increases damage by 10.";

            case 3:
                return "Increases attack range.";

            case 4:
                return "Increases damage by 20.";

            case 5:
                return "Increases attack range.";

            default:
                return "Max Level";
        }
    }

    protected override void ApplyEvolution()
    {
        garlic.Evolve();
    }
}

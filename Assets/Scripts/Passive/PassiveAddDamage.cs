using UnityEngine;

public class PassiveAddDamage : PassiveController
{
    [SerializeField] private float bonusPerLevel = 0.3f;

    public float Multiplier { get; private set; } = 1f;

    protected override void Apply()
    {

        Multiplier = 1f + bonusPerLevel * Level;

    }

    public override string GetDescription()
    {
        return $"Increases weapon damage by {bonusPerLevel:P0} of base damage per level.";
    }
}

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
        return GameLanguage.Choose(
            $"Increases weapon damage by {bonusPerLevel:P0} of base damage per level.",
            $"레벨당 무기 피해량이 기본 피해량의 {bonusPerLevel:0%}만큼 증가합니다.");
    }
}

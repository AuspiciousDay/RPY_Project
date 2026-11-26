using UnityEngine;

public abstract class WeaponController : UpgradeBase
{
    [Header("Evolution")]
    [SerializeField] private PassiveController requiredPassive;

    [SerializeField] private string evolvedName;
    [SerializeField] private Sprite evolvedIcon;

    public bool IsEvolved { get; private set; }

    public bool CanEvolve => !IsEvolved && IsMaxLevel && requiredPassive != null && requiredPassive.IsMaxLevel;

    public bool Evolve()
    {
        if (!CanEvolve)
            return false;

        IsEvolved = true;

        ApplyEvolution();

        upgradeName = evolvedName;
        icon = evolvedIcon;

        return true;
    }

    protected abstract void ApplyEvolution();
}
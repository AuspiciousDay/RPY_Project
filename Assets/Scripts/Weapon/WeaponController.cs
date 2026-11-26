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
        if (!CanEvolve) return false;

        ApplyEvolution();

        IsEvolved = true;
        upgradeName = evolvedName;
        icon = evolvedIcon;

        return true;
    }

    protected abstract void ApplyEvolution();

#if UNITY_EDITOR
    public void DebugEvolve()
    {
        if (IsEvolved) return;

        while (!IsMaxLevel)
        {
            Upgrade();
        }

        if (requiredPassive != null)
        {
            while (!requiredPassive.IsMaxLevel)
            {
                requiredPassive.Upgrade();
            }
        }

        Evolve();
    }
#endif
}
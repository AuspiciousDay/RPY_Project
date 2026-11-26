using UnityEngine;

public abstract class WeaponController : UpgradeBase
{
    [Header("HUD")]
    [SerializeField] private string skillKey;

    [Header("Evolution")]
    [SerializeField] private PassiveController requiredPassive;
    [SerializeField] private string evolvedName;
    [SerializeField] private Sprite evolvedIcon;

    public bool IsEvolved { get; private set; }

    public PassiveController RequiredPassive => requiredPassive;
    private bool HasRequiredPassive => requiredPassive != null && requiredPassive.Level > 0;
    public bool NeedsEvolutionPassive => !IsEvolved && Level > 0 && requiredPassive != null
        && !HasRequiredPassive;
    public bool CanEvolve => !IsEvolved && IsMaxLevel && HasRequiredPassive;

    public bool Evolve()
    {
        if (!CanEvolve) return false;

        ApplyEvolution();

        IsEvolved = true;
        upgradeName = evolvedName;
        icon = evolvedIcon;

        GameAudio.Play("weapon_evolution", 0.9f, 0.3f);
        UiManager.Instance?.ChangeSkillIcon(skillKey, evolvedIcon);

        return true;
    }

    protected abstract void ApplyEvolution();

    protected override void OnFirstUpgrade()
    {
        UiManager ui = UiManager.Instance;
        if (ui == null) return;

        ui.ChangeSkillIcon(skillKey, icon);
        ui.ShowSkillIcon(skillKey);
    }

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

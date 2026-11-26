using System.Collections.Generic;
using UnityEngine;

public abstract class UpgradeBase : MonoBehaviour
{
    public string upgradeName;

    [TextArea]
    public string description;

    public Sprite icon;

    public int Level { get; private set; } = 0;
    public int maxLevel = 5;

    public bool IsMaxLevel => Level >= maxLevel;

    public static List<UpgradeBase> GetRunUpgradeCandidates(UpgradeBase[] upgrades)
    {
        const int categoryLimit = 4;
        int weaponCount = 0;
        int passiveCount = 0;
        foreach (UpgradeBase upgrade in upgrades)
        {
            if (upgrade == null || upgrade.Level == 0) continue;
            if (upgrade is WeaponController) weaponCount++;
            if (upgrade is PassiveController) passiveCount++;
        }

        var candidates = new List<UpgradeBase>();
        foreach (UpgradeBase upgrade in upgrades)
        {
            if (upgrade == null || upgrade.IsMaxLevel) continue;
            if (upgrade.Level == 0 &&
                ((upgrade is WeaponController && weaponCount >= categoryLimit) ||
                 (upgrade is PassiveController && passiveCount >= categoryLimit)))
                continue;

            candidates.Add(upgrade);
        }
        return candidates;
    }

    public void Upgrade()
    {
        if (IsMaxLevel) return;

        Level++;

        Apply();

        if (Level == 1)
        {
            OnFirstUpgrade();
        }
    }

    protected virtual void OnFirstUpgrade()
    {
    }

    public virtual string GetDescription()
    {
        return description;
    }

    protected abstract void Apply();
}
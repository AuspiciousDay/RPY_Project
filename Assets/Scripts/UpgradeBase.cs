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

    public void Upgrade()
    {
        if (IsMaxLevel) return;

        Level++;
        Apply();
    }

    public virtual string GetDescription()
    {
        return description;
    }

    protected abstract void Apply();
}
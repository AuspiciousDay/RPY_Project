using UnityEngine;

public abstract class PassiveController : MonoBehaviour
{
    public string passiveName;

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

    protected abstract void Apply();
}
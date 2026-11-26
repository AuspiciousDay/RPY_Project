using UnityEngine;

public abstract class PassiveController : UpgradeBase
{
    [SerializeField] private string passiveKey;

    protected override void OnFirstUpgrade()
    {
        UiManager.Instance?.ShowPassiveIcon(passiveKey, icon);
    }
}

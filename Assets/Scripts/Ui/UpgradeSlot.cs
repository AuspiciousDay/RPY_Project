using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class UpgradeSlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text descriptionText;

    private UpgradeBase upgrade;
    private PassiveUpgradeUI upgradeUI;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void Setup(UpgradeBase upgrade, PassiveUpgradeUI upgradeUI)
    {
        this.upgrade = upgrade;
        this.upgradeUI = upgradeUI;

        icon.sprite = upgrade.icon;
        nameText.text = upgrade.upgradeName;
        descriptionText.text = upgrade.GetDescription();

        levelText.text = upgrade.Level == 0
            ? "NEW!"
            : $"Lv. {upgrade.Level + 1}";
    }

    public void OnClick()
    {
        if (upgrade == null) return;

        upgrade.Upgrade();
        upgradeUI.Close();
    }
}
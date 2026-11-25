using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeSlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text descriptionText;

    private PassiveController passive;
    private PassiveUpgradeUI upgradeUI;
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void Setup(PassiveController passive, PassiveUpgradeUI upgradeUI)
    {
        this.passive = passive;
        this.upgradeUI = upgradeUI;

        icon.sprite = passive.icon;
        nameText.text = passive.passiveName;
        descriptionText.text = passive.description;

        levelText.text = passive.Level == 0 ? "NEW!" : $"Lv. {passive.Level + 1}";
    }

    public void OnClick()
    {
        if (passive == null) return;

        passive.Upgrade();
        upgradeUI.Close();
    }
}
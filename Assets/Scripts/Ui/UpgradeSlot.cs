using System;
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

    private Action select;
    private PassiveUpgradeUI upgradeUI;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void Setup(UpgradeBase upgrade, PassiveUpgradeUI upgradeUI)
    {
        string levelLabel = upgrade.Level == 0 ? "NEW!" : GameLanguage.Choose($"Lv. {upgrade.Level + 1}", $"레벨 {upgrade.Level + 1}");
        SetupChoice(upgrade.upgradeName, upgrade.GetDescription(), levelLabel, upgrade.icon, upgrade.Upgrade, upgradeUI);
    }

    public void SetupChoice(string title, string description, string levelLabel, Sprite sprite, Action reward, PassiveUpgradeUI upgradeUI)
    {
        select = reward;
        this.upgradeUI = upgradeUI;
        icon.sprite = sprite;
        icon.enabled = sprite != null;
        nameText.text = GameLanguage.Translate(title);
        descriptionText.text = GameLanguage.Translate(description);
        levelText.text = GameLanguage.Translate(levelLabel);
    }

    public void OnClick()
    {
        if (select == null) return;

        Action reward = select;
        select = null;
        GameAudio.Play("upgrade_select");
        reward();
        upgradeUI.Close();
    }
}
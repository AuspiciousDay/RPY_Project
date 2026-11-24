using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponUpgrade : MonoBehaviour
{
    public Button countUpgradeButton;
    public Button penetrateUpgradeButton;
    public Button damageUpgradeButton;

    public TMP_Text countUpgradeDescription;
    public TMP_Text penetrateUpgradeDescription;
    public TMP_Text damageUpgradeDescription;

    public PlayerShooter playerShooter;

    private void Awake()
    {
        countUpgradeButton.onClick.AddListener(UpgradeCount);
        penetrateUpgradeButton.onClick.AddListener(UpgradePenetrate);
        damageUpgradeButton.onClick.AddListener(UpgradeDamage);
    }

    private void Start()
    {
        CloseUpgrade();
    }

    private void OnEnable()
    {
        countUpgradeDescription.text =
            $"Count: {playerShooter.AttackCount}";

        penetrateUpgradeDescription.text =
            $"Penetrate: {playerShooter.PenetrateCount}";

        damageUpgradeDescription.text =
            $"Damage: {playerShooter.AttackDamage}";
    }

    public void UpgradeCount()
    {
        playerShooter.AttackCount++;
        CloseUpgrade();
    }

    public void UpgradePenetrate()
    {
        playerShooter.PenetrateCount++;
        CloseUpgrade();
    }

    public void UpgradeDamage()
    {
        playerShooter.AttackDamage += 5;
        CloseUpgrade();
    }

    private void CloseUpgrade()
    {
        gameObject.SetActive(false);
        GameManager.PauseGame(1f);
    }
}
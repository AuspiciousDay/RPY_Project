using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponUpgrade : MonoBehaviour
{
    public Button countUpgradeButton;
    public Button intervalUpgradeButton;
    public Button damageUpgradeButton;

    public TMP_Text countUpgradeDescription;
    public TMP_Text intervalUpgradeDescription;
    public TMP_Text damageUpgradeDescription;

    public PlayerAbility weaponAbility;
    public PlayerShooter playerShooter;

    private void Start()
    {
        countUpgradeButton.onClick.AddListener(UpgradeCount);
        intervalUpgradeButton.onClick.AddListener(UpgradeInterval);
        damageUpgradeButton.onClick.AddListener(UpgradeDamage);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        countUpgradeDescription.text = $"Count: {playerShooter.AttackCount}";
        intervalUpgradeDescription.text = $"Interval: {playerShooter.AttackInterval:F2}s";
        damageUpgradeDescription.text = $"Damage: {weaponAbility.Damage}";
    }

    public void UpgradeCount()
    {
        playerShooter.AttackCount++;
        gameObject.SetActive(false);
        GameManager.PauseGame(1f);
    }
    public void UpgradeInterval()
    {
        playerShooter.AttackInterval -= 0.1f; 
        gameObject.SetActive(false);
        GameManager.PauseGame(1f);
    }
    public void UpgradeDamage()
    {
        weaponAbility.Damage += 10;
        gameObject.SetActive(false);
        GameManager.PauseGame(1f);
    }
}

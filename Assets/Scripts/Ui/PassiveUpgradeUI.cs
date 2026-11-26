using System.Collections.Generic;
using UnityEngine;

public class PassiveUpgradeUI : MonoBehaviour
{
    [SerializeField] private UpgradeBase[] upgrades;
    [SerializeField] private UpgradeSlot[] slots;
    [SerializeField] private bool favorOwnedWeaponUpgrades;
    [SerializeField] private Sprite goldRewardIcon;
    [SerializeField] private Sprite healRewardIcon;

    private const int GoldReward = 100;
    private const int HealReward = 20;
    private PlayerLevel player;
    private bool openedForLevelUp;

    public void Open(PlayerLevel levelOwner)
    {
        if (gameObject.activeInHierarchy) return;

        player = levelOwner;
        openedForLevelUp = true;
        GameManager.PauseGame(0f);
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        ShowOptions();
    }
    private void Start()
    {
        if (!openedForLevelUp) gameObject.SetActive(false);
    }

    private void ShowOptions()
    {
        List<UpgradeBase> candidates = UpgradeBase.GetRunUpgradeCandidates(upgrades);

        if (candidates.Count == 0)
        {
            ShowFallbackRewards();
            return;
        }

        var ownedWeapons = new List<UpgradeBase>();
        var newWeapons = new List<UpgradeBase>();
        var evolutionPassives = new List<UpgradeBase>();
        int ownedWeaponCount = 0;
        if (favorOwnedWeaponUpgrades)
        {
            foreach (UpgradeBase upgrade in upgrades)
            {
                if (upgrade is not WeaponController weapon) continue;
                if (weapon.Level == 0)
                {
                    if (candidates.Contains(weapon)) newWeapons.Add(weapon);
                    continue;
                }

                ownedWeaponCount++;
                if (candidates.Contains(weapon)) ownedWeapons.Add(weapon);
                if (weapon.NeedsEvolutionPassive && candidates.Contains(weapon.RequiredPassive)
                    && !evolutionPassives.Contains(weapon.RequiredPassive))
                    evolutionPassives.Add(weapon.RequiredPassive);
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            UpgradeSlot slot = slots[i];
            if (candidates.Count == 0)
            {
                slot.gameObject.SetActive(false);
                continue;
            }

            List<UpgradeBase> pool = candidates;
            if (i == 0 && ownedWeapons.Count > 0) pool = ownedWeapons;
            if (i == 1 && evolutionPassives.Count > 0) pool = evolutionPassives;
            if (i == 2 && ownedWeaponCount < 2 && newWeapons.Count > 0) pool = newWeapons;

            UpgradeBase selected = pool[Random.Range(0, pool.Count)];
            candidates.Remove(selected);
            newWeapons.Remove(selected);
            evolutionPassives.Remove(selected);

            slot.gameObject.SetActive(true);
            slot.Setup(selected, this);
        }
    }

    private void ShowFallbackRewards()
    {
        if (player == null || slots.Length < 2)
        {
            Close();
            return;
        }

        slots[0].SetupChoice(GameLanguage.Choose($"{GoldReward} Gold", $"골드 {GoldReward} 획득"), "Gain gold for the shop.", "", goldRewardIcon, GiveGold, this);
        slots[1].SetupChoice(GameLanguage.Choose($"Heal {HealReward} HP", $"체력 {HealReward} 회복"), "Restore health up to your maximum.", "", healRewardIcon, HealPlayer, this);
        for (int i = 0; i < slots.Length; i++)
            slots[i].gameObject.SetActive(i < 2);
    }

    private void GiveGold()
    {
        if (GoldManager.Instance == null) return;

        GoldManager.Instance.AddGold(GoldReward);
        player.uiManager.UpdateCoinCount(GoldReward);
    }

    private void HealPlayer()
    {
        if (player.TryGetComponent(out PlayerHealth health))
            health.Heal(HealReward);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        if (UiManager.Instance == null || !UiManager.Instance.IsRunOver)
            GameManager.PauseGame(1f);
    }
}

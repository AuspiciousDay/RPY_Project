using UnityEngine;
using System.Collections.Generic;

public class TreasureRewardManager : MonoBehaviour
{
    [Header("강화 목록")]
    [SerializeField] private UpgradeBase[] upgrades;
    [SerializeField] private EvolutionManager evolutionManager;

    [Header("행운")]
    [SerializeField] private PermanentPowerUp permanentPowerUp;
    [SerializeField, Range(0f, 1f)] private float tripleRewardChance = 0.1f;

    [Header("골드 보상")]
    [SerializeField] private int goldReward = 100;
    [SerializeField] private Sprite goldIcon;

    public int GetRewardCount()
    {
        float luck = permanentPowerUp != null ? permanentPowerUp.LuckMultiplier : 1f;

        float chance = Mathf.Clamp01(tripleRewardChance * luck);

        return Random.value < chance ? 3 : 1;
    }

    public List<Sprite> GiveRewards(int count)
    {
        List<Sprite> rewards = new List<Sprite>();

        for (int i = 0; i < count; i++)
        {
            UpgradeBase reward = GiveReward();

            if (reward != null)
            {
                rewards.Add(reward.icon);
            }
            else
            {
                GoldManager.Instance.AddGold(goldReward);

                UiManager.Instance?.UpdateCoinCount(goldReward);

                rewards.Add(goldIcon);
                break;
            }
        }

        return rewards;
    }

    private UpgradeBase GiveReward()
    {
        if (evolutionManager != null)
        {
            List<WeaponController> evolvableWeapons = evolutionManager.GetEvolvableWeapons();

            if (evolvableWeapons.Count > 0)
            {
                int index = Random.Range(0, evolvableWeapons.Count);

                WeaponController weapon = evolvableWeapons[index];

                if (weapon.Evolve())
                {
                    return weapon;
                }
            }
        }

        List<UpgradeBase> candidates = UpgradeBase.GetRunUpgradeCandidates(upgrades);

        if (candidates.Count == 0) return null;

        int randomIndex = Random.Range(0, candidates.Count);

        UpgradeBase selected = candidates[randomIndex];

        selected.Upgrade();

        return selected;
    }
}
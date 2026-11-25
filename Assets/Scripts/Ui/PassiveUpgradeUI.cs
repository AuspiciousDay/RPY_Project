using System.Collections.Generic;
using UnityEngine;

public class PassiveUpgradeUI : MonoBehaviour
{
    [SerializeField] private UpgradeBase[] upgrades;
    [SerializeField] private UpgradeSlot[] slots;

    private void OnEnable()
    {
        ShowOptions();
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void ShowOptions()
    {
        List<UpgradeBase> candidates = new List<UpgradeBase>();

        foreach (UpgradeBase upgrade in upgrades)
        {
            if (upgrade != null && !upgrade.IsMaxLevel)
            {
                candidates.Add(upgrade);
            }
        }

        if (candidates.Count == 0)
        {
            Close();
            return;
        }

        foreach (UpgradeSlot slot in slots)
        {
            if (candidates.Count == 0)
            {
                slot.gameObject.SetActive(false);
                continue;
            }

            int index = Random.Range(0, candidates.Count);

            UpgradeBase selected = candidates[index];

            candidates.RemoveAt(index);

            slot.gameObject.SetActive(true);
            slot.Setup(selected, this);
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
        GameManager.PauseGame(1f);
    }
}
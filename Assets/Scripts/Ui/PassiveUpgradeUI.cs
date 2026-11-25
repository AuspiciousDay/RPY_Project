using System.Collections.Generic;
using UnityEngine;

public class PassiveUpgradeUI : MonoBehaviour
{
    [SerializeField] private PassiveController[] passives;
    [SerializeField] private UpgradeSlot[] slots;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ShowOptions();
    }

    private void ShowOptions()
    {
        List<PassiveController> candidates =
            new List<PassiveController>();

        foreach (PassiveController passive in passives)
        {
            if (!passive.IsMaxLevel)
            {
                candidates.Add(passive);
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

            PassiveController selected = candidates[index];

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
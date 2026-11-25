using UnityEngine;

public class PassiveAddMaxHp : PassiveController
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private int addMaxHp = 20;

    protected override void Apply()
    {
        UiManager.Instance?.ShowPassiveIcon("MaxHp");
        playerHealth.HealthMax += addMaxHp;
        playerHealth.Health += addMaxHp;

        playerHealth.uiManager.UpdateHp(playerHealth.Health, playerHealth.HealthMax);
    }
}
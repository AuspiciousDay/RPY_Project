using UnityEngine;

public class HealthItem : MagneticPickup
{
    public int HealAmount = 20;

    protected override bool ApplyPickup(PlayerLevel player)
    {
        if (!player.TryGetComponent(out PlayerHealth playerHealth)) return false;

        GameAudio.Play("heal_pickup");
        playerHealth.Heal(HealAmount);
        Debug.Log("회복 아이템 획득 : " + HealAmount);
        return true;
    }
}

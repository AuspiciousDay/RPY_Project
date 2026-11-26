using UnityEngine;

public class Coin : MagneticPickup
{
    [SerializeField] private int goldAmount = 10;

    protected override bool ApplyPickup(PlayerLevel player)
    {
        if (GoldManager.Instance == null) return false;

        GameAudio.Play("gem_large_pickup", 0.45f, 0.1f);
        GoldManager.Instance.AddGold(goldAmount);
        UiManager.Instance?.UpdateCoinCount(goldAmount);
        return true;
    }
}

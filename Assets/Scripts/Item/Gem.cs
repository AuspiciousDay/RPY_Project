using UnityEngine;

public class Gem : MagneticPickup
{
    [SerializeField] private float expAmount = 30f;

    protected override bool ApplyPickup(PlayerLevel player)
    {
        GameAudio.Play(expAmount >= 50f ? "gem_large_pickup" : "gem_pickup", 0.6f, 0.07f);
        player.GainExp(expAmount);
        return true;
    }
}

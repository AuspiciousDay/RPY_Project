using UnityEngine;

public class PassiveAddMagnet : PassiveController
{
    [SerializeField] private CircleCollider2D gemMagnet;
    [SerializeField] private float addRadius = 0.5f;

    protected override void Apply()
    {
        UiManager.Instance?.ShowPassiveIcon("Magnet");
        gemMagnet.radius += addRadius;
    }
}
using UnityEngine;

public class PassiveAddMagnet : PassiveController
{
    [SerializeField] private CircleCollider2D gemMagnet;
    [SerializeField] private float addRadius = 0.5f;

    protected override void Apply()
    {
        gemMagnet.radius += addRadius;
    }
}
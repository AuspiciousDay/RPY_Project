using UnityEngine;

public class PassiveAddSpeed : PassiveController
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float addSpeed = 0.5f;

    protected override void Apply()
    {
        playerMovement.MoveSpeed += addSpeed;
    }
}
using UnityEngine;

public class PassiveAddSpeed : PassiveController
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float addSpeed = 0.5f;

    protected override void Apply()
    {
        UiManager.Instance?.ShowPassiveIcon("Speed");
        playerMovement.MoveSpeed += addSpeed;
    }
}
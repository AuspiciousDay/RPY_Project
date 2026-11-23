  using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static readonly int IsMoving = Animator.StringToHash("IsMoving");
    public float MoveSpeed = 5f;

    private Animator PlayerAnimator;
    private PlayerInput PlayerInput;
    private Rigidbody2D PlayerRigidbody;

    private void Awake()
    {
        PlayerAnimator = GetComponent<Animator>();
        PlayerInput = GetComponent<PlayerInput>();
        PlayerRigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector2 deita = new Vector2(PlayerInput.Side,  PlayerInput.Move).normalized * MoveSpeed * Time.fixedDeltaTime; // 수정 필요. 대각선 이동시 속도가 좀더 빠름

        PlayerRigidbody.MovePosition(PlayerRigidbody.position + deita);
        if(deita != Vector2.zero)
        {
            PlayerAnimator.SetBool(IsMoving, true);
        }
        else
        {
            PlayerAnimator.SetBool(IsMoving, false);
        }
    }

}
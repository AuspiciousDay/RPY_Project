using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static readonly int IsMoving = Animator.StringToHash("IsMoving");

    public float MoveSpeed = 1f;

    private Animator PlayerAnimator;
    private PlayerInput PlayerInput;
    private Rigidbody2D PlayerRigidbody;
    private SpriteRenderer PlayerSprite;

    private void Awake()
    {
        PlayerAnimator = GetComponent<Animator>();
        PlayerInput = GetComponent<PlayerInput>();
        PlayerRigidbody = GetComponent<Rigidbody2D>();
        PlayerSprite = GetComponent<SpriteRenderer>();

        PlayerRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        PlayerRigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
    }
    private void Update()
    {
        Vector2 direction = new Vector2(PlayerInput.Side, PlayerInput.Move);

        PlayerAnimator.SetBool(IsMoving, direction.sqrMagnitude > 0.001f);

        if (PlayerInput.Side < 0)
        {
            PlayerSprite.flipX = true;
        }
        else if (PlayerInput.Side > 0)
        {
            PlayerSprite.flipX = false;
        }
    }

    private void FixedUpdate()
    {
        Vector2 direction = new Vector2(PlayerInput.Side, PlayerInput.Move).normalized;

        if (direction == Vector2.zero)
        {
            PlayerRigidbody.linearVelocity = Vector2.zero;
            PlayerRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            return;
        }

        PlayerRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        PlayerRigidbody.linearVelocity = direction * MoveSpeed;
    }

}
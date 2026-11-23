  using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static readonly int HasMove = Animator.StringToHash("Move"); 
    public float MoveSpeed = 5f;

    private Animator PlayerAnimator;
    private PlayerInput PlayerInput;
    private Rigidbody2D PlayerRigidbody;
    private Transform MainCamera;

    private void Awake()
    {
        PlayerAnimator = GetComponent<Animator>();
        PlayerInput = GetComponent<PlayerInput>();
        PlayerRigidbody = GetComponent<Rigidbody2D>();
        MainCamera = Camera.main.transform;
    }

    private void Update()
    {
        PlayerAnimator.SetFloat(HasMove, PlayerInput.Move);
    }
    private void FixedUpdate()
    {
        Vector2 deita = new Vector2(PlayerInput.Side,  PlayerInput.Move) * MoveSpeed * Time.fixedDeltaTime;

        PlayerRigidbody.MovePosition(PlayerRigidbody.position + deita);
    }
    private void LateUpdate()
    {
        Vector3 cameraPoition = transform.position;
        cameraPoition.z = MainCamera.position.z;

        MainCamera.position = cameraPoition;
    }
}
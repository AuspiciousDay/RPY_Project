using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static readonly string MoveAxis = "Vertical";
    public static readonly string SideAxis = "Horizontal";

    public float Move { get; private set; }
    public float Side { get; private set; }
    [SerializeField] 
    private SpriteRenderer spriteRenderer;

    private void Update()
    {
        Move = Input.GetAxis(MoveAxis);
        Side = Input.GetAxis(SideAxis);

        if (Side > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (Side < 0)
        {
            spriteRenderer.flipX = true;
        }
    }
    private void OnEnable()
    {
        Move = 0f;
        Side = 0f;
    }

}

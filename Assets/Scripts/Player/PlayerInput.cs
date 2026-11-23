using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static readonly string MoveAxis = "Vertical";
    public static readonly string SideAxis = "Horizontal";

    public float Move { get; private set; }
    public float Side { get; private set; }

    private void OnEnable()
    {
        Move = 0f;
        Side = 0f;       
    }
    public void Update()
    {
        Move = Input.GetAxis(MoveAxis);
        Side = Input.GetAxis(SideAxis);
        
    }

}

using UnityEngine;

public abstract class MagneticPickup : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float collectDistance = 0.1f;

    private PlayerLevel target;
    private bool isCollected;

    public void SetTarget(PlayerLevel player)
    {
        if (target == null && !isCollected) target = player;
    }

    protected void Update()
    {
        if (target == null || isCollected || Time.timeScale <= 0f) return;
        if (target.uiManager != null && target.uiManager.IsRunOver) return;

        moveSpeed += acceleration * Time.deltaTime;
        Vector3 destination = target.transform.position;
        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
        if ((transform.position - destination).sqrMagnitude > collectDistance * collectDistance) return;

        isCollected = true;
        if (ApplyPickup(target))
            Destroy(gameObject);
        else
            isCollected = false;
    }

    protected abstract bool ApplyPickup(PlayerLevel player);
}

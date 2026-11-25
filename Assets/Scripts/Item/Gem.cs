using UnityEngine;

public class Gem : MonoBehaviour
{
    [SerializeField] private float expAmount = 30f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float collectDistance = 0.1f;

    private PlayerLevel target;
    private bool isCollected;

    public void SetTarget(PlayerLevel player)
    {
        if (target != null || isCollected) return;

        target = player;
    }

    private void Update()
    {
        if (target == null || isCollected) return;
        if (Time.timeScale == 0f) return;

        moveSpeed += acceleration * Time.deltaTime;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.transform.position,
            moveSpeed * Time.deltaTime
        );

        float distance = Vector3.Distance(
            transform.position,
            target.transform.position
        );

        if (distance <= collectDistance)
        {
            Collect();
        }
    }

    private void Collect()
    {
        if (isCollected) return;

        isCollected = true;

        target.GainExp(expAmount);

        Destroy(gameObject);
    }
}
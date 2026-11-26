using UnityEngine;

public class CageSkeleton : Monster
{
    [Header("케이지 수축 설정")]
    [SerializeField] private float shrinkSpeed = 0.2f;
    [SerializeField] private float minRadius = 1.5f;

    private Vector2 centerPos;
    private bool isInitialized = false;

    public void Initialize(Vector2 cageCenter, float duration, float shrinkSpeedOverride)
    {
        centerPos = cageCenter;
        if (shrinkSpeedOverride > 0f) shrinkSpeed = shrinkSpeedOverride;
        isInitialized = true;
        Destroy(gameObject, duration);
    }

    protected override void Update()
    {
        if (Time.timeScale == 0f || isDead || !isInitialized) return;

        Vector2 toCenter = centerPos - (Vector2)transform.position;
        float currentRadius = toCenter.magnitude;

        if (currentRadius > minRadius)
        {
            Vector2 dir = toCenter / currentRadius;
            float nextRadius = Mathf.Max(minRadius, currentRadius - shrinkSpeed * Time.deltaTime);
            transform.position = centerPos - dir * nextRadius;
        }

        // 필요하면 수축 방향에 맞춰 좌우 반전 적용 가능
        if (spriteRenderer != null && Mathf.Abs(toCenter.x) > 0.01f)
        {
            spriteRenderer.flipX = toCenter.x < 0f;
        }
    }
}

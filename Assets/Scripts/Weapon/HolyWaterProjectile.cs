using UnityEngine;

public class HolyWaterProjectile : MonoBehaviour
{
    [SerializeField] private float flightTime = 0.5f;
    [SerializeField] private float arcHeight = 2f;
    [SerializeField] private float rotateSpeed = 360f;

    private Vector2 startPosition;
    private Vector2 targetPosition;

    private HolyWaterAbility areaPrefab;

    private float damage;
    private float radius;
    private float duration;
    private float timer;

    public void Init(Vector2 targetPosition, HolyWaterAbility areaPrefab, float damage, float radius, float duration)
    {
        startPosition = transform.position;

        this.targetPosition = targetPosition;
        this.areaPrefab = areaPrefab;
        this.damage = damage;
        this.radius = radius;
        this.duration = duration;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        float t = Mathf.Clamp01(timer / flightTime);

        Vector2 position = Vector2.Lerp(startPosition, targetPosition, t);

        position.y += Mathf.Sin(t * Mathf.PI) * arcHeight;

        transform.position = position;

        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);

        if (t >= 1f)
        {
            Land();
        }
    }

    private void Land()
    {
        HolyWaterAbility area = Instantiate(areaPrefab, targetPosition, Quaternion.identity);

        area.Init(damage, radius, duration);

        Destroy(gameObject);
    }
}
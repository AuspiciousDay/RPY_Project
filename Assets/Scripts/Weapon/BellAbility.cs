using System.Collections.Generic;
using UnityEngine;

public class BellAbility : MonoBehaviour
{
    private Transform target;

    private float angle;
    private float radius;
    private float rotateSpeed;
    private float damage;

    [SerializeField] private float hitInterval = 0.5f;

    private readonly Dictionary<IDamageable, float> nextHitTime = new Dictionary<IDamageable, float>();
    public void Init(Transform target, float angle, float radius, float rotateSpeed, float damage, float duration, bool isEvolved = false)
    {
        this.target = target;
        this.angle = angle;
        this.radius = radius;
        this.rotateSpeed = rotateSpeed;
        this.damage = damage;

        UpdatePosition();

        if (!isEvolved)
        {
            Destroy(gameObject, duration);
        }
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        angle += rotateSpeed * Time.fixedDeltaTime;

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        float radian = angle * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0f) * radius;

        transform.position = target.position + offset;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Hit(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Hit(collision);
    }

    private void Hit(Collider2D other)
    {
        IDamageable target = other.GetComponentInParent<IDamageable>();

        if (target == null) return;

        if (nextHitTime.TryGetValue(target, out float nextTime))
        {
            if (Time.time < nextTime) return;
        }

        target.TakeDamage(damage);

        nextHitTime[target] = Time.time + hitInterval;
    }
}
using System.Collections.Generic;
using UnityEngine;

public class BellAbility : MonoBehaviour
{
    private Transform target;

    private float angle;
    private float radius;
    private float rotateSpeed;
    private float damage;
    private BellWeapon evolvedWeapon;

    public float OrbitAngle => angle;

    [SerializeField] private float hitInterval = 0.5f;

    private readonly Dictionary<IDamageable, float> nextHitTime = new Dictionary<IDamageable, float>();
    private readonly List<IDamageable> expiredTargets = new List<IDamageable>();
    private float nextCleanupTime;
    public void Init(Transform target, float angle, float radius, float rotateSpeed, float damage, float duration, BellWeapon evolvedWeapon = null)
    {
        this.target = target;
        this.angle = angle;
        this.radius = radius;
        this.rotateSpeed = rotateSpeed;
        this.damage = damage;
        this.evolvedWeapon = evolvedWeapon;

        UpdatePosition();
        if (evolvedWeapon == null)
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

        float currentSpeed = evolvedWeapon != null ? evolvedWeapon.rotateSpeed : rotateSpeed;
        angle = Mathf.Repeat(angle + currentSpeed * Time.fixedDeltaTime, 360f);

        UpdatePosition();

        if (Time.time >= nextCleanupTime)
        {
            nextCleanupTime = Time.time + 2f;
            foreach (var hit in nextHitTime)
            {
                if (hit.Value <= Time.time || hit.Key is Object targetObject && targetObject == null)
                    expiredTargets.Add(hit.Key);
            }

            foreach (IDamageable expired in expiredTargets)
                nextHitTime.Remove(expired);

            expiredTargets.Clear();
        }
    }

    private void UpdatePosition()
    {
        float radian = angle * Mathf.Deg2Rad;

        float currentRadius = evolvedWeapon != null ? evolvedWeapon.radius : radius;
        Vector3 offset = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0f) * currentRadius;

        transform.position = target.position + offset;
    }

    public void SetOrbitAngle(float newAngle)
    {
        angle = newAngle;
        UpdatePosition();
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

        target.TakeDamage(evolvedWeapon != null ? evolvedWeapon.EffectiveDamage : damage);

        nextHitTime[target] = Time.time + hitInterval;
    }
}

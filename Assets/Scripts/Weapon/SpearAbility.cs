using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SpearAbility : MonoBehaviour
{
    private Rigidbody2D rb;

    private Vector2 direction;
    private float damage;
    private float speed;

    private HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 direction, float damage, float speed, float lifeTime)
    {
        this.direction = direction.normalized;
        this.damage = damage;
        this.speed = speed;

        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
            return;
        }

        IDamageable target = other.GetComponentInParent<IDamageable>();

        if (target != null && hitTargets.Add(target))
        {
            target.TakeDamage(damage);
        }
    }
}
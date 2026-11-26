using System.Collections.Generic;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    public float speed = 5f;
    public float damage = 30f;
    public int count = 0;
    public int maxCount = 1;
    [SerializeField, Min(0.1f)] private float lifetime = 6f;

    private Vector2 Direction;
    private Rigidbody2D AbilityRigidbody;
    private readonly HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();
    private bool spent;


    private void Awake()
    {
        AbilityRigidbody = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }

    public void SetDirection(Vector2 direction)
    {
        Direction = direction.normalized;
    }

    private void FixedUpdate()
    {
        AbilityRigidbody.MovePosition(AbilityRigidbody.position + Direction * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (spent) return;

        if (collision.CompareTag("Wall"))
        {
            spent = true;
            Destroy(gameObject);
            return;
        }

        IDamageable target = collision.GetComponentInParent<IDamageable>();

        if (target != null && hitTargets.Add(target))
        {
            target.TakeDamage(damage);
            count++;
        }

        if (count >= maxCount)
        {
            spent = true;
            Destroy(gameObject);
        }
    }
}

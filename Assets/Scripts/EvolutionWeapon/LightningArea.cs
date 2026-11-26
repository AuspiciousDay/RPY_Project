using UnityEngine;
using System.Collections.Generic;

public class LightningArea : MonoBehaviour
{
    private float damage;
    private float radius;
    private float timer;

    [SerializeField] private float hitInterval = 0.5f;

    private readonly HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();

    public void Init(float damage, float radius, float duration)
    {
        this.damage = damage;
        this.radius = radius;
        if (TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            Vector3 size = spriteRenderer.bounds.size;
            float diameter = radius * 2f;
            if (size.x > 0f && size.y > 0f)
                transform.localScale = Vector3.Scale(transform.localScale, new Vector3(diameter / size.x, diameter / size.y, 1f));
        }

        timer = 0f;

        Destroy(gameObject, duration);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= hitInterval)
        {
            Attack();
            timer = 0f;
        }
    }

    private void Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        hitTargets.Clear();

        foreach (Collider2D hit in hits)
        {
            Monster monster = hit.GetComponentInParent<Monster>();

            if (monster != null && hitTargets.Add(monster))
            {
                monster.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

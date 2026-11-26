using UnityEngine;
using System.Collections.Generic;

public class LightningArea : MonoBehaviour
{
    private float damage;
    private float radius;
    private float timer;

    [SerializeField] private float hitInterval = 0.5f;

    private readonly HashSet<Monster> hitMonsters = new HashSet<Monster>();

    public void Init(float damage, float radius, float duration)
    {
        this.damage = damage;
        this.radius = radius;

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

        hitMonsters.Clear();

        foreach (Collider2D hit in hits)
        {
            Monster monster = hit.GetComponentInParent<Monster>();

            if (monster != null && hitMonsters.Add(monster))
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
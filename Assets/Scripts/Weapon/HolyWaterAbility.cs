using System.Collections.Generic;
using UnityEngine;

public class HolyWaterAbility : MonoBehaviour
{
    [SerializeField] private float hitInterval = 0.5f;

    private float damage;
    private float radius;
    private float timer;

    private readonly HashSet<Monster> hitMonsters = new HashSet<Monster>();

    public void Init(float damage, float radius, float duration)
    {
        this.damage = damage;
        this.radius = radius;

        timer = hitInterval;
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();

        if (sprite != null && sprite.sprite != null)
        {
            Vector2 spriteSize = sprite.sprite.bounds.size;

            transform.localScale = new Vector3(radius * 2f / spriteSize.x, radius * 2f / spriteSize.y, 1f);
        }

        Destroy(gameObject, duration);
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;

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
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
using System.Collections.Generic;
using UnityEngine;

public class HolyWaterAbility : MonoBehaviour
{
    [SerializeField] private float hitInterval = 0.5f;

    private float damage;
    private float radius;
    private float timer;

    private readonly HashSet<Monster> hitMonsters = new HashSet<Monster>();

    private bool isEvolved;

    [SerializeField] private float pullSpeed = 5f;
    [SerializeField] private float pullRangeMultiplier = 1.5f;

    private readonly HashSet<Monster> pulledMonsters = new HashSet<Monster>();

    public void Init(float damage, float radius, float duration, bool isEvolved)
    {
        this.damage = damage;
        this.radius = radius;
        this.isEvolved = isEvolved;

        timer = hitInterval;

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

    private void PullMonsters()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            radius * pullRangeMultiplier
        );

        pulledMonsters.Clear();

        foreach (Collider2D hit in hits)
        {
            Monster monster = hit.GetComponentInParent<Monster>();

            if (monster == null)
                continue;

            if (!pulledMonsters.Add(monster))
                continue;

            monster.transform.position = Vector3.MoveTowards(
                monster.transform.position,
                transform.position,
                pullSpeed * Time.fixedDeltaTime
            );
        }
    }

    private void FixedUpdate()
    {
        if (isEvolved)
        {
            PullMonsters();
        }
    }
}
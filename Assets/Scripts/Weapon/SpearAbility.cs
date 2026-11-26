using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class SpearAbility : MonoBehaviour
{
    private Rigidbody2D rb;

    private Vector2 direction;
    private float damage;
    private float speed;

    private HashSet<Monster> hitMonsters
        = new HashSet<Monster>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(
        Vector2 direction,
        float damage,
        float speed,
        float lifeTime)
    {
        this.direction = direction.normalized;
        this.damage = damage;
        this.speed = speed;

        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(
            rb.position +
            direction * speed * Time.fixedDeltaTime
        );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Monster monster = collision.GetComponentInParent<Monster>();

        if (monster != null)
        {
            if (hitMonsters.Add(monster))
            {
                monster.TakeDamage(damage);
            }

            return;
        }

        if (collision.GetComponent<TilemapCollider2D>() != null)
        {
            Destroy(gameObject);
        }
    }
}
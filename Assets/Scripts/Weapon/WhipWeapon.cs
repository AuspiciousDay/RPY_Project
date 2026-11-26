using System.Collections.Generic;
using UnityEngine;

public class WhipWeapon : MonoBehaviour
{
    public float damage = 20f;
    public float attackInterval = 1.5f;
    public float attackWidth = 3f;
    public float attackHeight = 1.5f;
    public int attackCount = 1;

    [SerializeField] private GameObject whipPrefab;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private LayerMask monsterLayer;

    private float timer;
    private float lifeTime = 0.3f;

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        timer += Time.deltaTime;

        if (timer >= attackInterval)
        {
            Attack();
            timer = 0f;
        }
    }

    private void Attack()
    {
        int direction = playerSprite.flipX ? -1 : 1;

        for (int i = 0; i < attackCount; i++)
        {
            int attackDirection = i % 2 == 0 ? direction : -direction;

            Swing(attackDirection);
        }
    }

    private void Swing(int direction)
    {
        Vector2 center = (Vector2)transform.position + Vector2.right * direction * attackWidth / 2f;

        Vector2 size = new Vector2(attackWidth, attackHeight);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f);

        HashSet<Monster> hitMonsters = new HashSet<Monster>();

        foreach (Collider2D hit in hits)
        {
            Monster monster = hit.GetComponentInParent<Monster>();

            if (monster != null && hitMonsters.Add(monster))
            {

                monster.TakeDamage(damage);
            }
        }

        if (whipPrefab != null)
        {
            GameObject whip = Instantiate(whipPrefab, center, Quaternion.identity);

            Vector3 scale = whip.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * direction;
            whip.transform.localScale = scale;

            Destroy(whip, lifeTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 size = new Vector3(attackWidth, attackHeight, 0f);

        Gizmos.DrawWireCube(
            transform.position + Vector3.right * attackWidth / 2f, size);

        Gizmos.DrawWireCube(
            transform.position - Vector3.right * attackWidth / 2f, size);
    }
}

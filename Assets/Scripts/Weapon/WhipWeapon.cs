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

    public bool IsEvolved { get; private set; }

    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private float lifeSteal = 0.2f;

    [SerializeField] private GameObject evolvedPrefab;

    private PassiveAddDamage damagePassive;

    private void Awake()
    {
        damagePassive = GetComponentInParent<PassiveAddDamage>();
    }

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
        if (attackCount > 0) GameAudio.Play(IsEvolved ? "whip_evolved" : "whip_normal", 0.65f, 0.2f);
        float effectiveDamage = damage * (damagePassive != null ? damagePassive.Multiplier : 1f);
        int direction = playerSprite.flipX ? -1 : 1;

        for (int i = 0; i < attackCount; i++)
        {
            int attackDirection = i % 2 == 0 ? direction : -direction;

            Swing(attackDirection, effectiveDamage);
        }
    }

    private void Swing(int direction, float effectiveDamage)
    {
        Vector2 center = (Vector2)transform.position + Vector2.right * direction * attackWidth / 2f;
        Vector2 size = new Vector2(attackWidth, attackHeight);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f);

        HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();

        foreach (Collider2D hit in hits)
        {
            IDamageable target = hit.GetComponentInParent<IDamageable>();

            if (target == null || !hitTargets.Add(target))
            {
                continue;
            }

            if (IsEvolved && target is Monster monster)
            {
                float healthBeforeHit = Mathf.Max(0f, monster.health);

                monster.TakeDamage(effectiveDamage);

                float actualDamage = healthBeforeHit - Mathf.Max(0f, monster.health);
                playerHealth.Heal(actualDamage * lifeSteal);
            }
            else
            {
                target.TakeDamage(effectiveDamage);
            }
        }

        if (whipPrefab != null)
        {
            GameObject whip = Instantiate(whipPrefab, center, Quaternion.identity);

            Vector3 scale = whip.transform.localScale;
            if (whip.TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                Vector3 visualSize = spriteRenderer.bounds.size;
                if (visualSize.x > 0f && visualSize.y > 0f)
                {
                    scale.x *= size.x / visualSize.x;
                    scale.y *= size.y / visualSize.y;
                }
            }
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

    public void Evolve()
    {
        if (IsEvolved) return;

        IsEvolved = true;

        if (evolvedPrefab != null)
        {
            whipPrefab = evolvedPrefab;
        }
    }
}

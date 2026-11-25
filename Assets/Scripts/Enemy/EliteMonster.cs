using UnityEngine;

public class EliteMonster : Monster
{
    [SerializeField] private Transform eliteShadow;
    [SerializeField] private Collider2D attackHitbox;

    private float attackRange = 0.9f;
    private bool isAttack = false;
    private bool hasHitInCurrentAttack = false;

    protected override void Start()
    {
        base.Start();

        if (damage == 10)
        {
            damage = 12;
        }

        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }
    }

    protected override void Update()
    {
        if (health <= 0)
        {
            if (attackHitbox != null) attackHitbox.enabled = false;
            return;
        }
        if (playerTarget == null) return;

        UpdateHitboxDirection();

        if (isAttack) return;

        float distance = Vector2.Distance(transform.position, playerTarget.position);
        if (distance <= attackRange)
        {
            isAttack = true;
            spriteRenderer.flipX = (playerTarget.position.x < transform.position.x);
            animator.SetTrigger("Attack");
        }
        else
        {
            base.Update();
        }
    }

    private void UpdateHitboxDirection()
    {
        if (attackHitbox == null || spriteRenderer == null) return;

        Vector3 scale = attackHitbox.transform.localScale;
        scale.x = spriteRenderer.flipX ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        attackHitbox.transform.localScale = scale;
    }

    public void EnableAttackHitbox()
    {
        hasHitInCurrentAttack = false;
        if (attackHitbox != null)
        {
            attackHitbox.enabled = true;
        }
    }

    public void DisableAttackHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }
    }

    public void OnAttackEnd()
    {
        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }
        isAttack = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (attackHitbox == null || !attackHitbox.enabled) return;
        if (hasHitInCurrentAttack) return;
        if (!collision.CompareTag("Player")) return;

        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            playerHealth = collision.GetComponentInParent<PlayerHealth>();
        }

        if (playerHealth != null)
        {
            hasHitInCurrentAttack = true;
            playerHealth.Damage(damage);
        }
    }
}
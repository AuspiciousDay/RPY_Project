using UnityEngine;

public class EliteMonster : Monster
{
    [SerializeField] private Collider2D attackHitbox;

    [Header("검기 잔상 설정")]
    [SerializeField] private GameObject slashVfxPrefab;

    [Header("상자 드랍")]
    [SerializeField] private GameObject treasurePrefab;

    private float attackRange = 2f;
    private bool isAttack = false;
    private bool hasHitInCurrentAttack = false;

    protected override void Start()
    {
        base.Start();

        if (damage == 10)
        {
            damage = 12;
        }

        // 시작할 때는 검기 위치 콜라이더를 꺼둠
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
            hasHitInCurrentAttack = false;
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

        // 몬스터가 왼쪽을 보면 콜라이더 위치도 X축 반전
        Vector3 localPos = attackHitbox.transform.localPosition;
        localPos.x = spriteRenderer.flipX ? -Mathf.Abs(localPos.x) : Mathf.Abs(localPos.x);
        attackHitbox.transform.localPosition = localPos;

        Vector3 scale = attackHitbox.transform.localScale;
        scale.x = spriteRenderer.flipX ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        attackHitbox.transform.localScale = scale;
    }

    // Attack 애니메이션에서 검 휘두르는 프레임에 호출
    public void EnableAttackHitbox()
    {
        hasHitInCurrentAttack = false;

        if (attackHitbox != null)
        {
            attackHitbox.enabled = true;
        }

        if (slashVfxPrefab != null && attackHitbox != null)
        {
            GameObject vfx = Instantiate(slashVfxPrefab, attackHitbox.transform.position, Quaternion.identity);
            Vector3 s = vfx.transform.localScale;
            s.x = (spriteRenderer != null && spriteRenderer.flipX) ? -Mathf.Abs(s.x) : Mathf.Abs(s.x);
            vfx.transform.localScale = s;
            Destroy(vfx, 0.35f);
        }
    }

    public void DisableAttackHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }
    }

    // Attack 애니메이션 끝 프레임에 호출
    public void OnAttackEnd()
    {
        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }
        isAttack = false;
        hasHitInCurrentAttack = false;
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

    public override void Die()
    {
        if (treasurePrefab != null)
        {
            Instantiate(treasurePrefab, transform.position, Quaternion.identity);
        }

        base.Die();
    }
}
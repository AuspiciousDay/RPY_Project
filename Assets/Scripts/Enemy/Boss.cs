using UnityEngine;

public class Boss : Monster
{
    private Animator bossAnimator;

    [Header("공격 1")]
    [SerializeField] private GameObject attack1Prefab;
    [SerializeField] private Transform attack1Position;
    private int attack1Damage = 15;

    [Header("공격 2")]
    [SerializeField] private GameObject attack2Prefab;
    [SerializeField] private Transform attack2Position;
    private int attack2Damage = 20;
    private float attackCooldown = 3f;

    private Transform bossPlayerTarget;
    private SpriteRenderer bossSpriteRenderer;

    [Header("그림자")]
    [SerializeField] private Transform shadowPosition;

    private float cooldownTimer = 0f;
    private float bossMoveSpeed;
    private bool isEntryDone = false;
    private bool isAttacking = false;
    private float invulnerableUntil;

    public void SetSpawnInvulnerability(float duration)
    {
        invulnerableUntil = Time.time + duration;
    }

    public override void TakeDamage(float damage)
    {
        if (Time.time < invulnerableUntil) return;

        base.TakeDamage(damage);
    }

    private void LateUpdate()
    {
        if (isDead) return;

        if (bossSpriteRenderer == null)
            bossSpriteRenderer = GetComponent<SpriteRenderer>();

        if (bossAnimator == null)
            bossAnimator = GetComponent<Animator>();

        if (bossPlayerTarget == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                bossPlayerTarget = playerObj.transform;
            }
        }

        if (!isEntryDone)
        {
            if (moveSpeed > 0f)
            {
                bossMoveSpeed = moveSpeed;
                moveSpeed = 0f;
            }
            return;
        }

        if (bossPlayerTarget == null) return;

        UpdateSkillPointsDirection();

        if (!isAttacking)
        {
            cooldownTimer += Time.deltaTime;

            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0f;
                PlayAttack();
            }
        }
    }

    private void UpdateSkillPointsDirection()
    {
        if (bossSpriteRenderer == null) return;

        float dirSign = bossSpriteRenderer.flipX ? -1f : 1f;

        if (attack1Position != null)
        {
            Vector3 pos = attack1Position.localPosition;
            pos.x = Mathf.Abs(pos.x) * dirSign;
            attack1Position.localPosition = pos;
        }

        if (attack2Position != null)
        {
            Vector3 pos = attack2Position.localPosition;
            pos.x = Mathf.Abs(pos.x) * dirSign;
            attack2Position.localPosition = pos;
        }

        if (shadowPosition != null)
        {
            Vector3 pos = shadowPosition.localPosition;
            pos.x = Mathf.Abs(pos.x) * dirSign;
            shadowPosition.localPosition = pos;
        }
    }

    private void PlayAttack()
    {
        if (moveSpeed <= 0f || bossAnimator == null) return;

        bossMoveSpeed = moveSpeed;
        moveSpeed = 0f;
        isAttacking = true;
        bossAnimator.SetTrigger(Random.Range(0, 2) == 0 ? "Attack1" : "Attack2");
    }

    public void OnEntryEnd()
    {
        isEntryDone = true;
        if (bossMoveSpeed > 0f)
        {
            moveSpeed = bossMoveSpeed;
        }
    }

    public void SpawnAttack1Effect()
    {
        if (isDead || attack1Prefab == null || bossPlayerTarget == null) return;

        Vector3 spawnPos = attack1Position != null ? attack1Position.position : transform.position;
        GameObject projObj = Instantiate(attack1Prefab, spawnPos, Quaternion.identity);

        BossAbility projectile = projObj.GetComponent<BossAbility>();
        if (projectile != null)
        {
            projectile.Init(bossPlayerTarget, attack1Damage);
        }
    }

    public void SpawnAttack2Effect()
    {
        if (isDead || attack2Prefab == null || bossPlayerTarget == null) return;

        Vector3 spawnPos = attack2Position != null ? attack2Position.position : transform.position;
        GameObject projObj = Instantiate(attack2Prefab, spawnPos, Quaternion.identity);

        BossAbility projectile = projObj.GetComponent<BossAbility>();
        if (projectile != null)
        {
            projectile.Init(bossPlayerTarget, attack2Damage);
        }
    }

    public void OnAttackEnd()
    {
        isAttacking = false;
        moveSpeed = bossMoveSpeed;
    }

    public override void Die()
    {
        base.Die();

        UiManager.Instance?.Victory();
    }
}

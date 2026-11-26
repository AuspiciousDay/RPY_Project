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

    public override void Die()
    {
        base.Die();
        if (uiManager != null) uiManager.ShowVictory();
    }

    private void LateUpdate()
    {
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
                BossPlayeAttack();
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

    private void BossPlayeAttack()
    {
        int randomAttack = Random.Range(0, 3);

        if (randomAttack == 0)
        {
            if (moveSpeed > 0f)
            {
                bossMoveSpeed = moveSpeed;
                moveSpeed = 0f;
                isAttacking = true;
                bossAnimator.SetTrigger("Attack1");
            }
            return;

        }
        else if (randomAttack == 1)
        {
            if (moveSpeed > 0f)
            {
                bossMoveSpeed = moveSpeed;
                moveSpeed = 0f;
                isAttacking = true;
                bossAnimator.SetTrigger("Attack2");
            }
            return;
        }
        else
        {
            isAttacking = false;
        }
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
        if (attack1Prefab == null || bossPlayerTarget == null) return;

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
        if (attack2Prefab == null || bossPlayerTarget == null) return;

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
}

using UnityEngine;

public class Monster : MonoBehaviour, IDamageable
{
    [Header("몬스터 Data 연결")]
    [SerializeField] private MonsterData monsterData;

    [SerializeField] private GameObject expGem;
    [SerializeField] protected Animator animator;

    public float health = 100f;
    public float damage = 10f;

    public float moveSpeed = 3f;
    public float attackTime = 0.5f;

    protected Transform playerTarget;
    protected SpriteRenderer spriteRenderer;

    public UiManager uiManager;

    private HitFlash hitFlash;

    private float slowMultiplier = 1f;

    public float CurrentMoveSpeed => moveSpeed * slowMultiplier;

    [Header("장애물 회피")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float detectDistance = 0.8f;
    [SerializeField] private float detectRadius = 0.25f;

    private int avoidDirection = 0;

    private float nextAttackTime;


    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hitFlash = GetComponent<HitFlash>();
    }

    protected virtual void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
    }

    protected virtual void Update()
    {
        if (playerTarget == null || isDead) return;

        float distance = Vector2.Distance(transform.position, playerTarget.position);

        if (distance > 0.06f)
        {
            Vector2 direction = (playerTarget.position - transform.position).normalized;

            direction = AvoidObstacle(direction);

            transform.position += (Vector3)direction * CurrentMoveSpeed * Time.deltaTime;

            if (direction.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (direction.x > 0)
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    public void Setup(MonsterData data, UiManager uiManager)
    {
        monsterData = data;
        this.uiManager = uiManager;

        health = data.maxHp;
        damage = (int)data.damage;
        moveSpeed = data.speed;
        attackTime = data.attackTime;
    }

    private bool isDead = false;

    public void TakeDamage(float damage)
    {
        if (isDead) return;

#if UNITY_EDITOR
        if (DebugManager.Instance != null &&
            DebugManager.Instance.oneHitMode)
        {
            damage = health;
        }
#endif

        health -= damage;

        hitFlash.Flash();

        if (health <= 0)
        {
            isDead = true;
            Die();
        }
    }


    public void Die()
    {
        moveSpeed = 0f;
        int percent = 100;
        int randomValue = Random.Range(0, percent);
        uiManager.UpdateKillCount(1);

        if (expGem != null && randomValue < 70)
        {
            Instantiate(expGem, transform.position, Quaternion.identity);
        }
        Destroy(gameObject, 0.5f);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead) return;
        if (!collision.gameObject.CompareTag("Player")) return;
        if (Time.time < nextAttackTime) return;

        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.Damage(damage);
            nextAttackTime = Time.time + attackTime;
        }
    }

    public void ApplySlow(float multiplier)
    {
        slowMultiplier = Mathf.Clamp01(multiplier);
    }

    public void RemoveSlow()
    {
        slowMultiplier = 1f;
    }
    private Vector2 AvoidObstacle(Vector2 direction)
    {
        if (!HasObstacle(direction))
        {
            avoidDirection = 0;
            return direction;
        }

        if (avoidDirection == 0)
        {
            Vector2 left = new Vector2(-direction.y, direction.x);
            Vector2 right = new Vector2(direction.y, -direction.x);

            if (!HasObstacle(left))
            {
                avoidDirection = 1;
            }
            else if (!HasObstacle(right))
            {
                avoidDirection = -1;
            }
            else
            {
                return Vector2.zero;
            }
        }

        for (int angle = 30; angle <= 150; angle += 30)
        {
            Vector2 newDirection = Quaternion.Euler(0f, 0f, angle * avoidDirection) * direction;

            if (!HasObstacle(newDirection))
            {
                return newDirection;
            }
        }

        avoidDirection *= -1;

        return Vector2.zero;
    }

    private bool HasObstacle(Vector2 direction)
    {
        return Physics2D.CircleCast(transform.position, detectRadius, direction, detectDistance, obstacleLayer).collider != null;
    }
}
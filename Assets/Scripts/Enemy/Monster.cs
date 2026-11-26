using UnityEngine;

public class Monster : MonoBehaviour
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
        if (playerTarget == null) return;

        float distance = Vector2.Distance(transform.position, playerTarget.position);
        if (distance > 0.06f)
        {
            Vector2 direction = (playerTarget.position - transform.position).normalized;
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

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
        if (damage > 0f && EditorCombatModes.OneHitKill)
            damage = health;
#endif

        health -= damage;
        hitFlash.Flash();

        if (health <= 0)
        {
            isDead = true;

            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            if (animator != null) animator.SetTrigger("Die");
            Die();
        }
    }

    public virtual void Die()
    {
        moveSpeed = 0f;
        int percent = 100;
        int randomValue = Random.Range(0, percent);
        if (uiManager != null) uiManager.UpdateKillCount(1);

        if (expGem != null && randomValue < 70)
        {
            Instantiate(expGem, transform.position, Quaternion.identity);
        }
        Destroy(gameObject, 0.5f);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.Damage(damage);
        }
    }
}

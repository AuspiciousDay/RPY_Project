using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("몬스터 Data 연결")]
    [SerializeField] private MonsterData monsterData;

    [SerializeField] private GameObject expGem;

    public float Health = 100f;
    public float Damage = 10f;

    public float moveSpeed = 3f;
    public float attackTime = 0.1f;
    private float nextAttackTime;

    private Transform playerTarget;
    private SpriteRenderer spriteRenderer;

    public UiManager uiManager;

    public HitFlash hitFlash;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hitFlash = GetComponent<HitFlash>();
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
    }

    private void Update()
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

        Health = data.maxHp;
        Damage = (int)data.damage;
        moveSpeed = data.speed;
        attackTime = data.attackTime;
    }

    private bool isDead = false;

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        Health -= damage;
        hitFlash.Flash();

        if (Health <= 0)
        {
            isDead = true;
            Die();
        }
    }

    public void Die()
    {
        int percent = 100;
        int randomValue = Random.Range(0, percent);
        uiManager.UpdateKillCount(1);

        if (expGem != null && randomValue < 70)
        {
            Instantiate(expGem, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        if (Time.time < nextAttackTime) return;

        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.Damage(Damage);

            nextAttackTime = Time.time + attackTime;
        }
    }
}
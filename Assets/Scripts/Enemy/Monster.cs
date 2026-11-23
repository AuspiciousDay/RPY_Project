using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("몬스터 Data 연결")]
    [SerializeField]private MonsterData monsterData;

    [SerializeField]private GameObject expGem;

    public int Health = 100;
    public int Damage = 10;

    public float moveSpeed = 3f;
    public float attackTime = 0.5f;

    private Transform playerTarget;
    private SpriteRenderer spriteRenderer;

    [SerializeField]private SpriteRenderer bgSpriteRenderer;
    private float minX, maxX, minY, maxY;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }

        if(bgSpriteRenderer != null)
        {
            Bounds bounds = bgSpriteRenderer.bounds;
            minX = bounds.min.x;
            maxX = bounds.max.x;
            minY = bounds.min.y;
            maxY = bounds.max.y;
        }
    }
    
    private void Update()
    {
        if (playerTarget == null) return;

        float distance = Vector2.Distance(transform.position, playerTarget.position);
        if (distance > 0.06f)
        {
            Vector2 direction = (playerTarget.position - transform.position).normalized;
            Vector2 nextPos = transform.position + (Vector3)direction * moveSpeed * Time.deltaTime;

            if(bgSpriteRenderer != null)
            {
                nextPos.x = Mathf.Clamp(nextPos.x, minX, maxX);
                nextPos.y = Mathf.Clamp(nextPos.y, minY, maxY);
            }

            transform.position = nextPos;

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

    public void Setup(MonsterData data)
    {
        monsterData = data;

        Health = data.maxHp;
        Damage = (int)data.damage;
        moveSpeed = data.speed;
        attackTime = data.attackTime;
    }

    private bool isDead = false;

    public void TakeDamage(int damage)
    {
        if(isDead) return;

        Health -= damage;

        Debug.Log("Monster Health : " + Health);

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

        if(expGem != null && randomValue < 50)
        {
            Instantiate(expGem, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerHealth playerHealth =
            collision.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.Damage(Damage);
        }
    }
}
using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("몬스터 Data 연결")]
    [SerializeField] private MonsterData monsterData;

    public int Health = 100;
    public int Damage = 10;

    public float moveSpeed = 3f;
    public float attackTime = 0.5f;

    public void Setup(MonsterData data)
    {
        monsterData = data;

        Health = data.maxHp;
        Damage = (int)data.damage;
        moveSpeed = data.speed;
        attackTime = data.attackTime;
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;

        Debug.Log("Monster Health : " + Health);

        if (Health <= 0)
        {
            Destroy(gameObject);
        }
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
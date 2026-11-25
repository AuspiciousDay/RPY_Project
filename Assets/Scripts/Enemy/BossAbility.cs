using Unity.VisualScripting;
using UnityEngine;

public class BossAbility : MonoBehaviour
{
    [Header("발사체 설정")]
    public float speed = 6f;
    public float rotateSpeed = 100f;
    public float lifeTime = 5f;
    public int damage = 15;

    private Transform target;

    public void Init(Transform playerTarget, int SkillDamage)
    {
        target = playerTarget;
        damage = SkillDamage;

        // 공격이 나오자마자 대상을 따라감
        if (target != null)
        {
            Vector2 dir = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (target != null && rotateSpeed > 0f)
        {
            Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }

        transform.position += transform.right * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;


        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            playerHealth = collision.GetComponentInParent<PlayerHealth>();
        }
        if (playerHealth != null)
        {
            playerHealth.Damage(damage);
        }
        Destroy(gameObject);

    }
}
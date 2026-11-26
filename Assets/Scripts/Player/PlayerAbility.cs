using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    public float speed = 5f;
    public float damage = 30f;
    public int count = 0;
    public int maxCount = 1;

    private Vector2 Direction;
    private Rigidbody2D AbilityRigidbody;


    private void Awake()
    {
        AbilityRigidbody = GetComponent<Rigidbody2D>();
    }

    public void SetDirection(Vector2 direction)
    {
        Direction = direction.normalized;
    }

    private void FixedUpdate()
    {
        AbilityRigidbody.MovePosition(AbilityRigidbody.position + Direction * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
            return;
        }

        IDamageable target = collision.GetComponentInParent<IDamageable>();

        if (target != null)
        {
            target.TakeDamage(damage);
            count++;
        }

        if (count >= maxCount)
        {
            Destroy(gameObject);
        }
    }
}

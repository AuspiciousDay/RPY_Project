using UnityEngine;
using UnityEngine.Tilemaps;

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
        Monster monster = collision.GetComponent<Monster>();
        TilemapCollider2D tilemapCollider = collision.GetComponent<TilemapCollider2D>();

        Torchlight torchlight = collision.GetComponent<Torchlight>();

        if (monster != null)
        {
            monster.TakeDamage((int)damage);
            count++;
        }
        if(torchlight != null)
        {
            torchlight.TakeDamage((int)damage);
            count++;
        }
        if (tilemapCollider != null)
        {
            Destroy(gameObject);
        }
        if (count >= maxCount)
        {
            Destroy(gameObject);
        }
        
    }
}

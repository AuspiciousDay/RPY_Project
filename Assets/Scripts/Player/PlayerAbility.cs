using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerAbility : MonoBehaviour
{
    public float Speed = 5f;
    public int Damage = 10;

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
        AbilityRigidbody.MovePosition(AbilityRigidbody.position + Direction * Speed * Time.fixedDeltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Monster monster = collision.GetComponent<Monster>();
        TilemapCollider2D tilemapCollider = collision.GetComponent<TilemapCollider2D>();

        if (monster != null)
        {
            monster.TakeDamage(Damage);
            Destroy(gameObject);
        }
        if(tilemapCollider != null)
        {
            Destroy(gameObject);
        }
    }
}

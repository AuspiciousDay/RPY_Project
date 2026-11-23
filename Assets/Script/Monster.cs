using System.Data.Common;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("몬스터 Data연결")]
    [SerializeField] private MonsterData monsterData;

    public int health = 100;
    public float moveSpeed = 3f;
    public float damage = 10;
    public float attackTime = 0.5f;

    public void Setup(MonsterData data)
    {
        monsterData = data;
        health = data.maxHp;
        moveSpeed = data.speed;
        damage = data.damage;
        attackTime = data.attackTime;
    }
}

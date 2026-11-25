using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "ScriptableObject/MonsterData")]
public class MonsterData : ScriptableObject
{
    public int maxHp = 100;
    public float damage = 10f;
    public float speed = 2f;
    public float attackTime = 0.1f;
}

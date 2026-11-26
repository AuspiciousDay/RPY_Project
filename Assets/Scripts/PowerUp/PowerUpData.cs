using UnityEngine;

[CreateAssetMenu(fileName = "PowerUpData", menuName = "Shop/PowerUp Data")]
public class PowerUpData : ScriptableObject
{
    public PowerUpType type;

    public string powerUpName;

    [TextArea]
    public string description;

    public Sprite icon;

    [Header("강화 설정")]
    public int maxLevel = 5;
    public float valuePerLevel = 0.05f;

    [Header("가격 설정")]
    public int basePrice = 200;
    public float priceMultiplier = 1.5f;

    public int GetPrice(int currentLevel)
    {
        return Mathf.RoundToInt(basePrice * Mathf.Pow(priceMultiplier, currentLevel));
    }

    public float GetValue(int currentLevel)
    {
        return valuePerLevel * currentLevel;
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance { get; private set; }

    [SerializeField] private PowerUpData[] powerUpDatas;

    private readonly Dictionary<PowerUpType, int> levels = new Dictionary<PowerUpType, int>();

    private readonly Dictionary<PowerUpType, PowerUpData> datas = new Dictionary<PowerUpType, PowerUpData>();

    public event Action<PowerUpType> OnPowerUpChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        LoadPowerUps();
    }

    private void LoadPowerUps()
    {
        foreach (PowerUpData data in powerUpDatas)
        {
            if (data == null) continue;

            datas[data.type] = data;

            int level = PlayerPrefs.GetInt(GetSaveKey(data.type), 0);

            levels[data.type] = Mathf.Clamp(level, 0, data.maxLevel);
        }
    }

    public int GetLevel(PowerUpType type)
    {
        return levels.TryGetValue(type, out int level) ? level : 0;
    }

    public float GetValue(PowerUpType type)
    {
        if (!datas.TryGetValue(type, out PowerUpData data)) return 0f;

        return data.GetValue(GetLevel(type));
    }

    public int GetPrice(PowerUpType type)
    {
        if (!datas.TryGetValue(type, out PowerUpData data)) return 0;

        return data.GetPrice(GetLevel(type));
    }

    public bool IsMaxLevel(PowerUpType type)
    {
        if (!datas.TryGetValue(type, out PowerUpData data)) return false;

        return GetLevel(type) >= data.maxLevel;
    }

    public bool CanPurchase(PowerUpType type)
    {
        if (!datas.ContainsKey(type)) return false;
        if (IsMaxLevel(type)) return false;
        if (GoldManager.Instance == null) return false;

        return GoldManager.Instance.Gold >= GetPrice(type);
    }

    public bool Purchase(PowerUpType type)
    {
        if (!CanPurchase(type)) return false;

        int price = GetPrice(type);

        if (!GoldManager.Instance.SpendGold(price)) return false;

        levels[type]++;

        SavePowerUp(type);

        OnPowerUpChanged?.Invoke(type);

        return true;
    }

    private void SavePowerUp(PowerUpType type)
    {
        PlayerPrefs.SetInt(GetSaveKey(type), GetLevel(type));

        PlayerPrefs.Save();
    }

    private string GetSaveKey(PowerUpType type)
    {
        return $"PowerUp_{type}";
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            ResetPowerUps();
        }
    }

    private void ResetPowerUps()
    {
        foreach (PowerUpData data in powerUpDatas)
        {
            if (data == null) continue;

            levels[data.type] = 0;

            PlayerPrefs.DeleteKey(GetSaveKey(data.type));

            OnPowerUpChanged?.Invoke(data.type);
        }

        PlayerPrefs.Save();

        Debug.Log("모든 영구 강화 초기화");
    }
#endif
}

using System;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; }

    public int Gold { get; private set; }

    public event Action<int> OnGoldChanged;

    private const string GoldKey = "TotalGold";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Gold = PlayerPrefs.GetInt(GoldKey, 0);
    }

    public void AddGold(int amount)
    {
        if (amount <= 0) return;

        Gold += amount;

        SaveGold();

        OnGoldChanged?.Invoke(Gold);
    }

    public bool SpendGold(int amount)
    {
        if (amount <= 0 || Gold < amount) return false;

        Gold -= amount;

        SaveGold();

        OnGoldChanged?.Invoke(Gold);

        return true;
    }

    private void SaveGold()
    {
        PlayerPrefs.SetInt(GoldKey, Gold);
        PlayerPrefs.Save();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F4))
        {
            AddGold(1000);
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            Gold = 0;
            SaveGold();
            OnGoldChanged?.Invoke(Gold);
        }
    }
#endif
}

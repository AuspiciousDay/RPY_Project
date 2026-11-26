using UnityEngine;

public class DebugManager : MonoBehaviour
{
#if UNITY_EDITOR

    public static DebugManager Instance { get; private set; }

    public bool godMode;
    public bool oneHitMode;

    [SerializeField] private WeaponController[] weapons;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            godMode = !godMode;
            Debug.Log($"무적 모드: {godMode}");
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            oneHitMode = !oneHitMode;
            Debug.Log($"한방 모드: {oneHitMode}");
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            EvolveAllWeapons();
        }
    }

    private void EvolveAllWeapons()
    {
        foreach (WeaponController weapon in weapons)
        {
            if (weapon == null) continue;

            weapon.DebugEvolve();

            Debug.Log($"{weapon.upgradeName} 진화 상태: {weapon.IsEvolved}");
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

#endif
}
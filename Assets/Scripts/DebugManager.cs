using UnityEngine;

public class DebugManager : MonoBehaviour
{
#if UNITY_EDITOR

    public static DebugManager Instance { get; private set; }

    public bool godMode;
    public bool oneHitMode;

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
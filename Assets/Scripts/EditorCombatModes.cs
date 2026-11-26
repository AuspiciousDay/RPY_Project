#if UNITY_EDITOR
using UnityEditor;

internal static class EditorCombatModes
{
    private const string InvincibleMenu = "Tools/Combat Debug/Invincible";
    private const string OneHitKillMenu = "Tools/Combat Debug/One Hit Kill";
    private const string InvincibleKey = "RPY.EditorCombat.Invincible";
    private const string OneHitKillKey = "RPY.EditorCombat.OneHitKill";

    internal static bool Invincible => SessionState.GetBool(InvincibleKey, false);
    internal static bool OneHitKill => SessionState.GetBool(OneHitKillKey, false);

    [MenuItem(InvincibleMenu)]
    private static void ToggleInvincible()
    {
        SessionState.SetBool(InvincibleKey, !Invincible);
    }

    [MenuItem(InvincibleMenu, true)]
    private static bool ValidateInvincible()
    {
        Menu.SetChecked(InvincibleMenu, Invincible);
        return true;
    }

    [MenuItem(OneHitKillMenu)]
    private static void ToggleOneHitKill()
    {
        SessionState.SetBool(OneHitKillKey, !OneHitKill);
    }

    [MenuItem(OneHitKillMenu, true)]
    private static bool ValidateOneHitKill()
    {
        Menu.SetChecked(OneHitKillMenu, OneHitKill);
        return true;
    }
}
#endif

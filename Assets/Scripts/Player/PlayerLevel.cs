using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    public UiManager uiManager;
    public float exp;
    public int level;
    public float ExpMultiplier = 1f;

    public PassiveUpgradeUI passiveUpgradeUI;
    [SerializeField, Min(1)] private int startingLevelExperience = 50;
    [SerializeField, Min(0)] private int experienceGrowthPerLevel = 50;

    private int ExperienceToNextLevel => startingLevelExperience + (level - 1) * experienceGrowthPerLevel;

    private void Start()
    {
        exp = 0;
        level = 1;
    }

    private void Update()
    {
        if (Time.timeScale <= 0f || (uiManager != null && uiManager.IsRunOver)) return;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameAudio.Play("gem_large_pickup", 0.6f, 0.07f);
            GainExp(50);
            return;
        }
#endif

        if (exp >= ExperienceToNextLevel)
            CheckLevelUp();
    }
    public void GainExp(float amount)
    {
        exp += Mathf.RoundToInt(amount * ExpMultiplier);
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        int requiredExperience = ExperienceToNextLevel;
        uiManager.UpdateExp(exp, requiredExperience, level);
        if (Time.timeScale <= 0f || uiManager.IsRunOver) return;

        if (exp >= requiredExperience)
        {
            exp -= requiredExperience;
            level++;
            uiManager.UpdateExp(exp, ExperienceToNextLevel, level);
            GameAudio.Play("level_up");
            passiveUpgradeUI.Open(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (uiManager != null && uiManager.IsRunOver) return;

        MagneticPickup pickup = collision.GetComponent<MagneticPickup>();
        if (pickup != null)
        {
            pickup.SetTarget(this);
        }
        else if (collision.gameObject.CompareTag("Gem50"))
        {
            GameAudio.Play("gem_large_pickup", 0.6f, 0.07f);
            GainExp(50);
            Destroy(collision.gameObject);
        }
    }
}

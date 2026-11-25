
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    public UiManager uiManager;
    public float exp;
    public int level;

    public WeaponUpgrade weaponUpgrade;

    // Garlic 연결
    public Garlic garlic;

    // 레벨업마다 증가할 범위
    public float RangeIncreasePerLevel = 0.2f;

    private void Start()
    {
        exp = 0;
        level = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            GainExp(10);
        }
    }

    public void GainExp(float amount)
    {
        exp += amount;
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        uiManager.UpdateExp(exp, level * 50, level);

        if (exp >= level * 50)
        {
            exp -= level * 50;
            level++;

            // Garlic 범위 증가
            if (garlic != null)
            {
                garlic.IncreaseRange(RangeIncreasePerLevel);
            }

            uiManager.UpdateExp(exp, level * 50, level);

            GameManager.PauseGame(0f);
            weaponUpgrade.gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Gem10"))
        {
            GainExp(10);
            Destroy(collision.gameObject);

            Debug.Log("Player Level : " + level + ", Exp : " + exp);
        }
        else if (collision.gameObject.CompareTag("Gem30"))
        {
            GainExp(30);
            Destroy(collision.gameObject);

            Debug.Log("Player Level : " + level + ", Exp : " + exp);
        }
    }
}

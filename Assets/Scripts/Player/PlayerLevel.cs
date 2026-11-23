using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    public UiManager uiManager;
    public float exp;
    public int level;

    public WeaponUpgrade weaponUpgrade;

    private void Start()
    {
        exp = 0;
        level = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // test code
        {
            GainExp(90);
        }

    }
    public void GainExp(float amount)
    {
        exp += amount;
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        uiManager.UpdateExp(exp, level * 100, level);
        if (exp >= level * 100)
        {
            exp -= level * 100;
            level++;
            uiManager.UpdateExp(exp, level * 100, level);
            GameManager.PauseGame();
            weaponUpgrade.gameObject.SetActive(true);
        }
    }
}

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
            GameManager.PauseGame(0f);
            weaponUpgrade.gameObject.SetActive(true);
        }
    }

    // private void OnCollisionEnter2D(Collision2D collision)
    // {

    //     if (collision.gameObject.CompareTag("Gem10"))
    //     {
    //         GainExp(10);
    //         Destroy(collision.gameObject);
    //         Debug.Log("Player Level : " + level + ", Exp : " + exp);
    //     }
    //     else if (collision.gameObject.CompareTag("Gem30"))
    //     {
    //         GainExp(30);
    //         Destroy(collision.gameObject);
    //         Debug.Log("Player Level : " + level + ", Exp : " + exp);
    //     }
    // }
}

using UnityEngine;

public class VineHealth : MonoBehaviour
{
    [SerializeField] int maxHealth = 50;

    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log(
            "Vines took "
            + damage
            + " damage. HP: "
            + currentHealth
        );

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
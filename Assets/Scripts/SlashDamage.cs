using UnityEngine;

public class SlashDamage : MonoBehaviour
{
    [SerializeField] int damage = 25;

    private bool hasHit = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasHit)
            return;

        // Tic-Tac-Toe: one slash selects one square.
        TicTacToeCell cell = other.GetComponentInParent<TicTacToeCell>();

        if (cell != null)
        {
            Debug.Log("SLASH HIT CELL: " + cell.gameObject.name);

            hasHit = true;
            cell.TrySelect();

            Destroy(gameObject);
            return;
        }

        // Ghost damage.
        ghostHealth ghost = other.GetComponentInParent<ghostHealth>();

        if (ghost != null)
        {
            hasHit = true;
            ghost.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Vine damage.
        VineHealth vines = other.GetComponentInParent<VineHealth>();

        if (vines != null)
        {
            hasHit = true;
            vines.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
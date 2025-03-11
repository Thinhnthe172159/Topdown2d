using Pathfinding;
using UnityEngine;

public class EnemyRangeAttack : MonoBehaviour
{
    public AIPath aiPath; // Tham chiếu đến AIPath của Enemy
    private bool playerInRange = false;

    void Start()
    {
        aiPath.canMove = false; // Mặc định không cho Enemy di chuyển
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            aiPath.canMove = true; // Khi Player vào phạm vi -> Cho phép di chuyển
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            aiPath.canMove = false; // Khi Player rời phạm vi -> Dừng di chuyển
        }
    }
}


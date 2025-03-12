using UnityEngine;

public class AttackRange : MonoBehaviour
{
    public bool playerInRange = false; // Biến kiểm tra Player có trong vùng hay không

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;  // Khi Player vào tầm
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false; // Khi Player rời khỏi tầm
        }
    }
}

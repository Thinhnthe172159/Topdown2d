using UnityEngine;
using System.Collections;
public class RespawnManager : MonoBehaviour
{
    public static RespawnManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    //public IEnumerator RespawnPlayer(GameObject player, Animator animator, HealthBar healthBar)
    //{
    //    animator.SetTrigger("isDead");
    //    yield return new WaitForSeconds(3f);

    //    player.SetActive(false);
    //    yield return new WaitForSeconds(3f);

    //    player.SetActive(true);
    //    animator.SetTrigger("Revive");

    //    var playerHealth = player.GetComponent<PlayerHealth>();
    //    playerHealth.SetHealth(100);
    //    healthBar.SetHealth(100);
    //}
}

using UnityEngine;

public class HydraObstacle : MonoBehaviour {
    public int damage;

    public void AutoDestroy() {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            collision.gameObject.GetComponent<PlayerHealth>().decreaseHealth(damage);
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            rb.linearVelocity = new Vector2(-rb.linearVelocity.x, -rb.linearVelocity.y);
        }
    }

    public void PlaySound() {
        SoundManager.PlaySound("Obstacle");
    }
}

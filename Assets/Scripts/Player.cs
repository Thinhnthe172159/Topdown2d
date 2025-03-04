using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        MovePlayer();
    }
    void MovePlayer()
    {
        Vector2 playerInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        rb.linearVelocity = playerInput * moveSpeed;
        if(playerInput.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (playerInput.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }   
    
}

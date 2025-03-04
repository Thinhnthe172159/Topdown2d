using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float JumpForce = 15f;
    [SerializeField] private LayerMask GroundLayer;
   // [SerializeField] private Transform GroundCheck;
    private Animator animator;
    private bool isGround;

    private Rigidbody2D rb;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleJump();
        UpdateAnimator();
    }


    //xử lý di chuyển
    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");
        float moveInputUpDown = Input.GetAxis("Vertical");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, moveInputUpDown * moveSpeed);
        if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    //xử lý nhảy
    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGround)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, JumpForce);
        }
        //isGround = Physics2D.OverlapCircle(GroundCheck.position, 0.2f, GroundLayer);
    }

    private void UpdateAnimator()
    {
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.05f;
        bool isJumping = !isGround;
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumping", isJumping);
    }
}

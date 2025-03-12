using UnityEngine;

public class Minotaur_Walk : StateMachineBehaviour
{
    public float speed;

    private GameObject minotaur;
    private Transform player;
    private Rigidbody2D rb;
    private AttackRange detection;
    private Vector2 target;
    private float oldTime;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.Find("Player").transform;
        minotaur = animator.transform.parent.gameObject;
        rb = minotaur.GetComponent<Rigidbody2D>();
        detection = minotaur.GetComponentInChildren<AttackRange>(); // Lấy AttackRange

        // Chỉ đặt target nếu Player đã vào vùng tấn công
        if (detection.playerInRange)
        {
            target = new Vector2(player.position.x, player.position.y);
            oldTime = Time.time;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!detection.playerInRange)
        {
            rb.linearVelocity = Vector2.zero; // Không di chuyển nếu Player chưa vào vùng tấn công
            return;
        }

        // Tiếp tục di chuyển về phía Player
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        // Chuyển sang trạng thái Attack nếu chạm tới mục tiêu hoặc di chuyển quá 3 giây
        if (rb.position == target || Mathf.Abs(oldTime - Time.time) > 3)
        {
            animator.SetBool("Attacking", true);
        }
    }
}

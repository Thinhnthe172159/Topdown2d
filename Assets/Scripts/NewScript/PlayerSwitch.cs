using UnityEngine;

public class PlayerSwitch : MonoBehaviour
{
    public PlayerMovement playerController;
    public PlayerMovement player2Controller;
    public bool player1Active = true;
    public CameraMovement cameraMovement;

    void Start()
    {
        // Đảm bảo khi game bắt đầu chỉ nhân vật được điều khiển được hiển thị
        if (player1Active)
        {
            playerController.gameObject.SetActive(true);
            player2Controller.gameObject.SetActive(false);
        }
        else
        {
            playerController.gameObject.SetActive(false);
            player2Controller.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchPlayer();
        }
    }

    public void SwitchPlayer()
    {
        if (player1Active)
        {
            // Lưu vị trí của nhân vật hiện đang điều khiển (Player 1)
            Vector2 currentPosition = playerController.transform.position;

            // Dừng chuyển động của Player 1
            playerController.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            // Vô hiệu hóa điều khiển và ẩn Player 1
            playerController.enabled = false;
            playerController.gameObject.SetActive(false);

            // Đưa Player 2 đến vị trí của Player 1
            player2Controller.transform.position = currentPosition;
            // Hiển thị và kích hoạt điều khiển cho Player 2
            player2Controller.gameObject.SetActive(true);
            player2Controller.enabled = true;

            player1Active = false;
            cameraMovement.SetTarget(player2Controller.gameObject);
        }
        else
        {
            // Lưu vị trí của nhân vật hiện đang điều khiển (Player 2)
            Vector2 currentPosition = player2Controller.transform.position;

            // Dừng chuyển động của Player 2
            player2Controller.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            // Vô hiệu hóa điều khiển và ẩn Player 2
            player2Controller.enabled = false;
            player2Controller.gameObject.SetActive(false);

            // Đưa Player 1 đến vị trí của Player 2
            playerController.transform.position = currentPosition;
            // Hiển thị và kích hoạt điều khiển cho Player 1
            playerController.gameObject.SetActive(true);
            playerController.enabled = true;

            player1Active = true;
            cameraMovement.SetTarget(playerController.gameObject);
        }
    }
}

using UnityEngine;

public class PlayerSwitch : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public PlayerMovement playerController;
    public PlayerMovement player2Controller;
    public bool player1Active = true;
    public CameraMovement cameraMovement;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwicthPlayer();
        }
    }

    public void SwicthPlayer()
    {
        if (player1Active)
        {
            playerController.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero; // D?ng Player 1
            playerController.enabled = false;
            player2Controller.enabled = true;
            player1Active = false;
            cameraMovement.SetTarget(player2Controller.gameObject);
        }
        else
        {
            player2Controller.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero; // D?ng Player 2
            playerController.enabled = true;
            player2Controller.enabled = false;
            player1Active = true;
            cameraMovement.SetTarget(playerController.gameObject);
        }
    }
}


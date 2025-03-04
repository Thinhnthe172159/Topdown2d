using UnityEngine;

public class CameraMovement : MonoBehaviour {
    public GameObject player;
    private Vector3 playerPosition;

    void Start() {
       // player = GameObject.Find("Player");
    }

    void FixedUpdate() {
        playerPosition = player.transform.position;
        playerPosition.z = -10;
        transform.position = playerPosition;
    }

    public void SetTarget(GameObject newTarget)
    {
        player = newTarget;
    }
}

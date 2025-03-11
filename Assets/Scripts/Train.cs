using System;
using UnityEngine;

public class Train : MonoBehaviour
{
    public float speed;
    public float timeDuration = 60f;

    private float countDown = 0f;
    private bool isCountingDown = false;
    private Vector3 startPosition = new Vector3(191.8f, -72.43f, 0f);
    private Vector3 destroyPoint = new Vector3(-118.2f, -72.43f, 0f);

    void Update()
    {
        if (!isCountingDown)
        {
            // Di chuyển tàu sang trái
            transform.position += Vector3.left * speed * Time.deltaTime;

            // Nếu tàu chạm điểm kết thúc, bắt đầu đếm ngược
            if (transform.position.x <= destroyPoint.x)
            {
                isCountingDown = true;
                countDown = 0f;
            }
        }
        else
        {
            countDown += Time.deltaTime;

            // Khi hết thời gian chờ, đặt lại vị trí tàu
            if (countDown >= timeDuration)
            {
                transform.position = startPosition;
                isCountingDown = false; // Reset trạng thái để tàu chạy tiếp
            }
        }
    }
}

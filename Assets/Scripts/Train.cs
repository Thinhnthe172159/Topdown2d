using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Train : MonoBehaviour
{
    public GameObject trainPrefab;
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
            transform.position += Vector3.left * speed * Time.deltaTime;
            if (transform.position.x <= destroyPoint.x)
            {
                isCountingDown = true;
                countDown = 0f;
            }
        }
        else
        {
            countDown += Time.deltaTime;
            if (countDown >= timeDuration)
            {
                Instantiate(trainPrefab, startPosition, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}

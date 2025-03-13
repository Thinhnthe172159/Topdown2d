﻿using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Movement attributes
    private Vector3 movementSpeed;
    private float speedX;
    private float speedY;
    public PlayerAim playerAim; // Kéo thả PlayerAim từ Inspector
    private SpriteRenderer spriteRenderer;

    public GameObject dust;
    private bool canDust;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        InvokeRepeating("PlaySound", 0.0f, Random.Range(0.25f, 0.45f));
        canDust = true;
    }

    void Update()
    {
        Movement();
    }

    void PlaySound()
    {
        if (Mathf.Abs(speedX) > 0 || Mathf.Abs(speedY) > 0)
            SoundManager.PlaySound("Steps");
    }

    // Player movement
    void Movement()
    {
        float movementFactor = 10f;
        float speedX = Input.GetAxis("Horizontal") * movementFactor;
        float speedY = Input.GetAxis("Vertical") * movementFactor;
        Vector3 movementSpeed = new Vector3(speedX, speedY, 0f);

        if (movementSpeed.magnitude > 0 && canDust)
            StartCoroutine(WaitToDust());

        GetComponent<Rigidbody2D>().linearVelocity = movementSpeed;

        Animator animator = GetComponent<Animator>();
        animator.SetBool("isMoving", speedX != 0 || speedY != 0);

        // **Lật nhân vật theo hướng Aim Point**
        if (playerAim != null)
        {
            float aimDirection = Mathf.Cos(playerAim.AngleOfAim * Mathf.Deg2Rad);
            spriteRenderer.flipX = aimDirection < 0; // Nếu Aim Point bên trái thì lật sprite
        }
    }

    private IEnumerator WaitToDust()
    {
        canDust = !canDust;
        yield return new WaitForSeconds(Random.Range(0.15f, 0.25f));
        canDust = !canDust;
        if (movementSpeed.magnitude > 0)
            Instantiate(dust, transform.Find("Shadow").transform.position, Quaternion.identity);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{

    [SerializeField] float movementPower = 5f;

    Rigidbody2D rb;

    // left: -1, right: 1
    int direction = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (transform.position.x > 0)
        {
            direction = -1;
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(movementPower * direction, 0);
    }


}

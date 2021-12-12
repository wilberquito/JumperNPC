using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orientation : MonoBehaviour
{
    Rigidbody2D rb2d;
    // [SerializeField] Transform body;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(rb2d.velocity.x) <= Mathf.Epsilon) return;

        if (rb2d.velocity.x > 0)
        {
            transform.rotation = Quaternion.Euler(Vector3.up * 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(Vector3.up * 180);
        }
    }
}
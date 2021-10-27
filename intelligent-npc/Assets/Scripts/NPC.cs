using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPC : MonoBehaviour
{

    Rigidbody2D _rigidbody2D;

    [SerializeField] float _movingRange = 5f;
    [SerializeField] float velocity = 4f;

    public float movingRange
    {
        get
        {
            return _movingRange;
        }
    }

    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        _rigidbody2D.velocity = new Vector2(0, 0);
    }


    void FixedUpdate()
    {
        var force = Input.GetAxis("Horizontal");
        _rigidbody2D.velocity = new Vector2(force * velocity, 0);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Max Max Max");
    }




}

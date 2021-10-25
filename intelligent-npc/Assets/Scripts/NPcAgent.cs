using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPcAgent : MonoBehaviour
{
   
    Rigidbody2D _rigidbody2D;

    [SerializeField] float _movingRange = 5f;

    public float movingRange {
        get {
            return _movingRange;
        }
    }


    void Awake() {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Start() {
        _rigidbody2D.velocity = new Vector2(1f, 0);
    }



}

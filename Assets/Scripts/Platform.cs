using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    // Start is called before the first frame update

    Collider2D c2d;

    void Start()
    {
        c2d = GetComponent<Collider2D>();
    }

    public Vector3 Size()
    {
        return c2d.bounds.size;
    }

    public Vector2 Position()
    {
        return transform.position;
    }

}

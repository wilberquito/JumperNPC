using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteScale : MonoBehaviour
{
    Rigidbody2D rb2d;
    [SerializeField] Transform body;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!body)
        {
            Debug.LogError("No body linked");
            return;
        }

        var scale = body.transform.localScale;

        if (rb2d.velocity.x >= 0 && scale.x < 0)
        {
            Debug.Log("1");
            body.transform.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
        }
        else if (rb2d.velocity.x < 0 && scale.x >= 0)
        {
            Debug.Log("2");
            body.transform.localScale = new Vector3(scale.x * -1, scale.y, scale.z);
        }
    }
}

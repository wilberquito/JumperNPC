using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limit : MonoBehaviour
{
    [SerializeField] Color inColor;

    SpriteRenderer _renderer;

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void Restart()
    {
        _renderer.color = Color.white;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        this._renderer.color = new Color(inColor.r, inColor.g, inColor.b);
    }

    // is one of the limits and the agent is outside of them
    private void OnTriggerExit2D(Collider2D other)
    {
        Restart();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limit : MonoBehaviour
{
    [SerializeField] Color outColor;

    SpriteRenderer _renderer;

    public void Restart()
    {
        _renderer.color = Color.white;
    }

    // is one of the limits and the agent is outside of them
    private void OnTriggerExit2D(Collider2D other)
    {
        this._renderer.color = new Color(outColor.r, outColor.g, outColor.b);
    }
}

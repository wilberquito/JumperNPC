using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchor : MonoBehaviour
{

    // referencia a la posicion inicial del Agent
    Vector3 originNPC;

    void Start()
    {
        var agent = transform.parent.Find("Agent");
        if (agent && agent.GetComponent<NPC>())
        {
            originNPC = transform.parent.position;
        }
        else
        {
            Debug.LogError("NPC not found");
        }
    }

    private void OnDrawGizmos()
    {
        // Gizmos.DrawWireSphere(originNPC, range);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoArea : MonoBehaviour
{

    float radius = 5f;


    // referencia a la posicion inicial del Agent
    Vector3 startParentPosition;


    private void Awake()
    {
        startParentPosition = transform.parent.position;
    }

    void Start() {
        radius = transform.parent.gameObject.GetComponent<NPcAgent>().movingRange;
    }

    void Update() {
        transform.position = startParentPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

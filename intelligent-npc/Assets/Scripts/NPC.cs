using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class NPC : Agent
{

    [SerializeField] float _movingRange = 5f;
    [SerializeField] float velocity = 4f;
    [SerializeField] bool trainningMode = false;


    Rigidbody2D _rigidbody2D;
    float gainObtein = 0f;

    public float movingRange
    {
        get
        {
            return _movingRange;
        }
    }

    public override void Initialize()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();

        // infinite steps for session
        if (!trainningMode) MaxStep = 0;

        Debug.Log("Initialize...");
    }


    public override void OnEpisodeBegin()
    {
        Debug.Log("Episode begin...");

        // reset gain obtein
        gainObtein = 0;

        // reseting movement inercy
        _rigidbody2D.velocity = Vector2.zero;

        // finding the moving target
        FindMovingTarget();
    }

    private void FindMovingTarget()
    {
        Debug.Log("searching for moving target");
        // I will search on parent childs for an object with tarjet 
        // LeftTarget or RightTarget

        Transform tr = transform.parent.transform;
        // the lenght of this target before the loop should be 2
        List<Transform> targets = new List<Transform>();

        foreach (Transform child in tr)
        {
            if (child.tag == "LeftTarget" || child.tag == "RightTarget")
            {
                targets.Add(child);
            }
        }


        foreach (Transform target in targets)
        {
            Debug.Log(target.name);
        }

    }

    // void Awake()
    // {
    //     _rigidbody2D = GetComponent<Rigidbody2D>();
    // }

    // void Start()
    // {
    //  Debug.Log("start");
    //     // _rigidbody2D.velocity = new Vector2(0, 0);
    // }


    // void FixedUpdate()
    // {
    //     Debug.Log();

    //     // var force = Input.GetAxis("Horizontal");
    //     // _rigidbody2D.velocity = new Vector2(force * velocity, 0);
    // }

    // private void OnTriggerEnter2D(Collider2D other) {
    //     Debug.Log("Max Max Max");
    // }



}

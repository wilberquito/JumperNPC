using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class NPC : Agent
{
    [SerializeField] float _movingRange = 5f;
    [SerializeField] bool trainningMode = false;
    [SerializeField] float movementForce = 2f;
    [SerializeField] Transform currentTarget;

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

        // Debug.Log("Initialize...");
    }

    public override void OnEpisodeBegin()
    {
        // Debug.Log("Episode begin...");

        // reset gain obtein
        gainObtein = 0;

        // reseting movement inercy
        _rigidbody2D.velocity = Vector2.zero;

        // changin randomnes
        Random.InitState(System.DateTime.Now.Millisecond);

        // finding the moving target
        FindMovingTarget();
    }

    private void FindMovingTarget()
    {
        // Debug.Log("searching for moving target");
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

        // once found, pick one randomly
        // the array should be of length 2
        this.currentTarget = targets[Random.Range(0, 2)];
    }

    // called when action is received from either {player, neural network}
    // each buffer position refers to an action, I decide what it means for each positions
    // inside that structure has continuos and continuos actions
    // index 0: -1 means move to the left, +1 means move to the right
    public override void OnActionReceived(ActionBuffers actions)
    {
        // Debug.Log("from on action received");
        // Debug.Log(actions.ContinuousActions[0]);
        Vector2 movement = new Vector2(actions.ContinuousActions[0]*movementForce, 0);
        _rigidbody2D.velocity = movement;
    }

    // I observe the horientation of the NPC to the nereast target
    // also I inform about the distace from the NPC to nearest target
    public override void CollectObservations(VectorSensor sensor)
    {

        if (!currentTarget) return;

        Vector2 toTarget = currentTarget.position - transform.position;
        // 2 observations (horientation)
        sensor.AddObservation(toTarget.normalized);
        // 1 observation (distance)
        sensor.AddObservation(Vector2.Distance(currentTarget.position, transform.position));
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuosActions = actionsOut.ContinuousActions;
        var force = Input.GetAxis("Horizontal");
        // Debug.Log("from Heuristic");

        // Debug.Log(force);

        continuosActions[0] = force;
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
    //     var force = Input.GetAxis("Horizontal");
    //     _rigidbody2D.velocity = new Vector2(force * velocity, 0);
    // }

    // private void OnTriggerEnter2D(Collider2D other) {
    //     Debug.Log("Max Max Max");
    // }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class NPC : Agent
{
    [SerializeField] bool trainningMode = false;
    [SerializeField] float movementForce = 8f;
    // how much ml agent wins every time it touches the current target
    [SerializeField] float gainTouchBarTarget = 1f;
    [SerializeField] Transform outTarget;

    List<Transform> barsTarget;
    Transform currentTarget;
    Rigidbody2D _rigidbody2D;
    float gain = 0f;

    public override void Initialize()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        // infinite steps for session
        if (!trainningMode) MaxStep = 0;
    }

    public override void OnEpisodeBegin()
    {
        // reseting movement inercy
        _rigidbody2D.velocity = Vector2.zero;
        //reseting positions
        transform.position = transform.parent.position;
        // changin randomnes
        Random.InitState(System.DateTime.Now.Millisecond);
        // finding the moving target
        FindMovingTarget();
    }


    // this methods sets the start target
    // and obtain those bars that works as movement limitations
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
        this.outTarget = currentTarget;
        this.barsTarget = targets;
    }

    // called when action is received from either {player, neural network}
    // each buffer position refers to an action, I decide what it means for each positions
    // inside that structure has continuos and discrete actions
    // index 0: -1 means move to the left, +1 means move to the right
    // the cool thing about the neural network, is that it figurates it all automatic
    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector2 movement = new Vector2(actions.ContinuousActions[0] * movementForce, 0);
        _rigidbody2D.velocity = movement;
    }

    // Should include all variables relevant for following 
    // to take the agent the optimally informed desition.
    // No extraneous information here please
    public override void CollectObservations(VectorSensor sensor)
    {
        if (!currentTarget) return;

        Vector2 currentPos = new Vector2(transform.position.x, 0);
        Vector2 targetPos = new Vector2(currentTarget.position.x, 0);

        Vector2 toTarget = targetPos - currentPos;
        // 2 observations (horientation)
        sensor.AddObservation(toTarget.normalized);
        // 1 observation (distance)
        sensor.AddObservation(Vector2.Distance(targetPos, currentPos));
        // 2 observations for current target position
        sensor.AddObservation(targetPos);
        // 2 observations for movement velocity
        sensor.AddObservation(_rigidbody2D.velocity);
        // Note: curiosamente si normalizo la velocidad, le cuesta mucho aprenderx
    }

    // this method allows me to interact with the game
    // when the ml agents is not set to trainning
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuosActions = actionsOut.ContinuousActions;
        var force = Input.GetAxis("Horizontal");
        continuosActions[0] = force;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // check if agent is colliding with range movement bars
        // especific if its touching the current target
        // Note: everything diff from current target should not count
        if (other.transform == this.currentTarget)
        {
            // iff we are in training mode
            if (trainningMode)
            {
                // add reward is method from MLAgents class
                AddReward(gainTouchBarTarget);
            }
            ChangeCurrentTarget();
        }
    }

    // Returns the vector distance from current position to target
    private Vector2 ToTarget()
    {
        // Vector2 currentPos = new Vector2(transform.position.x, 0);
        // Vector2 targetPos = new Vector2(currentTarget.position.x, 0);
        return currentTarget.position - transform.position;
    }

    private void FixedUpdate()
    {
            // // giving rewards if the horientation of the 
            // // vector velocity if the correct horientation
            // if (trainningMode && this.currentTarget)
            // {
            //     AddReward(Vector2.Dot(ToTarget().normalized, _rigidbody2D.velocity.normalized) * (gainTouchBarTarget / 4));
            // }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Vector2 currentPos = new Vector2(transform.position.x, 0);
        Vector2 targetPos = new Vector2(currentTarget.position.x, 0);
        Vector2 toTarget = targetPos - currentPos;
        var horientation = Vector2.Dot(toTarget.normalized, _rigidbody2D.velocity.normalized);

        if (trainningMode && horientation <= 0)
        {
            AddReward(-gainTouchBarTarget * 4);
            EndEpisode();
        }
    }


    // this method is thought to change the current target
    // basically it change the current target bar
    // maybe latter this method also will pick and enemy as possible target
    // Note: this function is not pure and modify the state of `currentTarget`
    private void ChangeCurrentTarget()
    {
        // changing current bar target for now...
        foreach (Transform target in barsTarget)
        {
            if (target != currentTarget)
            {
                currentTarget = target;
                outTarget = currentTarget;
                break;
            }
        }
    }


}

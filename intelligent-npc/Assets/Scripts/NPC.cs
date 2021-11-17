using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

// TODO: rename tags as limitleft and limitright

public class NPC : Agent
{
    [SerializeField] bool trainningMode = false;
    [SerializeField] float movementForce = 8f;
    // how much ml agent wins every time it touches the current target
    [SerializeField] float gainTouchBarTarget = 1f;
    List<Transform> barsTarget = new List<Transform>();
    GameObject currentTarget;

    Rigidbody2D _rigidbody2D;

    private GameObject target
    {
        get => currentTarget;
        set
        {
            currentTarget = value;
        }
    }

    public override void Initialize()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        // infinite steps for session
        if (!trainningMode) MaxStep = 0;

        CatchLimits();
    }

    public override void OnEpisodeBegin()
    {
        // reseting movement inercy
        _rigidbody2D.velocity = Vector2.zero;
        //reseting positions
        transform.position = transform.parent.position;
        // changin randomnes
        Random.InitState(System.DateTime.Now.Millisecond);
        // current target rebooted
        target = null;
        // finding the moving target
        PickOneLimitAsTarget();
    }

    // sets the left and the right limit
    private void CatchLimits()
    {
        // the length of barTarget should be 2
        // in first position should be the left and in second position should be the right limit
        foreach (Transform child in transform.parent.transform)
        {
            if (child.tag == "LeftTarget")
            {
                this.barsTarget.Add(child);
                break;
            }
        }
        foreach (Transform child in transform.parent.transform)
        {
            if (child.tag == "RightTarget")
            {
                this.barsTarget.Add(child);
                break;
            }
        }
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
        if (!target) return;

        Vector2 currentPos = new Vector2(transform.position.x, 0);
        Vector2 targetPos = new Vector2(target.transform.position.x, 0);

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
        if (other.transform == target)
        {
            // iff we are in training mode
            if (trainningMode)
            {
                // add reward is method from MLAgents class
                AddReward(gainTouchBarTarget);
            }
            PickOneLimitAsTarget();
        }
    }

    // Returns the vector distance from current position to target
    private Vector2 ToTarget()
    {
        // Vector2 currentPos = new Vector2(transform.position.x, 0);
        // Vector2 targetPos = new Vector2(currentTarget.position.x, 0);
        return target.transform.position - transform.position;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Vector2 currentPos = new Vector2(transform.position.x, 0);
        Vector2 targetPos = new Vector2(target.transform.position.x, 0);
        Vector2 toTarget = targetPos - currentPos;
        var horientation = Vector2.Dot(toTarget.normalized, _rigidbody2D.velocity.normalized);

        if (trainningMode && horientation <= 0)
        {
            AddReward(-gainTouchBarTarget * 4);
            EndEpisode();
        }
    }

    // private void FixedUpdate()
    // {
    //     RayPerceptionSensorComponent2D[] sensors = GetComponentsInChildren<RayPerceptionSensorComponent2D>();

    //     foreach (RayPerceptionSensorComponent2D sensor in sensors)
    //     {
    //         var rays = sensor.RaySensor.RayPerceptionOutput.RayOutputs;
    //         foreach (RayPerceptionOutput.RayOutput ray in rays)
    //         {
    //             if (ray.HasHit)
    //             {
    //                 // ChangeCurrentTarget(ray.HitGameObject);
    //                 return;
    //             }
    //         }
    //     }
    // }

    // There should be two bars
    // when this method is called, it should 
    // take the other bar as target if it is defined
    // otherwise, it should select the nearest
    private void PickOneLimitAsTarget()
    {
        // if no target setted, pick one of limits randomnly as target
        if (!target)
        {
            target = this.barsTarget[Random.Range(0, 2)].gameObject;
        }
        else
        {
            // picking the opposite limit
            foreach (Transform limit in this.barsTarget)
            {
                if (target != limit)
                {
                    target = limit.gameObject;
                    break;
                }
            }
        }
    }
}

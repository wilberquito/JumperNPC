using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;


public class NPC : Agent
{
    [SerializeField] bool trainningMode = false;

    [SerializeField] float movementPower = 8f;

    // how much ml agent wins every time it touches the current target
    [SerializeField] float gain = 1f;

    [SerializeField] GameObject leftLimit;

    [SerializeField] GameObject rightLimit;

    [Header("Avoid collision with same gameobject or other")]
    [SerializeField] float distToGround = 0.36f;

    [Header("Defines how much power the npc has jumping")]
    [SerializeField] float jumpPower;

    [SerializeField] float attackModeDuration = 1f;

    GameObject currentTarget;

    Rigidbody2D rb;

    bool attackMode = false;


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
        rb = GetComponent<Rigidbody2D>();
        // infinite steps for session
        if (!trainningMode) MaxStep = 0;
    }

    public override void OnEpisodeBegin()
    {
        // reseting movement inercy
        rb.velocity = Vector2.zero;
        //reseting positions
        transform.position = transform.parent.position;
        // changin randomnes
        Random.InitState(System.DateTime.Now.Millisecond);
        // current target rebooted
        target = null;
        // setting attack mode to false`
        attackMode = false;

        // finding the moving target
        PickOneLimitAsTarget();
    }

    // called when action is received from either {player, neural network}
    // each buffer position refers to an action, I decide what it means for each positions
    // inside that structure has continuos and discrete actions
    // index 0: -1 means move to the left, +1 means move to the right
    // the cool thing about the neural network, is that it figurates it all automatic

    // the second element is discrete parameter, which says jump or not [0,1]
    // when npc jumps has the hability to make damage and only should jump when there is a enemy nearby
    public override void OnActionReceived(ActionBuffers actions)
    {
        int jump = actions.DiscreteActions[0];
        float horizontal = actions.ContinuousActions[0];
        Vector2 v = rb.velocity;

        // run attack mode
        if (jump == 1 && !attackMode)
        {
            StartCoroutine(AttackModeCoroutine());
        }

        Vector2 movement = new Vector2(horizontal * movementPower, jump == 1 ? jump * jumpPower : v.y);
        rb.velocity = movement;
    }

    // this routine update the state of attack mode after the yield to avoid
    // agent to jump all the time in case there is an enemy nearby
    private IEnumerator AttackModeCoroutine()
    {
        Debug.Log("To attack mode...");
        attackMode = true;
        yield return new WaitForSeconds(attackModeDuration);
        Debug.Log("To normal mode...");
        attackMode = false;
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
        sensor.AddObservation(rb.velocity);
        // Note: curiosamente si normalizo la velocidad, le cuesta mucho aprenderx
    }

    // this method allows me to interact with the game
    // when the ml agents is not set to trainning
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuosActions = actionsOut.ContinuousActions;
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        discreteActions[0] = Input.GetKey(KeyCode.Space) && IsGrounded() ? 1 : 0;
        continuosActions[0] = Input.GetAxis("Horizontal");
    }

    // two types of collision are thought
    // 1. collision with movement limits
    // 2. possible enemy
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger");
        Debug.Log(other.gameObject);
        // check if agent is colliding with range movement bars
        // especific if its touching the current target
        // Note: everything diff from current target should not count
        if (other.transform == target)
        {
            // iff we are in training mode
            if (trainningMode)
            {
                AddReward(gain);
            }
            PickOneLimitAsTarget();
        }
    }

    // // Returns the vector distance from current position to target
    // private Vector2 ToTarget()
    // {
    //     // Vector2 currentPos = new Vector2(transform.position.x, 0);
    //     // Vector2 targetPos = new Vector2(currentTarget.position.x, 0);
    //     return target.transform.position - transform.position;
    // }

    private void OnTriggerExit2D(Collider2D other)
    {
        Vector2 currentPos = new Vector2(transform.position.x, 0);
        Vector2 targetPos = new Vector2(target.transform.position.x, 0);
        Vector2 toTarget = targetPos - currentPos;
        var horientation = Vector2.Dot(toTarget.normalized, rb.velocity.normalized);

        if (trainningMode && horientation <= 0)
        {
            AddReward(-gain * 4);
            EndEpisode();
        }
    }
    // check if the NPC is grounded
    private bool IsGrounded()
    {
        Debug.DrawRay(transform.position, -Vector3.up * 5, Color.red, 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector3.up, distToGround + 0.01f, LayerMask.GetMask("Ground"));
        return hit;
    }

    private void FixedUpdate()
    {
        // RayPerceptionSensorComponent2D[] sensors = GetComponentsInChildren<RayPerceptionSensorComponent2D>();

        // foreach (RayPerceptionSensorComponent2D sensor in sensors)
        // {
        //     var rays = sensor.RaySensor.RayPerceptionOutput.RayOutputs;
        //     foreach (RayPerceptionOutput.RayOutput ray in rays)
        //     {
        //         if (ray.HasHit)
        //         {
        //             this.target = ray.HitGameObject;
        //             return;
        //         }
        //     }
        // }

        // // There is no enemy or it is out of range
        // if (!(this.target == this.leftLimit || this.target == this.rightLimit))
        // {
        //     Debug.Log("Picking one limit as target");
        //     this.target = null;
        //     PickOneLimitAsTarget();
        // }

    }

    // There should be two bars
    // when this method is called, it should 
    // take the other bar as target if it is defined
    // otherwise, it select one of them
    private void PickOneLimitAsTarget()
    {
        // Debug.Log("Picking limit as target");
        // if no target setted, pick one of limits randomnly as target
        if (!target)
        {
            target = Random.Range(0, 2) == 0 ? this.leftLimit : this.rightLimit;
        }
        else
        {
            this.target = this.leftLimit == this.target ? this.rightLimit : this.leftLimit;
        }
    }
}

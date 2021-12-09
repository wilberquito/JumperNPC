using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;


public class NPC : Agent
{
    [SerializeField] bool trainning = false;

    [SerializeField] Limit leftLimit;

    [SerializeField] Limit rightLimit;

    [SerializeField] GameObject currentTarget;

    [SerializeField] float gain = 1f;

    [SerializeField] float distToGround = 0.36f;

    [SerializeField] float jumpPower;

    [SerializeField] float movementPower = 8f;

    [SerializeField] float attackModeDuration = 1f;

    [SerializeField] RayPerceptionSensorComponent2D[] sensors;

    AnimatorHandler animatorHandler;

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

    private void Start()
    {
        animatorHandler = GetComponent<AnimatorHandler>();
    }

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        // infinite steps for session
        if (!trainning) MaxStep = 0;
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

        // restart limits
        RestartLimits();
    }

    private void RestartLimits()
    {
        leftLimit.Restart();
        rightLimit.Restart();
    }


    // this routine update the state of attack mode after the yield to avoid
    // agent to jump all the time in case there is an enemy nearby
    private IEnumerator AttackModeCoroutine()
    {
        Debug.Log("To attack mode...");
        attackMode = true;
        animatorHandler.AttackMode();
        yield return new WaitForSeconds(attackModeDuration);
        Debug.Log("To normal mode...");
        attackMode = false;
        animatorHandler.NormalMode();
    }

    // called when action is received from either {player, neural network}
    // each buffer position refers to an action, I decide what it means for each positions
    // inside that structure has continuos and discrete actions
    // index 0: -1 means move to the left, +1 means move to the right
    // the cool thing about the neural network, is that it figurates it all automatic

    // the second element is discrete parameter, which says jump or not [0,1]
    // when npc jumps has the hability to make damage and only should jump when there is a enemy nearby
    // and it is in the ground.

    // in case it is not in the ground I penalize the agent
    public override void OnActionReceived(ActionBuffers actions)
    {
        int jump = actions.DiscreteActions[0];
        float horizontal = actions.ContinuousActions[0];
        Vector2 v = rb.velocity;

        // try to learn not to jump when it is not in the ground
        bool unneededJump = jump == 1 && !ShouldJump();
        if (unneededJump)
        {
            Debug.Log("Unneded jump");
        }
        if (trainning && unneededJump)
        {
            AddReward(-gain);
            EndEpisode();
        }

        // run attack mode
        if (jump == 1 && !attackMode)
        {
            StartCoroutine(AttackModeCoroutine());
        }

        Vector2 movement = new Vector2(horizontal * movementPower, jump == 1 ? jump * jumpPower : v.y);
        rb.velocity = movement;
    }

    // The Agent should only jump when is grounded or one of its ray touches a hero
    // from Unity I had configure which elements can be touched by this rays
    private bool ShouldJump()
    {
        if (!IsGrounded()) return false;

        foreach (var sensor in sensors)
        {
            var rays = sensor.RaySensor.RayPerceptionOutput.RayOutputs;
            foreach (RayPerceptionOutput.RayOutput ray in rays)
            {
                if (ray.HasHit) return true;
            }
        }

        return false;
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


    // just one type of trigger is thought
    // the limits
    private void OnTriggerEnter2D(Collider2D other)
    {
        // check if agent is colliding with range movement bars
        // especific if its touching the current target
        // Note: everything diff from current target should not count

        bool limit = other.tag.Equals("LimitLeft") || other.tag.Equals("LimitRight");

        if (trainning && limit && other.gameObject == target)
        {
            // it had touched one of the limits
            AddReward(gain);
            PickOneLimitAsTarget();
        }

        if (trainning && limit && other.gameObject != target)
        {
            AddReward(-gain);
            EndEpisode();
        }

    }

    // This method should add gain to agent
    // if the collision is made with a hero layer
    private void OnCollisionEnter2D(Collision2D other)
    {
        bool hero = other.gameObject.layer == LayerMask.NameToLayer("Hero");

        if (trainning && hero && attackMode)
        {
            Debug.Log("Collision with hero in attack mode:");
            AddReward(gain * 5);
            Destroy(other.gameObject);
            target = null;
            return;
        }

        if (trainning && hero && !attackMode)
        {
            Debug.Log("Collision with hero in normal mode:");
            AddReward(-gain * 5);
            EndEpisode();
            return;
        }

    }

    // This method should penalize agent in case the current target
    // is one of the limits and the agent is outside of them
    private void OnTriggerExit2D(Collider2D other)
    {
        bool limit = other.tag.Equals("LimitLeft") || other.tag.Equals("LimitRight");

        if (trainning && limit)
        {
            Vector2 currentPos = new Vector2(transform.position.x, 0);
            Vector2 targetPos = new Vector2(target.transform.position.x, 0);
            Vector2 toTarget = targetPos - currentPos;
            var horientation = Vector2.Dot(toTarget.normalized, rb.velocity.normalized);

            if (horientation <= 0)
            {
                AddReward(-gain * 4);
                EndEpisode();
            }
        }
    }

    // check if the NPC is grounded
    private bool IsGrounded()
    {
        Debug.DrawRay(transform.position, -Vector3.up * 5, Color.red, 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector3.up, distToGround + 0.01f, LayerMask.GetMask("Ground"));
        // Debug.Log(hit.distance);
        return hit;
    }

    // 1. Check if there is target, if not updates target as one of limits
    // 2. Check if any ray is hitting hero
    // 3. If none ray is hitting hero if means that
    // in previous state it was and now no, updates target as one of limits
    private void FixedUpdate()
    {
        if (!target)
        {
            PickOneLimitAsTarget();
            return;
        }
    }

    // There should be two bars
    // when this method is called, it should 
    // take the other bar as target if it is defined
    // otherwise, it select one of them
    private void PickOneLimitAsTarget()
    {
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

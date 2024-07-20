using System.Resources;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//Movement code courtesy of Ketra Games

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public enum State { Idle, Run, Jump, Attack }
    [HideInInspector] public enum AttackState { Slash, Backhand, DoubleFist, NotAttacking }

    State state;
    AttackState attackState;

    /*******************Movement******************/
    //---------public-------
    public float speed_run;
    public float speed_jump;
    public float speed_rotation;


    //--------private-------
    CharacterController characterController;
    Transform cameraTransform;
    Animator animator;
    float speed_current_run;
    float speed_current_rotation;
    float speed_y;
    bool inAir;
    float jumpTimer;
    float inAirTimer;

    /*******************Attacking******************/
    float health;
    float attackTimer;
    bool canAttack;
    float animationLength_Slash;
    float animationLength_Backhand;
    bool inflictDamage;

    /*******************UI******************/
    bool gameIsPaused;


    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(gameIsPaused);

        if(!gameIsPaused)
        { 
            float horizontal, vertical;

            GetInput(out horizontal, out vertical);

            Vector3 moveDirection, velocity;
            Vector2 velocity_run;
            SetDirection(horizontal, vertical, out moveDirection, out velocity, out velocity_run);

            Run(moveDirection);

            JumpOrFall();

            Attack();

            SetState(velocity_run);

            Move(velocity);
        }
    }
    private void Init()
    {
        //Player State
        state = State.Idle;

        //----------Components-------
        characterController = GetComponent<CharacterController>();
        cameraTransform = GameObject.Find("Main Camera").GetComponent<Transform>();
        animator = GetComponent<Animator>();

        //-------Movement-----------
        speed_current_run = speed_run;
        speed_y = 0.0f;
        inAir = false;
        inAirTimer = 0.0f;
        jumpTimer = 0.5f;

        //-------Attacking-----------
        health = 1.0f;
        attackTimer = 0.0f;
        canAttack = true;
        animationLength_Slash = 1.75f;
        animationLength_Backhand = 2.43f;

        //-------UI-----------
        gameIsPaused = false;


    }
    private static void GetInput(out float horizontal, out float vertical)
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
    }
    private void SetDirection(float horizontal, float vertical, out Vector3 moveDirection, out Vector3 velocity, out Vector2 velocity_run)
    {
        moveDirection = new Vector3(horizontal, 0.0f, vertical);
        float magnitude = Mathf.Clamp01(moveDirection.magnitude) * speed_current_run;
        moveDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * moveDirection;
        moveDirection.Normalize();
        velocity = moveDirection * magnitude;
        velocity.y = speed_y;
        velocity_run = new Vector2(velocity.x, velocity.z);

    }
    private void Run(Vector3 moveDirection)
    {
        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, speed_current_rotation * Time.deltaTime);
            state = State.Run;
        }
    }
    private void JumpOrFall()
    {
        if (characterController.isGrounded)
        {
            inAirTimer = 0.0f;

            if (inAir && jumpTimer < 0.0f)
            {
                jumpTimer = 0.5f;
                inAir = false;
            }
        }
        else
        {
            speed_y += Physics.gravity.y * Time.deltaTime;

            inAirTimer -= Time.deltaTime;
            jumpTimer -= Time.deltaTime;

            //player is falling
            if (inAirTimer < -0.4f)
            {
                inAir = true;

            }
        }
    }
    private void Attack()
    {
        if (attackTimer > 0.0f)
        {
            attackTimer -= Time.deltaTime;

            switch(attackState)
            {
                case AttackState.Slash:

                    if(attackTimer < animationLength_Slash - 0.40 && attackTimer > animationLength_Slash - 0.90)
                    {
                        inflictDamage = true;
                    }
                    else
                    {
                        inflictDamage = false;
                    }



                    if (attackTimer < animationLength_Slash / 3.0f)
                    {
                        canAttack = true;
                    }

                    break;

                case AttackState.Backhand:

                    if (attackTimer < animationLength_Backhand / 3.0f)
                    {
                        canAttack = true;
                    }

                    break;
            }
        }
        else
        {
            //Attack has ended
            attackTimer = 0.0f;
            attackState = AttackState.NotAttacking;

            //Stop Animations
            animator.SetBool("StopAttacking", true);
            
            //player can move
            speed_current_run = speed_run;
            speed_current_rotation = speed_rotation;

        }
    }
    private void SetState(Vector2 velocity_run)
    {
        animator.SetFloat("runVelocity", velocity_run.magnitude);
        animator.SetBool("inAir", inAir);

        //if running on ground
        if(velocity_run.magnitude > 0.0f && !inAir && attackTimer <= 0.0f)
        {
            state = State.Run;
        }
        else if(inAir) //in air
        {
            state = State.Jump;

        }
        else if(!inAir && attackTimer > 0.0f)
        {
            state = State.Attack;
        }
        else
        {
            state = State.Idle;

        }
    }
    private void Move(Vector3 velocity)
    {
        characterController.Move(velocity * Time.deltaTime);
    }
    public State GetState()
    {
        return state;
    }
    public float GetHealth()
    {
        return health;
    }
    public float GetAttackTimer()
    {
        return attackTimer;
    }
    public bool PausedGame()
    {
        return gameIsPaused;
    }
    public bool InflictDamage()
    {
        return inflictDamage;
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if(context.performed && characterController.isGrounded && !gameIsPaused)
        {
            speed_y = speed_jump;
            inAirTimer = 0.0f;
            inAir = true;
        }
    }
    public void Attack(InputAction.CallbackContext context)
    {
        if(context.performed && characterController.isGrounded && canAttack && !gameIsPaused)
        {
            //Stop Charcter
            speed_current_run = 0.0f;
            speed_current_rotation = 0.0f;
            canAttack = false;

            if(attackTimer <= 0.0f)
            {
                attackState = AttackState.Slash;
                attackTimer = animationLength_Slash;
                animator.SetTrigger("Attack_Slash");
            }
            else if(attackState.Equals(AttackState.Slash))
            {
                attackState = AttackState.Backhand;
                attackTimer = animationLength_Backhand;
                animator.SetTrigger("Attack_Backhand");
            }


        }
    }

    public void PauseGame(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            if(gameIsPaused)
            {
                gameIsPaused = false;
            }
            else
            {
                gameIsPaused = true;
            }
        }
        
    }
    public void SetGameIsPaused(bool pauseGame)
    {
        gameIsPaused = pauseGame;
    }

}

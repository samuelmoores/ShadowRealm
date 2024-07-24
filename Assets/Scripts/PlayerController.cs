using Cinemachine;
using System.Collections.Generic;
using System.Resources;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//Movement code courtesy of Ketra Games

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public enum State { Idle, Run, Jump, Attack }
    [HideInInspector] public enum AttackState { Attack_01, Attack_02, Attack_03, Poison, NotAttacking }

    State state;
    AttackState attackState;

    /*******************Movement******************/
    public float speed_run;
    public float speed_jump;
    public float speed_rotation;

    CharacterController characterController;
    Transform cameraTransform;
    CinemachineFreeLook cam;
    Animator animator;
    float speed_current_run;
    float speed_current_rotation;
    float speed_y;
    float speed_camera_x;
    float speed_camera_y;
    bool inAir;
    float jumpTimer;
    float inAirTimer;

    /*******************Attacking******************/
    float attackTimer;
    bool canAttack;
    float animationLength_Attack_01;
    float animationLength_Attack_02;
    bool inflictDamage;

    /*******************Damaging******************/
    float health;
    int damageCount = 0;
    int damageAnimation;
    bool isDead;

    /*******************Crafting******************/
    public GameObject testIngrediant;
    GameObject[] ingredients;
    bool isCrafting;
    int numOfIngredients;


    /*******************UI******************/
    HUD hud;
    bool gameIsPaused;
    [HideInInspector] public float unPauseTimer;
    [HideInInspector] public float unPauseTimer_current;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameIsPaused && !isDead)
        {
            //when exiting crafting guard against jumping 
            if(unPauseTimer_current > 0.0f && !isCrafting)
            {
                unPauseTimer_current -= Time.deltaTime;
            }

            float horizontal, vertical;
            GetInput(out horizontal, out vertical);
            Vector3 moveDirection, velocity;
            Vector2 velocity_run;
            SetDirection(horizontal, vertical, out moveDirection, out velocity, out velocity_run);

            //let the game run while player is crafting but inhibit movement
            if (isCrafting)
            {
                moveDirection = Vector3.zero;
                velocity = Vector3.zero;
                unPauseTimer_current = unPauseTimer;
            }
            else
            {
                cam.m_XAxis.m_MaxSpeed = speed_camera_x;
                cam.m_YAxis.m_MaxSpeed = speed_camera_y;
                Run(moveDirection);
                JumpOrFall();
                Attack();
                Move(velocity);
            }
            SetState(velocity_run);
        }
        
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------Functions-----------------------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Init()
    {
        //Player State
        state = State.Idle;

        //----------Components-------
        characterController = GetComponent<CharacterController>();
        cameraTransform = GameObject.Find("Main Camera").GetComponent<Transform>();
        animator = GetComponent<Animator>();
        hud = GameObject.Find("Canvas").GetComponent<HUD>();
        cam = GameObject.Find("ThirdPersonCamera").GetComponent<CinemachineFreeLook>();

        //-------Movement-----------
        speed_current_run = speed_run;
        speed_y = 0.0f;
        inAir = false;
        inAirTimer = 0.0f;
        jumpTimer = 0.5f;
        speed_camera_x = cam.m_XAxis.m_MaxSpeed;
        speed_camera_y = cam.m_YAxis.m_MaxSpeed;


        //-------Attacking-----------
        attackTimer = 0.0f;
        canAttack = true;
        animationLength_Attack_01 = 1.267f;
        animationLength_Attack_02 = 1.833f;
        
        //---------Damage----------
        health = 1.0f;
        isDead = false;

        //-------UI-----------
        gameIsPaused = false;
        unPauseTimer = 0.2f;
        unPauseTimer_current = 0.0f;

        //-------Crafting-----------
        ingredients = new GameObject[9];
        numOfIngredients = 0;

    }
    //**********Movement*********
    private void GetInput(out float horizontal, out float vertical)
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (isCrafting)
        {
            horizontal = 0.0f;
            vertical = 0.0f;
            cam.m_XAxis.m_MaxSpeed = 0.0f;
            cam.m_YAxis.m_MaxSpeed = 0.0f;

        }

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
    private void Move(Vector3 velocity)
    {
        characterController.Move(velocity * Time.deltaTime);
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

    //**********Input*********
    public void Jump(InputAction.CallbackContext context)
    {
        if(context.performed && characterController.isGrounded && unPauseTimer_current <= 0.0f && !isCrafting)
        {
            speed_y = speed_jump;
            inAirTimer = 0.0f;
            inAir = true;
        }
    }
    public void Attack(InputAction.CallbackContext context)
    {
        if(context.performed && characterController.isGrounded && canAttack && unPauseTimer_current <= 0.0f && ! isCrafting)
        {
            //Stop Charcter
            speed_current_run = 0.0f;
            speed_current_rotation = 0.0f;
            canAttack = false;

            if(attackTimer <= 0.0f)
            {
                attackState = AttackState.Attack_01;
                attackTimer = animationLength_Attack_01;
                animator.SetTrigger("Attack_01");
            }
            else if(attackState.Equals(AttackState.Attack_01))
            {
                attackState = AttackState.Attack_02;
                attackTimer = animationLength_Attack_02;
                animator.SetTrigger("Attack_02");
            }

        }
    }

    //**********Attacking/Damaging*********
    private void Attack()
    {
        if (attackTimer > 0.0f)
        {
            attackTimer -= Time.deltaTime;

            switch(attackState)
            {
                case AttackState.Attack_01:

                    if((attackTimer < animationLength_Attack_01 - 0.20f) && (attackTimer > animationLength_Attack_01 - 1.00f))
                    {
                        inflictDamage = true;
                    }
                    else
                    {
                        inflictDamage = false;
                    }

                    if (attackTimer < animationLength_Attack_01 / 3.0f)
                    {
                        canAttack = true;
                    }

                    break;

                case AttackState.Attack_02:

                    if (attackTimer < animationLength_Attack_02 - 0.60f && attackTimer > animationLength_Attack_02 - 1.40f)
                    {
                        inflictDamage = true;
                    }
                    else
                    {
                        inflictDamage = false;
                    }

                    if (attackTimer < animationLength_Attack_02 / 3.0f)
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
    public bool InflictDamage()
    {
        return inflictDamage;
    }
    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        damageCount++;
        damageAnimation = 1;

        if(health <= 0.0f)
        {
            isDead = true;
            animator.SetBool("isDead", true);
        }
        else
        {
            switch (damageAnimation)
            {
                case 1:
                    animator.SetTrigger("Damage_01");
                    break;
                case 2:
                    animator.SetTrigger("Damage_02");
                    break;
                case 3:
                    animator.SetTrigger("Damage_03");
                    break;
            }
        }
        
    }

    //**********Crafting*********
    public void AddIngredient(GameObject ingrediant)
    {
        ingredients[numOfIngredients++] = ingrediant;
        hud.AddIngrediantImage(ingrediant);
    }
    public void UseIngredient(int index)
    {
        if(numOfIngredients > 0)
        {
            numOfIngredients--;
            hud.RemoveIngredientImage(index);

        }
    }
    public void PrintIngedients()
    {
        Debug.Log("-----------");
        for(int i = 0; i < numOfIngredients; i++)
        {
            Debug.Log("[" + i + "]" + ingredients[i]);
        }
        Debug.Log("-----------");

    }

    //**********UI*********
    public bool PausedGame()
    {
        return gameIsPaused;
    }
    public void SetGameIsPaused(bool pauseGame)
    {
        gameIsPaused = pauseGame;
    }

    //**********Helper*********
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
    public bool GetIsCrafting()
    {
        return isCrafting;
    }
    public GameObject GetIngrediant(int index)
    {
        return ingredients[index];

    }
    public GameObject[] GetIngrediants()
    {
        return ingredients;
    }
    public int GetIngredientCount()
    {
        return numOfIngredients;
    }
    public void SetIsCrafting(bool value)
    {
        isCrafting = value;
    }

}

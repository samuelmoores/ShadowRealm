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
    [HideInInspector] public enum AttackState { Attack_01, Attack_02, Attack_03, NotAttacking }

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
    HUD hud;
    float speed_current_run;
    float speed_current_rotation;
    float speed_y;
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
    GameObject[] ingrediants;
    bool isCrafting;
    int addIndex;
    int removalIndex;


    /*******************UI******************/
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
            //let the game run while player is crafting but inhibit movement
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
            
            if(isCrafting)
            {
                moveDirection = Vector3.zero;
                velocity = Vector3.zero;
                unPauseTimer_current = unPauseTimer;
            }
            else
            {
                Run(moveDirection);

                JumpOrFall();

                Attack();

                Move(velocity);
            }
            
            SetState(velocity_run);

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
        hud = GameObject.Find("Canvas").GetComponent<HUD>();

        //-------Movement-----------
        speed_current_run = speed_run;
        speed_y = 0.0f;
        inAir = false;
        inAirTimer = 0.0f;
        jumpTimer = 0.5f;

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
        ingrediants = new GameObject[9];
        addIndex = 0;
        removalIndex = 0;

    }
    private void GetInput(out float horizontal, out float vertical)
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (isCrafting)
        {
            horizontal = 0.0f;
            vertical = 0.0f;
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
    private void Attack()
    {
        if (attackTimer > 0.0f)
        {
            attackTimer -= Time.deltaTime;

            switch(attackState)
            {
                case AttackState.Attack_01:

                    if(attackTimer < animationLength_Attack_01 - 0.50 && attackTimer > animationLength_Attack_01 - 0.90)
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

    public void SetIsCrafting(bool value)
    {
        isCrafting = value;
    }

    public bool GetIsCrafting()
    {
        return isCrafting;
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

    public GameObject[] GetIngrediants()
    {
        return ingrediants;
    }

    public GameObject GetIngrediant(int index)
    {
        return ingrediants[index];

    }

    public int GetIngredientCount()
    {
        return addIndex;
    }

    public void AddIngrediant(GameObject ingrediant)
    {
        ingrediants[addIndex++] = ingrediant;
        hud.AddIngrediantImage(ingrediant);
    }

    public void UseIngredient(int index)
    {
        if(addIndex > 0)
        {
            addIndex--;
            hud.RemoveIngredientImage(index);

        }
    }

    public void PrintIngedients()
    {
        Debug.Log("-----------");
        for(int i = 0; i < addIndex; i++)
        {
            Debug.Log("[" + i + "]" + ingrediants[i]);
        }
        Debug.Log("-----------");

    }

    public bool PausedGame()
    {
        return gameIsPaused;
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

    public void SetGameIsPaused(bool pauseGame)
    {
        gameIsPaused = pauseGame;
    }

   

}

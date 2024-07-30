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
    [HideInInspector] public enum AttackState { Attack_01, Attack_02, Attack_03, Poison, ShadowFist, NotAttacking }

    State state;
    AttackState attackState;

    /*******************Movement******************/
    public float default_speed_run;
    public float speed_jump;
    public float default_speed_rotation;

    public GameObject skeleton;
    public GameObject paladin;
    public Transform SwordBackSocket;


    CharacterController characterController;
    Transform cameraTransform;
    CinemachineFreeLook cam;
    Animator animator;

    public RuntimeAnimatorController skeletonAnimController;
    public RuntimeAnimatorController paladinAnimController;

    public Avatar skeletonAvatar;
    public Avatar paladinAvatar;

    float speed_run;
    float speed_rotation;
    float speed_y;
    float speed_camera_x;
    float speed_camera_y;
    bool inAir;
    float jumpTimer;
    float inAirTimer;

    /*******************Attacking******************/
    public GameObject ShadowFist;
    bool isAttacking;
    bool canAttack;
    bool identityShadowed;
    int attackNumber;
    int numOfAttacks;
    bool inflictDamage;
    bool dodgeRolling;
    float dodgeRollTimer;
    bool isBlocking;
    bool hasPoison;
    bool shadowRealmActivated;
    float attackTimer;
    float shadowFistScalar;
    float currentAttackAnimationLength;
    float animationLength_Attack_01;
    float animationLength_Attack_02;
    float animationLength_DumpPoison;
    float animationLength_ShadowFist;
    float animationLength_DodgeRoll;


    /*******************Damaging******************/
    float health;
    bool isDamaged;
    int damageCount = 0;
    int damageAnimation;
    bool isDead;
    float damageTimer;
    bool hit;
    bool startRespawnTimer;
    float respawnTimer;
    float respawnTimerDefault;

    /*******************Crafting******************/
    GameObject Potion;
    GameObject[] ingredients;
    int[] ingredientIndeces;
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
        //Player State
        state = State.Idle;

        //----------Components-------
        characterController = GetComponent<CharacterController>();
        cameraTransform = GameObject.Find("Main Camera").GetComponent<Transform>();
        animator = GetComponent<Animator>();

        hud = GameObject.Find("Canvas").GetComponent<HUD>();
        cam = GameObject.Find("ThirdPersonCamera").GetComponent<CinemachineFreeLook>();

        //-------Movement-----------
        speed_run = default_speed_run;
        speed_rotation = default_speed_rotation;
        speed_y = 0.0f;
        inAir = false;
        inAirTimer = 0.0f;
        jumpTimer = 0.5f;
        speed_camera_x = cam.m_XAxis.m_MaxSpeed;
        speed_camera_y = cam.m_YAxis.m_MaxSpeed;

        //-------Attacking-----------
        isAttacking = false;
        identityShadowed = false;
        canAttack = true;
        attackNumber = 0;
        numOfAttacks = 2;
        hasPoison = false;
        shadowRealmActivated = false;
        dodgeRolling = false;
        isBlocking = false;
        attackTimer = 0.0f;
        dodgeRollTimer = 0.0f;
        shadowFistScalar = 0.0f;
        animationLength_Attack_01 = 1.267f;
        animationLength_Attack_02 = 1.833f;
        animationLength_DumpPoison = 4.0f;
        animationLength_ShadowFist = 2.4f;
        animationLength_DodgeRoll = 1.167f;

        //---------Damage----------
        health = 1.0f;
        isDead = false;
        damageTimer = 0.0f;
        hit = false;
        startRespawnTimer = false;
        respawnTimerDefault = 1.5f;
        respawnTimer = 0.0f;

        //-------UI-----------
        gameIsPaused = false;
        unPauseTimer = 0.2f;
        unPauseTimer_current = 0.0f;

        //-------Crafting-----------
        ingredients = new GameObject[9];
        numOfIngredients = 0;
        ingredientIndeces = new int[9] { -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        //****************DEBUG**********************
        //ShadowIdentity();
        //ActivateShadowRealm();
        //*****************DEBUG*********************
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            respawnTimerDefault = 4.0f;
            startRespawnTimer = true;
        }

        if (!gameIsPaused)
        {
            if (startRespawnTimer)
            {
                cam.enabled = false;
                respawnTimer += Time.deltaTime;

                if (respawnTimer > respawnTimerDefault)
                {
                    SceneManager.LoadScene("Level");

                }
            }

            //when exiting crafting guard against jumping 
            if (unPauseTimer_current > 0.0f && !isCrafting)
            {
                unPauseTimer_current -= Time.deltaTime;
            }

            float horizontal, vertical;
            GetInput(out horizontal, out vertical);
            Vector3 moveDirection, velocity;
            Vector2 velocity_run;
            SetDirection(horizontal, vertical, out moveDirection, out velocity, out velocity_run);

            //let the game run while player is crafting but inhibit movement
            if (isCrafting || isDead || isBlocking)
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
                if (isAttacking)
                {
                    Attack();

                }

                if (isDamaged)
                {
                    RunDamageTimer(1.3f);
                }

                if(dodgeRolling)
                {
                    RunDodgeRollTimer();
                }

            }
            Move(velocity);
            SetState(velocity_run);
        }

    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------Functions-----------------------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
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
        float magnitude = Mathf.Clamp01(moveDirection.magnitude) * speed_run;
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
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, speed_rotation * Time.deltaTime);
            state = State.Run;
        }
    }
    private void JumpOrFall()
    {
        if (characterController.isGrounded)
        {
            inAirTimer = 0.0f;
            speed_y = -9.8f;

            if (inAir)
            {
                inAir = false;
            }
        }
        else if (!isCrafting)
        {
            speed_y += Physics.gravity.y * Time.deltaTime;

            inAirTimer -= Time.deltaTime;

            //guard against subtle times the player is not grounded
            //the fall animation should not play if the play is in air
            //for only a few frames so check to make sure the inAir time 
            //has been running for more than a few milliseconds
            if (inAirTimer < -0.1f)
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
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("inAir", inAir);

	    if(identityShadowed)
	    {
            animator.SetBool("isBlocking", isBlocking);
	    }

        //if running on ground
        if (velocity_run.magnitude > 0.0f && !inAir && attackTimer <= 0.0f)
        {
            state = State.Run;
        }
        else if (inAir) //in air
        {
            state = State.Jump;

        }
        else if (!inAir && attackTimer > 0.0f)
        {
            state = State.Attack;
        }
        else
        {
            state = State.Idle;

        }
    }

    //**********Input*********
    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && characterController.isGrounded && canAttack && unPauseTimer_current <= 0.0f && !isCrafting)
        {
            if (!isDamaged)
            {
                attackTimer = 0.0f;
                canAttack = false;
                isAttacking = true;

                if (hasPoison)
                {
                    attackNumber = 3;
                    inflictDamage = true;
                }
                else if (shadowRealmActivated)
                {
                    attackNumber = 4;
                }
                else
                {
                    attackNumber++;
                    if (attackNumber > numOfAttacks)
                    {
                        attackNumber = 1;
                    }

                }


                switch (attackNumber)
                {
                    case 1:
                        animator.SetTrigger("Attack_01");
                        break;
                    case 2:
                        animator.SetTrigger("Attack_02");
                        break;
                    case 3:
                        animator.SetTrigger("DumpPoison");
                        break;
                    case 4:
                        animator.SetTrigger("ShadowFistPunch");
                        break;
                }


            }

        }
    }
    public void DodgeRoll(InputAction.CallbackContext context)
    {
        if(context.performed && characterController.isGrounded && !isDamaged && !isAttacking && !isCrafting && !dodgeRolling && shadowRealmActivated)
        {
            dodgeRolling = true;
            animator.SetTrigger("DodgeRoll");
        }
    }

    public void Block(InputAction.CallbackContext context)
    {
        if (context.performed && characterController.isGrounded && !isDamaged && !isAttacking && !isCrafting && identityShadowed)
        {
            isBlocking = true;
        }

        if (context.canceled)
        {
            isBlocking = false;

        }
    }

    //**********Attacking/Damaging*********
    private void Attack()
    {
        //setup animation timer
        switch (attackNumber)
        {
            case 1:
                currentAttackAnimationLength = animationLength_Attack_01;
                break;
            case 2:
                currentAttackAnimationLength = animationLength_Attack_02;
                break;
            case 3:
                currentAttackAnimationLength = animationLength_DumpPoison;
                break;
            case 4:
                currentAttackAnimationLength = animationLength_ShadowFist;
                break;
        }


        //run animation timer until the length of the animation
        //make sure the character can't move, they are attacking
        //and are not able to attack again until a certain time
        if (attackTimer < currentAttackAnimationLength)
        {
            //Stop Charcter
            speed_run = 0.0f;
            speed_rotation = 0.0f;
            isAttacking = true;
            canAttack = false;
            attackTimer += Time.deltaTime;

            if (shadowRealmActivated)
            {
                ShadowFist.SetActive(true);
                inflictDamage = true;

                if (attackTimer < 1.0f)
                {
                    if(ShadowFist.transform.localScale.x > 0.0f)
                    {
                        shadowFistScalar += Time.deltaTime * 200.0f;
                        //Debug.Log(shadowFistScalar);
                        ShadowFist.transform.localScale += new Vector3(Time.deltaTime * shadowFistScalar, Time.deltaTime * shadowFistScalar, Time.deltaTime * shadowFistScalar);
                    }
                    
                }
                else
                {
                    if(ShadowFist.transform.localScale.x > 0.0f)
                    {
                        shadowFistScalar += Time.deltaTime * 600.0f;
                        ShadowFist.transform.localScale -= new Vector3(Time.deltaTime * shadowFistScalar, Time.deltaTime * shadowFistScalar, Time.deltaTime * shadowFistScalar);
                        //Debug.Log(shadowFistScalar); 
                    }

                }

            }

        }

        //buffer time to start a combo
        if (attackTimer > currentAttackAnimationLength / 2.0f)
        {
            canAttack = true;
        }

        //end the animation
        if (attackTimer >= currentAttackAnimationLength)
        {
            if (hasPoison)
            {
                Potion.SetActive(false);
                hasPoison = false;
            }
            else if (shadowRealmActivated)
            {
                ShadowFist.SetActive(false);
                ShadowFist.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                shadowFistScalar = 0.0f;
            }
            isAttacking = false;
            inflictDamage = false;
            canAttack = true;
            attackNumber = 0;

            //Attack has ended
            attackTimer = 0.0f;
            attackState = AttackState.NotAttacking;

            //player can move
            if(shadowRealmActivated)
            {
                speed_run = default_speed_run * 2.0f;
            }
            else
            {
                speed_run = default_speed_run;
            }

            speed_rotation = default_speed_rotation;

        }
    }
    public bool InflictDamage()
    {
        return inflictDamage;
    }

    public void SetInflictDamage(int value)
    {
        if (value == 0)
        {
            inflictDamage = false;
        }
        else if (value == 1)
        {
            inflictDamage = true;

        }
    }
    public bool IsAlive()
    {
        return !isDead;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("KingShadowFist") && !dodgeRolling)
        {
            hit = true;
        }

        if (other.CompareTag("Respawn"))
        {
            startRespawnTimer = true;
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("KingShadowFist"))
        {
            hit = false;
        }


    }

    public void SetHit(bool value)
    {
        hit = value;
    }

    public bool IsHit()
    {
        return hit;
    }
    public void TakeDamage(float damageAmount)
    {
        if (hit)
        {
            health -= damageAmount;
            damageCount++;
            damageAnimation = 1;
            isDamaged = true;
            damageTimer = 0.0f;

            if (health <= 0.0f)
            {
                isDead = true;
                animator.SetBool("isDead", true);
            }
            else
            {
                switch (damageAnimation)
                {
                    case 1:
                        animator.SetTrigger("Damage");
                        break;
                }
            }
        }

    }

    void RunDamageTimer(float animationLength)
    {
        if (damageTimer < animationLength)
        {
            damageTimer += Time.deltaTime;
            speed_run = 0.0f;
            speed_rotation = 0.0f;
        }
        else
        {
            isDamaged = false;
	    if(shadowRealmActivated)
	    {
		speed_run = default_speed_run * 2.0f;
	    }
	    else
	    {
                speed_run = default_speed_run;
            }
	    speed_rotation = default_speed_rotation;
            damageTimer = 0.0f;
        }
    }

    public void ShadowIdentity()
    {
        animator.runtimeAnimatorController = paladinAnimController;
        animator.avatar = paladinAvatar;

        skeleton.SetActive(false);
        paladin.SetActive(true);
        identityShadowed = true;

    }

    public void UnShadow()
    {
        animator.runtimeAnimatorController = skeletonAnimController;
        animator.avatar = skeletonAvatar;

        skeleton.SetActive(true);
        paladin.SetActive(false);
        identityShadowed = false;
    }

    public bool IsShadowed()
    {
        return identityShadowed;
    }
    public void ActivateShadowRealm()
    {
        if(identityShadowed)
        {
            UnShadow();
        }
        GameObject.Find("Sword").gameObject.SetActive(false);
        GameObject.Find("SwordBack").gameObject.GetComponent<MeshRenderer>().enabled = true;
        animator.SetBool("ShadowSprint", true);
        speed_run *= 2.0f;


        shadowRealmActivated = true;
    }

    void RunDodgeRollTimer()
    {
        if(dodgeRollTimer < animationLength_DodgeRoll)
        {
            dodgeRollTimer += Time.deltaTime;
        }
        else
        {
            dodgeRollTimer = 0.0f;
            dodgeRolling = false;
        }
    }

    public bool IsDodgingBlocking()
    {
        return dodgeRolling || isBlocking;
    }

    //**********Crafting*********
    public void AddIngredient(GameObject ingrediant)
    {
        int index = 0;

        for (int i = 0; i < 9; i++)
        {
            if (ingredientIndeces[i] == -1)
            {
                ingredientIndeces[i] = i;
                ingredients[i] = ingrediant;
                index = i;
                break;
            }
        }

        hud.AddIngrediantImage(ingrediant, index);
        numOfIngredients++;
    }
    public void UseIngredient(int index)
    {
        if (numOfIngredients > 0)
        {
            numOfIngredients--;
            ingredientIndeces[index] = -1;
            hud.RemoveIngredientImage(index);

        }
    }
    public void PrintIngedients()
    {
        Debug.Log("-----------");
        for (int i = 0; i < numOfIngredients; i++)
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
    public int getIngredientIndex(int index)
    {
        return ingredientIndeces[index];
    }
    public GameObject[] GetIngrediants()
    {
        return ingredients;
    }
    public bool HasPoison()
    {
        return hasPoison;
    }
    public int GetIngredientCount()
    {
        return numOfIngredients;
    }
    public bool HasActivatedShadowRealm()
    {
        return shadowRealmActivated;
    }
    public void SetIsCrafting(bool value)
    {
        isCrafting = value;
    }
    public void SetHasPoison(bool value)
    {
        hasPoison = value;
    }
    public void SetPotion(GameObject NewPotion, string potionName)
    {
        if (potionName == "Poison")
        {
            hasPoison = true;
        }
        Potion = NewPotion;
    }

}
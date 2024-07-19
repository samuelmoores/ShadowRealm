using UnityEngine;
using UnityEngine.InputSystem;

//Movement code courtesy of Ketra Games

public class PlayerController : MonoBehaviour
{
    /*******************Movement******************/
    //-------------public-------------
    public float speed_run;
    public float speed_jump;
    public float speed_rotation;
    
    //------------private-------------
    CharacterController characterController;
    Transform cameraTransform;
    Animator animator;
    float speed_current_run;
    float speed_y;
    bool inAir;
    float jumpTimer;
    float inAirTimer;

    /*******************Attacking******************/
    float attackTimer;
    float animationLength_Slash;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal, vertical;
        GetInput(out horizontal, out vertical);

        Vector3 moveDirection, velocity;
        Vector2 velocity_run;
        SetDirection(horizontal, vertical, out moveDirection, out velocity, out velocity_run);

        Run(moveDirection);

        JumpOrFall();

        if(attackTimer > 0.0f)
        {
            attackTimer -= Time.deltaTime;
        }
        else
        {
            attackTimer = 0.0f;
            speed_current_run = speed_run;
        }

        Debug.Log(attackTimer);

        SetAnimations(velocity_run);

        Move(velocity);
    }

    private void Move(Vector3 velocity)
    {
        characterController.Move(velocity * Time.deltaTime);
    }

    private void SetAnimations(Vector2 velocity_run)
    {
        animator.SetFloat("runVelocity", velocity_run.magnitude);
        animator.SetBool("inAir", inAir);
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

    private void Run(Vector3 moveDirection)
    {
        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, speed_rotation * Time.deltaTime);
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

    private static void GetInput(out float horizontal, out float vertical)
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
    }

    private void Init()
    {
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
        attackTimer = 0.0f;
        animationLength_Slash = 1.75f;

    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(context.performed && characterController.isGrounded)
        {
            speed_y = speed_jump;
            inAirTimer = 0.0f;
            inAir = true;
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if(context.performed && characterController.isGrounded)
        {
            attackTimer = animationLength_Slash;
            speed_current_run = 0.0f;
            animator.SetTrigger("Attack");
        }
    }
}

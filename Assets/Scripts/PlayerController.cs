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
    float speed_y;
    bool inAir;
    float jumpTimer;
    float inAirTimer;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        //Get input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        //Set direction
        Vector3 moveDirection = new Vector3(horizontal, 0.0f, vertical);
        float magnitude = Mathf.Clamp01(moveDirection.magnitude) * speed_run;
        moveDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * moveDirection;
        moveDirection.Normalize();
        Vector3 velocity = moveDirection * magnitude;
        velocity.y = speed_y;
        Vector2 velocity_run = new Vector2(velocity.x, velocity.z);

        //run
        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, speed_rotation * Time.deltaTime);
        }


        //jump or fall
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
            if(inAirTimer < -0.4f)
            {
                inAir = true;
                Debug.Log(inAirTimer);

            }
        }

        //set animator
        animator.SetFloat("runVelocity", velocity_run.magnitude);
        animator.SetBool("inAir", inAir);

        //apply movement
        characterController.Move(velocity * Time.deltaTime);
    }

    private void Init()
    {
        characterController = GetComponent<CharacterController>();
        cameraTransform = GameObject.Find("Main Camera").GetComponent<Transform>();
        animator = GetComponent<Animator>();
        speed_y = 0.0f;
        inAir = false;
        inAirTimer = 0.0f;
        jumpTimer = 0.5f;

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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//code from "THIRD PERSON MOVEMENT in Unity" by brackeys

public class PlayerController : MonoBehaviour
{
    /*******************Movement******************/
    //-------------public-------------
    public float speed;
    
    //------------private-------------
    CharacterController characterController;
    Transform cameraTransform;
    float turnVelocity;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        cameraTransform = GameObject.Find("Main Camera").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0.0f, vertical).normalized; 

        if(direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle_smoothing = Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref turnVelocity, 0.1f);
            transform.rotation = Quaternion.Euler(0.0f, angle_smoothing, 0.0f);

            Vector3 moveDirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            characterController.Move(moveDirection.normalized * speed * Time.deltaTime);
        }
    }
}

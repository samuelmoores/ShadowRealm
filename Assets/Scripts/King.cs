using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class King : MonoBehaviour
{
    NavMeshAgent agent;
    PlayerController player;
    Animator animator;
    CharacterController controller;
    float distanceFromPlayer;
    bool isStanding;
    bool playerFound;


    //-----animations----
    float animationTimer;
    float standUp;

    //------defending----
    float backupTimer;

    //------attacking-----
    int currentAttack;
    int numOfAttacks;
    bool isAttacking;
    bool canAttack;
    float jumpAttack;

    // Start is called before the first frame update
    void Start()
    {
        //----components
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        currentAttack = 0;
        numOfAttacks = 3;

        //----animations
        standUp = 4.833f;
        jumpAttack = 3.8f;

        //---timers---
        animationTimer = 0.0f;
        backupTimer = 0.0f;

        //---triggers---
        canAttack = false;
        isAttacking = false;
        isStanding = false;
        playerFound = false;
    }

    // Update is called once per frame
    void Update()
    {
        Init();

        //chase player
        if (playerFound && isStanding)
        {
            agent.destination = player.transform.position;
            RotateTowardsPlayer();

            if (distanceFromPlayer <= agent.stoppingDistance)
            {
                canAttack = true;
            }

            if(canAttack)
            {
                switch(currentAttack)
                {
                    case 0:
                        Attack(2.733f, "AttackCombo01");
                        break;
                    case 1:
                        Attack(3.167f, "Attack360");
                        break;
                    case 2:
                        Attack(2.267f, "AttackDownward");
                        break;
                }
            }
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector3 playerDirection = player.transform.position - transform.position;
        playerDirection.y = 0.0f;
        playerDirection.Normalize();
        Quaternion toRotation = Quaternion.LookRotation(playerDirection, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, agent.angularSpeed * Time.deltaTime);
    }

    private void Attack(float animationLength, string name)
    {
        if (!isAttacking)
        {
            animator.SetTrigger(name);
            isAttacking = true;
        }

        //wait for animation
        if (animationTimer < animationLength && isAttacking)
        {
            animationTimer += Time.deltaTime;
            agent.isStopped = true;
        }
        else
        {
            animationTimer = 0.0f;
            isAttacking = false;
            canAttack = false;
            agent.isStopped = false;
            currentAttack++;
            if(currentAttack == numOfAttacks)
            {
                currentAttack = 0;
            }
            
        }
    }

    private void Init()
    {
        distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);
        animator.SetFloat("RunVelocity", agent.velocity.magnitude);

        if (distanceFromPlayer < 20.0f)
        {
            playerFound = true;
        }

        if (!isStanding)
        {
            //start animation
            animator.SetBool("StandUp", true);

            //time animation
            if (animationTimer < standUp)
            {
                animationTimer += Time.deltaTime;
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
                animationTimer = 0.0f;
                isStanding = true;
            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("hit wall");
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    //---------public---------------
    public float damageTimer;
    public float attackCoolDown;

    //-----------Components------------
    CharacterController control;
    PlayerController player;
    Animator animator;
    NavMeshAgent agent;

    //------------Damage-------------
    float damageTimer_current;
    bool damaged;

    //-------AI---------
    float velocity;
    float distanceFromPlayer;
    float startTimer;

    //---------Attacking--------
    bool isAttacking;
    float attackTimer;

    // Start is called before the first frame update
    void Start()
    {
        //Init
        player = GameObject.Find("Player").GetComponentInChildren<PlayerController>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        control = GetComponent<CharacterController>();

        //Damage
        damaged = false;
        damageTimer_current = damageTimer;

        //Ai
        distanceFromPlayer = 100.0f;
        startTimer = 1.0f;

        //Attacking
        isAttacking = false;
        attackTimer = attackCoolDown;

    }

    // Update is called once per frame
    void Update()
    {
        if(startTimer > 0.0f)
        {
            startTimer -= Time.deltaTime;
        }
        //variables
        velocity = agent.velocity.magnitude;
        distanceFromPlayer = agent.remainingDistance;

        //AI
        agent.destination = player.transform.position;

        //Attacking
        if(velocity <= 0.0f && distanceFromPlayer < 2.0f && startTimer <= 0.0f && !isAttacking)
        {
            animator.SetTrigger("Attack_01");
            isAttacking = true;
        }

        if(isAttacking && attackTimer > 0.0f)
        {
            attackTimer -= Time.deltaTime;
        }
        else
        {
            isAttacking = false;
            attackTimer = attackCoolDown;
        }

        Debug.Log(attackTimer);

        Damage();

        animator.SetFloat("runVelocity", velocity);
    }

    private void Damage()
    {
        if (damaged && damageTimer_current > 0.0f)
        {
            damageTimer_current -= Time.deltaTime;
        }
        else
        {
            damageTimer_current = damageTimer;
            damaged = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Sword"))
        {
            if (player.GetState().Equals(PlayerController.State.Attack))
            {
                if (!damaged && player.InflictDamage())
                {
                    Debug.Log("Damage");
                    damaged = true;
                    animator.SetTrigger("Damage");
                }
            }
        }
    }
}

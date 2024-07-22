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
        if (startTimer > 0.0f)
        {
            startTimer -= Time.deltaTime;
        }

        velocity = agent.velocity.magnitude;
        distanceFromPlayer = agent.remainingDistance;
        agent.destination = player.transform.position;

        Attack();

        Damage();

        animator.SetFloat("runVelocity", velocity);
    }

    private void Attack()
    {
        if (CanAttack())
        {
            animator.SetTrigger("Attack_01");
            isAttacking = true;
        }

        //run the attak cooldown
        if (isAttacking && attackTimer > 0.0f)
        {
            attackTimer -= Time.deltaTime;
        }
        else
        {
            isAttacking = false;
            attackTimer = attackCoolDown;
        }

        //let the enemy finish the attack if they player ran away
        if (isAttacking)
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;

        }

    }

    private bool CanAttack()
    {
        bool enemyStopped = velocity <= 0.0f;
        bool closeToPlayer = distanceFromPlayer < 2.0f;
        bool gameStarted = startTimer <= 0.0f;
        bool notAlreadyAttacking = !isAttacking;

        bool canAttack = enemyStopped && closeToPlayer && gameStarted && notAlreadyAttacking;

        return canAttack;
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

    public float GetAttackCooldown()
    {
        return attackTimer;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("PlayerSword"))
        {
            if (player.GetState().Equals(PlayerController.State.Attack))
            {
                if (!damaged && player.InflictDamage())
                {
                    damaged = true;
                    animator.SetTrigger("Damage");
                }
            }
        }
    }
}

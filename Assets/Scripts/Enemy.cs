using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

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
    float health;
    bool isDead;

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
        health = 1.0f;

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
        distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (startTimer > 0.0f)
        {
            startTimer -= Time.deltaTime;
        }

        if (!isDead && distanceFromPlayer < 10.0f)
        {
            Move();

            Attack();

            Damage();

            animator.SetFloat("runVelocity", velocity);
        }
        else
        {
            agent.isStopped = true;
        }

        
    }

    private void Move()
    {
        velocity = agent.velocity.magnitude;
        agent.destination = player.transform.position;

        Vector3 moveDirection = player.transform.position - transform.position;
        moveDirection.Normalize();
        moveDirection.y = 0.0f;

        Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, agent.angularSpeed * Time.deltaTime);

        //let the enemy finish the attack if they player ran away
        if (isAttacking || damaged)
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;

        }
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

        

    }

    private bool CanAttack()
    {
        bool enemyStopped = velocity <= 0.0f;
        bool closeToPlayer = distanceFromPlayer <= agent.stoppingDistance;
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
        if (other.CompareTag("PlayerWeapon"))
        {
            if(player.HasActivatedShadowRealm())
            {
                TakeDamage(0.6f);
            }
            else
            {
                TakeDamage(0.3f);

            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Poison"))
        {
            TakeDamage(2.0f);
        }
    }

    private void TakeDamage(float damageAmount)
    {
        if (player.GetState().Equals(PlayerController.State.Attack))
        {
            if (!damaged && player.InflictDamage())
            {
                damaged = true;
                animator.SetTrigger("Damage");
                health -= damageAmount;

                if(health <= 0.0f)
                {
                    isDead = true;
                    animator.SetBool("isDead", true);
                }
            }
        }
    }
}

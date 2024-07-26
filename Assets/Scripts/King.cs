using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class King : MonoBehaviour
{
    public Slider healthBar; 
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

    //------attacking-----
    int currentAttack;
    int numOfAttacks;
    bool isAttacking;
    bool canAttack;
    bool isInflictingDamage;

    //----Damage-----
    float damageTimer;
    float health;
    bool isDead;
    bool isDamaged;

    // Start is called before the first frame update
    void Start()
    {
        //----components
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        //-----attacking----
        isInflictingDamage = false;
        numOfAttacks = 3;
        currentAttack = -1;


        //----takingDamage----
        health = 1.0f;
        isDamaged = false;
        damageTimer = 0.0f;
        isDead = false;

        //----animations
        standUp = 4.833f;

        //---timers---
        animationTimer = 0.0f;

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
        if (playerFound && isStanding && !isDead)
        {
            agent.destination = player.transform.position;
            RotateTowardsPlayer();
            
            healthBar.value = health;

            if (!player.IsAlive())
            {
                isAttacking = false;
            }

            if (distanceFromPlayer <= agent.stoppingDistance && !canAttack)
            {
                canAttack = true;
                currentAttack = 0;
            }
            else if(!canAttack)
            {
                currentAttack = -1;
                isAttacking = false;
                canAttack = false;
                agent.isStopped = false;
            }

            if(canAttack && player.IsAlive())
            {
                switch(currentAttack)
                {
                    case 0:
                        Attack(2.733f, "AttackCombo01", 0.8f, 1.0f);
                        break;
                    case 1:
                        Attack(3.167f, "Attack360", 0.7f, 1.0f);
                        break;
                    case 2:
                        Attack(2.267f, "AttackDownward", 0.2f, 0.3f);
                        break;
                }
            }

            if(isDamaged)
            {
                RunDamageTimer(0.967f);
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

    private void Attack(float animationLength, string name, float start, float stop)
    {
        isAttacking = true;
        
        //wait for animation
        if (animationTimer < animationLength && isAttacking)
        {
            animationTimer += Time.deltaTime;
            agent.isStopped = true;

            if(animationTimer > start && animationTimer < animationLength - stop)
            {
                isInflictingDamage = true;
            }
            else
            {
                isInflictingDamage = false;
            }

        }
        else
        {
            animationTimer = 0.0f;
            currentAttack++;
            if(currentAttack == numOfAttacks)
            {
                currentAttack = 0;
            }
            
        }
    }

    public void InflictDamage()
    {

        player.TakeDamage(0.1f);
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;

        if(health <= 0.0f)
        {
            isDead = true;
        }
        else
        {
            isDamaged = true;
        }
    }

    void RunDamageTimer(float animationLength)
    {
        if(damageTimer < animationLength)
        {
            damageTimer += Time.deltaTime;
        }
        else
        {
            damageTimer = 0.0f;
            isDamaged = false;

        }
    }

    private void Init()
    {
        distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);
        animator.SetFloat("RunVelocity", agent.velocity.magnitude);
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isDamaged", isDamaged);
        animator.SetBool("isDead", isDead);
        animator.SetInteger("AttackNumber", currentAttack);


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

    public bool GetIsInflictingDamage()
    {
        return isInflictingDamage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PlayerWeapon") && player.InflictDamage() && !isDamaged &&!isAttacking)
        {
            TakeDamage(0.3f);
        }
    }

}

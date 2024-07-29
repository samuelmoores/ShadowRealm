using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class King : MonoBehaviour
{
    public GameObject healthBarObject;
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
    bool goToNextAttack;

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
        currentAttack = 0;


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
        goToNextAttack = true;
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
            animator.SetBool("isAttacking", isAttacking);
            animator.SetInteger("AttackNumber", currentAttack);

            if (distanceFromPlayer <= agent.stoppingDistance)
            {
                isAttacking = true;
            }
            else
            {
                currentAttack = 0;
                isAttacking = false;
                canAttack = false;
                agent.isStopped = false;
            }

            if(isAttacking && player.IsAlive())
            {
                switch(currentAttack)
                {
                    case 0:

                        Attack(2.733f);
                        break;
                    case 1:

                        Attack(3.167f);
                        break;
                    case 2:

                        Attack(2.267f);
                        break;
                }
            }

            if(isDamaged)
            {
                RunDamageTimer(0.967f);
            }

            if (!player.IsAlive())
            {
                isAttacking = false;
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

    private void Attack(float animationLength)
    {
        //wait for animation
        if (animationTimer < animationLength && isAttacking)
        {
            //Debug.Log(animationTimer);
            animationTimer += Time.deltaTime;
            agent.isStopped = true;

        }
        else
        {
            animationTimer = 0.0f;
            goToNextAttack = true;
        }

        if(animationTimer > 0.5f && goToNextAttack)
        {
            currentAttack++;

            if (currentAttack == numOfAttacks)
            {
                currentAttack = 0;
            }
            goToNextAttack = false;
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
        animator.SetBool("isDamaged", isDamaged);
        animator.SetBool("isDead", isDead);


        if (distanceFromPlayer < 20.0f)
        {
            playerFound = true;
        }
        else
        {
            healthBarObject.SetActive(false);

        }

        if (!isStanding && playerFound) //&& player.HasActivatedShadowRealm())
        {
            //show health bar
            healthBarObject.SetActive(true);

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

        if (other.CompareTag("ShadowFist") && player.InflictDamage())
        {
            animator.SetTrigger("Hurt");
            isAttacking = false;
            TakeDamage(0.3f);
        }

    }

}

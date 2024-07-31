using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using static Unity.VisualScripting.Member;

public class Enemy : MonoBehaviour
{
    //---------public---------------
    public GameObject Chest;
    public float damageTimer;
    public float attackCoolDown; 
    public AudioClip[] footsteps;
    public AudioClip[] grunts;
    public AudioClip boomSound;

    int previousFootstep;
    int previousGrunt;

    //-----------Components------------
    CharacterController control;
    PlayerController player;
    Animator animator;
    NavMeshAgent agent;
    Rigidbody[] ragdollColliders;
    AudioSource source;

    //------------Damage-------------
    float damageTimer_current;
    bool damaged;
    float health;
    bool isDead;
    float gruntTimerDefault;
    float gruntTimer;
    float gruntSoundTime;
    bool isGrunting;

    //-------AI---------
    float velocity;
    float distanceFromPlayer;
    float startTimer;

    //---------Attacking--------
    bool isAttacking;
    float attackTimer;
    bool inflictDamage;
    bool playerFound;

    // Start is called before the first frame update
    void Start()
    {
        //Init
        player = GameObject.Find("Player").GetComponentInChildren<PlayerController>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        control = GetComponent<CharacterController>();
        source = GetComponent<AudioSource>();

        ragdollColliders = this.gameObject.GetComponentsInChildren<Rigidbody>();

        foreach (var rb in ragdollColliders)
        {
            rb.isKinematic = true;
        }

        //Damage
        damaged = false;
        damageTimer_current = damageTimer;
        health = 1.0f;
        isGrunting = false;

        //Ai
        distanceFromPlayer = 100.0f;
        startTimer = 1.0f;

        //Attacking
        isAttacking = false;
        attackTimer = attackCoolDown;
        inflictDamage = false;
        playerFound = false;

    }

    // Update is called once per frame
    void Update()
    {

        if (!isDead && !player.IsShadowed())
        {
            distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);

            if(distanceFromPlayer < 20.0f)
            {
                playerFound = true;
            }

            if (startTimer > 0.0f)
            {
                startTimer -= Time.deltaTime;
            }

            if(isGrunting)
            {
                gruntTimer -= Time.deltaTime;
                if(gruntTimer <= 0.0f)
                {
                    isGrunting = false;
                }
            }

            if (playerFound)
            {

                Move();

                if(player.IsAlive())
                {
                    Attack();

                }

                Damage();

                animator.SetFloat("runVelocity", velocity);
            }
            else
            {
                agent.isStopped = true;
            }
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

    public void SetInflictDamage(int value)
    {
        if(value == 0)
        {
            inflictDamage = false;
        }
        else
        {
            inflictDamage = true;
        }
    }

    public bool GetInflictDamage()
    {
        return inflictDamage;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("ShadowFist"))
        {
            if(player.HasActivatedShadowRealm())
            {
                TakeDamage(2.0f);
                AudioSource.PlayClipAtPoint(boomSound, transform.position);
            }
            else if(!player.HasPoison() && player.InflictDamage())
            {
                Debug.Log("enemy hit by player sword");
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
                //the player cannot damage a castle guard when they are not shadowed and have not activated shadow realm
                if(gameObject.tag == "CastleGuard" && !player.IsShadowed() && !player.HasActivatedShadowRealm())
                {
                    //do nothing
                }
                else
                {
                    damaged = true;
                    animator.SetTrigger("Damage");

                    if(!isGrunting && gruntTimer <= 0.0f)
                    {
                        Grunt();
                        isGrunting = true;
                        gruntTimer = source.clip.length;
                    }
                    
                    health -= damageAmount;
                    player.SetInflictDamage(0);
                }

                

                if (health <= 0.0f)
                {
                    isDead = true;

                    Debug.Log(gameObject.tag);

                    if(gameObject.tag == "DungeonGuard")
                    {
                        GameObject.Find("MinionCage").GetComponent<Cage>().Open();

                    }

                    if (player.HasActivatedShadowRealm())
                    {
                        EnableRagdoll();

                    }
                    else
                    {
                        animator.SetBool("isDead", true);

                    }

                }
            }
        }
    }

    public void FootStep()
    {
        if(distanceFromPlayer < 20.0f)
        {
            int footStep = Random.Range(0, 4);
            while (footStep == previousFootstep)
            {
                footStep = Random.Range(0, 4);
            }
            source.clip = footsteps[footStep];
            source.volume = 0.20f;
            source.Play();
            previousFootstep = footStep;
        }
        
    }

    public void Grunt()
    {
        int grunt = Random.Range(0, 23);
        while (grunt == previousGrunt)
        {
            grunt = Random.Range(0, 23);
        }
        source.clip = grunts[grunt];
        source.volume = 0.50f;
        source.Play();
        previousGrunt = grunt;
    }

    private void EnableRagdoll()
    {
        animator.enabled = false;
        GetComponent<CharacterController>().enabled = false;
        CapsuleCollider[] cols = GetComponents<CapsuleCollider>();

        for (int i = 0; i < 2; i++)
        {
            cols[i].enabled = false;
        }

        agent.enabled = false;

        foreach (var rb in ragdollColliders)
        {
            rb.isKinematic = false;
        }

        Vector3 shadowForce = transform.position - player.transform.position;
        shadowForce.Normalize();
        shadowForce.y = 0.0f;

        Debug.Log(shadowForce * 50000f);

        Chest.GetComponent<Rigidbody>().AddForce(shadowForce * 50000f);
        Chest.GetComponent<Rigidbody>().AddForce(Vector3.up * 15000f);
    }
}

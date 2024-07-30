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
    public Transform Chest;
    NavMeshAgent agent;
    PlayerController player;
    Animator animator;
    CharacterController controller;
    Rigidbody[] ragdollColliders;
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
    bool attackInit;

    //----Damage-----
    float damageTimer;
    float health;
    bool isDead;
    bool isDamaged;
    float damagedAnimationLength;
    bool isHurt;
    bool isPunishable;

    // Start is called before the first frame update
    void Start()
    {
        //----components
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        ragdollColliders = this.gameObject.GetComponentsInChildren<Rigidbody>();

        foreach (var rb in ragdollColliders)
        {
            rb.isKinematic = true;
        }


        //-----attacking----
        isInflictingDamage = false;
        numOfAttacks = 4;
        currentAttack = 0;
        attackInit = true;


        //----takingDamage----
        health = 1.0f;
        isDamaged = false;
        damageTimer = 0.0f;
        isDead = false;
        damagedAnimationLength = 0.0f;
        isHurt = false;
        isPunishable = false;

        //----animations
        standUp = 4.833f;

        //---timers---
        animationTimer = 0.0f;

        //---triggers---
        canAttack = true;
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
            
            if (distanceFromPlayer <= agent.stoppingDistance)
            {
                isAttacking = true;
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
		            case 3:
			            Attack(3.8f);
			            break;
                }
            }

            if(isDamaged)
            {
                RunDamageTimer(damagedAnimationLength);
		
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

        if(canAttack)
        {
            if(!isHurt)
            {
                currentAttack = Random.Range(0, 3);

            }
            else
            {
                currentAttack = 3;
                agent.angularSpeed = 0.0f;
            }

            canAttack = false;
            switch(currentAttack)
            {
                case 0:
                    animator.SetTrigger("Attack_01");
                    break;
                case 1:
                    animator.SetTrigger("Attack_02");
                    break;
                case 2:
                    animator.SetTrigger("Attack_03");
                    break;
                case 3:
                    animator.SetTrigger("Attack_04");
                    break;
            }
        }

        float finalThird = animationLength - (animationLength / 3.0f);

        //wait for animation
        if (animationTimer < animationLength && isAttacking)
        {
            animationTimer += Time.deltaTime;
            agent.isStopped = true;

        }
        else
        {
            animationTimer = 0.0f;
	        agent.isStopped = false;
	        isAttacking = false;
            canAttack = true;
            agent.angularSpeed = 720.0f;

        }


        if (animationTimer > finalThird)
        {
            isPunishable = true;
            Debug.Log("Punishable");
        }
        else
        {
            Debug.Log("----------------------------------");

            isPunishable = false;
        }
	
    }

    public bool IsPunishable()
    {
        return isPunishable;
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
	        isDamaged = false;
            isDead = true;
            healthBar.value = 0.0f;
            EnableRagdoll();
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
	        agent.isStopped = true;
        }
        else
        {
            damageTimer = 0.0f;
            isDamaged = false;
	        isAttacking = false;
	        agent.isStopped = false;

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


        if (!isStanding && playerFound) //&& player.HasActivatedShadowRealm())
        {
            //show health bar
            healthBarObject.SetActive(true);
	        animator.SetBool("isHurt", false);

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
            isAttacking = false;
	        damagedAnimationLength = 0.967f;
            TakeDamage(0.3f);
        }

        if (other.CompareTag("PlayerWeapon") && player.InflictDamage())
        {
	        if(!isHurt)
	        {
		        animator.SetTrigger("ShadowHit");
	            animator.SetBool("isHurt", true);
	            damagedAnimationLength = 6.033f;
		        isHurt = true;
		        agent.stoppingDistance = 5.0f;
	        }
	     
            isAttacking = false;

            TakeDamage(0.6f);
        }

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    PlayerController player;
    Animator animator;
    bool damaged;
    float damageTimer;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponentInChildren<PlayerController>();
        animator = GetComponent<Animator>();
        damaged = false;
        damageTimer = 1.0f;

    }

    // Update is called once per frame
    void Update()
    {
       if(damaged && damageTimer > 0.0f)
        {
            damageTimer -= Time.deltaTime;
        }
       else
        {
            damageTimer = 1.0f;
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
                    damaged = true;
                    animator.SetTrigger("Damage");
                }
            }
        }
    }
}

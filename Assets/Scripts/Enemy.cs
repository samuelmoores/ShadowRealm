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

        if(player.GetAttackTimer() < 1.2f && player.GetAttackTimer() > 0.8f)
        {
            //Debug.Log(player.GetAttackTimer());

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Sword"))
        {
            if (player.GetState().Equals(PlayerController.State.Attack))
            {
                if (!damaged && player.GetAttackTimer() > 0.0f)
                {
                    damaged = true;
                    animator.SetTrigger("Damage");
                }
            }
        }
    }
}

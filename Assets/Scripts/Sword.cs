using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sword : MonoBehaviour
{
    Enemy enemy;
    PlayerController player;
    float damageAmount;
    float attackTime;
    int attackCount;
    bool inflictDamage;
    bool canInflictDamage;
    bool damagedPlayer;

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        attackCount = 0;
        canInflictDamage = true;
        damagedPlayer = false;
        damageAmount = 0.3f;
    }

    // Update is called once per frame
    void Update()
    {
        if(enemy)
        {
            attackTime = enemy.GetAttackCooldown();

            if (attackTime < 1.9f && attackTime > 0.2f && canInflictDamage && !damagedPlayer)
            {
                inflictDamage = true;
            }
            else
            {
                inflictDamage = false;
            }

            if (attackTime <= 0.0f)
            {
                canInflictDamage = true;
                damagedPlayer = false;
                player.SetHit(false);

            }
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !damagedPlayer && inflictDamage && !player.IsDodgingBlocking())
        {
            Debug.Log("sword hit player");
            damagedPlayer = true;
            player.SetHit(true);

            player.TakeDamage(damageAmount);
        }
    }


}

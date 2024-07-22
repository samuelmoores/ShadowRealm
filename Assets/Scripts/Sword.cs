using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sword : MonoBehaviour
{
    Enemy enemy;
    PlayerController player;
    float attackTime;
    int attackCount;
    bool inflictDamage;
    bool canInflictDamage;

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        attackCount = 0;
        canInflictDamage = true;
    }

    // Update is called once per frame
    void Update()
    {
        attackTime = enemy.GetAttackCooldown();

        if (attackTime < 1.9f && attackTime > 1.7f && canInflictDamage)
        {
            inflictDamage = true;
        }
        else
        {
            inflictDamage = false;
        }

        if(attackTime <= 0.0f)
        {
            canInflictDamage = true;
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player") && canInflictDamage && inflictDamage)
        {
            player.TakeDamage(0.4f);
            canInflictDamage = false;
            attackCount++;
        }
    }

}

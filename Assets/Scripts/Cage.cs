using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cage : MonoBehaviour
{
    Minion minion;
    PlayerController player;
    bool hit;

    // Start is called before the first frame update
    void Start()
    {
        minion = GameObject.Find("Minion").GetComponent<Minion>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        hit = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(player.GetAttackTimer() <= 0.0f)
        {
            hit = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PlayerWeapon") && player.GetAttackTimer() > 0.3f && !hit)
        {
            Debug.Log("destory cage");
            minion.animator.SetTrigger("CageHit");
            hit = true;
        }
    }

}

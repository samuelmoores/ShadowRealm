using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cage : MonoBehaviour
{
    public GameObject message;
    bool isOpen;

    Minion minion;
    PlayerController player;
    bool hit;

    // Start is called before the first frame update
    void Start()
    {
        message.SetActive(false);
        isOpen = false;

        minion = GameObject.Find("Minion").GetComponent<Minion>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        hit = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetAttackTimer() <= 0.0f)
        {
            hit = false;
        }
    }

    public void Open()
    {
        GetComponent<MeshRenderer>().enabled = false;
        minion.animator.SetBool("Cheer", true);
        isOpen = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isOpen)
        {
            message.SetActive(true);
            player.ShadowIdentity();

        }

        if (other.CompareTag("PlayerWeapon") && player.GetAttackTimer() > 0.3f && !hit)
        {
            Debug.Log("destory cage");
            minion.animator.SetTrigger("CageHit");
            hit = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            message.SetActive(false);
        }
    }

}




using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    PlayerController player;
    bool damaged;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponentInChildren<PlayerController>();
        damaged = false;

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Damage(int i)
    {
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Sword"))
        {
            if (player.GetState().Equals(PlayerController.State.Attack))
            {
                if (!damaged)
                {
                    damaged = true;
                    Debug.Log("Damage");
                }
            }
        }
    }
}

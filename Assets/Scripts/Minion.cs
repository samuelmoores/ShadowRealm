using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour
{
    GameObject Cage;
    [HideInInspector] public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        Cage = GameObject.Find("Cage");
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

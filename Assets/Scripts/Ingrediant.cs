using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ingrediant : MonoBehaviour
{
    PlayerController player;
    float rotateSpeed;
    bool isActive;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        rotateSpeed = Random.Range(10, 60);
        isActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, Time.deltaTime * rotateSpeed);
        
        
    }

    public void SetActive(bool value)
    {
        
        gameObject.SetActive(true);

        isActive = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            player.AddIngredient(gameObject);
            gameObject.SetActive(false);
            isActive = false;

        }
    }
}

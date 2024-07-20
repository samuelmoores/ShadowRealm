using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    Slider slider;
    PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = player.GetHealth();
    }
}

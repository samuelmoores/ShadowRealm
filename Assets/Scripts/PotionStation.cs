using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PotionStation : MonoBehaviour
{
    public GameObject potionStation;
    public GameObject firstSelectedButton;
    public List<GameObject> InputImages;

    PlayerController player;
    int inputOrder;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        inputOrder = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(!player.GetIsCrafting())
        {
            potionStation.SetActive(false);

        }
    }

    public void SelectIngrediant(int ingrediant)
    {
        switch(inputOrder)
        {
            case 0:
                //input 1
                Debug.Log("ingrediant " + ingrediant + ": [" + inputOrder + "]");
                //set first input to the first selected ingrediant
                //InputImages[inputOrder].GetComponent<Image>().sprite = player.GetIngrediant(ingrediant);
                break;
            case 1:
                //input 2
                Debug.Log("ingrediant " + ingrediant + ": [" + inputOrder + "]");
                //InputImages[inputOrder].GetComponent<Image>().sprite = player.GetIngrediant(ingrediant);

                break;
            case 2:
                //input 2
                Debug.Log("ingrediant " + ingrediant + ": [" + inputOrder + "]");
                //InputImages[inputOrder].GetComponent<Image>().sprite = player.GetIngrediant(ingrediant);

                inputOrder = 0;
                break;
        }
        inputOrder++;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            potionStation.SetActive(true);
            player.SetIsCrafting(true);

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            potionStation.SetActive(false);

        }
    }
}

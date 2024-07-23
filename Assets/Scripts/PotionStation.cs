using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PotionStation : MonoBehaviour
{
    public GameObject potionStation;
    public GameObject firstSelectedButton;
    public List<GameObject> Inputs;
    public GameObject Output;
    public Sprite ImageToOutput;

    public Sprite Image_None;

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

    void ClearOutput()
    {
        for(int i = 0; i < Inputs.Count; i++)
        {
            Inputs[i].GetComponent<Image>().sprite = Image_None;

        }

        Output.GetComponent<Image>().sprite = Image_None;
    }

    //when ingredient is clicked
    public void SelectIngrediant(int ingredient)
    {
        if (player.GetIngredientCount() > 0)
        {
            //set input to the selected ingrediant
            Debug.Log("---------SelectIngrediant---------");
            Debug.Log("inputOrder: " + inputOrder);
            Debug.Log("ingrediant: " + ingredient);

            Inputs[inputOrder].GetComponent<Image>().sprite = player.GetIngrediant(ingredient).GetComponent<Image>().sprite;
            player.UseIngredient(ingredient);
            inputOrder++;

            if (inputOrder == 3)
            {
                SetOutput();
                inputOrder = 0;
            }

        }
    }

    private void SetOutput()
    {
        Output.GetComponent<Image>().sprite = ImageToOutput;

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
            ClearOutput();
        }
    }
}

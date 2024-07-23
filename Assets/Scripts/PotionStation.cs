using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PotionStation : MonoBehaviour
{
    //--------UI-------
    public GameObject potionStation_UI;
    public GameObject firstSelectedButton;
    public List<GameObject> Inputs;
    public GameObject Output;
    public Sprite ImageToOutput;
    public Sprite Image_None;
    public Sprite Image_Poison;


    //----Crafting-------
    PlayerController player;
    int inputOrder;
    string[] poisonRecipe;
    string[] selectedIngredients;
    bool[] poisonRecipeValid;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        inputOrder = 0;

        poisonRecipe = new string[3] { "WATER", "ALCOHOL", "HERB" };
        selectedIngredients = new string[3];
        poisonRecipeValid = new bool[3];


    }

    // Update is called once per frame
    void Update()
    {
        if(!player.GetIsCrafting())
        {
            potionStation_UI.SetActive(false);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            potionStation_UI.SetActive(true);
            player.SetIsCrafting(true);

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }

    //when ingredient is clicked
    public void SelectIngrediant(int ingredient)
    {
        //Does player have any?
        if (player.GetIngredientCount() > 0)
        {
            //Put first selected ingredient in input 1, second to 2 and third to 3
            Inputs[inputOrder].GetComponent<Image>().sprite = player.GetIngrediant(ingredient).GetComponent<Image>().sprite;

            GameObject SelectedIngredient = player.GetIngrediant(ingredient);

            if (SelectedIngredient.name == ("Water"))
            {
                poisonRecipeValid[inputOrder] = true;
            }

            if (SelectedIngredient.name == ("Alcohol"))
            {
                poisonRecipeValid[inputOrder] = true;
            }

            if (SelectedIngredient.name == ("Herb"))
            {
                poisonRecipeValid[inputOrder] = true;
            }

            //Remove it from players ingredients
            player.UseIngredient(ingredient);

            inputOrder++;

            //Once all three inputs are filled, show the output
            if (inputOrder == 3)
            {
                SetOutput();
                inputOrder = 0;
            }

        }
    }
    private void SetOutput()
    {
        if (poisonRecipeValid[0] && poisonRecipeValid[1] && poisonRecipeValid[2])
        {
            ImageToOutput = Image_Poison;

        }
        else
        {
            ImageToOutput = Image_None;
        }

        Output.GetComponent<Image>().sprite = ImageToOutput;
    }

    void ClearOutput()
    {
        for(int i = 0; i < Inputs.Count; i++)
        {
            Inputs[i].GetComponent<Image>().sprite = Image_None;

        }

        Output.GetComponent<Image>().sprite = Image_None;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            potionStation_UI.SetActive(false);
            ClearOutput();
        }
    }
}

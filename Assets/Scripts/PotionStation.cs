using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PotionStation : MonoBehaviour
{
    //---------Potions Spawning-----------------------------
    public Transform PotionSpawnTransform;
    public GameObject Poison_Prefab;
    GameObject Spawned_Poison_Prefab;

    //---------Potions-----------------------------


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
    int[] playerIngredients;
    int[] selectedIngredients;
    bool validInput;
    int inputOrder;
    string[] poisonRecipe;
    bool[] poisonRecipeValid;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        inputOrder = 0;
        poisonRecipe = new string[3] { "Water", "Alcohol", "Herb" };
        selectedIngredients = new int[3] { -1, -1, -1 };
        playerIngredients = new int[9] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        poisonRecipeValid = new bool[3];
        validInput = true;
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

    //----------------------------when ingredient is clicked------------------------------
    public void SelectIngrediant(int ingredient)
    {
        //Does player have any?
        if (player.GetIngredientCount() > 0)
        {
            validInput = true;

            //is this the first input?
            if(inputOrder == 0)
            {
                ClearInput();
                selectedIngredients[0] = ingredient;
                selectedIngredients[1] = -1;
                selectedIngredients[2] = -1;

            }
            else
            {
                //check if player has inputed an ingredient twice
                selectedIngredients[inputOrder] = ingredient;
                switch (inputOrder)
                {
                    case 1:
                        if (selectedIngredients[0] == ingredient)
                        {
                            validInput = false;
                        }
                        break;
                    case 2:
                        if (ingredient == selectedIngredients[0] || ingredient == selectedIngredients[1])
                        {
                            validInput = false;
                        }
                        break;
                }
            }

            //Put first selected ingredient in input 1, second to 2 and third to 3
            //if they have not inputed that same ingredient
            if(validInput && playerIngredients[ingredient] != -1)
            {
                Inputs[inputOrder].GetComponent<Image>().sprite = player.GetIngrediant(ingredient).GetComponent<Image>().sprite;

                GameObject SelectedIngredient = player.GetIngrediant(ingredient);

                if (SelectedIngredient.name == poisonRecipe[0])
                {
                    poisonRecipeValid[inputOrder] = true;
                }

                if (SelectedIngredient.name == poisonRecipe[1])
                {
                    poisonRecipeValid[inputOrder] = true;
                }

                if (SelectedIngredient.name == poisonRecipe[2])
                {
                    poisonRecipeValid[inputOrder] = true;
                }

                //let the player go get a new one
                SelectedIngredient.GetComponent<Ingrediant>().gameObject.SetActive(true);

                //Remove it from players ingredients
                player.UseIngredient(ingredient);
                playerIngredients[ingredient] = -1;

                inputOrder++;
            }
            

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
            Spawned_Poison_Prefab = GameObject.Instantiate(Poison_Prefab, PotionSpawnTransform, false);
            player.SetPotion(Spawned_Poison_Prefab);

        }
        else
        {
            ImageToOutput = Image_None;
        }

        Output.GetComponent<Image>().sprite = ImageToOutput;
    }

    void ClearInput()
    {
        for (int i = 0; i < Inputs.Count; i++)
        {
            Inputs[i].GetComponent<Image>().sprite = Image_None;

        }
    }

    void ClearOutput()
    {
        Output.GetComponent<Image>().sprite = Image_None;
        for(int i = 0; i < 3; i++)
        {
            poisonRecipeValid[i] = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            potionStation_UI.SetActive(false);
            ClearOutput();
            ClearInput();
            for(int i = 0; i < 9; i++)
            {
                playerIngredients[i] = i;
            }
        }
    }
}

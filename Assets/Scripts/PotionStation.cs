using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PotionStation : MonoBehaviour
{
    //---------Potions Spawning-----------------------------
    string[] recipeNames;
    public Transform PotionSpawnTransform;
    //---------Potions-----------------------------
    public GameObject Poison_Prefab;
    GameObject Spawned_Poison_Prefab;
    public Sprite Image_Poison;
    string[] poisonRecipe;

    //shadow realm
    public GameObject ShadowRealm_Prefab;
    GameObject Spawned_ShadowRealm_Prefab;
    public Sprite Image_ShadowRealm;

    string[] shadowRealmRecipe;


    //--------UI-------
    public GameObject potionStation_UI;
    public GameObject firstSelectedButton;
    public List<GameObject> Inputs_Images;
    public GameObject Output;
    public Sprite ImageToOutput;
    public Sprite Image_None;

    //----Crafting-------
    PlayerController player;
    GameObject[] Input_Ingredients;
    int[] playerIngredients;
    int[] selectedIngredients;
    bool validInput;
    int inputOrder;
    string recipeName_current;
    bool[] recipeValid;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        Input_Ingredients = new GameObject[3] { GameObject.CreatePrimitive(PrimitiveType.Cube), GameObject.CreatePrimitive(PrimitiveType.Cube), GameObject.CreatePrimitive(PrimitiveType.Cube) };
        poisonRecipe = new string[4] { "Water", "Alcohol", "Herb", "Poison" };
        shadowRealmRecipe = new string[4] { "Taxes", "Pizza", "Egg", "ShadowRealm" };
        recipeNames = new string[2] { "None", "None"};
        selectedIngredients = new int[3] { -1, -1, -1 };
        playerIngredients = new int[9] { -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        recipeValid = new bool[3] { false, false, false};
        inputOrder = 0;
        validInput = false;
        recipeName_current = "None";
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
            for(int i = 0; i < 9; i++)
            {
                playerIngredients[i] = player.getIngredientIndex(i);
            }


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
        if (player.GetIngredientCount() > 0 && playerIngredients[ingredient] != -1)
        {
            //is this the first input?
            if(inputOrder == 0)
            {
                ClearInput(ingredient);
            }
            else
            {
                //check if player has inputed an ingredient twice
                ValidateInput(ingredient);
            }

            //Put first selected ingredient in input 1, second to 2 and third to 3
            if (validInput)
            {
                SetInput(ingredient);
            }

            //Once all three inputs are filled, show the output
            if (inputOrder == 3)
            {
                SetOutput();
                inputOrder = 0;
            }

        }
    }
    private void ValidateInput(int ingredient)
    {
        //check if player selected same ingredient twice
        validInput = playerIngredients[ingredient] != -1;

        //store the current selection
        selectedIngredients[inputOrder] = ingredient;

        //run through the previous selections
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

    private void SetInput(int ingredient)
    {
        Input_Ingredients[inputOrder] = player.GetIngrediant(ingredient);
        Inputs_Images[inputOrder].GetComponent<Image>().sprite = player.GetIngrediant(ingredient).GetComponent<Image>().sprite;

        GameObject SelectedIngredient = player.GetIngrediant(ingredient);

        //let the player go get a new one
        SelectedIngredient.GetComponent<Ingrediant>().gameObject.SetActive(true);

        //Remove it from players ingredients
        player.UseIngredient(ingredient);
        playerIngredients[ingredient] = -1;

        inputOrder++;
    }

    private void ValidateRecipe(GameObject SelectedIngredient, string[] recipe, bool ordered, int inputIndex)
    {
        
        if (ordered)
        {
            switch(inputIndex)
            {
                case 0: // is first input valid?
                    if (SelectedIngredient.name == recipe[0])
                    {
                        recipeValid[inputIndex] = true;
                    }
                    break;
                case 1: //is second input valid?
                    if (SelectedIngredient.name == recipe[1])
                    {
                        recipeValid[inputIndex] = true;
                    }
                    break;
                case 2: //if all selected ingredients are valid...
                     
                    if (SelectedIngredient.name == recipe[2])
                    {
                        recipeValid[inputIndex] = true;

                        //set the recipe up for output
                        if (recipeValid[0] && recipeValid[1])
                        {
                            recipeName_current = recipe[3];

                        }
                    }

                    break;
            }



        }
        else
        {
            if (SelectedIngredient.name == recipe[0])
            {
                recipeValid[inputIndex] = true;
            }

            if (SelectedIngredient.name == recipe[1])
            {
                recipeValid[inputIndex] = true;
            }

            if (SelectedIngredient.name == recipe[2])
            {
                recipeValid[inputIndex] = true;

                //set the recipe up for output
                if (recipeValid[0] && recipeValid[1])
                {
                    recipeName_current = recipe[3];
                }
            }
        }

        
    }

    private void SetOutput()
    {
        for(int i = 0; i < 3; i++)
        {
            ValidateRecipe(Input_Ingredients[i], poisonRecipe, false, i);
            ValidateRecipe(Input_Ingredients[i], shadowRealmRecipe, true, i);

        }


        if (recipeValid[0] && recipeValid[1] && recipeValid[2])
        {
            if (recipeName_current.Equals("Poison"))
            {
                ImageToOutput = Image_Poison;
                Spawned_Poison_Prefab = GameObject.Instantiate(Poison_Prefab, PotionSpawnTransform, false);
                player.SetPotion(Spawned_Poison_Prefab, recipeName_current);
            }
            else if(recipeName_current.Equals("ShadowRealm"))
            {
                ImageToOutput = Image_ShadowRealm;
                Spawned_ShadowRealm_Prefab = GameObject.Instantiate(ShadowRealm_Prefab, PotionSpawnTransform, false);
                player.SetPotion(Spawned_ShadowRealm_Prefab, recipeName_current);
                player.ActivateShadowRealm();
            }
            
        }
        else
        {
            ImageToOutput = Image_None;
        }

        Output.GetComponent<Image>().sprite = ImageToOutput;

        for(int i = 0; i < 3; i++)
        {
            recipeValid[i] = false;
        }
        recipeName_current = "None";

    }

    void ClearInput(int ingredient)
    {
        //the first input is automatically valid
        validInput = true;

        //reset input order
        inputOrder = 0;

        //clear the input images
        for (int i = 0; i < Inputs_Images.Count; i++)
        {
            Inputs_Images[i].GetComponent<Image>().sprite = Image_None;

        }

        //only one ingredient has been selected
        selectedIngredients[0] = ingredient;
        selectedIngredients[1] = -1;
        selectedIngredients[2] = -1;
    }

    void ClearOutput()
    {
        Output.GetComponent<Image>().sprite = Image_None;
        for(int i = 0; i < 3; i++)
        {
            recipeValid[i] = false;
        }
        recipeName_current = "None";
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            potionStation_UI.SetActive(false);
            ClearOutput();
            ClearInput(-1);
            
        }
    }
}

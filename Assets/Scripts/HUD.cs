using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cinemachine;
using TMPro;
using UnityEngine.TextCore.Text;

public class HUD : MonoBehaviour
{
    //-----------DRAG THESE ITEMS INTO THE COMPONENT---------------
    public GameObject PauseMenu, SettingsMenu, ControlsMenu;
    public CinemachineFreeLook cam;
    public GameObject PauseMenuFirst, SettingsMenuFirst, ControlsMenuFirst;
    public Toggle Toggle_InvertCamera;
    public Slider Slider_Sensitivity;
    public TextMeshProUGUI Text_EncryptionAlphabet;
    public TextMeshProUGUI Text_EncryptedMessage;
    public TextMeshProUGUI Text_Alphabet;
    public GameObject playerHealthBar;
    public GameObject enemyHealthBar;
    public GameObject credits;



    public GameObject[] Ingrediant_Images;
    public Sprite Image_None;
    //-----------DRAG THESE ITEMS INTO THE COMPONENT---------------

    Slider slider;
    PlayerController player;
    PotionLedger potionLedger;
    int numOfIngredientImages;
    float value_y;
    float value_x;




    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        potionLedger = GameObject.Find("AlchemyStationLedger").GetComponent<PotionLedger>();
        PauseMenu.SetActive(false);
        SettingsMenu.SetActive(false);
        Toggle_InvertCamera.isOn = true;
        Slider_Sensitivity.value = 0.5f;
        
        numOfIngredientImages = 0;
    }

    // Update is called once per frame
    void Update()
    {
        SetHealthBar();
        Text_EncryptionAlphabet.text = potionLedger.encryptionAlphabet_display;
        Text_Alphabet.text = potionLedger.alphabet_display;
        Text_EncryptedMessage.text = potionLedger.encryptedMessage;


    }

    public void AddIngrediantImage(GameObject ingrediant,int index)
    {
        //Take image from ingrdient and put it on the HUD
        Ingrediant_Images[index].GetComponent<Image>().sprite = ingrediant.GetComponent<Image>().sprite;
        numOfIngredientImages++;
    }

    public void RemoveIngredientImage(int index)
    {
        Ingrediant_Images[index].GetComponent<Image>().sprite = Image_None;
        numOfIngredientImages--;

    }

    public void AdjustSensitivity()
    {
        float value = Slider_Sensitivity.value;

        if (value <= 0.5)
        {
            // Transform for range 0.0 to 0.5 -> 0.5 to 2.0
            value_y = 0.5f + (value * (2.0f - 0.5f) / 0.5f);
        }
        else
        {
            // Transform for range 0.5 to 1.0 -> 2.0 to 5.0
            value_y = 2.0f + ((value - 0.5f) * (5.0f - 2.0f) / 0.5f);
        }

        cam.m_YAxis.m_MaxSpeed = value_y;

        if (value <= 0.5)
        {
            // Transform for range 0.0 to 0.5 -> 100 to 300
            value_x = 100f + (value * (300f - 100f) / 0.5f);
        }
        else
        {
            // Transform for range 0.5 to 1.0 -> 300 to 500
            value_x = 300f + ((value - 0.5f) * (500f - 300f) / 0.5f);
        }

        cam.m_XAxis.m_MaxSpeed = value_x;

    }

    public void InvertCamera()
    {
        if(cam.m_YAxis.m_InvertInput)
        {
            cam.m_YAxis.m_InvertInput = false;
            //Toggle_InvertCamera.isOn = true;
        }
        else
        {
            cam.m_YAxis.m_InvertInput = true;
            //Toggle_InvertCamera.isOn = false;
        }
    }

    public void HideHealthBars()
    {
        playerHealthBar.SetActive(false);
        enemyHealthBar.SetActive(false);
        credits.SetActive(true);

    }

    private void SetHealthBar()
    {
        slider.value = player.GetHealth();
    }

    public void ShowPauseMenu()
    {
        //----Pause Game----------
        player.SetGameIsPaused(true);
        Time.timeScale = 0.0f;

        Cursor.visible = true;


        //Prevent player from jumping/attacking when game is unpause
        //since selecting resume and jump/attack are the same button
        player.unPauseTimer_current = player.unPauseTimer;

        SettingsMenu.SetActive(false);
        ControlsMenu.SetActive(false);
        PauseMenu.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(PauseMenuFirst);
    }

    public void ShowSettingsMenu()
    {
        //turn off pause menu
        PauseMenu.SetActive(false);

        //turn on Settings Menu
        SettingsMenu.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(SettingsMenuFirst);
    }

    public void ShowControlsMenu()
    {
        //turn off pause menu
        PauseMenu.SetActive(false);

        //turn on Controls Menu
        ControlsMenu.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(ControlsMenuFirst);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
        Cursor.visible = false;

        PauseMenu.SetActive(false);
        player.SetGameIsPaused(false);
        cam.m_YAxis.m_MaxSpeed = value_y;
        cam.m_XAxis.m_MaxSpeed = value_x;
    }
}

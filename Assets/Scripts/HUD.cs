using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cinemachine;

public class HUD : MonoBehaviour
{
    //-----------DRAG THESE ITEMS INTO THE COMPONENT---------------
    public GameObject PauseMenu, SettingsMenu, ControlsMenu;
    public CinemachineFreeLook cam;
    public GameObject PauseMenuFirst, SettingsMenuFirst, ControlsMenuFirst;
    public Toggle Toggle_InvertCamera;
    public Slider Slider_Sensitivity;
    //-----------DRAG THESE ITEMS INTO THE COMPONENT---------------

    //----Objects-----
    Slider slider;
    PlayerController player;


    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        PauseMenu.SetActive(false);
        SettingsMenu.SetActive(false);
        Toggle_InvertCamera.isOn = false;
        Slider_Sensitivity.value = 0.5f;

    }

    // Update is called once per frame
    void Update()
    {
        SetHealthBar();

    }

    public void AdjustSensitivity()
    {
        float value = Slider_Sensitivity.value;
        float value_y;

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

        float value_x;

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
            Toggle_InvertCamera.isOn = false;
            cam.m_YAxis.m_InvertInput = false;
        }
        else
        {
            Toggle_InvertCamera.isOn = true;
            cam.m_YAxis.m_InvertInput = true;
        }
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

        //Prevent player from jumping/attacking when game is unpause
        //since selecting resume and jump/attack are the same button
        player.unPauseTimer = 0.2f;


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
        PauseMenu.SetActive(false);
        player.SetGameIsPaused(false);
    }
}

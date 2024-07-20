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

    }

    // Update is called once per frame
    void Update()
    {
        SetHealthBar();

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

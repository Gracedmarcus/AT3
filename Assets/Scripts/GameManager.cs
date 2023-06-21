using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour, IPointerClickHandler
{
    public static GameManager Instance { get; private set; }
    public Waypoint[] Waypoints { get; set; }
    public int batteries;
    public Waypoint spawnPoint;
    public GameObject pauseMenu, optionsMenu, finished;
    public bool paused;
    [SerializeField]private Text goalCurr, winCon;
    public Image[] BatteriesList;
    public GameObject[] goalObj;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        GoalUpdate(0);
        paused = false;
    }
    void Start()
    {
        batteries = 0;
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        finished.SetActive(false);
        foreach(Image batt in BatteriesList)
        {
            batt.gameObject.SetActive(true);
            batteries++;
        }
    }

    void Update()
    {
        if(Input.GetButtonUp("Pause"))
        {
            if (paused == false)
            {
                Debug.Log("toggle" + paused);
                Pause(true);
            }
            else
            {
                Debug.Log("toggle" + paused);
                Pause(false);
            }
        }
        if(batteries != BatteriesList.Length)
        {
            BatteriesList[batteries].gameObject.SetActive(false);
        }
    }

    public void GoalUpdate(int goal)
    {
        switch (goal)
        {
            case 0:
                goalCurr.text = "Find the sphere";
                break;
            case 1:
                goalCurr.text = "Find the cube";
                break;
            case 2:
                goalCurr.text = "Escape!";
                break;
        }
    }

    public void GameOver() //ends game
    {
        finished.SetActive(true);
        winCon.text = "Oh no, you lost!";
        Time.timeScale = 0;
    }

    public void GameWin() //wins game
    {
        finished.gameObject.SetActive(true);
        winCon.text = "You've won!";
        Time.timeScale = 0;
    }
    public void Resume() //unpauses
    {
        Pause(false);
    }
    public void Options() //opens options menu
    {
        optionsMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }
    public void MainMenu() //returns to mainmenu
    {
        SceneManager.LoadScene(0);
    }
    public void Return()
    {
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }
    public void Pause(bool state) //pause state and time freeze
    {
        if (state)
        {
            paused = true;
            pauseMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
        }
        if (!state)
        {
            paused = false;
            pauseMenu.SetActive(false);
            optionsMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
} 
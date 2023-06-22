using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Waypoint[] Waypoints { get; set; }
    public int batteries, goalInt;
    public Waypoint spawnPoint;
    public Player player;
    public GameObject pauseMenu, optionsMenu, finished, exit;
    public bool paused, pauseBlock;
    [SerializeField]private Text goalCurr, winCon;
    public Image[] BatteriesList;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        paused = false;
    }
    void Start()
    {
        exit.SetActive(false);
        batteries = 0;
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        finished.SetActive(false);
        foreach(Image batt in BatteriesList)
        {
            batt.fillAmount = 1f;
            batteries++;
        }
        goalInt = 0;
        goalCurr.text = "Open the door!";
        Debug.Log(BatteriesList.Length + "Batts");
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if(Input.GetButtonUp("Pause"))
        {
            if (paused == false)
            {
                Pause(true);
            }
            else
            {
                Pause(false);
            }
        }
    }

    public void GoalUpdate(GameObject goal)
    {
        if(goalInt == 0 && goal.gameObject.name=="door")
        {
            goal.SetActive(false);
            goalCurr.text = "Find the BALL!";
            goalInt++;
        }
        if(goalInt == 1 && goal.gameObject.name == "sphere")
        {
            goal.SetActive(false);
            exit.SetActive(true);
            goalInt++;
            goalCurr.text = "Escape from the BOI!";
        }
    }

    public void GameOver() //ends game
    {
        finished.SetActive(true);
        winCon.text = "Wasted!";
        pauseBlock = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
    }

    public void GameWin() //wins game
    {
        finished.SetActive(true);
        winCon.text = "You've won!";
        pauseBlock = true;
        Cursor.lockState = CursorLockMode.None;
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
        if (state && !pauseBlock)
        {
            paused = true;
            pauseMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
        }
        if (!state && !pauseBlock)
        {
            paused = false;
            pauseMenu.SetActive(false);
            optionsMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        }
    }
} 
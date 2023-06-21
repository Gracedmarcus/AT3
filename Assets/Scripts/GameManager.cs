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
    public int batteries, goalInt, battTotal;
    public Waypoint spawnPoint;
    public Player player;
    public GameObject pauseMenu, optionsMenu, finished, currentObj, nextObj;
    public bool paused;
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
        batteries = 0;
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        finished.SetActive(false);
        foreach(Image batt in BatteriesList)
        {
            batt.gameObject.SetActive(true);
            batteries++;
        }
        goalInt = 1;
        battTotal = BatteriesList.Length;
        goalCurr.text = "Find the Golden Ball!";
        Debug.Log(BatteriesList.Length);
    }

    private void Update()
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
        if(batteries != battTotal)
        {
            Image target = BatteriesList[batteries];
            int fillNum = 100;
            while(fillNum <= 0)
            {
                target.fillAmount = fillNum;
                fillNum--;
                StartCoroutine(TimeDown());
            }
            battTotal--;
            player.torchOn = false;
        }
    }

    private IEnumerator TimeDown()
    {
        yield return new WaitForSeconds(0.1f);
    }

    public void GoalUpdate(GameObject goal)
    {
        if(goal != currentObj)
        { 
            //random text here
        }
        else if(goal == currentObj)
        {
            goalInt = 2;
            goalCurr.text = "Escape!";
            currentObj = nextObj;
        }
    }

    public void GameOver() //ends game
    {
        finished.SetActive(true);
        winCon.text = "Oh no, you died-ed!";
        Time.timeScale = 0;
    }

    public void GameWin() //wins game
    {
        finished.gameObject.SetActive(true);
        winCon.text = "You've won-eded!";
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
} 
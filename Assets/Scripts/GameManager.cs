using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Waypoint[] Waypoints { get; set; }
    public Player player { get; private set; }
    public Image img { get; private set; }
    public Waypoint spawnPoint;
    public GameObject pauseMenu, optionsMenu, finished;
    public bool paused;
    [SerializeField]private Text goalCurr, winCon;
    public Image[] BatteriesList;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        player = FindObjectOfType<Player>();
        GoalUpdate(0);
        paused = false;
    }
    void Start()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        finished.SetActive(false);
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
        }
    }

    public void GameOver()
    {
        finished.SetActive(true);
        winCon.text = "Oh no, you lost!";
        Time.timeScale = 0;
    }

    public void GameWin()
    {
        finished.gameObject.SetActive(true);
        winCon.text = "You've won!";
        Time.timeScale = 0;
    }
    public void Resume()
    {
        Pause(false);
    }
    public void Options()
    {
        optionsMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
    public void Pause(bool state)
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
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        }
    }
} 
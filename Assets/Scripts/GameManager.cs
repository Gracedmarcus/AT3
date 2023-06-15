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
    public MenuUI pauseMenu;
    public bool paused;
    [SerializeField]private Text goalCurr;
    public Image[] BatteriesList;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        player = FindObjectOfType<Player>();
        GoalUpdate("Enter the Bunker");
    }
    void Start()
    {
        if(spawnPoint == null)
        {
            Debug.Log("No spawn point");
        }
    }
    void Update()
    {
        if(paused && Time.timeScale != 0)
        {
            Time.timeScale = 0;
        }    
        else if(!paused && Time.timeScale != 1)
        {
            Time.timeScale = 1;
        }
        if(Input.GetKey(KeyCode.Escape))
        {
            if(paused == false)
            {
                paused = true;
                pauseMenu.Paused(paused);
            }
            if (paused == true)
            {
                paused = false;
                pauseMenu.Paused(paused);
            }
        }
    }

    void GoalUpdate(string goal)
    {
        goalCurr.text = "> " + goal;
    }

    /*public void PlayerUI(int num)
    {
        img = BatteriesList[num];
        toggle = true;
    }

    void OnUpdateFader()
    {
        while(img.fillAmount >= 0)
        {
            new WaitForSeconds(0.1f);
            img.fillAmount -= 0.10f;
            Debug.Log(img.fillAmount);
        }
        Debug.Log("Stun was successful");
        toggle = false;
    }*/

    public void MainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
} 
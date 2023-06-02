using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Waypoint[] Waypoints { get; set; }
    public Player Player { get { return Player; } }
    public Waypoint spawnPoint;
    private bool paused;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        if(spawnPoint == null)
        {
        Debug.Log("No spawn point");
        }
    }
    void OnUpdate()
    {
        if (!paused)
        {
            //open menu pause game
        }
        //close menu
    }
} 
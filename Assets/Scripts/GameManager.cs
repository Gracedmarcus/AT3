using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Waypoint[] Waypoints { get; }
    public Player Player { get { return Player; } }
    public Waypoint SpawnPoint { get; set; }
    private bool paused;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        foreach (Waypoint way in Waypoints)
        {
            if (way.name == "Spawn")
            {
                SpawnPoint = way;
                break;
            }
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
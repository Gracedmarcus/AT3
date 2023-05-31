using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private Waypoint[] waypoints;
    public static GameManager Instance { get; private set; }
    public Waypoint[] Waypoints { get { return waypoints; } }
    public Transform currentPoint, targetPoint;
    
    public Player Player { get { return Player; } }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
} 
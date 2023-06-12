using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Waypoint[] Waypoints { get; set; }
    private Canvas canvas;
    public Player player { get; private set; }
    public Image img { get; private set; }
    public Waypoint spawnPoint;
    private bool paused, toggle;
    [SerializeField] private Text goalCurr;
    public Image[] BatteriesList;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        player = FindObjectOfType<Player>();
        BatteriesList = GameObject.Find("BatteriesUI").gameObject.GetComponentsInChildren<Image>();
    }
    void Start()
    {
        if(spawnPoint == null)
        {
            Debug.Log("No spawn point");
        }
        GoalUpdate("Enter the Bunker");
    }
    void Update()
    {
        if (!paused)
        {
            //open menu pause game
            paused = true;
        }
        if (toggle)
        {
            OnUpdateFader();
        }
    }

    void GoalUpdate(string goal)
    {
        goalCurr.text = "> " + goal;
    }

    public void PlayerUI(int num)
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
    }
} 
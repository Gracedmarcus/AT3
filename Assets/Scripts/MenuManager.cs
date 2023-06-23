using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    public GameObject start, options, extras;
    public EventSystem eventSys;
    public GameManager game;

    public void Start()
    {
        if (eventSys == null)
        {
            eventSys = EventSystem.current;
        }
        game = GameManager.Instance;
        if (eventSys.firstSelectedGameObject == null)
        { 
        eventSys.SetSelectedGameObject(start.GetComponentInChildren<Button>().gameObject);
        }
    }

    private void Update()
    {
        if (eventSys.currentSelectedGameObject == null)
        {
            eventSys.SetSelectedGameObject(start.GetComponentInChildren<Button>().gameObject);
        }
    }

    public void StartGame()
    {
        start.SetActive(false);
        options.SetActive(false);
        extras.SetActive(false);
        SceneManager.LoadScene(1);
    }

    public void Options()
    {
        start.SetActive(false);
        options.SetActive(true);
        eventSys.SetSelectedGameObject(options.GetComponentInChildren<Button>().gameObject);
        extras.SetActive(false);
    }
    public void Extras()
    {
        start.SetActive(false);
        options.SetActive(false);
        extras.SetActive(true);
        eventSys.SetSelectedGameObject(extras.GetComponentInChildren<Button>().gameObject);
    }

    public void Return()
    {
        start.SetActive(true);
        options.SetActive(false);
        extras.SetActive(false);
        eventSys.SetSelectedGameObject(start.GetComponentInChildren<Button>().gameObject);
    }

    public void Quit()
    {
        Application.Quit();
    }
}

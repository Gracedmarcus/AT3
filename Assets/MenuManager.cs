using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject start, options, extras;

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Options()
    {
        start.SetActive(false);
        options.SetActive(true);
        extras.SetActive(false);
    }
    public void Extras()
    {
        start.SetActive(false);
        options.SetActive(false);
        extras.SetActive(true);
    }

    public void Return()
    {
        start.SetActive(true);
        options.SetActive(false);
        extras.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}

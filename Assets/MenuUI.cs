using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField]private Button resume, options, menu;
    private Canvas canvas;
    private Image panel;
    private bool paused;
    // Start is called before the first frame update

    private void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        panel = canvas.GetComponentInChildren<Image>();
        panel.enabled = false;
    }
    public void Resume()
    {
        paused = false;
        panel.enabled = false;
    }
    public void Options()
    {

    }
    public void QuitToMenu()
    {
        GameManager.Instance.MainMenu();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
            {
                panel.enabled = true;
                paused = true;
            }
            panel.enabled = false;
            paused = false;
        }
    }
}
